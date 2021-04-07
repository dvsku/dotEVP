using System;
using System.IO;
using dotEVP.Core.Events;
using dotEVP.Core.Helpers;
using dotEVP.Core.Security;

namespace dotEVP.Core {
    public sealed class Packer {

        public event EventHandler PackingStarted;
        public event EventHandler PackingStopped;
        public event EventHandler PackingFinished;
        public event EventHandler<ErrorOccurredArgs> ErrorOccurred;
        public event EventHandler<double> ProgressUpdate;

        public Packer() { }

        public void Pack(string input, string output) {
            OnPackingStarted();
            string[] files = Directory.GetFiles(input, "*", SearchOption.AllDirectories);
            int currentDataOffset = Configuration.DataStartOffset;
            int fileDetailsSize = 0;
            int previousProgress = 0;
            double currentProgress = 0;
            double progressChangeData = 0.95 / (files.Length);
            double progressChangeDetails = 0.05 / (files.Length);

            try {
                using (FileStream fs = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    EVP[] inputFiles = new EVP[files.Length];
                    using (BinaryWriter binaryWriter = new BinaryWriter(fs)) {
                        binaryWriter.Write(Configuration.UnknownHeaderBytes);
                        binaryWriter.Write(Configuration.Padding);

                        int index = 0;
                        EVP inputFile = new EVP();
                        foreach (string file in files) {
                            
                            inputFile.path = file;
                            inputFile.data = File.ReadAllBytes(inputFile.path);
                            inputFile.dataLen = inputFile.data.Length;
                            inputFile.dataHash = MD5.ComputeHash(inputFile.data);
                            binaryWriter.Write(inputFile.data);
                            inputFile.data = null;
                            inputFile.dataStartOffset = currentDataOffset;
                            inputFiles[index++] = inputFile;
                            currentDataOffset += inputFile.dataLen;

                            currentProgress += progressChangeData;
                            if ((int)(currentProgress * 100) > previousProgress) {
                                previousProgress = (int)(currentProgress * 100);
                                OnProgressUpdate(previousProgress);
                            }
                        }

                        binaryWriter.Write(Configuration.Padding);

                        for(index = 0; index < inputFiles.Length; index++) {
                            inputFiles[index].path = inputFiles[index].path.Replace(input + "\\", "");
                            byte[] fileDetails = PackerHelpers.GenerateFileDetails(inputFiles[index]);
                            binaryWriter.Write(fileDetails);
                            fileDetailsSize += fileDetails.Length;

                            currentProgress += progressChangeDetails;
                            if ((int)(currentProgress * 100) > previousProgress) {
                                previousProgress = (int)(currentProgress * 100);
                                OnProgressUpdate(previousProgress);
                            }
                        }

                        fs.Seek(Configuration.HeaderEndOffset, SeekOrigin.Begin);
                        binaryWriter.Write(currentDataOffset);
                        binaryWriter.Write(fileDetailsSize);
                        binaryWriter.Write(Convert.ToInt64(files.Length));
                    }
                }
            }
            catch (IOException){
                OnErrorOccurred(new ErrorOccurredArgs("Packing", "Packing failed because output file is in use by another program."));
                OnPackingStopped();
                return;
            }
            OnPackingFinished();
        }

        private void OnPackingStarted() {
            EventHandler handler = PackingStarted;
            handler?.Invoke(this, null);
        }

        private void OnPackingStopped() {
            EventHandler handler = PackingStopped;
            handler?.Invoke(this, null);
        }

        private void OnPackingFinished() {
            EventHandler handler = PackingFinished;
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