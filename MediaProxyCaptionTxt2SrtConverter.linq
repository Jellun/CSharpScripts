<Query Kind="Program">
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	//Modify the following four variables
	string videoStartDateTimeString = "2022-07-18T11:30:08+10:00";
	TimeSpan subtitleTimeCodeOffset = TimeSpan.FromMilliseconds(-5000);
	string sourceFileName = @"C:\Video\CaptionsTXT\9HD_Sydney_2022-07-18T11_30_07_0180000_10_00D1795720.txt";
	string destinationFileName = @"C:\Video\9HD_Sydney_2022-07-18T11_30_07_0180000_10_00D1795720.srt";


	DateTime videoStartTimeCode = DateTime.ParseExact(videoStartDateTimeString, "yyyy-MM-ddTHH:mm:ss%K", null);
	DateTime aSubStartTimeCode = new DateTime();
	DateTime aSubEndTimeCode = new DateTime();

	string text = File.ReadAllText(sourceFileName);
	string[] subs = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
	int i = 1;
	string aDateTimeString = String.Empty;
	string aDateTimeStringNext = String.Empty;
	string[] aSubtitleLine = new string[2];
	string[] aSubtitleLineNext = new string[2];
	string microsecondString = String.Empty;
	int microsecondLength = 6;
	TimeSpan lineStart = TimeSpan.FromMilliseconds(0);
	TimeSpan lineEnd = TimeSpan.FromMilliseconds(0);
	StringBuilder stringBuilder = new StringBuilder();

	for (int k=0; k<subs.Length-1; k++)
	{
		if (!String.IsNullOrWhiteSpace(subs[k]))
		{
			aSubtitleLine = subs[k].Split(new string[] { "     " }, StringSplitOptions.None);
			aSubtitleLineNext = subs[k+1].Split(new string[] { "     " }, StringSplitOptions.None);
			//string aDateTime = "2022-07-18T11:30:08.7310585+10:00";
			//string aDateTime = "2022-07-18T11:46:04.587203+10:00";
			aDateTimeString = aSubtitleLine[0].Trim();
			aDateTimeStringNext = aSubtitleLineNext[0].Trim();

			try
			{
				microsecondLength = aDateTimeStringNext.LastIndexOf('+') - aDateTimeStringNext.LastIndexOf('.') - 1;
				microsecondString = new string(Enumerable.Repeat('f', microsecondLength).ToArray());
				aSubEndTimeCode = DateTime.ParseExact(aDateTimeStringNext, $"yyyy-MM-ddTHH:mm:ss.{microsecondString}%K", null);

				microsecondLength = aDateTimeString.LastIndexOf('+') - aDateTimeString.LastIndexOf('.') - 1;
				microsecondString = new string(Enumerable.Repeat('f', microsecondLength).ToArray());
				aSubStartTimeCode = DateTime.ParseExact(aDateTimeString, $"yyyy-MM-ddTHH:mm:ss.{microsecondString}%K", null);

				stringBuilder.Append(i++ + Environment.NewLine);

				lineStart = aSubStartTimeCode - videoStartTimeCode + subtitleTimeCodeOffset;
				lineEnd = aSubEndTimeCode - videoStartTimeCode + subtitleTimeCodeOffset;

				if ((aSubEndTimeCode - aSubStartTimeCode) >= TimeSpan.FromSeconds(5))
				{
					// This is to fix the possible negative timecode values resulting from the offest
					// being applied to the first several subtitles
					if (lineStart < TimeSpan.FromMilliseconds(0))
					{
						lineStart = TimeSpan.FromMilliseconds(40);
						if (lineEnd - lineStart >= TimeSpan.FromSeconds(5))
						{
							stringBuilder.Append(lineStart.ToString(@"hh\:mm\:ss\,fff") + " --> " + (lineStart + TimeSpan.FromSeconds(5)).ToString(@"hh\:mm\:ss\,fff") + Environment.NewLine);
						}
						else
						{
							stringBuilder.Append(lineStart.ToString(@"hh\:mm\:ss\,fff") + " --> " + lineEnd.ToString(@"hh\:mm\:ss\,fff") + Environment.NewLine);
						}
					}
					else
					{
						stringBuilder.Append(lineStart.ToString(@"hh\:mm\:ss\,fff") + " --> " + (lineStart + TimeSpan.FromSeconds(5)).ToString(@"hh\:mm\:ss\,fff") + Environment.NewLine);
					}
				}
				else
				{
					// This is to fix the possible negative timecode values resulting from the offest
					// being applied to the first several subtitles
					if (lineStart < TimeSpan.FromMilliseconds(0))
					{
						lineStart = TimeSpan.FromMilliseconds(40);
						lineEnd = lineStart + (aSubEndTimeCode - aSubStartTimeCode);
					}
					stringBuilder.Append(lineStart.ToString(@"hh\:mm\:ss\,fff") + " --> " + lineEnd.ToString(@"hh\:mm\:ss\,fff") + Environment.NewLine);
				}
				stringBuilder.Append(aSubtitleLine[1].Trim() + Environment.NewLine + Environment.NewLine);
			}
			catch (FormatException)
			{
				Console.WriteLine($"FormatException: Unable to convert '{aDateTimeString}'");
			}
		}
	}

	stringBuilder.Append(i + Environment.NewLine);
	stringBuilder.Append(lineEnd.ToString(@"hh\:mm\:ss\,fff") + " --> " + (lineEnd + TimeSpan.FromSeconds(2)).ToString(@"hh\:mm\:ss\,fff") + Environment.NewLine);
	stringBuilder.Append(aSubtitleLineNext[1].Trim());

	using (System.IO.StreamWriter file = new System.IO.StreamWriter(destinationFileName))
	{
		file.Write(stringBuilder.ToString());
	}
}