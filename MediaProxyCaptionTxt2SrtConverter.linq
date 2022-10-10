<Query Kind="Program">
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	//Modify the four variables below
	string videoStartTimeCode = "2022-07-18T11:30:08+10:00";
	TimeSpan offset = TimeSpan.FromMilliseconds(0);
	//TimeSpan offset = TimeSpan.FromMilliseconds(-5000);
	string sourceFileName = @"C:\Video\CaptionsTXT\9HD_Sydney_2022-07-18T11_30_07_0180000_10_00D1795720.txt";
	string destinationFileName = @"C:\Video\9HD_Sydney_2022-07-18T11_30_07_0180000_10_00D1795720.srt";


	DateTime startTC = DateTime.ParseExact(videoStartTimeCode, "yyyy-MM-ddTHH:mm:ss%K", null);
	DateTime aDT = new DateTime();
	DateTime aDTnext = new DateTime();

	string text = File.ReadAllText(sourceFileName);
	string[] subs = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
	int i = 1;
	string aDTstring = String.Empty;
	string aDTstringNext = String.Empty;
	string[] aSub = new string[2];
	string[] aSubNext = new string[2];
	string fStr = String.Empty;
	int fLength = 6;
	StringBuilder strB = new StringBuilder();

	for (int k=0; k<subs.Length-1; k++)
	{
		if (!String.IsNullOrWhiteSpace(subs[k]))
		{
			aSub = subs[k].Split(new string[] { "     " }, StringSplitOptions.None);
			aSubNext = subs[k+1].Split(new string[] { "     " }, StringSplitOptions.None);
			//string aDateTime = "2022-07-18T11:30:08.7310585+10:00";
			//string aDateTime = "2022-07-18T11:46:04.587203+10:00";
			aDTstring = aSub[0].Trim();
			aDTstringNext = aSubNext[0].Trim();

			try
			{
				fLength = aDTstringNext.LastIndexOf('+') - aDTstringNext.LastIndexOf('.') - 1;
				fStr = new string(Enumerable.Repeat('f', fLength).ToArray());
				//Console.WriteLine(aDTstringNext + "   " + fLength + "   " + fStr);
				aDTnext = DateTime.ParseExact(aDTstringNext, $"yyyy-MM-ddTHH:mm:ss.{fStr}%K", null);

				fLength = aDTstring.LastIndexOf('+') - aDTstring.LastIndexOf('.') - 1;
				fStr = new string(Enumerable.Repeat('f', fLength).ToArray());
				aDT = DateTime.ParseExact(aDTstring, $"yyyy-MM-ddTHH:mm:ss.{fStr}%K", null);

				strB.Append(i++ + Environment.NewLine);
				if ((aDTnext - aDT) >= TimeSpan.FromSeconds(5))
				{
					strB.Append((aDT - startTC + offset).ToString(@"hh\:mm\:ss\,fff") + " --> " + (aDT - startTC + offset + TimeSpan.FromSeconds(5)).ToString(@"hh\:mm\:ss\,fff") + Environment.NewLine);
				}
				else
				{
					strB.Append((aDT - startTC + offset).ToString(@"hh\:mm\:ss\,fff") + " --> " + (aDTnext - startTC + offset).ToString(@"hh\:mm\:ss\,fff") + Environment.NewLine);
				}
				strB.Append(aSub[1].Trim() + Environment.NewLine + Environment.NewLine);
			}
			catch (FormatException)
			{
				Console.WriteLine($"FormatException: Unable to convert '{aDTstring}'");
			}
		}
	}

	strB.Append(i + Environment.NewLine);
	strB.Append((aDTnext - startTC + offset).ToString(@"hh\:mm\:ss\,fff") + " --> " + (aDTnext - startTC  + offset + TimeSpan.FromSeconds(2)).ToString(@"hh\:mm\:ss\,fff") + Environment.NewLine);
	strB.Append(aSubNext[1].Trim());

	using (System.IO.StreamWriter file = new System.IO.StreamWriter(destinationFileName))
	{
		file.Write(strB.ToString());
	}
}