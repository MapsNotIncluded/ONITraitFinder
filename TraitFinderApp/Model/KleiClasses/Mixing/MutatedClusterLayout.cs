using TraitFinderApp.Client.Model;

namespace TraitFinderApp.Model.KleiClasses.Mixing
{
	public class MutatedClusterLayout
	{
		public ClusterLayout layout;

		public MutatedClusterLayout(ClusterLayout layout)
		{
			//since we dont actively remix, ignore cloning
			//this.layout = SerializingCloner.Copy(layout);
			this.layout = (layout);
		}
	}
}
