﻿using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TJ.Apresentacao;
using TJ.Apresentacao.InterfacesApp;
using TJ.Dominio.Entidades;
using static TJ.View.Enumeracoes;

namespace TJ.View
{
    /// <summary>
    /// Interaction logic for winCumprimento.xaml
    /// </summary>
    public partial class winCadEdCumprimento : Window
    {
        public class cumprimentoTela
        {
            public string Dia { get; set; }
            public TimeSpan HorarioEntrada { get; set; }
            public TimeSpan HorarioSaida { get; set; }
            public TimeSpan HorarioEntradaAlmoco { get; set; }
            public TimeSpan HorarioSaidaAlmoco { get; set; }
            public TimeSpan DiferencaHoras { get; set; }
        }

        private Sentenciado sentenciadoSelecionado;
        private Usuario usuarioLogado;
        private CumprimentoMes cumprimentoMesSelecionado;
        private List<cumprimentoTela> cumprimentosTela = new List<cumprimentoTela>();
        private TimeSpan tempoCumprido;
        private UCCumprimentoPSCLista pai;

        public winCadEdCumprimento(Usuario usuario, Sentenciado sentenciado, UCCumprimentoPSCLista pai)
        {
            usuarioLogado = usuario;
            sentenciadoSelecionado = sentenciado;
            this.pai = pai;
            InitializeComponent();
            CarregarTela();
        }

        public winCadEdCumprimento(Usuario usuario, Sentenciado sentenciado, UCCumprimentoPSCLista pai, CumprimentoMes cumprimentoMes)
        {
            usuarioLogado = usuario;
            sentenciadoSelecionado = sentenciado;
            this.pai = pai;
            using (IAppServiceCumprimentoMes serviceCumprimentoMes = MinhaNinject.Kernel.Get<IAppServiceCumprimentoMes>())
            {
                cumprimentoMesSelecionado = serviceCumprimentoMes.RetornaPorId(cumprimentoMes.Id);
            }
            InitializeComponent();
            CarregarTela();

            calcularTempoCumprido();
            lblTempoCumprido.Content = tempoCumprido.Minutes > 0 ? String.Format("Foram cumpridas {0}h{1}min no total.", tempoCumprido.Hours, tempoCumprido.Minutes) : String.Format("Foram cumpridas {0}h no total.", tempoCumprido.Hours);
            this.Title = "Alteração de cumprimento";
            btnGravar.Content = "Alterar";

        }

        private void CarregarTela()
        {
            tblNome.Text = sentenciadoSelecionado.Nome;
            cbxMes.ItemsSource = Enum.GetValues(typeof(Meses));

            using (IAppServiceSentenciadoEntidade serviceSentenciadoEntidade = MinhaNinject.Kernel.Get<IAppServiceSentenciadoEntidade>())
            {
                cbxInstituicao.ItemsSource = (serviceSentenciadoEntidade.RetornarPorSentenciado(sentenciadoSelecionado.Id).Where(se => se.DataFim == null) as IEnumerable<SentenciadoEntidade>);
            }
            cbxInstituicao.DisplayMemberPath = "Entidade.Nome";

            if (cumprimentoMesSelecionado == null)
            {
                int i = 1;
                while (i <= 31)
                {
                    cumprimentoTela cumprimento = new cumprimentoTela();
                    cumprimento.Dia = i.ToString("D2");
                    cumprimentosTela.Add(cumprimento);
                    i++;
                }
                dgvCumprimento.ItemsSource = cumprimentosTela;
            }
            else
            {
                for (int b = 0; b < cbxInstituicao.Items.Count; b++)
                {
                    if ((cbxInstituicao.Items[b] as SentenciadoEntidade).Id == cumprimentoMesSelecionado.SentenciadoEntidadeId)
                    {
                        cbxInstituicao.SelectedItem = cbxInstituicao.Items[b];
                        b = cbxInstituicao.Items.Count;
                    }
                }

                for (int b = 0; b < cbxMes.Items.Count; b++)
                {
                    if (cbxMes.Items[b].ToString() == (cumprimentoMesSelecionado.Mes))
                    {
                        cbxMes.SelectedItem = cbxMes.Items[b];
                        b = cbxMes.Items.Count;
                    }
                }
                tbxAno.Text = cumprimentoMesSelecionado.Ano.ToString();

                int i = 0;
                while (i < cumprimentoMesSelecionado.Cumprimentos.Count())
                {
                    cumprimentoTela cumprimento = new cumprimentoTela();
                    cumprimento.HorarioEntrada = cumprimentoMesSelecionado.Cumprimentos.ElementAt(i).HorarioEntrada;
                    cumprimento.HorarioEntradaAlmoco = cumprimentoMesSelecionado.Cumprimentos.ElementAt(i).HorarioEntradaAlmoco;
                    cumprimento.HorarioSaida = cumprimentoMesSelecionado.Cumprimentos.ElementAt(i).HorarioSaida;
                    cumprimento.HorarioSaidaAlmoco = cumprimentoMesSelecionado.Cumprimentos.ElementAt(i).HorarioSaidaAlmoco;
                    cumprimento.DiferencaHoras = cumprimentoMesSelecionado.Cumprimentos.ElementAt(i).DiferencaHoras;
                    i++;
                    cumprimento.Dia = i.ToString("D2");
                    cumprimentosTela.Add(cumprimento);
                }
                while (i < 31)
                {
                    cumprimentoTela cumprimento = new cumprimentoTela();
                    i++;
                    cumprimento.Dia = i.ToString("D2");
                    cumprimentosTela.Add(cumprimento);
                }
                dgvCumprimento.ItemsSource = cumprimentosTela;
            }
        }

