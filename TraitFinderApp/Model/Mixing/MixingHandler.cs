using System.Numerics;
using TraitFinderApp.Client.Model;

namespace TraitFinderApp.Model.Mixing
{
	public class MixingHandler
	{
		public static List<MixingSettingConfig> DlcMixingSettings = new List<MixingSettingConfig>(4);
		public static List<MixingSettingConfig> WorldMixingSettings = new List<MixingSettingConfig>(4);
		public static List<MixingSettingConfig> SubworldMixingSettings = new List<MixingSettingConfig>(16);

		public static Dictionary<Dlc, MixingSettingConfig> DlcMixingSettingsDict = new Dictionary<Dlc, MixingSettingConfig>(4);


		public static void SetMixingStateWhere(Func<MixingSettingConfig,bool> ConditionFulfilled, bool enabled)
		{
			foreach(var mixing in DataImport.MixingSettings)
			{
				if (ConditionFulfilled(mixing))
					mixing.ForceEnabledState(enabled);
			}
		}

		public static void InitMixingSettings()
		{
			foreach(var mixing in DataImport.MixingSettings)
			{
				 mixing.InitBindings();
				switch (mixing.SettingType)
				{
					case (MixingType.DLC):
						mixing.DlcFrom = Dlc.KeyValues[mixing.Id];
						DlcMixingSettingsDict[mixing.DlcFrom] = mixing;
						DlcMixingSettings.Add(mixing);
						break;
					case (MixingType.World):
						WorldMixingSettings.Add(mixing);
						break;
					case (MixingType.Subworld):
						SubworldMixingSettings.Add(mixing);
						break;
				}

			}
		}



		public static string GetMixingSettingsCode()
		{
			BigInteger input = (BigInteger)0;
			foreach (MixingSettingConfig mixingSetting in DataImport.MixingSettings)
			{
				input *= (BigInteger)mixingSetting.coordinate_range;
				input += (BigInteger)mixingSetting.GetLevel().coordinate_value;
			}
			return BinarytoBase36(input);
		}
		private static string BinarytoBase36(BigInteger input)
		{
			if (input == 0L)
				return "0";
			BigInteger bigInteger = input;
			string result = "";
			while (bigInteger > 0L)
			{
				result += hexChars[(int)(bigInteger % (BigInteger)36)].ToString();
				bigInteger /= (BigInteger)36;
			}
			return result;
		}
		private static BigInteger Base36toBinary(string input)
		{
			if (input == "0")
				return 0;
			BigInteger output = 0;
			for (int index = input.Length - 1; index >= 0; --index)
				output = output * (BigInteger)36 + (BigInteger)(long)hexChars.IndexOf(input[index]);
			return output;
		}
		private static string hexChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	}
}
