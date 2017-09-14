namespace PrintLabel
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.PrintLabelProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.PrintLabelInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // PrintLabelProcessInstaller
            // 
            this.PrintLabelProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.PrintLabelProcessInstaller.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.PrintLabelInstaller});
            this.PrintLabelProcessInstaller.Password = null;
            this.PrintLabelProcessInstaller.Username = null;
            // 
            // PrintLabelInstaller
            // 
            this.PrintLabelInstaller.Description = "Stampa etichette produzione";
            this.PrintLabelInstaller.ServiceName = "Print Labels";
            this.PrintLabelInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.PrintLabelProcessInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller PrintLabelProcessInstaller;
        private System.ServiceProcess.ServiceInstaller PrintLabelInstaller;
    }
}