        private void dgvCumprimento_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                if (e.OriginalSource is TextBox)
                {
                    if ((e.OriginalSource as TextBox).Text.Length == 2)
                    {
                        (e.OriginalSource as TextBox).Text = (e.OriginalSource as TextBox).Text + ":";
                        (e.OriginalSource as TextBox).CaretIndex = 3;
                    }
                }
            }
        }

        private void dgvCumprimento_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                if (e.OriginalSource is TextBox)
                {
                    if ((e.OriginalSource as TextBox).Text.Length >= 5)
                    {
                        if (dgvCumprimento.SelectedIndex < 30)
                        {
                            if (dgvCumprimento.CurrentColumn.DisplayIndex == 2 || dgvCumprimento.CurrentColumn.DisplayIndex == 4)
                            {
                                dgvCumprimento.SelectedIndex = dgvCumprimento.SelectedIndex + 1;
                                dgvCumprimento.CurrentCell = new DataGridCellInfo(dgvCumprimento.Items[dgvCumprimento.SelectedIndex], dgvCumprimento.Columns[dgvCumprimento.CurrentColumn.DisplayIndex - 1]);

                                atualizarGrid();
                                dgvCumprimento.CurrentCell = new DataGridCellInfo(dgvCumprimento.Items[dgvCumprimento.SelectedIndex], dgvCumprimento.Columns[dgvCumprimento.CurrentColumn.DisplayIndex + 1]);
                                dgvCumprimento.CurrentCell = new DataGridCellInfo(dgvCumprimento.Items[dgvCumprimento.SelectedIndex], dgvCumprimento.Columns[dgvCumprimento.CurrentColumn.DisplayIndex - 1]);

                                calcularTempoCumprido();
                                lblTempoCumprido.Content = tempoCumprido.Minutes > 0 ? String.Format("Foram cumpridas {0}h{1}min no total.", tempoCumprido.Hours, tempoCumprido.Minutes) : String.Format("Foram cumpridas {0}h no total.", tempoCumprido.Hours);
                            }
                            else if (dgvCumprimento.CurrentColumn.DisplayIndex == 1 || dgvCumprimento.CurrentColumn.DisplayIndex == 3)
                            {
                                dgvCumprimento.SelectedIndex = dgvCumprimento.SelectedIndex + 1;
                                dgvCumprimento.CurrentCell = new DataGridCellInfo(dgvCumprimento.Items[dgvCumprimento.SelectedIndex], dgvCumprimento.Columns[dgvCumprimento.CurrentColumn.DisplayIndex + 1]);
                                atualizarGrid();
                                dgvCumprimento.SelectedIndex = dgvCumprimento.SelectedIndex - 1;
                                dgvCumprimento.CurrentCell = new DataGridCellInfo(dgvCumprimento.Items[dgvCumprimento.SelectedIndex], dgvCumprimento.Columns[dgvCumprimento.CurrentColumn.DisplayIndex]);

                                calcularTempoCumprido();
                                lblTempoCumprido.Content = tempoCumprido.Minutes > 0 ? String.Format("Foram cumpridas {0}h{1}min no total.", tempoCumprido.Hours, tempoCumprido.Minutes) : String.Format("Foram cumpridas {0}h no total.", tempoCumprido.Hours);
                            }
                        }
                        else
                        {
                            if (dgvCumprimento.CurrentColumn.DisplayIndex == 2)
                            {
                                dgvCumprimento.SelectedIndex = 29;
                                dgvCumprimento.CurrentCell = new DataGridCellInfo(dgvCumprimento.Items[29], dgvCumprimento.Columns[dgvCumprimento.CurrentColumn.DisplayIndex + 1]);

                                if (cumprimentosTela[30].HorarioSaida > cumprimentosTela[30].HorarioEntrada)
                                {
                                    cumprimentosTela[30].DiferencaHoras = cumprimentosTela[30].HorarioSaida - cumprimentosTela[30].HorarioEntrada;
                                }
                                if (cumprimentosTela[30].HorarioSaidaAlmoco > cumprimentosTela[30].HorarioEntradaAlmoco)
                                {
                                    cumprimentosTela[30].DiferencaHoras += cumprimentosTela[30].HorarioSaidaAlmoco - cumprimentosTela[30].HorarioEntradaAlmoco;
                                }
                                dgvCumprimento.Items.Refresh();

                                calcularTempoCumprido();
                                lblTempoCumprido.Content = tempoCumprido.Minutes > 0 ? String.Format("Foram cumpridas {0}h{1}min no total.", tempoCumprido.Hours, tempoCumprido.Minutes) : String.Format("Foram cumpridas {0}h no total.", tempoCumprido.Hours);

                                dgvCumprimento.SelectedIndex = 0;
                                dgvCumprimento.CurrentCell = new DataGridCellInfo(dgvCumprimento.Items[0], dgvCumprimento.Columns[dgvCumprimento.CurrentColumn.DisplayIndex]);
                                scvCadEdCumprimento.ScrollToTop();
                            }
                            else if (dgvCumprimento.CurrentColumn.DisplayIndex == 4)
                            {
                                dgvCumprimento.SelectedIndex = 29;
                                dgvCumprimento.CurrentCell = new DataGridCellInfo(dgvCumprimento.Items[29], dgvCumprimento.Columns[dgvCumprimento.CurrentColumn.DisplayIndex - 1]);

                                if (cumprimentosTela[30].HorarioSaida > cumprimentosTela[30].HorarioEntrada)
                                {
                                    cumprimentosTela[30].DiferencaHoras = cumprimentosTela[30].HorarioSaida - cumprimentosTela[30].HorarioEntrada;
                                }
                                if (cumprimentosTela[30].HorarioSaidaAlmoco > cumprimentosTela[30].HorarioEntradaAlmoco)
                                {
                                    cumprimentosTela[30].DiferencaHoras += cumprimentosTela[30].HorarioSaidaAlmoco - cumprimentosTela[30].HorarioEntradaAlmoco;
                                }
                                dgvCumprimento.Items.Refresh();

                                calcularTempoCumprido();
                                lblTempoCumprido.Content = tempoCumprido.Minutes > 0 ? String.Format("Foram cumpridas {0}h{1}min no total.", tempoCumprido.Hours, tempoCumprido.Minutes) : String.Format("Foram cumpridas {0}h no total.", tempoCumprido.Hours);

                                dgvCumprimento.SelectedIndex = 30;
                                tblObservacao.Focus();
                            }
                            else if (dgvCumprimento.CurrentColumn.DisplayIndex == 1 || dgvCumprimento.CurrentColumn.DisplayIndex == 3)
                            {
                                dgvCumprimento.SelectedIndex = 0;
                                dgvCumprimento.CurrentCell = new DataGridCellInfo(dgvCumprimento.Items[dgvCumprimento.SelectedIndex], dgvCumprimento.Columns[dgvCumprimento.CurrentColumn.DisplayIndex + 1]);

                                if (cumprimentosTela[30].HorarioSaida > cumprimentosTela[30].HorarioEntrada)
                                {
                                    cumprimentosTela[30].DiferencaHoras = cumprimentosTela[30].HorarioSaida - cumprimentosTela[30].HorarioEntrada;
                                }
                                if (cumprimentosTela[30].HorarioSaidaAlmoco > cumprimentosTela[30].HorarioEntradaAlmoco)
                                {
                                    cumprimentosTela[30].DiferencaHoras += cumprimentosTela[30].HorarioSaidaAlmoco - cumprimentosTela[30].HorarioEntradaAlmoco;
                                }
                                dgvCumprimento.Items.Refresh();

                                calcularTempoCumprido();
                                lblTempoCumprido.Content = tempoCumprido.Minutes > 0 ? String.Format("Foram cumpridas {0}h{1}min no total.", tempoCumprido.Hours, tempoCumprido.Minutes) : String.Format("Foram cumpridas {0}h no total.", tempoCumprido.Hours);

                                dgvCumprimento.SelectedIndex = 30;
                                dgvCumprimento.CurrentCell = new DataGridCellInfo(dgvCumprimento.Items[dgvCumprimento.SelectedIndex], dgvCumprimento.Columns[dgvCumprimento.CurrentColumn.DisplayIndex]);
                            }
                        }
                    }
                }
            }
        }

        private void lblCancelar_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                lblCancelar.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0D9Cfa"));
                lblCancelar.FontWeight = FontWeights.Normal;
            }
            catch (Exception ex)
            {
                (App.Current.MainWindow as WpfTelaPrincipal)._vm.ShowError(ex.Message);
            }
        }

        private void lblCancelar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                (App.Current.MainWindow as WpfTelaPrincipal)._vm.ShowError(ex.Message);
            }
        }

        private void lblCancelar_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                lblCancelar.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0D9Cff"));
                lblCancelar.FontWeight = FontWeights.Bold;
            }
            catch (Exception ex)
            {
                (App.Current.MainWindow as WpfTelaPrincipal)._vm.ShowError(ex.Message);
            }
        }

        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Validacoes.validarCampos(new List<Control>() { cbxInstituicao, cbxMes, tbxAno }))
                    (App.Current.MainWindow as WpfTelaPrincipal)._vm.ShowInformation("Favor informar os dados obrigatórios.");
                else
                {
                    if (cumprimentoMesSelecionado == null)
                    {
                        Cumprimento cumprimento;
                        using (IAppServiceCumprimentoMes serviceCumprimentoMes = MinhaNinject.Kernel.Get<IAppServiceCumprimentoMes>())
                        {
                            using (IAppServiceCumprimento serviceCumprimento = MinhaNinject.Kernel.Get<IAppServiceCumprimento>())
                            {
                                serviceCumprimentoMes.Adiciona(cumprimentoMesSelecionado = popularCumprimentoMes(new CumprimentoMes()));
                                for (int i = 0; i < 31; i++)
                                {
                                    cumprimento = popularCumprimento(new Cumprimento(), cumprimentosTela[i], cumprimentoMesSelecionado.Id);
                                    if (cumprimento.Data != DateTime.MinValue)
                                    {
                                        serviceCumprimento.Adiciona(cumprimento);
                                    }
                                }
                                calcularTempoCumprido();
                                cumprimentoMesSelecionado.TempoCumprido = tempoCumprido;
                                serviceCumprimentoMes.Alterar(cumprimentoMesSelecionado);
                            }
                        }
                        pai.carregarDgvCumprimento();
                        Close();
                        (App.Current.MainWindow as WpfTelaPrincipal)._vm.ShowSuccess("Cumprimento cadastrado com sucesso");
                    }
                    else
                    {
                        Cumprimento cumprimento;
                        using (IAppServiceCumprimentoMes serviceCumprimentoMes = MinhaNinject.Kernel.Get<IAppServiceCumprimentoMes>())
                        {
                            using (IAppServiceCumprimento serviceCumprimento = MinhaNinject.Kernel.Get<IAppServiceCumprimento>())
                            {
                                cumprimentoMesSelecionado = popularCumprimentoMes(cumprimentoMesSelecionado);
                                for (int i = 0; i < 31; i++)
                                {
                                    if (i < cumprimentoMesSelecionado.Cumprimentos.Count)
                                    {
                                        cumprimento = popularCumprimento(cumprimentoMesSelecionado.Cumprimentos.ElementAt(i), cumprimentosTela[i], cumprimentoMesSelecionado.Id);
                                        if (cumprimento.Data != DateTime.MinValue)
                                        {
                                            serviceCumprimento.Alterar(cumprimento);
                                        }
                                    }
                                    else
                                    {
                                        cumprimento = popularCumprimento(new Cumprimento(), cumprimentosTela[i], cumprimentoMesSelecionado.Id);
                                        if (cumprimento.Data != DateTime.MinValue)
                                        {
                                            serviceCumprimento.Adiciona(cumprimento);
                                        }
                                    }
                                }
                                calcularTempoCumprido();
                                cumprimentoMesSelecionado.TempoCumprido = tempoCumprido;
                                serviceCumprimentoMes.Alterar(cumprimentoMesSelecionado);
                            }
                        }
                        pai.carregarDgvCumprimento();
                        Close();
                        (App.Current.MainWindow as WpfTelaPrincipal)._vm.ShowSuccess("Cumprimento cadastrado com sucesso");
                    }
                }
            }
            catch (Exception exception)
            {
                (App.Current.MainWindow as WpfTelaPrincipal)._vm.ShowError("Ao gravar a entidade: " + exception.Message);
            }
        }

        public Cumprimento popularCumprimento(Cumprimento cumprimento, cumprimentoTela cumprimentoTela, int cumprimentoMesId)
        {
            DateTime a;
            if (DateTime.TryParse(String.Format("{0}/{1}/{2}", cumprimentoTela.Dia, ((int)Enum.Parse(typeof(Meses), cbxMes.SelectedItem.ToString())).ToString("D2"), tbxAno.Text), out a))
            {
                cumprimento.Data = Convert.ToDateTime(String.Format("{0}/{1}/{2}", cumprimentoTela.Dia, ((int)Enum.Parse(typeof(Meses), cbxMes.SelectedItem.ToString())).ToString("D2"), tbxAno.Text));
                cumprimento.HorarioEntrada = cumprimentoTela.HorarioEntrada;
                cumprimento.HorarioSaida = cumprimentoTela.HorarioSaida;
                cumprimento.HorarioEntradaAlmoco = cumprimentoTela.HorarioEntradaAlmoco;
                cumprimento.HorarioSaidaAlmoco = cumprimentoTela.HorarioSaidaAlmoco;
                cumprimento.CumprimentoMesId = cumprimentoMesId;
                cumprimento.DiferencaHoras = cumprimentoTela.DiferencaHoras;
                cumprimento.Usuario = usuarioLogado.Login;
            }
            return cumprimento;
        }

        public CumprimentoMes popularCumprimentoMes(CumprimentoMes cumprimentoMes)
        {
            cumprimentoMes.Ano = Convert.ToInt16(tbxAno.Text);
            cumprimentoMes.Mes = cbxMes.SelectedValue.ToString();
            cumprimentoMes.Observacao = tblObservacao.Text;
            cumprimentoMes.SentenciadoEntidadeId = (cbxInstituicao.SelectedItem as SentenciadoEntidade).Id;
            cumprimentoMes.UsuarioId = usuarioLogado.Id;
            cumprimentoMes.TempoCumprido = new TimeSpan();
            return cumprimentoMes;
        }

        private void dgvCumprimento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvCumprimento.SelectedIndex > 0)
            {
                cumprimentosTela[dgvCumprimento.SelectedIndex - 1].DiferencaHoras = (cumprimentosTela[dgvCumprimento.SelectedIndex - 1].HorarioSaida - cumprimentosTela[dgvCumprimento.SelectedIndex - 1].HorarioEntrada) + (cumprimentosTela[dgvCumprimento.SelectedIndex - 1].HorarioSaidaAlmoco - cumprimentosTela[dgvCumprimento.SelectedIndex - 1].HorarioEntradaAlmoco);
            }
            else
            {
                cumprimentosTela[30].DiferencaHoras = (cumprimentosTela[30].HorarioSaida - cumprimentosTela[30].HorarioEntrada) + (cumprimentosTela[30].HorarioSaidaAlmoco - cumprimentosTela[30].HorarioEntradaAlmoco);
            }
        }

        private void atualizarGrid()
        {
            if (cumprimentosTela[dgvCumprimento.SelectedIndex - 1].HorarioSaida > cumprimentosTela[dgvCumprimento.SelectedIndex - 1].HorarioEntrada)
            {
                cumprimentosTela[dgvCumprimento.SelectedIndex - 1].DiferencaHoras = cumprimentosTela[dgvCumprimento.SelectedIndex - 1].HorarioSaida - cumprimentosTela[dgvCumprimento.SelectedIndex - 1].HorarioEntrada;
            }
            if (cumprimentosTela[dgvCumprimento.SelectedIndex - 1].HorarioSaidaAlmoco > cumprimentosTela[dgvCumprimento.SelectedIndex - 1].HorarioEntradaAlmoco)
            {
                cumprimentosTela[dgvCumprimento.SelectedIndex - 1].DiferencaHoras += cumprimentosTela[dgvCumprimento.SelectedIndex - 1].HorarioSaidaAlmoco - cumprimentosTela[dgvCumprimento.SelectedIndex - 1].HorarioEntradaAlmoco;
            }
            dgvCumprimento.Items.Refresh();
        }

        private void calcularTempoCumprido()
        {
            tempoCumprido = new TimeSpan();
            for (int i = 0; i < dgvCumprimento.Items.Count; i++)
            {
                tempoCumprido += cumprimentosTela[i].DiferencaHoras;
            }
        }
    }
}