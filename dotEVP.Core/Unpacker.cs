using System;
using System.IO;
using dotEVP.Core.Events;

namespace dotEVP.Core {
    public class Unpacker {
        public event EventHandler UnpackingStarted;
        public event EventHandler UnpackingStopped;
        public event EventHandler UnpackingFinished;
        public event EventHandler<ErrorOccurredArgs> ErrorOccurred;
        public event EventHandler<double> ProgressUpdate;

        public Unpacker() { }

        public void Unpack(string input, string output) {
            OnUnpackingStarted();
            int previousProgress = 0;
            double currentProgress = 0;
            double progressChange;
            int dataEndOffset;
            int fileNamesSize;
            long fileCount;

            try {
                using (FileStream fs = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.None)) {
                    using (BinaryReader binaryReader = new BinaryReader(fs)) {
                        binaryReader.BaseStream.Seek(Configuration.HeaderEndOffset, SeekOrigin.Begin);
                        dataEndOffset = binaryReader.ReadInt32();
                        fileNamesSize = binaryReader.ReadInt32();
                        fileCount = binaryReader.ReadInt64();

                        progressChange = 1.0 / fileCount;

                        int currentNameOffset = dataEndOffset + 16;
                        int currentDataOffset = Configuration.DataStartOffset;

                        for (int i = 0; i < fileCount; i++) {
                            EVP outputFile = new EVP();
                            binaryReader.BaseStream.Seek(currentNameOffset, SeekOrigin.Begin);
                            outputFile.pathLen = binaryReader.ReadInt32();
                            currentNameOffset += sizeof(int);
                            outputFile.path = new string(binaryReader.ReadChars(outputFile.pathLen));
                            currentNameOffset += outputFile.pathLen + sizeof(int);
                            binaryReader.BaseStream.Position += sizeof(int);
                            outputFile.dataLen = binaryReader.ReadInt32();
                            binaryReader.BaseStream.Seek(currentDataOffset, SeekOrigin.Begin);
                            outputFile.data = binaryReader.ReadBytes(outputFile.dataLen);
                            currentNameOffset += Configuration.OffsetBetweenNames;
                            currentDataOffset += outputFile.dataLen;

                            try {
                                string outputPath = Path.GetFullPath(output + "\\" + outputFile.path);
                                string directory = Path.GetDirectoryName(outputPath);
                                Directory.CreateDirectory(directory);
                                File.WriteAllBytes(outputPath, outputFile.data);
                            }
                            catch (Exception e) {
                                OnErrorOccurred(new ErrorOccurredArgs("Unpack", $"Unpacking {outputFile.path} failed: {e.Message}"));
                            }

                            currentProgress += progressChange;
                            if ((int)(currentProgress * 100) > previousProgress) {
                                previousProgress = (int)(currentProgress * 100);
                                OnProgressUpdate(previousProgress);
                            }
                        }
                    }
                }
            }
            catch (IOException) {
                OnErrorOccurred(new ErrorOccurredArgs("Unpack", "Packing failed because input file is in use by another program.\n"));
                return;
            }
            OnUnpackingFinished();
        }

        private void OnUnpackingStarted() {
            EventHandler handler = UnpackingStarted;
            handler?.Invoke(this, null);
        }

        private void OnUnpackingStopped() {
            EventHandler handler = UnpackingStopped;
            handler?.Invoke(this, null);
        }

        private void OnUnpackingFinished() {
            EventHandler handler = UnpackingFinished;
            handler?.Invoke(this, null);
        }

        private void OnErrorOccurred(ErrorOccurredArgs args) {
            EventHandler<ErrorOccurredArgs> handler = ErrorOccurred;
            handler?.Invoke(this, args);
        }

        private void OnProgressUpdate(double percentage) {
            EventHandler<double> handler = ProgressUpdate;
            handler?.Invoke(this, percentage);
        }
    }
}
