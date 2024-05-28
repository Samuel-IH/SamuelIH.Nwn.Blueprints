using System;
using System.Collections.Generic;
using System.IO;

namespace SamuelIH.Nwn.Blueprints
{
    public interface IBlueprintProvider
    {
        string Namespace { get; }
        IEnumerable<string> GetBlueprints();
    }

    public abstract class BlueprintProvider : IBlueprintProvider
    {
        public abstract string Namespace { get; }

        public virtual IEnumerable<string> GetBlueprints()
        {
            var dirs = GetBlueprintDirectories();
            var files = new List<string>();

            foreach (var dir in dirs)
            {
                if (!Directory.Exists(dir)) continue;

                files.AddRange(Directory.GetFiles(dir, "*.yaml", SearchOption.AllDirectories));
                files.AddRange(Directory.GetFiles(dir, "*.yml", SearchOption.AllDirectories));
            }

            return files;
        }

        /// <summary>
        ///     Get the directories that contain blueprints for this provider.
        ///     Unless GetBlueprints() is overridden, blueprints will be extracted recursively from these directories, and their
        ///     subdirectories.
        /// </summary>
        /// <returns>A list of all the directories to start searching for blueprints in.</returns>
        /// <exception cref="Exception"></exception>
        protected virtual IEnumerable<string> GetBlueprintDirectories()
        {
            throw new Exception("BlueprintProvider.GetBlueprintDirectories() not implemented");
        }
    }
}