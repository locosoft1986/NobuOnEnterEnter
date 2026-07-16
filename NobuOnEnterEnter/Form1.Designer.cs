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
            label1 = new Label();
            label2 = new Label();
            stopAllWindow = new Button();
            label3 = new Label();
            cmbLanguage = new ComboBox();
            cbModeSelection = new ComboBox();
            labelMode = new Label();
            panelModeEnter = new Panel();
            LabelDelay = new Label();
            delayInMS = new NumericUpDown();
            startStopOneWindow = new Button();
            panelModePalace = new Panel();
            labelPalaceHint = new Label();
            numPalaceBattleWaitTime = new NumericUpDown();
            labelPalaceBattleWaitTime = new Label();
            panelModeFountain = new Panel();
            labelHintFountain = new Label();
            numFountainStartFloor = new NumericUpDown();
            labelStartFloor = new Label();
            numFountainFinalBossWaitTime = new NumericUpDown();
            labelFountainFinalBossWaitTime = new Label();
            numFountainWaitTime = new NumericUpDown();
            labelFountainWaitTime = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            panelSlaves = new Panel();
            labelSlaves = new Label();
            clbSlavesSelector = new CheckedListBox();
            panelModeEnter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)delayInMS).BeginInit();
            panelModePalace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numPalaceBattleWaitTime).BeginInit();
            panelModeFountain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numFountainStartFloor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFountainFinalBossWaitTime).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFountainWaitTime).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            panelSlaves.SuspendLayout();
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
            cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLanguage.FormattingEnabled = true;
            resources.ApplyResources(cmbLanguage, "cmbLanguage");
            cmbLanguage.Name = "cmbLanguage";
            // 
            // cbModeSelection
            // 
            cbModeSelection.DropDownStyle = ComboBoxStyle.DropDownList;
            cbModeSelection.FormattingEnabled = true;
            resources.ApplyResources(cbModeSelection, "cbModeSelection");
            cbModeSelection.Name = "cbModeSelection";
            // 
            // labelMode
            // 
            resources.ApplyResources(labelMode, "labelMode");
            labelMode.Name = "labelMode";
            // 
            // panelModeEnter
            // 
            panelModeEnter.Controls.Add(LabelDelay);
            panelModeEnter.Controls.Add(delayInMS);
            resources.ApplyResources(panelModeEnter, "panelModeEnter");
            panelModeEnter.Name = "panelModeEnter";
            // 
            // LabelDelay
            // 
            resources.ApplyResources(LabelDelay, "LabelDelay");
            LabelDelay.Name = "LabelDelay";
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
            // startStopOneWindow
            // 
            resources.ApplyResources(startStopOneWindow, "startStopOneWindow");
            startStopOneWindow.FlatAppearance.BorderColor = SystemColors.HotTrack;
            startStopOneWindow.Name = "startStopOneWindow";
            startStopOneWindow.UseVisualStyleBackColor = true;
            startStopOneWindow.Click += startStopOneWindow_Click;
            // 
            // panelModePalace
            // 
            panelModePalace.Controls.Add(labelPalaceHint);
            panelModePalace.Controls.Add(numPalaceBattleWaitTime);
            panelModePalace.Controls.Add(labelPalaceBattleWaitTime);
            resources.ApplyResources(panelModePalace, "panelModePalace");
            panelModePalace.Name = "panelModePalace";
            // 
            // labelPalaceHint
            // 
            resources.ApplyResources(labelPalaceHint, "labelPalaceHint");
            labelPalaceHint.ForeColor = Color.Red;
            labelPalaceHint.Name = "labelPalaceHint";
            // 
            // numPalaceBattleWaitTime
            // 
            resources.ApplyResources(numPalaceBattleWaitTime, "numPalaceBattleWaitTime");
            numPalaceBattleWaitTime.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numPalaceBattleWaitTime.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numPalaceBattleWaitTime.Name = "numPalaceBattleWaitTime";
            numPalaceBattleWaitTime.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // labelPalaceBattleWaitTime
            // 
            resources.ApplyResources(labelPalaceBattleWaitTime, "labelPalaceBattleWaitTime");
            labelPalaceBattleWaitTime.Name = "labelPalaceBattleWaitTime";
            // 
            // panelModeFountain
            // 
            panelModeFountain.Controls.Add(labelHintFountain);
            panelModeFountain.Controls.Add(numFountainStartFloor);
            panelModeFountain.Controls.Add(labelStartFloor);
            panelModeFountain.Controls.Add(numFountainFinalBossWaitTime);
            panelModeFountain.Controls.Add(labelFountainFinalBossWaitTime);
            panelModeFountain.Controls.Add(numFountainWaitTime);
            panelModeFountain.Controls.Add(labelFountainWaitTime);
            resources.ApplyResources(panelModeFountain, "panelModeFountain");
            panelModeFountain.Name = "panelModeFountain";
            // 
            // labelHintFountain
            // 
            resources.ApplyResources(labelHintFountain, "labelHintFountain");
            labelHintFountain.ForeColor = Color.Red;
            labelHintFountain.Name = "labelHintFountain";
            // 
            // numFountainStartFloor
            // 
            resources.ApplyResources(numFountainStartFloor, "numFountainStartFloor");
            numFountainStartFloor.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            numFountainStartFloor.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numFountainStartFloor.Name = "numFountainStartFloor";
            numFountainStartFloor.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // labelStartFloor
            // 
            resources.ApplyResources(labelStartFloor, "labelStartFloor");
            labelStartFloor.Name = "labelStartFloor";
            // 
            // numFountainFinalBossWaitTime
            // 
            resources.ApplyResources(numFountainFinalBossWaitTime, "numFountainFinalBossWaitTime");
            numFountainFinalBossWaitTime.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numFountainFinalBossWaitTime.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numFountainFinalBossWaitTime.Name = "numFountainFinalBossWaitTime";
            numFountainFinalBossWaitTime.Value = new decimal(new int[] { 90, 0, 0, 0 });
            // 
            // labelFountainFinalBossWaitTime
            // 
            resources.ApplyResources(labelFountainFinalBossWaitTime, "labelFountainFinalBossWaitTime");
            labelFountainFinalBossWaitTime.Name = "labelFountainFinalBossWaitTime";
            // 
            // numFountainWaitTime
            // 
            resources.ApplyResources(numFountainWaitTime, "numFountainWaitTime");
            numFountainWaitTime.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numFountainWaitTime.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numFountainWaitTime.Name = "numFountainWaitTime";
            numFountainWaitTime.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // labelFountainWaitTime
            // 
            resources.ApplyResources(labelFountainWaitTime, "labelFountainWaitTime");
            labelFountainWaitTime.Name = "labelFountainWaitTime";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(panelModeEnter);
            flowLayoutPanel1.Controls.Add(panelModePalace);
            flowLayoutPanel1.Controls.Add(panelModeFountain);
            flowLayoutPanel1.Controls.Add(panelSlaves);
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // panelSlaves
            // 
            panelSlaves.Controls.Add(labelSlaves);
            panelSlaves.Controls.Add(clbSlavesSelector);
            resources.ApplyResources(panelSlaves, "panelSlaves");
            panelSlaves.Name = "panelSlaves";
            // 
            // labelSlaves
            // 
            resources.ApplyResources(labelSlaves, "labelSlaves");
            labelSlaves.Name = "labelSlaves";
            // 
            // clbSlavesSelector
            // 
            clbSlavesSelector.FormattingEnabled = true;
            resources.ApplyResources(clbSlavesSelector, "clbSlavesSelector");
            clbSlavesSelector.Name = "clbSlavesSelector";
            // 
            // NobuEnterEnter
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(flowLayoutPanel1);
            Controls.Add(startStopOneWindow);
            Controls.Add(labelMode);
            Controls.Add(cbModeSelection);
            Controls.Add(cmbLanguage);
            Controls.Add(label3);
            Controls.Add(stopAllWindow);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(removeWindow);
            Controls.Add(addWindow);
            Controls.Add(windowList);
            Name = "NobuEnterEnter";
            Load += NobuEnterEnter_Load;
            panelModeEnter.ResumeLayout(false);
            panelModeEnter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)delayInMS).EndInit();
            panelModePalace.ResumeLayout(false);
            panelModePalace.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numPalaceBattleWaitTime).EndInit();
            panelModeFountain.ResumeLayout(false);
            panelModeFountain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numFountainStartFloor).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFountainFinalBossWaitTime).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFountainWaitTime).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            panelSlaves.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox windowList;
        private Button addWindow;
        private Button removeWindow;
        private Label label1;
        private Label label2;
        private Button stopAllWindow;
        private Label label3;
        private ComboBox cmbLanguage;
        private Label labelWindowFullTitle;
        private ComboBox cbModeSelection;
        private Label labelMode;
        private Panel panelModeEnter;
        private Label LabelDelay;
        private NumericUpDown delayInMS;
        private Button startStopOneWindow;
        private Panel panelModePalace;
        private NumericUpDown numPalaceBattleWaitTime;
        private Label labelPalaceBattleWaitTime;
        private Panel panelModeFountain;
        private NumericUpDown numFountainFinalBossWaitTime;
        private Label labelFountainFinalBossWaitTime;
        private NumericUpDown numFountainWaitTime;
        private Label labelFountainWaitTime;
        private NumericUpDown numFountainStartFloor;
        private Label labelStartFloor;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label labelPalaceHint;
        private Label labelHintFountain;
        private Panel panelSlaves;
        private Label labelSlaves;
        private CheckedListBox clbSlavesSelector;
    }
}
