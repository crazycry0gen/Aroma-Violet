namespace NovusStoredProcedureExec
{
    partial class NovusStoredProcedureExec
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.eventNovus = new System.Diagnostics.EventLog();
            this.NovusSPExecController = new System.ServiceProcess.ServiceController();
            ((System.ComponentModel.ISupportInitialize)(this.eventNovus)).BeginInit();
            // 
            // NovusStoredProcedureExec
            // 
            this.ServiceName = "NovusStoredProcedureExec";
            ((System.ComponentModel.ISupportInitialize)(this.eventNovus)).EndInit();

        }

        #endregion

        private System.Diagnostics.EventLog eventNovus;
        private System.ServiceProcess.ServiceController NovusSPExecController;
    }
}
