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
            var OF = new OpenFileDialog{
                Filter = string.Join("|", model.Filters.Select(f => f.Filter))
                };

            if (OF.ShowDialog() == true)
            {
                using (var FS = new FileStream(OF.FileName, FileMode.Open, FileAccess.Read))
                {
                    //try
                    //{
                        model.Items = 
                            model.Filters[OF.FilterIndex-1].Serializator.Deserialize<ObservableCollection<object>>(FS);
                    //}
                    //catch(Exception err)
                    //{
                    //    MessageBox.Show($"Ошибка открытия:\n{err.Message}", "Ошибка");
                    //}

                    model.LastFilter = model.Filters[OF.FilterIndex-1];
                    model.LastFile = OF.FileName;
                }
            }
        }

        private void CommandBinding_Save(object sender, ExecutedRoutedEventArgs e)
        {
            if (model.LastFile == null || model.LastFilter == null)
            {
                ApplicationCommands.SaveAs.Execute(null, this);
            }
            else
            {
                using (var FS = new FileStream(model.LastFile, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        model.LastFilter.Serializator.Serialize(FS, model.Items);
                        //var dict = new Dictionary<Models.Pizza, int>();
                        //dict.Add(new Models.Pizza(), 123);
                        //dict.Add(new Models.CheesePizza(), 1223);
                        //model.LastFilter.Serializator.Serialize(FS, dict);
                    }
                    catch(Exception err)
                    {
                        MessageBox.Show($"Ошибка сохранения:\n{err.Message}", "Ошибка");
                    }
                }

                MessageBox.Show($"Информация успешно сохранена в файл {model.LastFile}!", "Сохранение");
            }
        }

        private void CommandBinding_SaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            var SF = new SaveFileDialog{
                Filter = string.Join("|", model.Filters.Select(f => f.Filter))
                };

            if (SF.ShowDialog() == true)
            {
                model.LastFile = SF.FileName;
                model.LastFilter = model.Filters[SF.FilterIndex-1];
                ApplicationCommands.Save.Execute(null, this);
            }
        }
    }
}
