using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

    public ListaProduto()
    {
        InitializeComponent();

        lst_produtos.ItemsSource = lista;

        picker_filtro_categoria.SelectedIndex = 0;
    }

    protected override async void OnAppearing()
    {
        try
        {

            base.OnAppearing();

            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Navigation.PushAsync(new Views.NovoProduto());

        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");

        }
    }
    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string q = e.NewTextValue ?? string.Empty;

            lista.Clear();

            List<Produto> tmp;

            if (string.IsNullOrWhiteSpace(q))
            {
                tmp = await App.Db.GetAll();
            }
            else
            {
                tmp = await App.Db.Search(q);
            }

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);

        string msg = $"O total é {soma:C}";

        DisplayAlert("Total dos Produtos", msg, "OK");
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is not MenuItem selecionado)
                return;

            if (selecionado.BindingContext is not Produto p)
                return;

            bool confirm = await DisplayAlert("Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }


    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Produto p = e.SelectedItem as Produto;

            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });
        }


        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {

            base.OnAppearing();

            lst_produtos.IsRefreshing = true;

            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");

        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async void ToolbarItem_Clicked_2(object sender, EventArgs e)
    {
        try
        {
            List<Produto> produtos = await App.Db.GetAll();

            double alimentos = produtos
                .Where(p => p.Categoria == "Alimentos")
                .Sum(p => p.Total);

            double higiene = produtos
                .Where(p => p.Categoria == "Higiene")
                .Sum(p => p.Total);

            double limpeza = produtos
                .Where(p => p.Categoria == "Limpeza")
                .Sum(p => p.Total);

            double outros = produtos
                .Where(p => p.Categoria == "Outros")
                .Sum(p => p.Total);

            string relatorio =
                $"Alimentos: {alimentos:C}\n" +
                $"Higiene: {higiene:C}\n" +
                $"Limpeza: {limpeza:C}\n" +
                $"Outros: {outros:C}";

            await DisplayAlert(
                "Relatório por Categoria",
                relatorio,
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
    private async void picker_filtro_categoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (picker_filtro_categoria.SelectedItem == null)
                return;

            string categoria = picker_filtro_categoria.SelectedItem.ToString();

            lista.Clear();

            List<Produto> produtos = await App.Db.GetAll();

            foreach (Produto p in produtos)
            {
                if (categoria == "Todas" || p.Categoria == categoria)
                {
                    lista.Add(p);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}    