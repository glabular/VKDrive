﻿using Ionic.Zip;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using System.Text;
using VKDrive.API.Interfaces;

namespace VKDrive.API.Services;

public class ZipService : IArchiveService
{
    public void CompressFile(string fileToCompress, string outputArchive, string password)
    {
        // TODO Hardcoded!
        var level = Ionic.Zlib.CompressionLevel.Default;

        using var zip = new ZipFile()
        {
            UseZip64WhenSaving = Zip64Option.Always,
            Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256,
            Password = password,
            CompressionLevel = level,
            AlternateEncoding = Encoding.UTF8,
            AlternateEncodingUsage = ZipOption.AsNecessary
        };

        zip.AddFile(fileToCompress, string.Empty);
        zip.Save(outputArchive);
    }

    public void CompressFolder(
        string folderToCompress, 
        string outputArchive, 
        string password)
    {
        // TODO Hardcoded!
        var level = Ionic.Zlib.CompressionLevel.Default;

        using var zip = new ZipFile()
        {
            UseZip64WhenSaving = Zip64Option.Always,
            Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256,
            Password = password,
            CompressionLevel = level,
            AlternateEncoding = Encoding.UTF8,
            AlternateEncodingUsage = ZipOption.AsNecessary
        };

        zip.AddDirectory(folderToCompress, Path.GetFileName(folderToCompress));
        zip.Save(outputArchive);
    }

    public void DecompressArchive(string archiveToDecompress, string outputFolder, string password)
    {
        using var zip = Ionic.Zip.ZipFile.Read(archiveToDecompress);
        zip.Password = password;
        zip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
        try
        {
            zip.ExtractAll(outputFolder, ExtractExistingFileAction.OverwriteSilently);
        }
        catch (BadPasswordException e)
        {
            // TODO:
            throw new Exception(e.Message);
        }
        catch (IOException e)
        {
            // TODO:
            throw new Exception(e.Message);
        }
    }
}