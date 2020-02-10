using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Java.IO;
using System;
using System.Threading.Tasks;
using Uno.UI;

namespace GyumeshiPolice
{
    public static class FileHelper
    {
        public static async Task<string> GetPathFromUri(Android.Net.Uri uri)
        {
            var isKitKat = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;

            // DocumentProvider
            if (isKitKat && DocumentsContract.IsDocumentUri(ContextHelper.Current, uri))
            {
                // ExternalStorageProvider
                if (IsExternalStorageDocument(uri))
                {
                    var docId = DocumentsContract.GetDocumentId(uri);
                    var split = docId.Split(':');
                    var type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                    }
                }
                // DownloadsProvider
                else if (IsDownloadsDocument(uri))
                {

                    var id = DocumentsContract.GetDocumentId(uri);

                    var contentUriPrefixesToTry = new string[]{
                        "content://downloads/public_downloads",
                        "content://downloads/my_downloads",
                        "content://downloads/all_downloads"
                    };

                    string path = null;
                    foreach (var contentUriPrefix in contentUriPrefixesToTry)
                    {
                        var contentUri = ContentUris.WithAppendedId(Android.Net.Uri.Parse(contentUriPrefix), long.Parse(id));
                        try
                        {
                            path = GetDataColumn(contentUri, null, null);
                            if (path != null)
                            {
                                return path;
                            }
                        }
                        catch { }
                    }

                    var fileName = GetFileName(uri);
                    var cacheDir = GetDocumentCacheDir();
                    var file = GenerateFileName(fileName, cacheDir);

                    if (file != null)
                    {
                        path = file.AbsolutePath;
                        await SaveFileFromUri(uri, path);
                    }

                    // last try
                    if (string.IsNullOrEmpty(path))
                        return Android.OS.Environment.ExternalStorageDirectory.ToString() + "/Download/" + GetFileName(uri);

                    return path;
                }
                // MediaProvider
                else if (IsMediaDocument(uri))
                {
                    var docId = DocumentsContract.GetDocumentId(uri);
                    var split = docId.Split(':');
                    var type = split[0];

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;
                    }

                    var selection = "_id=?";
                    var selectionArgs = new string[] { split[1] };

                    return GetDataColumn(contentUri, selection, selectionArgs);
                }
            }
            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return GetDataColumn(uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }

        private static string GetDataColumn(Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            ICursor cursor = null;
            var column = "_data";
            string[] projection = { column };

            try
            {
                cursor = ContextHelper.Current.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    var index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(index);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.Close();
            }
            return null;
        }

        private static bool IsExternalStorageDocument(Android.Net.Uri uri) => "com.android.externalstorage.documents".Equals(uri.Authority);

        private static bool IsDownloadsDocument(Android.Net.Uri uri) => "com.android.providers.downloads.documents".Equals(uri.Authority);

        private static bool IsMediaDocument(Android.Net.Uri uri) => "com.android.providers.media.documents".Equals(uri.Authority);

        private static byte[] ReadFile(string file)
        {
            try
            {
                return ReadFile(new Java.IO.File(file));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                return new byte[0];
            }
        }

        private static byte[] ReadFile(Java.IO.File file)
        {
            // Open file
            var f = new RandomAccessFile(file, "r");

            try
            {
                // Get and check length
                var longlength = f.Length();
                var length = (int)longlength;

                if (length != longlength)
                    throw new Java.IO.IOException("Filesize exceeds allowed size");
                // Read file and return data
                var data = new byte[length];
                f.ReadFully(data);
                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                return new byte[0];
            }
            finally
            {
                f.Close();
            }
        }

        private static string GetFileName(Android.Net.Uri uri)
        {
            var result = string.Empty;

            if (uri.Scheme.Equals("content"))
            {
                var cursor = ContextHelper.Current.ContentResolver.Query(uri, null, null, null, null);
                try
                {
                    if (cursor != null && cursor.MoveToFirst())
                        result = cursor.GetString(cursor.GetColumnIndex(OpenableColumns.DisplayName));
                }
                finally
                {
                    cursor.Close();
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                result = uri.Path;
                var cut = result.LastIndexOf('/');

                if (cut != -1)
                    result = result.Substring(cut + 1);
            }

            return result;
        }

        private static Java.IO.File GetDocumentCacheDir()
        {
            var dir = new Java.IO.File(ContextHelper.Current.CacheDir, "documents");

            if (!dir.Exists())
                dir.Mkdirs();

            return dir;
        }

        private static Java.IO.File GenerateFileName(string name, Java.IO.File directory)
        {
            if (name == null) return null;

            var file = new Java.IO.File(directory, name);

            if (file.Exists())
            {
                var dotIndex = name.LastIndexOf('.');
                if (dotIndex > 0)
                {
                    name = name.Substring(0, dotIndex);
                    var extension = name.Substring(dotIndex);
                    var index = 0;
                    while (file.Exists())
                    {
                        index++;
                        name = $"{name}({index}){extension}";
                        file = new Java.IO.File(directory, name);
                    }
                }
            }

            try
            {
                if (!file.CreateNewFile())
                    return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                return null;
            }

            return file;
        }

        private async static Task SaveFileFromUri(Android.Net.Uri uri, string destinationPath)
        {
            var stream = ContextHelper.Current.ContentResolver.OpenInputStream(uri);
            BufferedOutputStream bos = null;

            try
            {
                bos = new BufferedOutputStream(System.IO.File.OpenWrite(destinationPath));

                var bufferSize = 1024 * 4;
                var buffer = new byte[bufferSize];

                while (true)
                {
                    var len = await stream.ReadAsync(buffer, 0, bufferSize);
                    if (len == 0)
                        break;
                    await bos.WriteAsync(buffer, 0, len);
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
            finally
            {
                try
                {
                    if (stream != null) stream.Close();
                    if (bos != null) bos.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                }
            }
        }
    }
}