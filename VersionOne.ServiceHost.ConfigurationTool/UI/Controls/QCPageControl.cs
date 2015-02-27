using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;

namespace VersionOne.ServiceHost.ConfigurationTool.UI.Controls {
    public partial class QCPageControl : BasePageControl<QCServiceEntity>, IQualityCenterPageView {
        public event EventHandler ValidationRequested;

        public QCPageControl() {
            InitializeComponent();

            grdDefectFilters.AutoGenerateColumns = false;
            grdQCProjects.AutoGenerateColumns = false;
            grdPriorityMappings.AutoGenerateColumns = false;

            btnValidate.Click += btnValidate_Click;
            btnQCProjectsDelete.Click += btnQCProjectsDelete_Click;
            btnFiltersDelete.Click += btnFiltersDelete_Click;
            btnDeletePriorityMapping.Click += btnDeletePriorityMapping_Click;
            grdQCProjects.DataError += grdQCProjects_DataError;
            grdQCProjects.CellValidating += grdQCProjects_CellValidating;

            grdQCProjects.UserDeletingRow += delegate(object sender, DataGridViewRowCancelEventArgs e) {
                                                 e.Cancel = !ConfirmDelete();
                                             };

            grdDefectFilters.UserDeletingRow += delegate(object sender, DataGridViewRowCancelEventArgs e) {
                                                    e.Cancel = !ConfirmDelete();
                                                };

            grdPriorityMappings.UserDeletingRow += delegate(object sender, DataGridViewRowCancelEventArgs e) {
                                                       e.Cancel = !ConfirmDelete();
                                                   };

            grdPriorityMappings.DataError += grdPriorityMappings_DataError;

            AddValidationProvider(typeof(QCConnection));

            // TODO handle unique project ID validation manually?
            AddGridValidationProvider(typeof(QCProject), grdQCProjects);
            AddGridValidationProvider(typeof(QCDefectFilter), grdDefectFilters);
            AddGridValidationProvider(typeof(QCPriorityMapping), grdPriorityMappings);

            AddControlTextValidation<QCConnection>(txtUrl, QCConnection.ApplicationUrlProperty);
            AddControlTextValidation<QCConnection>(txtUsername, QCConnection.UsernameProperty);

            AddControlTextValidation<QCServiceEntity>(txtCreateStatusValue, QCServiceEntity.CreateStatusValueProperty);
            AddControlTextValidation<QCServiceEntity>(txtCloseStatusValue, QCServiceEntity.CloseStatusValueProperty);
            AddControlTextValidation<QCServiceEntity>(cboSourceFieldValue, QCServiceEntity.SourceFieldValueProperty);

            AddTabHighlightingSupport(tcData);
        }

        public override void DataBind() {
            AddControlBinding(chkDisabled, Model, BaseEntity.DisabledProperty);
            AddControlBinding(txtUrl, Model.Connection, QCConnection.ApplicationUrlProperty);
            AddControlBinding(txtUsername, Model.Connection, QCConnection.UsernameProperty);
            AddControlBinding(txtPassword, Model.Connection, QCConnection.PasswordProperty);
            AddControlBinding(nmdInterval, Model.Timer, TimerEntity.TimerProperty);
            AddSimpleComboboxBinding(cboSourceFieldValue, Model, QCServiceEntity.SourceFieldValueProperty);
            AddControlBinding(txtCreateStatusValue, Model, QCServiceEntity.CreateStatusValueProperty);
            AddControlBinding(txtCloseStatusValue, Model, QCServiceEntity.CloseStatusValueProperty);

            FillComboBoxWithStrings(cboSourceFieldValue, SourceList);

            BindProjectMappingsGrid();
            BimdPriorityMappingsGrid();
            BindDefectFiltersGrid();

            BindHelpStrings();

            InvokeValidationTriggered();
        }

        private void BindDefectFiltersGrid() {
            bsDefectFilters.DataSource = Model.DefectFilters;
            grdDefectFilters.DataSource = bsDefectFilters;
        }

        private void BimdPriorityMappingsGrid() {
            BindVersionOnePriorityColumn();
            bsPriorities.DataSource = Model.PriorityMappings;
            grdPriorityMappings.DataSource = bsPriorities;
        }

        private void BindProjectMappingsGrid() {
            BindProjectColumn();
            bsQCProjects.DataSource = Model.Projects;
            grdQCProjects.DataSource = bsQCProjects;
        }

