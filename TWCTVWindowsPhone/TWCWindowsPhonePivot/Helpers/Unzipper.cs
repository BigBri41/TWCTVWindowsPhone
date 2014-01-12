using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.Generic;
using System.Windows.Resources;

namespace TWCWindowsPhonePivot.Helpers
{
    /// <summary>
    /// Easy unzipping of ZIP files.
    /// </summary>
    /// <remarks>
    /// Note that some ZIP files which doesn't report file size before
    /// the file content is not supported by Silverlight, and therefore
    /// also ignore by this class.
    /// </remarks>
    public class UnZipper
    {
        private Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnZipper"/> class.
        /// </summary>
        /// <param name="zipFileStream">The zip file stream.</param>
        public UnZipper(Stream zipFileStream)
        {
            this.stream = zipFileStream;
        }

        /// <summary>
        /// Gets the file stream for the specified file. Returns null if the file could not be found.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>Stream to file inside zip stream</returns>
        public Stream GetFileStream(string filename)
        {
            Uri fileUri = new Uri(filename, UriKind.Relative);
            StreamResourceInfo info = new StreamResourceInfo(this.stream, null);
            if (this.stream is System.IO.FileStream) //For whatever reason, you have to this with file streams, or you will get null back in the next call
                this.stream.Seek(0, SeekOrigin.Begin);
            StreamResourceInfo stream = System.Windows.Application.GetResourceStream(info, fileUri);
            if (stream != null)
                return stream.Stream;
            return null;
        }

        /// <summary>
        /// Gets a list of file names embedded in a Zip file.
        /// </summary>
        /// <param name="stream">The stream for a zip file.</param>
        /// <returns>List of file names</returns>
        public IEnumerable<string> GetFileNamesInZip()
        {
            BinaryReader reader = new BinaryReader(stream);
            stream.Seek(0, SeekOrigin.Begin);
            string name = null;
            List<string> names = new List<string>();
            while (ParseFileHeader(reader, out name))
            {
                names.Add(name);
            }
            return names;
        }

        /// <summary>
        /// Parses the zip file header.
        /// </summary>
        /// <remarks>Based on http://www.pkware.com/documents/casestudies/APPNOTE.TXT Zip file spec</remarks>
        /// <param name="reader">The reader.</param>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        private static bool ParseFileHeader(BinaryReader reader, out string filename)
        {
            filename = null;
            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                int headerSignature = reader.ReadInt32();
                if (headerSignature == 67324752) //PKZIP
                {
                    reader.BaseStream.Seek(2, SeekOrigin.Current); //Skip the following value
                    //short version = reader.ReadInt16(); //Unused
                    short genPurposeFlag = reader.ReadInt16();
                    if (((((int)genPurposeFlag) & 0x08) != 0)) //Not supported by Silverlight
                        return false;
                    reader.BaseStream.Seek(10, SeekOrigin.Current); //Skip the following values
                    //short method = reader.ReadInt16(); //Unused
                    //short lastModTime = reader.ReadInt16(); //Unused
                    //short lastModDate = reader.ReadInt16(); //Unused
                    //int crc32 = reader.ReadInt32(); //Unused
                    int compressedSize = reader.ReadInt32();
                    int unCompressedSize = reader.ReadInt32();
                    short fileNameLenght = reader.ReadInt16();
                    short extraFieldLenght = reader.ReadInt16();
                    filename = new string(reader.ReadChars(fileNameLenght));
                    if (string.IsNullOrEmpty(filename))
                        return false;
                    //Seek to the next file header
                    reader.BaseStream.Seek(extraFieldLenght + compressedSize, SeekOrigin.Current);
                    if (unCompressedSize == 0) //Directory. Skip it
                        return ParseFileHeader(reader, out filename);
                    else
                        return true;
                }
            }
            return false;
        }
    }
}
