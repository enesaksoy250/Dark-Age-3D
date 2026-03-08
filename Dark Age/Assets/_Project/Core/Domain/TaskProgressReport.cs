namespace DarkAge.Core.Domain
{
    public sealed class TaskProgressReport
    {
        public TaskProgressReport(ResourceWallet grantedRewards, bool changed)
        {
            GrantedRewards = grantedRewards;
            Changed = changed;
        }

        public ResourceWallet GrantedRewards { get; }

        public bool Changed { get; }
    }
}
