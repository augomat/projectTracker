using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace ProjectTracker
{
    static class Program
    {
        

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 form = new Form1();
            Overlay overlay = new Overlay();
            var presenter = new Presenter(form, overlay);
            form.Presenter = presenter;

            ProjectChangeHandler mainHandler = new ProjectChangeHandler();
            var worktimebreakHandler = new ProjectChangeProcessorWorktimebreaks(mainHandler);
            var projectCorrectionHandler = new ProjectChangeNotifierCorrection(mainHandler);
            var projectAnalysisHandler = new ProjectChangeNotifierAnalysis(mainHandler);
            var storageEngine = new WorktimeRecordStorageNoSQL();
            var worktimeAnalyzer = new WorktimeAnalyzer(storageEngine, mainHandler);
            var worktrackerUpdater = new WorktrackerUpdater();

            //Change notifiers
            mainHandler.addProjectChangeNotifier(new ProjectChangeNotifierDexpot(mainHandler, presenter));
            mainHandler.addProjectChangeNotifier(new ProjectChangeNotifierTimeular(mainHandler, presenter));
            mainHandler.addProjectChangeNotifier(new ProjectChangeNotifierLockscreen(mainHandler));
            mainHandler.addProjectChangeNotifier(new ProjectChangeNotifierAppExit(mainHandler));
            mainHandler.addProjectChangeNotifier(projectAnalysisHandler);
            mainHandler.addProjectChangeNotifier(projectCorrectionHandler);

            //Change processors
            mainHandler.addProjectChangeProcessor(new ProjectChangeProcessorAppStart(mainHandler));
            mainHandler.addProjectChangeProcessor(new ProjectChangeProcessorNewDay(mainHandler, worktimeAnalyzer, worktrackerUpdater));
            mainHandler.addProjectChangeProcessor(new ProjectChangeProcessorLockscreen(mainHandler));
            mainHandler.addProjectChangeProcessor(worktimebreakHandler);
            //mainHandler.addProjectChangeProcessor(new ProjectChangeProcessorLongerThan10secs(mainHandler));

            //Change subscribers
            mainHandler.addProjectChangeSubscriber(new ProjectChangeSubscriberFormUpdater(presenter));
            mainHandler.addProjectChangeSubscriber(new ProjectChangeSubscriberBalloonInformant(presenter.showNotification));
            mainHandler.addProjectChangeSubscriber(new ProjectChangeSubscriberOverlayUpdater(overlay.setOverlayText));
            mainHandler.addProjectChangeSubscriber(new ProjectChangeSubscriberLogger());

            //Storages
            mainHandler.addWorktimeRecordStorage(new WorktimeRecordStorageCSV());
            mainHandler.addWorktimeRecordStorage(storageEngine);
            mainHandler.RaiseStorageExceptionEvent += new StorageExceptionBalloonInformant(presenter.showNotification).handleStorageException;

            //Presenter
            presenter.WorktimeAnalyzer = worktimeAnalyzer;
            presenter.WorktimebreakHandler = worktimebreakHandler;
            presenter.ProjectCorrectionHandler = projectCorrectionHandler;
            presenter.ProjectHandler = mainHandler;
            presenter.storage = storageEngine;
            presenter.wtUpdater = worktrackerUpdater;

            presenter.onInitCompleted();

            Application.Run(form);
        }
    }

}


