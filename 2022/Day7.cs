namespace AOC.AOC2022;

public class Day7 : Day<Day7.Dir>
{
    protected override string? SampleRawInput { get => "$ cd /\n$ ls\ndir a\n14848514 b.txt\n8504156 c.dat\ndir d\n$ cd a\n$ ls\ndir e\n29116 f\n2557 g\n62596 h.lst\n$ cd e\n$ ls\n584 i\n$ cd ..\n$ cd ..\n$ cd d\n$ ls\n4060174 j\n8033020 d.log\n5626152 d.ext\n7214296 k"; }

    public class Dir
    {
        public required string AbsolutePath { get; set; }
        public required string RelativePath { get; set; }

        public Dir? Parent { get; set; }            // for cd .. navigation

        public required List<Dir> SubDirs { get; set; }
        public required List<FileInfo> Files { get; set; }

        public int TotalSize;                   // total size of all files in this directory and all subdirectories
    }

    public class FileInfo
    {
        public required string Name { get; set; }
        public required int Size { get; set; }
    }

    protected override Answer Part1()
    {
        // PrintDirectory(Input, 0);
        var dirs = FindAll(Input, p => p.TotalSize <= 100000);

        return dirs.Sum(p => p.TotalSize);
    }

    protected override Answer Part2()
    {
        var neededSpace = 30000000 - (70000000 - Input.TotalSize);
        var dir = FindAll(Input, p => p.TotalSize >= neededSpace).OrderBy(p => p.TotalSize).First();

        return dir.TotalSize;
    }

    private List<Dir> FindAll(Dir currentDir, Func<Dir, bool> predicate)
    {
        List<Dir> result = new();

        if (predicate(currentDir))
        {
            result.Add(currentDir);
        }

        foreach (var subDir in currentDir.SubDirs)
        {
            result.AddRange(FindAll(subDir, predicate));
        }

        return result;
    }

    private void PrintDirectory(Dir dir, int indent)
    {
        Console.WriteLine($"{new string(' ', indent)}{dir.RelativePath}");
        foreach (var file in dir.Files)
        {
            Console.WriteLine($"{new string(' ', indent + 2)}{file.Name} {file.Size}");
        }

        foreach (var subDir in dir.SubDirs)
        {
            PrintDirectory(subDir, indent + 2);
        }
    }

    protected override Dir Parse(RawInput input)
    {
        var lines = input.Lines().ToArray();

        // create the top-level directory, we know the first command is "cd /"
        var result = new Dir()
        {
            AbsolutePath = "/",
            RelativePath = "/",
            SubDirs = new List<Dir>(),
            Files = new List<FileInfo>()
        };

        Dir currentDir = result;

        foreach (var line in lines)
        {
            if (line.StartsWith("$ cd "))
            {
                var path = line[5..];
                if (path == "..")
                {
                    currentDir = currentDir.Parent!;
                }
                else if (path == "/")
                {
                    currentDir = result;
                }
                else if (!currentDir.SubDirs.Exists(p => p.RelativePath == path))           // create if needed
                {
                    var newDir = new Dir()
                    {
                        AbsolutePath = currentDir.AbsolutePath + path + "/",
                        RelativePath = path,
                        Parent = currentDir,
                        SubDirs = new List<Dir>(),
                        Files = new List<FileInfo>()
                    };
                    currentDir.SubDirs.Add(newDir);
                    currentDir = newDir;
                }
                else
                {
                    // subdirectory exists, change to it
                    currentDir = currentDir.SubDirs.First(p => p.RelativePath == path);
                }
            }
            else if (line.StartsWith("$ ls"))
            {
                // do nothing, file/subdir listing of current directory to follow.
            }
            else
            {
                // either "dir [name]" or "[size] [filename]"
                var parts = line.Split(' ').ToArray();
                if (parts[0] == "dir")
                {
                    // if this subdirectory doesn't yet exist, create it (like with cd above)
                    if (!currentDir.SubDirs.Exists(p => p.RelativePath == parts[1]))
                    {
                        var dir = new Dir()
                            {
                                AbsolutePath = currentDir.AbsolutePath + parts[1] + "/",
                                RelativePath = parts[1],
                                Parent = currentDir,
                                SubDirs = new List<Dir>(),
                                Files = new List<FileInfo>()
                            };
                        currentDir.SubDirs.Add(dir);
                    }
                    else
                    {
                        Console.WriteLine("ERROR: found a subdir " + parts[1] + " that already exists at " + currentDir.AbsolutePath);
                    }
                }
                else
                {
                    var newFile = new FileInfo()
                    {
                        Name = parts[1],
                        Size = int.Parse(parts[0])
                    };
                    currentDir.Files.Add(newFile);
                }
            }
        }

        CalculateSizes(result);          // total sizes are needed for both problem parts.

        return result;
    }

    private void CalculateSizes(Dir currentDir)
    {
        currentDir.TotalSize = currentDir.Files.Sum(p => p.Size);
        foreach (var subDir in currentDir.SubDirs)
        {
            CalculateSizes(subDir);
            currentDir.TotalSize += subDir.TotalSize;
        }
    }

}