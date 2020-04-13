using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectTracker.Timetracker.KIS
{
    public partial class DialogKisTasks : Form
    {
        private WorktimeAnalyzer WorktimeAnalyzer { get; set; }
        private KISTimesCalculator KISTimesCalculator { get; set; }

        public DialogKisTasks(IWorktimeRecordStorage storage, IProjectHandler projectHandler)
        {
            InitializeComponent();

            WorktimeAnalyzer = new WorktimeAnalyzer(storage, projectHandler);
            KISTimesCalculator = new KISTimesCalculator();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /**
         * Was too lazy to create a decent GUI-architecture
         */
        public void Show(DateTime day)
        {
            var projectStatistics = WorktimeAnalyzer.AnalyzeWorkday(day);
            var kisTimes = KISTimesCalculator.generateKISTimes(projectStatistics, day);

            foreach(var kisTime in kisTimes)
            {
                kisTasksView.Rows.Add(
                        day.ToShortDateString(),
                        kisTime.Start.ToString("HHmm"),
                        kisTime.End.ToString("HHmm"),
                        Math.Round(kisTime.getLength().TotalMinutes, 0),
                        Math.Round(kisTime.QuantizationError.TotalMinutes, 0),
                        kisTime.ProjectTrackerProject,
                        "");
            }

            Show();
        }
    }
}
