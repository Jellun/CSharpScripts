<Query Kind="Program">
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	//Modify the four variables below
	string videoStartTimeCode = "2022-07-18T11:30:08+10:00";
	TimeSpan adjust = TimeSpan.FromMilliseconds(-5000);
	string sourceFileName = @"C:\ffmpeg\bin\New folder\9HD_Sydney_2022-07-18T11_30_07_0180000_10_00D1795720.txt";
	string destinationFileName = @"C:\ffmpeg\bin\9HD_Sydney_2022-07-18T11_30_07_0180000_10_00D1795720.srt";
	
	
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
				if (aDTstringNext.Length == 33)
				{
					aDTnext = DateTime.ParseExact(aDTstringNext, "yyyy-MM-ddTHH:mm:ss.fffffff%K", null);
				}
				else if (aDTstringNext.Length == 32)
				{
					aDTnext = DateTime.ParseExact(aDTstringNext, "yyyy-MM-ddTHH:mm:ss.ffffff%K", null);
				}

				if (aDTstring.Length == 33)
				{
					aDT = DateTime.ParseExact(aDTstring, "yyyy-MM-ddTHH:mm:ss.fffffff%K", null);
				}
				else if(aDTstring.Length == 32)
				{
					aDT = DateTime.ParseExact(aDTstring, "yyyy-MM-ddTHH:mm:ss.ffffff%K", null);
				}

				strB.Append(i++ + Environment.NewLine);
				if ((aDTnext - aDT) > TimeSpan.FromSeconds(5))
				{
					strB.Append((aDT - startTC + adjust).ToString(@"hh\:mm\:ss\,fff") + " --> " + (aDT - startTC + adjust + TimeSpan.FromSeconds(5)).ToString(@"hh\:mm\:ss\,fff") + Environment.NewLine);
				}
				else
				{
					strB.Append((aDT - startTC + adjust).ToString(@"hh\:mm\:ss\,fff") + " --> " + (aDTnext - startTC + adjust).ToString(@"hh\:mm\:ss\,fff") + Environment.NewLine);
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
	strB.Append((aDTnext - startTC + adjust).ToString(@"hh\:mm\:ss\,fff") + " --> " + (aDTnext - startTC  + adjust + TimeSpan.FromSeconds(2)).ToString(@"hh\:mm\:ss\,fff") + Environment.NewLine);
	strB.Append(aSubNext[1].Trim());

	using (System.IO.StreamWriter file = new System.IO.StreamWriter(destinationFileName))
	{
		file.Write(strB.ToString());
	}
}