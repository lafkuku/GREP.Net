using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;

/// <summary>
/// Grabbed from: http://stackoverflow.com/questions/13130052/directoryinfo-enumeratefiles-causes-unauthorizedaccessexception-and-other
/// 
/// Kudos buddy! 
/// 
/// </summary>
public class FileSystemEnumerable : IEnumerable<FileSystemInfo>
{
    private Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly DirectoryInfo _root;
    private readonly IList<string> _patterns;
    private readonly SearchOption _option;

    public FileSystemEnumerable(DirectoryInfo root, string pattern, SearchOption option)
    {
        _root = root; 
        _patterns = new List<string> { pattern };
        _option = option;
    }

    public FileSystemEnumerable(DirectoryInfo root, IList<string> patterns, SearchOption option)
    {
        _root = root;
        _patterns = patterns;
        _option = option;
    }

    public IEnumerator<FileSystemInfo> GetEnumerator()
    {
        if (_root == null || !_root.Exists)
            yield break;

        IEnumerable<FileSystemInfo> matches = new List<FileSystemInfo>();
        try
        {
            _logger.Debug("Attempting to enumerate '{0}'", _root.FullName);
            foreach (var pattern in _patterns)
            {
                _logger.Debug("Using pattern '{0}'", pattern);
                matches = matches.Concat(_root.EnumerateDirectories(pattern, SearchOption.TopDirectoryOnly))
                                 .Concat(_root.EnumerateFiles(pattern, SearchOption.TopDirectoryOnly));
            }
        }
        catch (UnauthorizedAccessException)
        {
            _logger.Warn("Unable to access '{0}'. Skipping...", _root.FullName);
            yield break;
        }
        catch (PathTooLongException ptle)
        {
            _logger.Warn(string.Format(@"Could not process path '{0}\{1}'.", _root.Parent.FullName, _root.Name), ptle);
            yield break;
        }

        _logger.Debug("Returning all objects that match the pattern(s) '{0}'", string.Join(",", _patterns));
        foreach (var file in matches)
        {
            yield return file;
        }

        if (_option == SearchOption.AllDirectories)
        {
            _logger.Debug("Enumerating all child directories.");
            foreach (var dir in _root.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
            {
                _logger.Debug("Enumerating '{0}'", dir.FullName);
                var fileSystemInfos = new FileSystemEnumerable(dir, _patterns, _option);
                foreach (var match in fileSystemInfos)
                {
                    yield return match;
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}