using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Pizza.Serialization;
using System;
using System.Linq;
using System.IO;
using Pizza.UI.ViewModels;
using System.Collections.ObjectModel;
using Pizza.Plugin;

namespace Pizza.UI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private void CommandBinding_Add(object sender, ExecutedRoutedEventArgs e)
        {
            var addWindow = new AddWindow();
            if (addWindow.ShowDialog() == true)
            {
                model.AddItem(addWindow.SelectedBuilder);
            }
        }

        private void CommandBinding_Delete(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить выбранный элемент?", "Удаление",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                model.RemoveItem(model.SelectedItem);
        }

        private void CommandBinding_Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = model.SelectedItem != null;
        }

        private void CommandBinding_Open(object sender, ExecutedRoutedEventArgs e)
        {
            var OF = new OpenFileDialog
            {
                Filter = string.Join("|", model.Serializers.Select(f => f.Filter))
            };

            if (OF.ShowDialog() == true)
            {
                using (var FS = new FileStream(OF.FileName, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        model.Items =
                            model.Serializers[OF.FilterIndex - 1].Serializer.Deserialize<ObservableCollection<object>>(FS);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show($"Ошибка открытия:\n{err.Message}", "Ошибка");
                    }

                    model.LastSerializer = model.Serializers[OF.FilterIndex - 1].Serializer;
                    model.LastFile = OF.FileName;
                }
            }
        }

        private void CommandBinding_Save(object sender, ExecutedRoutedEventArgs e)
        {
            var pluginSerialzier = model.LastSerializer as PluginSerializer;
            bool isBadPluginSerialzier = 
                pluginSerialzier != null && 
                (pluginSerialzier.SelectedPlugin == null || 
                 pluginSerialzier.SelectedSerializer == null);

            if (model.LastFile == null || model.LastSerializer == null || isBadPluginSerialzier)
            {
                ApplicationCommands.SaveAs.Execute(null, this);
            }
            else
            {
                using (var FS = new FileStream(model.LastFile, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        model.LastSerializer.Serialize(FS, model.Items);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show($"Ошибка сохранения:\n{err.Message}", "Ошибка");
                    }
                }

                MessageBox.Show($"Информация успешно сохранена в файл {model.LastFile}!", "Сохранение");
            }
        }

        private void CommandBinding_SaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            var SF = new SaveFileDialog
            {
                Filter = string.Join("|", model.Serializers.Select(f => f.Filter))
            };

            if (SF.ShowDialog() == true)
            {
                model.LastFile = SF.FileName;
                model.LastSerializer = model.Serializers[SF.FilterIndex - 1].Serializer;

                if (model.LastSerializer is PluginSerializer)
                {
                    var FP = CreateFilePluginWindow();
                    if (FP.ShowDialog() == true)
                    {
                        var serialzier = model.LastSerializer as PluginSerializer;
                        serialzier.SelectedSerializer = FP.SelectedSerializer;
                        serialzier.SelectedPlugin = FP.SelectedPlugin;
                        ApplicationCommands.Save.Execute(null, this);
                    }
                }
                else
                {
                    ApplicationCommands.Save.Execute(null, this);
                }
            }
        }

        private FilePluginWindow CreateFilePluginWindow()
        {
            var serialziersDescription = model.Serializers
                        .Select(s => new ValueDescription<ISerializer> { ValueTyped = s.Serializer, Description = s.Name })
                        .ToArray();

            var plugins = model.Serializers
                .Select(s => s.Serializer)
                .OfType<PluginSerializer>()
                .Single().Plugins;
            var pluginsDescription = plugins
                .Select(p => new ValueDescription<IDataPlugin>
                {
                    ValueTyped = p,
                    Description = p.GetType().Name.Replace("Plugin", "")
                })
                .ToArray();


            return new FilePluginWindow(serialziersDescription, pluginsDescription);
        }
    }
}
