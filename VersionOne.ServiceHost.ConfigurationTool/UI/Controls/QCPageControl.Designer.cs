using VersionOne.ServiceHost.ConfigurationTool.Entities;

namespace VersionOne.ServiceHost.ConfigurationTool.UI.Controls {
    partial class QCPageControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblUrl = new System.Windows.Forms.Label();
            this.chkDisabled = new System.Windows.Forms.CheckBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.btnValidate = new System.Windows.Forms.Button();
            this.lblValidationResult = new System.Windows.Forms.Label();
            this.grpConnectionSettings = new System.Windows.Forms.GroupBox();
            this.bsQCProjects = new System.Windows.Forms.BindingSource(this.components);
            this.bsDefectFilters = new System.Windows.Forms.BindingSource(this.components);
            this.cboSourceFieldValue = new System.Windows.Forms.ComboBox();
            this.lblSourceFieldValue = new System.Windows.Forms.Label();
            this.lblCreateStatusValue = new System.Windows.Forms.Label();
            this.txtCreateStatusValue = new System.Windows.Forms.TextBox();
            this.lblCloseStatusValue = new System.Windows.Forms.Label();
            this.txtCloseStatusValue = new System.Windows.Forms.TextBox();
            this.grpQCProjects = new System.Windows.Forms.GroupBox();
            this.btnQCProjectsDelete = new System.Windows.Forms.Button();
            this.grdQCProjects = new System.Windows.Forms.DataGridView();
            this.grpDefectFilters = new System.Windows.Forms.GroupBox();
            this.btnFiltersDelete = new System.Windows.Forms.Button();
            this.grdDefectFilters = new System.Windows.Forms.DataGridView();
            this.colFieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFieldValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsPriorities = new System.Windows.Forms.BindingSource(this.components);
            this.tcData = new System.Windows.Forms.TabControl();
            this.tpSettings = new System.Windows.Forms.TabPage();
            this.lblMin = new System.Windows.Forms.Label();
            this.lblTimerInterval = new System.Windows.Forms.Label();
            this.nmdInterval = new System.Windows.Forms.NumericUpDown();
            this.tpMappings = new System.Windows.Forms.TabPage();
            this.grpPriorityMappings = new System.Windows.Forms.GroupBox();
            this.btnDeletePriorityMapping = new System.Windows.Forms.Button();
            this.grdPriorityMappings = new System.Windows.Forms.DataGridView();
            this.colVersionOnePriority = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colQCPriorityName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDomain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQCProject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTestFolder = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.colTestStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colV1IdField = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colV1Project = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.grpConnectionSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsQCProjects)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsDefectFilters)).BeginInit();
            this.grpQCProjects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdQCProjects)).BeginInit();
            this.grpDefectFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDefectFilters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsPriorities)).BeginInit();
            this.tcData.SuspendLayout();
            this.tpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmdInterval)).BeginInit();
            this.tpMappings.SuspendLayout();
            this.grpPriorityMappings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPriorityMappings)).BeginInit();
            this.SuspendLayout();
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(118, 52);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(343, 20);
            this.txtUsername.TabIndex = 3;
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(118, 26);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(343, 20);
            this.txtUrl.TabIndex = 1;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(16, 52);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(55, 13);
            this.lblUsername.TabIndex = 2;
            this.lblUsername.Text = "Username";
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Location = new System.Drawing.Point(16, 26);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(84, 13);
            this.lblUrl.TabIndex = 0;
            this.lblUrl.Text = "Application URL";
            // 
            // chkDisabled
            // 
            this.chkDisabled.AutoSize = true;
            this.chkDisabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkDisabled.Location = new System.Drawing.Point(410, 12);
            this.chkDisabled.Name = "chkDisabled";
            this.chkDisabled.Size = new System.Drawing.Size(67, 17);
            this.chkDisabled.TabIndex = 0;
            this.chkDisabled.Text = "Disabled";
            this.chkDisabled.UseVisualStyleBackColor = true;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(118, 78);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(343, 20);
            this.txtPassword.TabIndex = 5;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(16, 75);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Password";
            // 
            // btnValidate
            // 
            this.btnValidate.Location = new System.Drawing.Point(386, 102);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(75, 27);
            this.btnValidate.TabIndex = 7;
            this.btnValidate.Text = "Validate";
            this.btnValidate.UseVisualStyleBackColor = true;
            // 
            // lblValidationResult
            // 
            this.lblValidationResult.AutoSize = true;
            this.lblValidationResult.Location = new System.Drawing.Point(115, 106);
            this.lblValidationResult.Name = "lblValidationResult";
            this.lblValidationResult.Size = new System.Drawing.Size(139, 13);
            this.lblValidationResult.TabIndex = 6;
            this.lblValidationResult.Text = "Validation result placeholder";
            this.lblValidationResult.Visible = false;
            // 
            // grpConnectionSettings
            // 
            this.grpConnectionSettings.Controls.Add(this.lblUrl);
            this.grpConnectionSettings.Controls.Add(this.txtUrl);
            this.grpConnectionSettings.Controls.Add(this.lblUsername);
            this.grpConnectionSettings.Controls.Add(this.txtUsername);
            this.grpConnectionSettings.Controls.Add(this.lblPassword);
            this.grpConnectionSettings.Controls.Add(this.txtPassword);
            this.grpConnectionSettings.Controls.Add(this.lblValidationResult);
            this.grpConnectionSettings.Controls.Add(this.btnValidate);
            this.grpConnectionSettings.Location = new System.Drawing.Point(12, 6);
            this.grpConnectionSettings.Name = "grpConnectionSettings";
            this.grpConnectionSettings.Size = new System.Drawing.Size(502, 133);
            this.grpConnectionSettings.TabIndex = 0;
            this.grpConnectionSettings.TabStop = false;
            this.grpConnectionSettings.Text = "Connection";
            // 
            // cboSourceFieldValue
            // 
            this.cboSourceFieldValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSourceFieldValue.FormattingEnabled = true;
            this.cboSourceFieldValue.Location = new System.Drawing.Point(130, 336);
            this.cboSourceFieldValue.Name = "cboSourceFieldValue";
            this.cboSourceFieldValue.Size = new System.Drawing.Size(343, 21);
            this.cboSourceFieldValue.TabIndex = 3;
            // 
            // lblSourceFieldValue
            // 
            this.lblSourceFieldValue.AutoSize = true;
            this.lblSourceFieldValue.Location = new System.Drawing.Point(23, 339);
            this.lblSourceFieldValue.Name = "lblSourceFieldValue";
            this.lblSourceFieldValue.Size = new System.Drawing.Size(41, 13);
            this.lblSourceFieldValue.TabIndex = 2;
            this.lblSourceFieldValue.Text = "Source";
            // 
            // lblCreateStatusValue
            // 
            this.lblCreateStatusValue.AutoSize = true;
            this.lblCreateStatusValue.Location = new System.Drawing.Point(23, 366);
            this.lblCreateStatusValue.Name = "lblCreateStatusValue";
            this.lblCreateStatusValue.Size = new System.Drawing.Size(101, 13);
            this.lblCreateStatusValue.TabIndex = 4;
            this.lblCreateStatusValue.Text = "Create Status Value";
            // 
            // txtCreateStatusValue
            // 
            this.txtCreateStatusValue.Location = new System.Drawing.Point(130, 363);
            this.txtCreateStatusValue.Name = "txtCreateStatusValue";
            this.txtCreateStatusValue.Size = new System.Drawing.Size(343, 20);
            this.txtCreateStatusValue.TabIndex = 5;
            // 
            // lblCloseStatusValue
            // 
            this.lblCloseStatusValue.AutoSize = true;
            this.lblCloseStatusValue.Location = new System.Drawing.Point(23, 392);
            this.lblCloseStatusValue.Name = "lblCloseStatusValue";
            this.lblCloseStatusValue.Size = new System.Drawing.Size(96, 13);
            this.lblCloseStatusValue.TabIndex = 6;
            this.lblCloseStatusValue.Text = "Close Status Value";
            // 
            // txtCloseStatusValue
            // 
            this.txtCloseStatusValue.Location = new System.Drawing.Point(130, 389);
            this.txtCloseStatusValue.Name = "txtCloseStatusValue";
            this.txtCloseStatusValue.Size = new System.Drawing.Size(343, 20);
            this.txtCloseStatusValue.TabIndex = 7;
            // 
            // grpQCProjects
            // 
            this.grpQCProjects.Controls.Add(this.btnQCProjectsDelete);
            this.grpQCProjects.Controls.Add(this.grdQCProjects);
            this.grpQCProjects.Location = new System.Drawing.Point(6, 6);
            this.grpQCProjects.Name = "grpQCProjects";
            this.grpQCProjects.Size = new System.Drawing.Size(512, 210);
            this.grpQCProjects.TabIndex = 17;
            this.grpQCProjects.TabStop = false;
            this.grpQCProjects.Text = "Projects";
            // 
            // btnQCProjectsDelete
            // 
            this.btnQCProjectsDelete.Image = global::VersionOne.ServiceHost.ConfigurationTool.Resources.DeleteIcon;
            this.btnQCProjectsDelete.Location = new System.Drawing.Point(341, 177);
            this.btnQCProjectsDelete.Name = "btnQCProjectsDelete";
            this.btnQCProjectsDelete.Size = new System.Drawing.Size(126, 27);
            this.btnQCProjectsDelete.TabIndex = 3;
            this.btnQCProjectsDelete.Text = "Delete selected row";
            this.btnQCProjectsDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnQCProjectsDelete.UseVisualStyleBackColor = true;
            // 
            // grdQCProjects
            // 
            this.grdQCProjects.AutoGenerateColumns = false;
            this.grdQCProjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdQCProjects.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId,
            this.colDomain,
            this.colQCProject,
            this.colTestFolder,
			this.colTestStatus,
            this.colV1IdField,
            this.colV1Project});
            this.grdQCProjects.DataSource = this.bsQCProjects;
            this.grdQCProjects.Location = new System.Drawing.Point(15, 19);
            this.grdQCProjects.MultiSelect = false;
            this.grdQCProjects.Name = "grdQCProjects";
            this.grdQCProjects.Size = new System.Drawing.Size(452, 152);
            this.grdQCProjects.TabIndex = 0;
            // 
            // grpDefectFilters
            // 
            this.grpDefectFilters.Controls.Add(this.btnFiltersDelete);
            this.grpDefectFilters.Controls.Add(this.grdDefectFilters);
            this.grpDefectFilters.Location = new System.Drawing.Point(12, 145);
            this.grpDefectFilters.Name = "grpDefectFilters";
            this.grpDefectFilters.Size = new System.Drawing.Size(502, 176);
            this.grpDefectFilters.TabIndex = 1;
            this.grpDefectFilters.TabStop = false;
            this.grpDefectFilters.Text = "Search Criteria";
            // 
            // btnFiltersDelete
            // 
            this.btnFiltersDelete.Image = global::VersionOne.ServiceHost.ConfigurationTool.Resources.DeleteIcon;
            this.btnFiltersDelete.Location = new System.Drawing.Point(335, 135);
            this.btnFiltersDelete.Name = "btnFiltersDelete";
            this.btnFiltersDelete.Size = new System.Drawing.Size(126, 27);
            this.btnFiltersDelete.TabIndex = 1;
            this.btnFiltersDelete.Text = "Delete selected row";
            this.btnFiltersDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFiltersDelete.UseVisualStyleBackColor = true;
            // 
            // grdDefectFilters
            // 
            this.grdDefectFilters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDefectFilters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFieldName,
            this.colFieldValue});
            this.grdDefectFilters.Location = new System.Drawing.Point(14, 19);
            this.grdDefectFilters.MultiSelect = false;
            this.grdDefectFilters.Name = "grdDefectFilters";
            this.grdDefectFilters.Size = new System.Drawing.Size(447, 108);
            this.grdDefectFilters.TabIndex = 0;
            // 
            // colFieldName
            // 
            this.colFieldName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFieldName.DataPropertyName = "FieldName";
            this.colFieldName.HeaderText = "Field Name";
            this.colFieldName.MinimumWidth = 100;
            this.colFieldName.Name = "colFieldName";
            // 
            // colFieldValue
            // 
            this.colFieldValue.DataPropertyName = "FieldValue";
            this.colFieldValue.HeaderText = "Field Value";
            this.colFieldValue.MinimumWidth = 100;
            this.colFieldValue.Name = "colFieldValue";
            this.colFieldValue.Width = 200;
            // 
            // tcData
            // 
            this.tcData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcData.Controls.Add(this.tpSettings);
            this.tcData.Controls.Add(this.tpMappings);
            this.tcData.Location = new System.Drawing.Point(0, 35);
            this.tcData.Name = "tcData";
            this.tcData.SelectedIndex = 0;
            this.tcData.Size = new System.Drawing.Size(540, 560);
            this.tcData.TabIndex = 0;
            // 
            // tpSettings
            // 
            this.tpSettings.Controls.Add(this.lblMin);
            this.tpSettings.Controls.Add(this.lblTimerInterval);
            this.tpSettings.Controls.Add(this.nmdInterval);
            this.tpSettings.Controls.Add(this.lblCreateStatusValue);
            this.tpSettings.Controls.Add(this.txtCreateStatusValue);
            this.tpSettings.Controls.Add(this.lblCloseStatusValue);
            this.tpSettings.Controls.Add(this.txtCloseStatusValue);
            this.tpSettings.Controls.Add(this.lblSourceFieldValue);
            this.tpSettings.Controls.Add(this.cboSourceFieldValue);
            this.tpSettings.Controls.Add(this.grpConnectionSettings);
            this.tpSettings.Controls.Add(this.grpDefectFilters);
            this.tpSettings.Location = new System.Drawing.Point(4, 22);
            this.tpSettings.Name = "tpSettings";
            this.tpSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tpSettings.Size = new System.Drawing.Size(532, 534);
            this.tpSettings.TabIndex = 0;
            this.tpSettings.Text = "Settings";
            this.tpSettings.UseVisualStyleBackColor = true;
            // 
            // lblMin
            // 
            this.lblMin.AutoSize = true;
            this.lblMin.Location = new System.Drawing.Point(192, 417);
            this.lblMin.Name = "lblMin";
            this.lblMin.Size = new System.Drawing.Size(43, 13);
            this.lblMin.TabIndex = 10;
            this.lblMin.Text = "minutes";
            // 
            // lblTimerInterval
            // 
            this.lblTimerInterval.AutoSize = true;
            this.lblTimerInterval.Location = new System.Drawing.Point(23, 417);
            this.lblTimerInterval.Name = "lblTimerInterval";
            this.lblTimerInterval.Size = new System.Drawing.Size(62, 13);
            this.lblTimerInterval.TabIndex = 8;
            this.lblTimerInterval.Text = "Poll Interval";
            // 
            // nmdInterval
            // 
            this.nmdInterval.Location = new System.Drawing.Point(130, 415);
			this.nmdInterval.Minimum = 1;
            // new decimal(new int[] {
            // 1,
            // 0,
            // 0,
            // 0});
            this.nmdInterval.Name = "nmdInterval";
            this.nmdInterval.Size = new System.Drawing.Size(52, 20);
            this.nmdInterval.TabIndex = 9;
			this.nmdInterval.Value = 5;
            // this.nmdInterval.Value = new decimal(new int[] {
            // 5,
            // 0,
            // 0,
            // 0});
            // 
            // tpMappings
            // 
            this.tpMappings.Controls.Add(this.grpPriorityMappings);
            this.tpMappings.Controls.Add(this.grpQCProjects);
            this.tpMappings.Location = new System.Drawing.Point(4, 22);
            this.tpMappings.Name = "tpMappings";
            this.tpMappings.Padding = new System.Windows.Forms.Padding(3);
            this.tpMappings.Size = new System.Drawing.Size(532, 534);
            this.tpMappings.TabIndex = 1;
            this.tpMappings.Text = "Project and Priority Mappings";
            this.tpMappings.UseVisualStyleBackColor = true;
            // 
            // grpPriorityMappings
            // 
            this.grpPriorityMappings.Controls.Add(this.btnDeletePriorityMapping);
            this.grpPriorityMappings.Controls.Add(this.grdPriorityMappings);
            this.grpPriorityMappings.Location = new System.Drawing.Point(6, 222);
            this.grpPriorityMappings.Name = "grpPriorityMappings";
            this.grpPriorityMappings.Size = new System.Drawing.Size(512, 236);
            this.grpPriorityMappings.TabIndex = 18;
            this.grpPriorityMappings.TabStop = false;
            this.grpPriorityMappings.Text = "Priority Mappings";
            // 
            // btnDeletePriorityMapping
            // 
            this.btnDeletePriorityMapping.Image = global::VersionOne.ServiceHost.ConfigurationTool.Resources.DeleteIcon;
            this.btnDeletePriorityMapping.Location = new System.Drawing.Point(341, 195);
            this.btnDeletePriorityMapping.Name = "btnDeletePriorityMapping";
            this.btnDeletePriorityMapping.Size = new System.Drawing.Size(126, 26);
            this.btnDeletePriorityMapping.TabIndex = 1;
            this.btnDeletePriorityMapping.Text = "Delete selected row";
            this.btnDeletePriorityMapping.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDeletePriorityMapping.UseVisualStyleBackColor = true;
            // 
            // grdPriorityMappings
            // 
            this.grdPriorityMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdPriorityMappings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVersionOnePriority,
            this.colQCPriorityName});
            this.grdPriorityMappings.Location = new System.Drawing.Point(15, 20);
            this.grdPriorityMappings.MultiSelect = false;
            this.grdPriorityMappings.Name = "grdPriorityMappings";
            this.grdPriorityMappings.Size = new System.Drawing.Size(452, 169);
            this.grdPriorityMappings.TabIndex = 0;
            // 
            // colVersionOnePriority
            // 
            this.colVersionOnePriority.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colVersionOnePriority.DataPropertyName = "VersionOnePriorityId";
            this.colVersionOnePriority.HeaderText = "VersionOne Priority";
            this.colVersionOnePriority.MinimumWidth = 100;
            this.colVersionOnePriority.Name = "colVersionOnePriority";
            // 
            // colQCPriorityName
            // 
            this.colQCPriorityName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colQCPriorityName.DataPropertyName = "QCPriorityName";
            this.colQCPriorityName.HeaderText = "Quality Center Priority";
            this.colQCPriorityName.MinimumWidth = 100;
            this.colQCPriorityName.Name = "colQCPriorityName";
            // 
            // colId
            // 
            this.colId.DataPropertyName = "Id";
            this.colId.HeaderText = "ID";
            this.colId.MinimumWidth = 40;
            this.colId.Name = "colId";
            this.colId.Width = 80;
            // 
            // colDomain
            // 
            this.colDomain.DataPropertyName = "Domain";
            this.colDomain.HeaderText = "Domain";
            this.colDomain.MinimumWidth = 50;
            this.colDomain.Name = "colDomain";
            // 
            // colQCProject
            // 
            this.colQCProject.DataPropertyName = "Project";
            this.colQCProject.HeaderText = "QualityCenter Project";
            this.colQCProject.MinimumWidth = 100;
            this.colQCProject.Name = "colQCProject";
            this.colQCProject.Width = 200;
            // 
            // colTestFolder
            // 
            this.colTestFolder.DataPropertyName = "TestFolder";
            this.colTestFolder.HeaderText = "Test Folder";
            this.colTestFolder.MinimumWidth = 50;
            this.colTestFolder.Name = "colTestFolder";
			// 
			// colTestFolder
			// 
			this.colTestStatus.DataPropertyName = "TestStatus";
			this.colTestStatus.HeaderText = "Test Status";
			this.colTestStatus.MinimumWidth = 50;
			this.colTestStatus.Name = "colTestStatus";
			// 
            // colV1IdField
            // 
            this.colV1IdField.DataPropertyName = "VersionOneIdField";
            this.colV1IdField.HeaderText = "VersionOne ID Field";
            this.colV1IdField.MinimumWidth = 100;
            this.colV1IdField.Name = "colV1IdField";
            this.colV1IdField.Width = 200;
            // 
            // colV1Project
            // 
            this.colV1Project.DataPropertyName = "VersionOneProject";
            this.colV1Project.HeaderText = "VersionOne Project";
            this.colV1Project.MinimumWidth = 100;
            this.colV1Project.Name = "colV1Project";
            this.colV1Project.Width = 200;
            // 
            // QCPageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcData);
            this.Controls.Add(this.chkDisabled);
            this.Name = "QCPageControl";
            this.Size = new System.Drawing.Size(540, 595);
            this.grpConnectionSettings.ResumeLayout(false);
            this.grpConnectionSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsQCProjects)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsDefectFilters)).EndInit();
            this.grpQCProjects.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdQCProjects)).EndInit();
            this.grpDefectFilters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdDefectFilters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsPriorities)).EndInit();
            this.tcData.ResumeLayout(false);
            this.tpSettings.ResumeLayout(false);
            this.tpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmdInterval)).EndInit();
            this.tpMappings.ResumeLayout(false);
            this.grpPriorityMappings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdPriorityMappings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.CheckBox chkDisabled;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.Label lblValidationResult;
        private System.Windows.Forms.GroupBox grpConnectionSettings;
        private System.Windows.Forms.BindingSource bsQCProjects;
        private System.Windows.Forms.BindingSource bsDefectFilters;
        private System.Windows.Forms.ComboBox cboSourceFieldValue;
        private System.Windows.Forms.Label lblSourceFieldValue;
        private System.Windows.Forms.Label lblCreateStatusValue;
        public System.Windows.Forms.TextBox txtCreateStatusValue;
        private System.Windows.Forms.Label lblCloseStatusValue;
        public System.Windows.Forms.TextBox txtCloseStatusValue;
        private System.Windows.Forms.GroupBox grpQCProjects;
        private System.Windows.Forms.DataGridView grdQCProjects;
        private System.Windows.Forms.GroupBox grpDefectFilters;
        private System.Windows.Forms.DataGridView grdDefectFilters;
        private System.Windows.Forms.Button btnQCProjectsDelete;
        private System.Windows.Forms.Button btnFiltersDelete;
        private System.Windows.Forms.BindingSource bsPriorities;
        private System.Windows.Forms.TabControl tcData;
        private System.Windows.Forms.TabPage tpSettings;
        private System.Windows.Forms.TabPage tpMappings;
        private System.Windows.Forms.GroupBox grpPriorityMappings;
        private System.Windows.Forms.Button btnDeletePriorityMapping;
        private System.Windows.Forms.DataGridView grdPriorityMappings;
        private System.Windows.Forms.DataGridViewComboBoxColumn colVersionOnePriority;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQCPriorityName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFieldValue;
        private System.Windows.Forms.Label lblMin;
        private System.Windows.Forms.Label lblTimerInterval;
        private System.Windows.Forms.NumericUpDown nmdInterval;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDomain;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQCProject;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTestFolder;
    	private System.Windows.Forms.DataGridViewTextBoxColumn colTestStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colV1IdField;
        private System.Windows.Forms.DataGridViewComboBoxColumn colV1Project;
    }
}
