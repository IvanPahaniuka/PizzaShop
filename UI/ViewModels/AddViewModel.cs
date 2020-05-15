using Pizza.Models.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.UI.ViewModels
{
    public class AddViewModel : INotifyPropertyChanged
    {
        private IEnumerable<PizzaBuilder> builders;
        private PizzaBuilder selectedBuilder;

        public IEnumerable<PizzaBuilder> Builders
        {
            get => builders;
            private set
            {
                builders = value;
                OnPropertyChanged();
            }
        }
        public PizzaBuilder SelectedBuilder
        {
            get => selectedBuilder;
            set
            {
                selectedBuilder = value;
                OnPropertyChanged();
            }
        }

        public AddViewModel()
        {
            Builders = new PizzaBuilder[]{
                new PizzaBuilder(
                    new Models.Pizza{ 
                        Name = "Стандарт", 
                        Box = new Models.Box(new Models.Size(10), Models.Box.BoxColor.Pink), 
                        Cost = 9.7M }),
                new PizzaBuilder(
                    new Models.CheesePizza{
                        Name = "Сырная",
                        Box = new Models.Box(new Models.Size(10), Models.Box.BoxColor.Yellow),
                        Cost = 10.1M,
                        CheeseMass = 1.1f
                        }),
                new PizzaBuilder(
                    new Models.PineapplePizza{
                        Name = "Ананасовая",
                        Box = new Models.Box(new Models.Size(10), Models.Box.BoxColor.Red),
                        Cost = 10.5M,
                        CheeseMass = 1.0f,
                        PineappleMass = 2.1f
                        }),
                new PizzaBuilder(
                    new Models.PepperoniPizza{
                        Name = "Пепперони",
                        Box = new Models.Box(new Models.Size(10), Models.Box.BoxColor.Red),
                        Cost = 10.5M,
                        CheeseMass = 1.0f,
                        SausageMass = 1.2f
                        }),
                new PizzaBuilder(
                    new Models.ItalianPizza{
                        Name = "Итальянская",
                        Box = new Models.Box(new Models.Size(10), Models.Box.BoxColor.Red),
                        Cost = 10.5M,
                        CheeseMass = 1.0f,
                        SausageMass = 1.2f,
                        VegetablesMass = 1.6f
                        })

                };
            SelectedBuilder = Builders.ElementAtOrDefault(0);
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