        private void BindHelpStrings() {
            AddHelpSupport(chkDisabled, Model, BaseEntity.DisabledProperty);
            AddHelpSupport(grdDefectFilters, Model, QCServiceEntity.DefectFiltersProperty);
            AddHelpSupport(txtCreateStatusValue, Model, QCServiceEntity.CreateStatusValueProperty);
            AddHelpSupport(txtCloseStatusValue, Model, QCServiceEntity.CloseStatusValueProperty);
            AddHelpSupport(cboSourceFieldValue, Model, QCServiceEntity.SourceFieldValueProperty);
            AddHelpSupport(grdQCProjects, Model, QCServiceEntity.ProjectMappingsProperty);
            AddHelpSupport(grdPriorityMappings, Model, QCServiceEntity.PriorityMappingsProperty);
            AddHelpSupport(lblMin, Model.Timer, TimerEntity.TimerProperty);
        }

        private void BindVersionOnePriorityColumn() {
            colVersionOnePriority.DisplayMember = ListValue.NameProperty;
            colVersionOnePriority.ValueMember = ListValue.ValueProperty;
            colVersionOnePriority.DataSource = VersionOnePriorities;
        }

        private void BindProjectColumn() {
            colV1Project.Items.Clear();

            foreach(string project in ProjectList) {
                colV1Project.Items.Add(project);
            }
        }

        private void btnValidate_Click(object sender, EventArgs e) {
            if(ValidationRequested != null) {
                ValidationRequested(this, EventArgs.Empty);
            }
        }

        private void btnQCProjectsDelete_Click(object sender, EventArgs e) {
            if(grdQCProjects.SelectedRows.Count != 0 && ConfirmDelete()) {
                bsQCProjects.Remove(grdQCProjects.SelectedRows[0].DataBoundItem);
            }
        }

        private void btnFiltersDelete_Click(object sender, EventArgs e) {
            if(grdDefectFilters.SelectedRows.Count != 0 && ConfirmDelete()) {
                bsDefectFilters.Remove(grdDefectFilters.SelectedRows[0].DataBoundItem);
            }
        }

        private void btnDeletePriorityMapping_Click(object sender, EventArgs e) {
            if(grdPriorityMappings.SelectedRows.Count > 0 && ConfirmDelete()) {
                bsPriorities.Remove(grdPriorityMappings.SelectedRows[0].DataBoundItem);
            }
        }

        private void grdQCProjects_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
            if(e.ColumnIndex != 0) {
                return;
            }

            var currentRow = grdQCProjects.Rows[e.RowIndex];
            currentRow.ErrorText = string.Empty;

            var idValue = currentRow.Cells[0].EditedFormattedValue as string;
            var project = (QCProject)currentRow.DataBoundItem;

            var duplicateIdItemsList = Model.Projects.FindAll(
                item => string.Equals(idValue, item.Id) && !item.Equals(project));

            if(duplicateIdItemsList.Count > 0) {
                currentRow.ErrorText = "Project ID should be unique";
            }
        }

        private void grdQCProjects_DataError(object sender, DataGridViewDataErrorEventArgs e) {
            if(ProjectList != null && ProjectList.Count != 0) {
                grdQCProjects.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = ProjectList[0];
            }

            e.ThrowException = false;
        }

        private void grdPriorityMappings_DataError(object sender, DataGridViewDataErrorEventArgs e) {
            if(VersionOnePriorities != null && VersionOnePriorities.Count > 0) {
                grdPriorityMappings.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = VersionOnePriorities[0];
            }
        }

        public IList<string> ProjectList { get; set; }
        public IList<ListValue> VersionOnePriorities { get; set; }
        public IList<string> SourceList { get; set; }

        public void SetGeneralTabValid(bool isValid) {
            TabHighlighter.SetTabPageValidationMark(tpSettings, isValid);
        }

        public void SetMappingTabValid(bool isValid) {
            TabHighlighter.SetTabPageValidationMark(tpMappings, isValid);
        }

        public void SetValidationResult(bool isSuccessful) {
            lblValidationResult.Visible = true;

            if(isSuccessful) {
                lblValidationResult.ForeColor = Color.Green;
                lblValidationResult.Text = Resources.ConnectionValidMessage;
            } else {
                lblValidationResult.ForeColor = Color.Red;
                lblValidationResult.Text = Resources.ConnectionInvalidMessage;
            }
        }
    }
}