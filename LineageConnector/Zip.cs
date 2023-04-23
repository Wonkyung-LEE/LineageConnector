using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace LineageConnector
{
    public class Zip
    {
        public bool Unzip(string FilePath, System.Windows.Forms.ProgressBar pbar = null, Downloader._ProgressChanged progresschanged = null) //by. cm01    2022-01-10
        {
            using (var zip = ZipFile.Open(FilePath, ZipArchiveMode.Read))
            {
                if (pbar != null)
                    pbar.Maximum = zip.Entries.Count;
                int count = 0;
                foreach (var entry in zip.Entries)
                {
                    count++;
                    if (pbar != null)
                        pbar.Value = count;
                    if (progresschanged != null) progresschanged(zip.Entries.Count, count, null);

                    string Dir = Path.GetDirectoryName(entry.FullName);
                    if (Dir != null && Dir.Length > 0)
                        if (!Directory.Exists(Path.GetDirectoryName(entry.FullName)))
                            Directory.CreateDirectory(Path.GetDirectoryName(entry.FullName));

                    string filename = Path.GetFileName(entry.FullName);
                    if (filename != null && filename.Length > 0)
                        entry.ExtractToFile(entry.FullName, true);
                }
                return true;
            }
            return false;
        }

        public void UnzipFromMemory(byte[] Memory) //by. cm01
        {
            using (MemoryStream ms = new MemoryStream(Memory))
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    using (Stream stream = entry.Open())
                    {
                        byte[] file = new byte[entry.Length];
                        if (stream.Read(file, 0, file.Length) > 0)
                            try
                            {
                                File.WriteAllBytes(entry.FullName, file);
                            }
                            catch (Exception ex
                            )
                            {
                                //하드코딩;
                                if (entry.Name.ToLower() == "dxwnd.dll") continue;
                                else throw ex;
                            }
                    }
                }
            }
        }

        internal void UnzipFromMemory(object windowMode)
        {
            throw new NotImplementedException();
        }
    }
}
