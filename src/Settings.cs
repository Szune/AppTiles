using AppTiles.Tiles;
using AppTiles.Windows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace AppTiles
{
    public class Settings
    {
        static Settings()
        {
            if (TryLoadSettings(out var loadedSettings) && loadedSettings.Tiles?.Count > 0)
            {
                Current = loadedSettings;
            }
            else
            {
                Current = new Settings(true);
            }
        }

        public Settings()
        {
            IsDefault = false;
        }

        public Settings(bool isDefault)
        {
            IsDefault = isDefault;
        }

        public static readonly Settings Current;

        public const int DefaultColumnAmount = 2;
        public const int DefaultRowAmount = 2;
        public const int DefaultWidth = 350;
        public const int DefaultHeight = 250;


        public static TileCollectionWindow MainWindow { get; private set; }
        public static bool IsChanged { get; private set; }

        public TileCollection Tiles { get; private set; } = new TileCollection();
        [JsonIgnore]
        public bool IsDefault { get; }

        private const string SettingsName = "settings.json";
        private const string SettingsBackupName = "settings.json.bak";

        public static void SetMainWindow(TileCollectionWindow window)
        {
            MainWindow = window;
        }

        public void Save()
        {
            try
            {
                if (File.Exists(SettingsName))
                {
                    if (File.Exists(SettingsBackupName))
                    {
                        File.Delete(SettingsBackupName);
                    }
                    File.Move(SettingsName, SettingsBackupName);
                }

                var json = JsonConvert.SerializeObject(this, Formatting.Indented,
                    new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto});

                using (var stream = new StreamWriter(SettingsName))
                {
                    stream.Write(json);
                }

                IsChanged = false;
                MessageBox.Show("Settings have been saved.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save {SettingsName}:{Environment.NewLine}\"{ex.Message}\"", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool TryLoadSettings(out Settings settings)
        {
            try
            {
                using (var stream = new StreamReader(SettingsName))
                {
                    settings = JsonConvert.DeserializeObject<Settings>(stream.ReadToEnd(),
                        new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto});
                }

                return settings != null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load '{SettingsName}', using default settings.{Environment.NewLine}\"{ex.Message}\"", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                settings = null;
                return false;
            }
        }

        public static void SetChanged()
        {
            IsChanged = true;
        }

        public void Clean()
        {
            var collectionsContainingUnusedTiles = new List<TileCollection>();
            var tileQueue = new Queue<TileCollection>();
            tileQueue.Enqueue(Tiles);
            while(tileQueue.Any())
            {
                var current = tileQueue.Dequeue();
                if(current.Any(t => t.Column > current.Columns - 1 || t.Row > current.Rows - 1))
                    collectionsContainingUnusedTiles.Add(current);

                var containerTiles = current.OfType<ContainerTile>().Where(t => t.Children != null).Select(t => t.Children);
                foreach(var container in containerTiles)
                    tileQueue.Enqueue(container);
            }

            foreach(var collection in collectionsContainingUnusedTiles)
                collection.Clean();

        }

        public void ResetTiles()
        {
            Tiles = new TileCollection();
        }

        /// <summary>
        /// Traverses all available collections of type <see cref="TileCollection"/> to find and replace the specified tile in its parent collection.
        /// </summary>
        /// <param name="tileToBeReplaced">The tile that is being replaced.</param>
        /// <param name="tileToReplaceWith">The tile that will replace the sought after tile.</param>
        public static void ReplaceTile(ITile tileToBeReplaced, ITile tileToReplaceWith)
        {
            var currentContainersCollection = new Queue<TileCollection>();
            currentContainersCollection.Enqueue(Current.Tiles);

            while (currentContainersCollection.Any())
            {
                var currentCollection = currentContainersCollection.Dequeue();
                var index = currentCollection.IndexOf(tileToBeReplaced);
                if (index != -1)
                {
                    currentCollection.Insert(index, tileToReplaceWith);
                    currentCollection.RemoveAt(index+1);
                    SetChanged();
                    return;
                }

                var containersInCollection =
                    currentCollection.OfType<ContainerTile>();

                foreach(var container in containersInCollection)
                    currentContainersCollection.Enqueue(container.Children);
            }

            // none found, throw exception
            throw new InvalidOperationException($"Tile with id '{tileToBeReplaced.Id}' and text '{tileToBeReplaced.Text}' does not exist in any {typeof(TileCollection)}.");
        }

        //TODO add search functionality to find specific tiles

    }
}
