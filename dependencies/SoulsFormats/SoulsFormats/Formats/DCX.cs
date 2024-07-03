using System;
using System.IO;
using System.IO.Compression;

namespace SoulsFormats
{
    /// <summary>
    /// A general-purpose single compressed file wrapper used in most fromsoft games.
    /// </summary>
    public static class DCX
    {
        internal static bool Is(BinaryReaderEx expression)
        {
            if (expression.Stream.Length < 4)
            { return false; }

            string ascii = expression.GetASCII(0, 4);
            // byte b0 = expression.GetByte(0); // potentially not needed for ER
            // byte b1 = expression.GetByte(1); // potentially not needed for ER
            return ascii == "DCP\0" || ascii == "DCX\0"; // || b0 == 0x78 && (b1 == 0x01 || b1 == 0x5E || b1 == 0x9C || b1 == 0xDA);
        }

        /// <summary>
        /// Returns true if the bytes appear to be a DCX file.
        /// </summary>
        public static bool Is(byte[] bytes)
        {
            var expression = new BinaryReaderEx(true, bytes);
            return Is(expression);
        }

        /// <summary>
        /// Returns true if the file appears to be a DCX file.
        /// </summary>
        public static bool Is(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                var expression = new BinaryReaderEx(true, stream);
                return Is(expression);
            }
        }

        #region Public Decompress
        /// <summary>
        /// Decompress a DCX file from an array of bytes and return the detected DCX type.
        /// </summary>
        public static byte[] Decompress(byte[] data, out Type type)
        {
            BinaryReaderEx expression = new BinaryReaderEx(true, data);
            return Decompress(expression, out type);
        }

        /// <summary>
        /// Decompress a DCX file from an array of bytes.
        /// </summary>
        public static byte[] Decompress(byte[] data)
        {
            return Decompress(data, out _);
        }

