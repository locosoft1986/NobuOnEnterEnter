namespace NobuOnEnterEnter
{
    partial class NobuEnterEnter
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NobuEnterEnter));
            windowList = new ListBox();
            addWindow = new Button();
            removeWindow = new Button();
            startStopOneWindow = new Button();
            startAllWindow = new Button();
            delayInMS = new NumericUpDown();
            LabelDelay = new Label();
            label1 = new Label();
            label2 = new Label();
            stopAllWindow = new Button();
            label3 = new Label();
            cmbLanguage = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)delayInMS).BeginInit();
            SuspendLayout();
            // 
            // windowList
            // 
            windowList.FormattingEnabled = true;
            resources.ApplyResources(windowList, "windowList");
            windowList.Name = "windowList";
            // 
            // addWindow
            // 
            resources.ApplyResources(addWindow, "addWindow");
            addWindow.Name = "addWindow";
            addWindow.UseVisualStyleBackColor = true;
            addWindow.Click += addWindow_Click;
            // 
            // removeWindow
            // 
            resources.ApplyResources(removeWindow, "removeWindow");
            removeWindow.Name = "removeWindow";
            removeWindow.UseVisualStyleBackColor = true;
            removeWindow.Click += removeWindow_Click;
            // 
            // startStopOneWindow
            // 
            resources.ApplyResources(startStopOneWindow, "startStopOneWindow");
            startStopOneWindow.Name = "startStopOneWindow";
            startStopOneWindow.UseVisualStyleBackColor = true;
            startStopOneWindow.Click += startStopOneWindow_Click;
            // 
            // startAllWindow
            // 
            resources.ApplyResources(startAllWindow, "startAllWindow");
            startAllWindow.Name = "startAllWindow";
            startAllWindow.UseVisualStyleBackColor = true;
            startAllWindow.Click += startAllWindow_Click;
            // 
            // delayInMS
            // 
            delayInMS.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            resources.ApplyResources(delayInMS, "delayInMS");
            delayInMS.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            delayInMS.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            delayInMS.Name = "delayInMS";
            delayInMS.Value = new decimal(new int[] { 300, 0, 0, 0 });
            // 
            // LabelDelay
            // 
            resources.ApplyResources(LabelDelay, "LabelDelay");
            LabelDelay.Name = "LabelDelay";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // stopAllWindow
            // 
            resources.ApplyResources(stopAllWindow, "stopAllWindow");
            stopAllWindow.Name = "stopAllWindow";
            stopAllWindow.UseVisualStyleBackColor = true;
            stopAllWindow.Click += stopAllWindow_Click;
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // cmbLanguage
            // 
            cmbLanguage.FormattingEnabled = true;
            resources.ApplyResources(cmbLanguage, "cmbLanguage");
            cmbLanguage.Name = "cmbLanguage";
            // 
            // NobuEnterEnter
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(cmbLanguage);
            Controls.Add(label3);
            Controls.Add(stopAllWindow);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(LabelDelay);
            Controls.Add(delayInMS);
            Controls.Add(startAllWindow);
            Controls.Add(startStopOneWindow);
            Controls.Add(removeWindow);
            Controls.Add(addWindow);
            Controls.Add(windowList);
            Name = "NobuEnterEnter";
            Load += NobuEnterEnter_Load;
            ((System.ComponentModel.ISupportInitialize)delayInMS).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox windowList;
        private Button addWindow;
        private Button removeWindow;
        private Button startStopOneWindow;
        private Button startAllWindow;
        private NumericUpDown delayInMS;
        private Label LabelDelay;
        private Label label1;
        private Label label2;
        private Button stopAllWindow;
        private Label label3;
        private ComboBox cmbLanguage;
    }
}
