using System.Drawing;

namespace TraitFinderApp.Model.KleiClasses
{
	public class UnityColor
	{
		public static Color Get(float r, float g, float b)
		{
			return Color.FromArgb(
				(int)(r * 255),
				(int)(g * 255),
				(int)(b * 255));
		}
	}
}