        /// <summary>
        /// Decompress a DCX file from the specified path and return the detected DCX type.
        /// </summary>
        public static byte[] Decompress(string path, out Type type)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                BinaryReaderEx expression = new BinaryReaderEx(true, stream);
                return Decompress(expression, out type);
            }
        }

        /// <summary>
        /// Decompress a DCX file from the specified path.
        /// </summary>
        public static byte[] Decompress(string path)
        {
            return Decompress(path, out _);
        }
        #endregion

        internal static byte[] Decompress(BinaryReaderEx expression, out Type type)
        {
            expression.BigEndian = true;
            type = Type.Unknown;
            string ascii = expression.ReadASCII(4);

            switch (ascii)
            { // branching to format
                case "DCP\0":
                    {
                        string format = expression.GetASCII(4, 4);
                        switch (format)
                        {
                            case "DFLT":
                                type = Type.DCP_DFLT;
                                break;
                            case "EDGE":
                                type = Type.DCP_EDGE;
                                break;
                        }
                        break;
                    }
                case "DCX\0":
                    {
                        string format = expression.GetASCII(0x28, 4);
                        switch (format)
                        {
                            case "EDGE":
                                type = Type.DCX_EDGE;
                                break;
                            case "ZSTD":
                                type = Type.DCX_ZSTD;
                                break;
                            case "KRAK":
                                int compressionLevel = expression.GetByte(0x30);
                                type = compressionLevel == 9 ? Type.DCX_KRAK_MAX : Type.DCX_KRAK;
                                break;
                            case "DFLT":
                                int unk04 = expression.GetInt32(0x4);
                                int unk10 = expression.GetInt32(0x10);
                                byte unk30 = expression.GetByte(0x30);
                                byte unk38 = expression.GetByte(0x38);

                                if (BinaryReaderEx.IsFlexible && unk04 != 0x11000)
                                    unk04 = 0x10000;

                                if (unk04 == 0x10000 && unk10 == 0x24 && unk30 == 9 && unk38 == 0)
                                    type = Type.DCX_DFLT_10000_24_9;
                                else if (unk04 == 0x10000 && unk10 == 0x44 && unk30 == 9 && unk38 == 0)
                                    type = Type.DCX_DFLT_10000_44_9;
                                else if (unk04 == 0x11000 && unk10 == 0x44 && unk30 == 8 && unk38 == 0)
                                    type = Type.DCX_DFLT_11000_44_8;
                                else if (unk04 == 0x11000 && unk10 == 0x44 && unk30 == 9 && unk38 == 0)
                                    type = Type.DCX_DFLT_11000_44_9;
                                else if (unk04 == 0x11000 && unk10 == 0x44 && unk30 == 9 && unk38 == 15)
                                    type = Type.DCX_DFLT_11000_44_9_15;
                                break;
                        }
                        break;
                    }
                default:
                    {
                        byte b0 = expression.GetByte(0);
                        byte b1 = expression.GetByte(1);
                        if (b0 == 0x78 && (b1 == 0x01 || b1 == 0x5E || b1 == 0x9C || b1 == 0xDA))
                        {
                            type = Type.Zlib;
                        }
                        break;
                    }
            }
            expression.Position = 0;

            switch (type)
            {
                case Type.Zlib:
                    return SFUtil.ReadZlib(expression, (int)expression.Length);
                case Type.DCX_ZSTD:
                    return DecompressDCXZSTD(expression);
                case Type.DCP_EDGE:
                    return DecompressDCPEDGE(expression);
                case Type.DCX_KRAK:
                    return DecompressDCXKRAK(expression);
                case Type.DCP_DFLT:
                    return DecompressDCPDFLT(expression);
                case Type.DCX_EDGE:
                    return DecompressDCXEDGE(expression);
                case Type.DCX_DFLT_10000_24_9:
                    return DecompressDCXDFLT(expression, type);
                case Type.DCX_DFLT_10000_44_9:
                    return DecompressDCXDFLT(expression, type);
                case Type.DCX_DFLT_11000_44_8:
                    return DecompressDCXDFLT(expression, type);
                case Type.DCX_DFLT_11000_44_9:
                    return DecompressDCXDFLT(expression, type);
                case Type.DCX_DFLT_11000_44_9_15:
                    return DecompressDCXDFLT(expression, type);
                default:
                    throw new FormatException("Unknown DCX format.");
            }
        }

        private static byte[] DecompressDCPDFLT(BinaryReaderEx expression)
        {
            expression.AssertASCII("DCP\0");
            expression.AssertASCII("DFLT");
            expression.AssertInt32(0x20);
            expression.AssertInt32(0x9000000);
            expression.AssertInt32(0);
            expression.AssertInt32(0);
            expression.AssertInt32(0);
            expression.AssertInt32(0x00010100);

            expression.AssertASCII("DCS\0");
            int uncompressedSize = expression.ReadInt32();
            int compressedSize = expression.ReadInt32();

            byte[] decompressed = SFUtil.ReadZlib(expression, compressedSize);

            expression.AssertASCII("DCA\0");
            expression.AssertInt32(8);

            return decompressed;
        }

        private static byte[] DecompressDCPEDGE(BinaryReaderEx expression)
        {
            expression.AssertASCII("DCP\0");
            expression.AssertASCII("EDGE");
            expression.AssertInt32(0x20);
            expression.AssertInt32(0x9000000);
            expression.AssertInt32(0x10000);
            expression.AssertInt32(0x0);
            expression.AssertInt32(0x0);
            expression.AssertInt32(0x00100100);

            expression.AssertASCII("DCS\0");
            int uncompressedSize = expression.ReadInt32();
            int compressedSize = expression.ReadInt32();
            expression.AssertInt32(0);
            long dataStart = expression.Position;
            expression.Skip(compressedSize);

            expression.AssertASCII("DCA\0");
            int dcaSize = expression.ReadInt32();
            // ???
            expression.AssertASCII("EgdT");
            expression.AssertInt32(0x00010000);
            expression.AssertInt32(0x20);
            expression.AssertInt32(0x10);
            expression.AssertInt32(0x10000);
            int egdtSize = expression.ReadInt32();
            int chunkCount = expression.ReadInt32();
            expression.AssertInt32(0x100000);

            if (egdtSize != 0x20 + chunkCount * 0x10)
                throw new InvalidDataException("Unexpected EgdT size in EDGE DCX.");

            byte[] decompressed = new byte[uncompressedSize];
            using (MemoryStream dcmpStream = new MemoryStream(decompressed))
            {
                for (int i = 0; i < chunkCount; i++)
                {
                    expression.AssertInt32(0);
                    int offset = expression.ReadInt32();
                    int size = expression.ReadInt32();
                    bool compressed = expression.AssertInt32(0, 1) == 1;

                    byte[] chunk = expression.GetBytes(dataStart + offset, size);

                    if (compressed)
                    {
                        using (MemoryStream cmpStream = new MemoryStream(chunk))
                        using (DeflateStream dfltStream = new DeflateStream(cmpStream, CompressionMode.Decompress))
                            dfltStream.CopyTo(dcmpStream);
                    }
                    else
                    {
                        dcmpStream.Write(chunk, 0, chunk.Length);
                    }
                }
            }

            return decompressed;
        }

        private static byte[] DecompressDCXEDGE(BinaryReaderEx expression)
        {
            expression.AssertASCII("DCX\0");
            expression.AssertInt32(0x10000);
            expression.AssertInt32(0x18);
            expression.AssertInt32(0x24);
            expression.AssertInt32(0x24);
            int unk1 = expression.ReadInt32();

            expression.AssertASCII("DCS\0");
            int uncompressedSize = expression.ReadInt32();
            int compressedSize = expression.ReadInt32();

            expression.AssertASCII("DCP\0");
            expression.AssertASCII("EDGE");
            expression.AssertInt32(0x20);
            expression.AssertInt32(0x9000000);
            expression.AssertInt32(0x10000);
            expression.AssertInt32(0x0);
            expression.AssertInt32(0x0);
            expression.AssertInt32(0x00100100);

            long dcaStart = expression.Position;
            expression.AssertASCII("DCA\0");
            int dcaSize = expression.ReadInt32();
            // ???
            expression.AssertASCII("EgdT");
            expression.AssertInt32(0x00010100);
            expression.AssertInt32(0x24);
            expression.AssertInt32(0x10);
            expression.AssertInt32(0x10000);
            // Uncompressed size of last block
            int trailingUncompressedSize = expression.AssertInt32(uncompressedSize % 0x10000, 0x10000);
            int egdtSize = expression.ReadInt32();
            int chunkCount = expression.ReadInt32();
            expression.AssertInt32(0x100000);

            if (unk1 != 0x50 + chunkCount * 0x10)
                throw new InvalidDataException("Unexpected unk1 value in EDGE DCX.");

            if (egdtSize != 0x24 + chunkCount * 0x10)
                throw new InvalidDataException("Unexpected EgdT size in EDGE DCX.");

            byte[] decompressed = new byte[uncompressedSize];
            using (MemoryStream dcmpStream = new MemoryStream(decompressed))
            {
                for (int i = 0; i < chunkCount; i++)
                {
                    expression.AssertInt32(0);
                    int offset = expression.ReadInt32();
                    int size = expression.ReadInt32();
                    bool compressed = expression.AssertInt32(0, 1) == 1;

                    byte[] chunk = expression.GetBytes(dcaStart + dcaSize + offset, size);

                    if (compressed)
                    {
                        using (MemoryStream cmpStream = new MemoryStream(chunk))
                        using (DeflateStream dfltStream = new DeflateStream(cmpStream, CompressionMode.Decompress))
                            dfltStream.CopyTo(dcmpStream);
                    }
                    else
                    {
                        dcmpStream.Write(chunk, 0, chunk.Length);
                    }
                }
            }

            return decompressed;
        }

        private static byte[] DecompressDCXDFLT(BinaryReaderEx expression, Type type)
        {
            int unk04 = (type == Type.DCX_DFLT_10000_24_9 || type == Type.DCX_DFLT_10000_44_9) ? 0x10000 : 0x11000;
            int unk10 = type == Type.DCX_DFLT_10000_24_9 ? 0x24 : 0x44;
            int unk14 = type == Type.DCX_DFLT_10000_24_9 ? 0x2C : 0x4C;
            byte unk30 = (byte)(type == Type.DCX_DFLT_11000_44_8 ? 8 : 9);
            byte unk38 = (byte)(type == Type.DCX_DFLT_11000_44_9_15 ? 15 : 0);

            expression.AssertASCII("DCX\0");
            expression.AssertInt32(unk04);
            expression.AssertInt32(0x18);
            expression.AssertInt32(0x24);
            expression.AssertInt32(unk10);
            expression.AssertInt32(unk14);

            expression.AssertASCII("DCS\0");
            int uncompressedSize = expression.ReadInt32();
            int compressedSize = expression.ReadInt32();

            expression.AssertASCII("DCP\0");
            expression.AssertASCII("DFLT");
            expression.AssertInt32(0x20);
            expression.AssertByte(unk30);
            expression.AssertByte(0);
            expression.AssertByte(0);
            expression.AssertByte(0);
            expression.AssertInt32(0x0);
            expression.AssertByte(unk38);
            expression.AssertByte(0);
            expression.AssertByte(0);
            expression.AssertByte(0);
            expression.AssertInt32(0x0);
            // These look suspiciously like flags
            expression.AssertInt32(0x00010100);

            expression.AssertASCII("DCA\0");
            int compressedHeaderLength = expression.ReadInt32();

            return SFUtil.ReadZlib(expression, compressedSize);
        }

        private static byte[] DecompressDCXKRAK(BinaryReaderEx expression)
        {
            expression.AssertASCII("DCX\0");
            expression.AssertInt32(0x11000);
            expression.AssertInt32(0x18);
            expression.AssertInt32(0x24);
            expression.AssertInt32(0x44);
            expression.AssertInt32(0x4C);
            expression.AssertASCII("DCS\0");
            uint uncompressedSize = expression.ReadUInt32();
            uint compressedSize = expression.ReadUInt32();
            expression.AssertASCII("DCP\0");
            expression.AssertASCII("KRAK");
            expression.AssertInt32(0x20);
            expression.AssertInt32(0x6000000);
            expression.AssertInt32(0);
            expression.AssertInt32(0);
            expression.AssertInt32(0);
            expression.AssertInt32(0x10100);
            expression.AssertASCII("DCA\0");
            expression.AssertInt32(8);

            byte[] compressed = expression.ReadBytes((int)compressedSize);
            return Oodle26.Decompress(compressed, uncompressedSize);
        }


        private static byte[] DecompressDCXZSTD(BinaryReaderEx expression)
        {   // thanks to ClayAmore for writing a method for this
            expression.AssertASCII("DCX\0");
            expression.AssertInt32(0x11000);
            expression.AssertInt32(0x18);
            expression.AssertInt32(0x24);
            expression.AssertInt32(0x44);
            expression.AssertInt32(0x4C);

            expression.AssertASCII("DCS\0");
            int uncompressedSize = expression.ReadInt32();
            int compressedSize = expression.ReadInt32();

            expression.AssertASCII("DCP\0");
            expression.AssertASCII("ZSTD");
            expression.AssertInt32(0x20);
            expression.AssertByte(0x15);
            //expression.ReadByte(); // compression level
            expression.AssertByte(0);
            expression.AssertByte(0);
            expression.AssertByte(0);
            expression.AssertInt32(0x0);
            expression.AssertByte(0);
            expression.AssertByte(0);
            expression.AssertByte(0);
            expression.AssertByte(0);
            expression.AssertInt32(0x0);
            expression.AssertInt32(0x010100);

            expression.AssertASCII("DCA\0");
            expression.AssertInt32(8);

            byte[] decompressed = SFUtil.ReadZstd(expression, compressedSize); // TODO verify method

            return decompressed;
        }

        #region Public Compress
        /// <summary>
        /// Compress a DCX file to an array of bytes using the specified DCX type.
        /// </summary>
        public static byte[] Compress(byte[] data, Type type)
        {
            BinaryWriterEx writer = new BinaryWriterEx(true);
            Compress(data, writer, type);
            return writer.FinishBytes();
        }

        /// <summary>
        /// Compress a DCX file to the specified path using the specified DCX type.
        /// </summary>
        public static void Compress(byte[] data, Type type, string path)
        {
            using (FileStream stream = File.Create(path))
            {
                BinaryWriterEx writer = new BinaryWriterEx(true, stream);
                Compress(data, writer, type);
                writer.Finish();
            }
        }
        #endregion

        internal static void Compress(byte[] data, BinaryWriterEx writer, Type type)
        {
            writer.BigEndian = true;

            switch (type)
            {
                case Type.Zlib:
                    SFUtil.WriteZlib(writer, 0xDA, data);
                    return;
                case Type.DCP_EDGE:
                    return;
                case Type.DCP_DFLT:
                    CompressDCPDFLT(data, writer);
                    return;
                case Type.DCX_ZSTD:
                    CompressDCPDFLT(data, writer); // workaround TODO add longterm implementation
                    //CompressDCXZSTD(data, writer); // TODO does not work
                    return;
                case Type.DCX_KRAK:
                    CompressDCXKRAK(data, writer);
                    return;
                case Type.DCX_EDGE:
                    CompressDCXEDGE(data, writer);
                    return;
                case Type.Unknown:
                    throw new ArgumentException("You cannot compress a DCX with an unknown type.");
                default:
                    if (type == Type.DCX_DFLT_10000_24_9 || type == Type.DCX_DFLT_10000_44_9
                     || type == Type.DCX_DFLT_11000_44_8 || type == Type.DCX_DFLT_11000_44_9
                     || type == Type.DCX_DFLT_11000_44_9_15)
                        CompressDCXDFLT(data, writer, type);
                    else
                        throw new NotImplementedException("Compression for the given type is not implemented.");
                    break;
            }
        }

        private static void CompressDCPDFLT(byte[] data, BinaryWriterEx writer)
        {
            writer.WriteASCII("DCP\0");
            writer.WriteASCII("DFLT");
            writer.WriteInt32(0x20);
            writer.WriteInt32(0x9000000);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0x00010100);

            writer.WriteASCII("DCS\0");
            writer.WriteInt32(data.Length);
            writer.ReserveInt32("CompressedSize");

            int compressedSize = SFUtil.WriteZlib(writer, 0xDA, data);
            writer.FillInt32("CompressedSize", compressedSize);

            writer.WriteASCII("DCA\0");
            writer.WriteInt32(8);
        }

        private static void CompressDCXEDGE(byte[] data, BinaryWriterEx bw)
        {
            int chunkCount = data.Length / 0x10000;
            if (data.Length % 0x10000 > 0)
                chunkCount++;

            bw.WriteASCII("DCX\0");
            bw.WriteInt32(0x10000);
            bw.WriteInt32(0x18);
            bw.WriteInt32(0x24);
            bw.WriteInt32(0x24);
            bw.WriteInt32(0x50 + chunkCount * 0x10);

            bw.WriteASCII("DCS\0");
            bw.WriteInt32(data.Length);
            bw.ReserveInt32("CompressedSize");

            bw.WriteASCII("DCP\0");
            bw.WriteASCII("EDGE");
            bw.WriteInt32(0x20);
            bw.WriteInt32(0x9000000);
            bw.WriteInt32(0x10000);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0x00100100);

            long dcaStart = bw.Position;
            bw.WriteASCII("DCA\0");
            bw.ReserveInt32("DCASize");
            long egdtStart = bw.Position;
            bw.WriteASCII("EgdT");
            bw.WriteInt32(0x00010100);
            bw.WriteInt32(0x24);
            bw.WriteInt32(0x10);
            bw.WriteInt32(0x10000);
            bw.WriteInt32(data.Length % 0x10000);
            bw.ReserveInt32("EGDTSize");
            bw.WriteInt32(chunkCount);
            bw.WriteInt32(0x100000);

            for (int i = 0; i < chunkCount; i++)
            {
                bw.WriteInt32(0);
                bw.ReserveInt32($"ChunkOffset{i}");
                bw.ReserveInt32($"ChunkSize{i}");
                bw.ReserveInt32($"ChunkCompressed{i}");
            }

            bw.FillInt32("DCASize", (int)(bw.Position - dcaStart));
            bw.FillInt32("EGDTSize", (int)(bw.Position - egdtStart));
            long dataStart = bw.Position;

            int compressedSize = 0;
            for (int i = 0; i < chunkCount; i++)
            {
                int chunkSize = 0x10000;
                if (i == chunkCount - 1)
                    chunkSize = data.Length % 0x10000;

                byte[] chunk;
                using (MemoryStream cmpStream = new MemoryStream())
                using (MemoryStream dcmpStream = new MemoryStream(data, i * 0x10000, chunkSize))
                {
                    DeflateStream dfltStream = new DeflateStream(cmpStream, CompressionMode.Compress);
                    dcmpStream.CopyTo(dfltStream);
                    dfltStream.Close();
                    chunk = cmpStream.ToArray();
                }

                if (chunk.Length < chunkSize)
                    bw.FillInt32($"ChunkCompressed{i}", 1);
                else
                {
                    bw.FillInt32($"ChunkCompressed{i}", 0);
                    chunk = data;
                }

                compressedSize += chunk.Length;
                bw.FillInt32($"ChunkOffset{i}", (int)(bw.Position - dataStart));
                bw.FillInt32($"ChunkSize{i}", chunk.Length);
                bw.WriteBytes(chunk);
                bw.Pad(0x10);
            }

            bw.FillInt32("CompressedSize", compressedSize);
        }

        private static void CompressDCXDFLT(byte[] data, BinaryWriterEx bw, Type type)
        {
            bw.WriteASCII("DCX\0");

            if (type == Type.DCX_DFLT_10000_24_9 || type == Type.DCX_DFLT_10000_44_9)
            {
                bw.WriteInt32(0x10000);
            }
            else if (type == Type.DCX_DFLT_11000_44_8 || type == Type.DCX_DFLT_11000_44_9 || type == Type.DCX_DFLT_11000_44_9_15)
            {
                bw.WriteInt32(0x11000);
            }

            bw.WriteInt32(0x18);
            bw.WriteInt32(0x24);

            if (type == Type.DCX_DFLT_10000_24_9)
            {
                bw.WriteInt32(0x24);
                bw.WriteInt32(0x2C);
            }
            else if (type == Type.DCX_DFLT_10000_44_9 || type == Type.DCX_DFLT_11000_44_8 || type == Type.DCX_DFLT_11000_44_9 || type == Type.DCX_DFLT_11000_44_9_15)
            {
                bw.WriteInt32(0x44);
                bw.WriteInt32(0x4C);
            }

            bw.WriteASCII("DCS\0");
            bw.WriteInt32(data.Length);
            bw.ReserveInt32("CompressedSize");
            bw.WriteASCII("DCP\0");
            bw.WriteASCII("DFLT");
            bw.WriteInt32(0x20);

            if (type == Type.DCX_DFLT_10000_24_9 || type == Type.DCX_DFLT_10000_44_9 || type == Type.DCX_DFLT_11000_44_9 || type == Type.DCX_DFLT_11000_44_9_15)
            {
                bw.WriteByte(9);
            }
            else if (type == Type.DCX_DFLT_11000_44_8)
            {
                bw.WriteByte(8);
            }
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteByte(0);

            bw.WriteInt32(0);

            if (type == Type.DCX_DFLT_11000_44_9_15)
            {
                bw.WriteByte(15);
            }
            else
            {
                bw.WriteByte(0);
            }
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteByte(0);

            bw.WriteInt32(0);
            bw.WriteInt32(0x00010100);
            bw.WriteASCII("DCA\0");
            bw.WriteInt32(8);

            long compressedStart = bw.Position;
            SFUtil.WriteZlib(bw, 0xDA, data);
            bw.FillInt32("CompressedSize", (int)(bw.Position - compressedStart));
        }

        private static void CompressDCXKRAK(byte[] data, BinaryWriterEx bw)
        {
            byte[] compressed = Oodle26.Compress(data, Oodle26.OodleLZ_Compressor.OodleLZ_Compressor_Kraken, Oodle26.OodleLZ_CompressionLevel.OodleLZ_CompressionLevel_Optimal2);

            bw.WriteASCII("DCX\0");
            bw.WriteInt32(0x11000);
            bw.WriteInt32(0x18);
            bw.WriteInt32(0x24);
            bw.WriteInt32(0x44);
            bw.WriteInt32(0x4C);
            bw.WriteASCII("DCS\0");
            bw.WriteUInt32((uint)data.Length);
            bw.WriteUInt32((uint)compressed.Length);
            bw.WriteASCII("DCP\0");
            bw.WriteASCII("KRAK");
            bw.WriteInt32(0x20);
            bw.WriteInt32(0x6000000);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0x10100);
            bw.WriteASCII("DCA\0");
            bw.WriteInt32(8);
            bw.WriteBytes(compressed);
            bw.Pad(0x10);
        }

        private static void CompressDCXZSTD(byte[] data, BinaryWriterEx bw, int compressionLevel = 5)
        {
            byte[] compressed = SFUtil.WriteZstd(data, compressionLevel);
            //TODO revise implementation, currently doesn't seem to work
            bw.WriteASCII("DCX\0");
            bw.WriteInt32(0x11000);
            bw.WriteInt32(0x18);
            bw.WriteInt32(0x24);
            bw.WriteInt32(0x44);
            bw.WriteInt32(0x4C);
            bw.WriteASCII("DCS\0");
            bw.WriteUInt32((uint)data.Length);
            bw.WriteUInt32((uint)compressed.Length);
            bw.WriteASCII("DCP\0");
            bw.WriteASCII("ZSTD");
            bw.WriteInt32(0x20);
            bw.WriteByte((byte)compressionLevel);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0x10100);
            bw.WriteASCII("DCA\0");
            bw.WriteInt32(8);
            bw.WriteBytes(compressed);
            bw.Pad(0x10);
        }

        /// <summary>
        /// Specific compression format used for a certain file.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// DCX type could not be detected.
            /// </summary>
            Unknown,

            /// <summary>
            /// The file is not compressed.
            /// </summary>
            None,

            /// <summary>
            /// Plain zlib-wrapped data; not really DCX, but it's convenient to support it here.
            /// </summary>
            Zlib,

            /// <summary>
            /// DCP header, chunked deflate compression. Used in ACE:R TPFs.
            /// </summary>
            DCP_EDGE,

            /// <summary>
            /// DCP header, deflate compression. Used in DeS test maps.
            /// </summary>
            DCP_DFLT,

            /// <summary>
            /// DCX header, chunked deflate compression. Primarily used in DeS.
            /// </summary>
            DCX_EDGE,

            /// <summary>
            /// DCX header, deflate compression. Primarily used in DS1 and DS2.
            /// </summary>
            DCX_DFLT_10000_24_9,

            /// <summary>
            /// DCX header, deflate compression. Primarily used in BB and DS3.
            /// </summary>
            DCX_DFLT_10000_44_9,

            /// <summary>
            /// DCX header, deflate compression. Used for the backup regulation in DS3 save files.
            /// </summary>
            DCX_DFLT_11000_44_8,

            /// <summary>
            /// DCX header, deflate compression. Used in Sekiro.
            /// </summary>
            DCX_DFLT_11000_44_9,

            /// <summary>
            /// DCX header, deflate compression. Used in the ER regulation.
            /// </summary>
            DCX_DFLT_11000_44_9_15,

            /// <summary>
            /// DCX header, Oodle compression. Used in Sekiro.
            /// </summary>
            DCX_KRAK,

            /// <summary>
            /// DCX header, TODO add the details for KRAK_MAX
            /// </summary>
            DCX_KRAK_MAX,

            /// <summary>
            /// DCX header, ZSTD compression. Used in Elden Ring: Shadow of the Erdtree.
            /// </summary>
            DCX_ZSTD,
        }

        /// <summary>
        /// Standard compression types used by various games; may be cast directly to DCX.Type.
        /// </summary>
        public enum DefaultType
        {
            /// <summary>
            /// Most common compression format for Demon's Souls.
            /// </summary>
            DemonsSouls = Type.DCX_EDGE,

            /// <summary>
            /// Most common compression format for Dark Souls 1.
            /// </summary>
            DarkSouls1 = Type.DCX_DFLT_10000_24_9,

            /// <summary>
            /// Most common compression format for Dark Souls 2.
            /// </summary>
            DarkSouls2 = Type.DCX_DFLT_10000_24_9,

            /// <summary>
            /// Most common compression format for Bloodborne.
            /// </summary>
            Bloodborne = Type.DCX_DFLT_10000_44_9,

            /// <summary>
            /// Most common compression format for Dark Souls 3.
            /// </summary>
            DarkSouls3 = Type.DCX_DFLT_10000_44_9,

            /// <summary>
            /// Most common compression format for Sekiro.
            /// </summary>
            Sekiro = Type.DCX_KRAK,

            /// <summary>
            /// Most common compression format for Elden Ring.
            /// </summary>
            EldenRing = Type.DCX_KRAK,
        }
    }
}
