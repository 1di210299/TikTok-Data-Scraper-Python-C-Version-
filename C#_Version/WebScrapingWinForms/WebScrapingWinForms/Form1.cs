using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using System.IO;

namespace WebScrapingWinForms
{
    public partial class Form1 : Form
    {
        private readonly ILogger<Form1> _logger;
        private readonly WebScrapingLogic _webScraping;
        private List<string> _videoUrls = new List<string>();

        public Form1()
        {
            InitializeComponent();
            var loggerFactory = LoggerFactory.Create(builder =>
                builder.AddProvider(new TextBoxLoggerProvider(logTextBox)));
            _logger = loggerFactory.CreateLogger<Form1>();
            _webScraping = new WebScrapingLogic(loggerFactory.CreateLogger<WebScrapingLogic>());

            // Cargar URLs de ejemplo (puedes modificar esto o cargarlas desde un archivo)
            _videoUrls.Add("https://www.tiktok.com/@eliavilesperu/video/7409023931874790662");
            _videoUrls.Add("");
        }

        // Método para el botón "Procesar Todos los Videos"
        private async void ProcessAllButton_Click(object sender, EventArgs e)
        {
            try
            {
                _logger.LogInformation("Iniciando procesamiento de todos los videos...");
                await _webScraping.ProcessMultipleVideosAsync(_videoUrls);
                _logger.LogInformation("Procesamiento de todos los videos completado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error durante el procesamiento de los videos.");
            }
        }

        // Método para el botón "Cargar URLs desde archivo"
        private void LoadUrlsButton_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadUrlsFromFile(openFileDialog.FileName);
            }
        }

        // Método para cargar las URLs desde un archivo
        private void LoadUrlsFromFile(string filePath)
        {
            try
            {
                _videoUrls = new List<string>(File.ReadAllLines(filePath));
                _logger.LogInformation("Se cargaron {Count} URLs desde el archivo", _videoUrls.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar URLs desde el archivo");
            }
        }
    }
}
