using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Infrastructure.Firebase.Auth;

namespace DarkAge.Infrastructure.Firebase.Persistence
{
    public sealed class FirestoreReflectionClient
    {
        private readonly FirebaseRuntimeAvailability _runtimeAvailability;

        public FirestoreReflectionClient(FirebaseRuntimeAvailability runtimeAvailability)
        {
            _runtimeAvailability = runtimeAvailability;
        }

        public async Task<IDictionary> GetDocumentAsync(string collectionName, string documentId, CancellationToken cancellationToken)
        {
            var documentReference = await GetDocumentReferenceAsync(collectionName, documentId).ConfigureAwait(false);
            var getTask = documentReference.GetType().GetMethod("GetSnapshotAsync", Type.EmptyTypes)?.Invoke(documentReference, null);
            await ReflectionTaskUtility.AwaitTaskAsync(getTask).ConfigureAwait(false);

            var snapshot = ReflectionTaskUtility.GetTaskResult(getTask);
            var exists = (bool)(snapshot?.GetType().GetProperty("Exists", BindingFlags.Public | BindingFlags.Instance)?.GetValue(snapshot) ?? false);
            if (!exists)
            {
                return null;
            }

            return snapshot.GetType().GetMethod("ToDictionary", Type.EmptyTypes)?.Invoke(snapshot, null) as IDictionary;
        }

        public async Task<IReadOnlyList<IDictionary>> GetCollectionAsync(string collectionName, CancellationToken cancellationToken)
        {
            var collectionReference = await GetCollectionReferenceAsync(collectionName).ConfigureAwait(false);
            var getTask = collectionReference.GetType().GetMethod("GetSnapshotAsync", Type.EmptyTypes)?.Invoke(collectionReference, null);
            await ReflectionTaskUtility.AwaitTaskAsync(getTask).ConfigureAwait(false);

            var snapshot = ReflectionTaskUtility.GetTaskResult(getTask);
            var documents = snapshot?.GetType().GetProperty("Documents", BindingFlags.Public | BindingFlags.Instance)?.GetValue(snapshot) as IEnumerable;
            if (documents == null)
            {
                return Array.Empty<IDictionary>();
            }

            return documents
                .Cast<object>()
                .Select(document => document.GetType().GetMethod("ToDictionary", Type.EmptyTypes)?.Invoke(document, null) as IDictionary)
                .Where(dictionary => dictionary != null)
                .ToArray();
        }

        public async Task SetDocumentAsync(string collectionName, string documentId, Dictionary<string, object> payload, CancellationToken cancellationToken)
        {
            var documentReference = await GetDocumentReferenceAsync(collectionName, documentId).ConfigureAwait(false);
            var setMethod = documentReference.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(method => method.Name == "SetAsync" && method.GetParameters().Length == 1);

            if (setMethod == null)
            {
                throw new InvalidOperationException("Firestore SetAsync(object) overload could not be located.");
            }

            var setTask = setMethod.Invoke(documentReference, new object[] { payload });
            await ReflectionTaskUtility.AwaitTaskAsync(setTask).ConfigureAwait(false);
        }

        private async Task<object> GetCollectionReferenceAsync(string collectionName)
        {
            if (!await _runtimeAvailability.IsAvailableAsync().ConfigureAwait(false))
            {
                throw new InvalidOperationException("Firebase runtime is not available.");
            }

            var firestoreType = Type.GetType("Firebase.Firestore.FirebaseFirestore, Firebase.Firestore");
            if (firestoreType == null)
            {
                throw new InvalidOperationException("Firebase.Firestore assembly could not be located.");
            }

            var firestore = firestoreType.GetProperty("DefaultInstance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
            if (firestore == null)
            {
                throw new InvalidOperationException("FirebaseFirestore.DefaultInstance is unavailable.");
            }

            return firestore.GetType().GetMethod("Collection", new[] { typeof(string) })?.Invoke(firestore, new object[] { collectionName });
        }

        private async Task<object> GetDocumentReferenceAsync(string collectionName, string documentId)
        {
            var collectionReference = await GetCollectionReferenceAsync(collectionName).ConfigureAwait(false);
            return collectionReference.GetType().GetMethod("Document", new[] { typeof(string) })?.Invoke(collectionReference, new object[] { documentId });
        }
    }
}
