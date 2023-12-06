using ARKBreedingStats.BreedingPlanning;
using ARKBreedingStats.multiplierTesting;
using ARKBreedingStats.Pedigree;
using ARKBreedingStats.raising;

namespace ARKBreedingStats
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadAndAddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.recentlyUsedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.openFolderOfCurrentFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.importingFromSavegameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importingFromSavegameEmptyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importExportedCreaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFromTabSeparatedFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.copyLibrarydumpToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbImprintedCount = new System.Windows.Forms.Label();
            this.labelImprintingTester = new System.Windows.Forms.Label();
            this.numericUpDownImprintingBonusTester = new ARKBreedingStats.uiControls.Nud();
            this.NumericUpDownTestingTE = new ARKBreedingStats.uiControls.Nud();
            this.labelTesterTE = new System.Windows.Forms.Label();
            this.groupBoxPossibilities = new System.Windows.Forms.GroupBox();
            this.listViewPossibilities = new System.Windows.Forms.ListView();
            this.columnHeaderWild = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDom = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTE = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLW = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBoxDetailsExtractor = new System.Windows.Forms.GroupBox();
            this.panelExtrImpr = new System.Windows.Forms.Panel();
            this.cbExactlyImprinting = new System.Windows.Forms.CheckBox();
            this.labelImprintingBonus = new System.Windows.Forms.Label();
            this.lbImprintingCuddleCountExtractor = new System.Windows.Forms.Label();
            this.numericUpDownImprintingBonusExtractor = new ARKBreedingStats.uiControls.Nud();
            this.panelExtrTE = new System.Windows.Forms.Panel();
            this.labelTE = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownUpperTEffBound = new ARKBreedingStats.uiControls.Nud();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownLowerTEffBound = new ARKBreedingStats.uiControls.Nud();
            this.lbLevel = new System.Windows.Forms.Label();
            this.lbBreedingValueTester = new System.Windows.Forms.Label();
            this.lbTesterWildLevel = new System.Windows.Forms.Label();
            this.lbTesterDomLevel = new System.Windows.Forms.Label();
            this.lbInfoYellowStats = new System.Windows.Forms.Label();
            this.labelFootnote = new System.Windows.Forms.Label();
            this.labelHBV = new System.Windows.Forms.Label();
            this.lbExtractorDomLevel = new System.Windows.Forms.Label();
            this.lbExtractorWildLevel = new System.Windows.Forms.Label();
            this.lbSum = new System.Windows.Forms.Label();
            this.lbSumDom = new System.Windows.Forms.Label();
            this.lbSumWild = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportValuesToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plainTextcurrentValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plainTextbreedingValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.forSpreadsheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSpreadsheetExportFieldsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.setStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aliveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unavailableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.obeliskToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.multiSetterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findDuplicatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.exactSpawnCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exactSpawnCommandDS2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.copyCreatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteCreatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.libraryFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.modValueManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customStatOverridesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.extraDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.openJsonDataFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.speciesSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetSortingToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.editSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applyChangedSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpAboutSpeciesSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.discordServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlinehelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BreedingPlanHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractionIssuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.checkForUpdatedStatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.devToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRandomCreaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorDefinitionsToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listenWithNewTokenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendExampleCreatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelSums = new System.Windows.Forms.Panel();
            this.lbShouldBe = new System.Windows.Forms.Label();
            this.lbSumDomSB = new System.Windows.Forms.Label();
            this.panelWildTamedBred = new System.Windows.Forms.Panel();
            this.rbBredExtractor = new System.Windows.Forms.RadioButton();
            this.rbTamedExtractor = new System.Windows.Forms.RadioButton();
            this.rbWildExtractor = new System.Windows.Forms.RadioButton();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageStatTesting = new System.Windows.Forms.TabPage();
            this.pictureBoxColorRegionsTester = new System.Windows.Forms.PictureBox();
            this.statPotentials1 = new ARKBreedingStats.uiControls.StatPotentials();
            this.gbStatChart = new System.Windows.Forms.GroupBox();
            this.radarChart1 = new ARKBreedingStats.RadarChart();
            this.panelWildTamedBredTester = new System.Windows.Forms.Panel();
            this.rbBredTester = new System.Windows.Forms.RadioButton();
            this.rbTamedTester = new System.Windows.Forms.RadioButton();
            this.rbWildTester = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelStatIOsTester = new System.Windows.Forms.FlowLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbCurrentValue = new System.Windows.Forms.Label();
            this.panelStatTesterFootnote = new System.Windows.Forms.Panel();
            this.LbWarningLevel255 = new System.Windows.Forms.Label();
            this.lbWildLevelTester = new System.Windows.Forms.Label();
            this.labelDomLevelSum = new System.Windows.Forms.Label();
            this.labelTesterTotalLevel = new System.Windows.Forms.Label();
            this.lbNotYetTamed = new System.Windows.Forms.Label();
            this.gpPreviewEdit = new System.Windows.Forms.GroupBox();
            this.lbCurrentCreature = new System.Windows.Forms.Label();
            this.labelCurrentTesterCreature = new System.Windows.Forms.Label();
            this.lbTestingInfo = new System.Windows.Forms.Label();
            this.creatureInfoInputTester = new ARKBreedingStats.CreatureInfoInput();
            this.tabPageExtractor = new System.Windows.Forms.TabPage();
            this.LbAsa = new System.Windows.Forms.Label();
            this.LbBlueprintPath = new System.Windows.Forms.Label();
            this.BtCopyIssueDumpToClipboard = new System.Windows.Forms.Button();
            this.llOnlineHelpExtractionIssues = new System.Windows.Forms.LinkLabel();
            this.PbCreatureColorsExtractor = new System.Windows.Forms.PictureBox();
            this.groupBoxRadarChartExtractor = new System.Windows.Forms.GroupBox();
            this.radarChartExtractor = new ARKBreedingStats.RadarChart();
            this.lbImprintingFailInfo = new System.Windows.Forms.Label();
            this.groupBoxTamingInfo = new System.Windows.Forms.GroupBox();
            this.labelTamingInfo = new System.Windows.Forms.Label();
            this.button2TamingCalc = new System.Windows.Forms.Button();
            this.gbStatsExtractor = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelStatIOsExtractor = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbCurrentStatEx = new System.Windows.Forms.Label();
            this.btExtractLevels = new System.Windows.Forms.Button();
            this.cbQuickWildCheck = new System.Windows.Forms.CheckBox();
            this.labelErrorHelp = new System.Windows.Forms.Label();
            this.creatureAnalysis1 = new ARKBreedingStats.uiControls.CreatureAnalysis();
            this.parentInheritanceExtractor = new ARKBreedingStats.uiControls.ParentInheritance();
            this.numericUpDownLevel = new ARKBreedingStats.uiControls.Nud();
            this.creatureInfoInputExtractor = new ARKBreedingStats.CreatureInfoInput();
            this.tabPageLibrary = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelLibrary = new System.Windows.Forms.TableLayoutPanel();
            this.listViewLibrary = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderOwner = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderNote = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderServer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDomesticated = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTopness = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTopStatsNr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderGen = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFound = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderMutations = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCooldown = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderHP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderOx = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderWa = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderWe = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderHPM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderStM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderToM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderOxM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFoM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderWaM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTmM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderWeM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDmM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSpM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFrM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCrM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderColor0 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderColor1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderColor2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderColor3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderColor4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderColor5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSpecies = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTribe = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderStatusIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderMutagen = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStripLibrary = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.editAllSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemGenerateCreatureName = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopyCreatureName = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.copyValuesToExtractorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wildValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToClipboardToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.plainTextcurrentValuesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.plainTextbreedingValuesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.forSpreadsheetToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.copyInfographicToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveInfographicsToFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.removeCooldownGrowingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setToMatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearMatingCooldownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.justMatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bestBreedingPartnersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.breedingPlanForSelectedCreaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStatus = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.obeliskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cryopodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applyMutagenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.adminCommandToSetColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adminCommandToSpawnExactDinoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adminCommandToSpawnExactDinoDS2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemOpenWiki = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControlLibFilter = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listBoxSpeciesLib = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.checkedListBoxConsiderStatTop = new System.Windows.Forms.CheckedListBox();
            this.buttonRecalculateTops = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.tabPageLibRadarChart = new System.Windows.Forms.TabPage();
            this.radarChartLibrary = new ARKBreedingStats.RadarChart();
            this.creatureBoxListView = new ARKBreedingStats.CreatureBox();
            this.tabPageLibraryInfo = new System.Windows.Forms.TabPage();
            this.tlpLibraryInfo = new System.Windows.Forms.TableLayoutPanel();
            this.CbLibraryInfoUseFilter = new System.Windows.Forms.CheckBox();
            this.BtCopyLibraryColorToClipboard = new System.Windows.Forms.Button();
            this.tabPagePedigree = new System.Windows.Forms.TabPage();
            this.pedigree1 = new ARKBreedingStats.Pedigree.PedigreeControl();
            this.tabPageTaming = new System.Windows.Forms.TabPage();
            this.tamingControl1 = new ARKBreedingStats.TamingControl();
            this.tabPageBreedingPlan = new System.Windows.Forms.TabPage();
            this.breedingPlan1 = new ARKBreedingStats.BreedingPlanning.BreedingPlan();
            this.tabPageHatching = new System.Windows.Forms.TabPage();
            this.hatching1 = new ARKBreedingStats.uiControls.Hatching();
            this.tabPageRaising = new System.Windows.Forms.TabPage();
            this.raisingControl1 = new ARKBreedingStats.raising.RaisingControl();
            this.tabPageTimer = new System.Windows.Forms.TabPage();
            this.timerList1 = new ARKBreedingStats.TimerControl();
            this.tabPagePlayerTribes = new System.Windows.Forms.TabPage();
            this.tribesControl1 = new ARKBreedingStats.TribesControl();
            this.tabPageNotes = new System.Windows.Forms.TabPage();
            this.notesControl1 = new ARKBreedingStats.NotesControl();
            this.TabPageOCR = new System.Windows.Forms.TabPage();
            this.ocrControl1 = new ARKBreedingStats.ocr.OCRControl();
            this.tabPageExtractionTests = new System.Windows.Forms.TabPage();
            this.extractionTestControl1 = new ARKBreedingStats.testCases.ExtractionTestControl();
            this.tabPageMultiplierTesting = new System.Windows.Forms.TabPage();
            this.statsMultiplierTesting1 = new ARKBreedingStats.multiplierTesting.StatsMultiplierTesting();
            this.btReadValuesFromArk = new System.Windows.Forms.Button();
            this.cbEventMultipliers = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripStatusLabelImport = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.newToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.openToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.TsbQuickSaveGameImport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCopy2Tester = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCopy2Extractor = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddPlayer = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddTribe = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDeleteExpiredIncubationTimers = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSaveCreatureValuesTemp = new System.Windows.Forms.ToolStripButton();
            this.toolStripCBTempCreatures = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButtonDeleteTempCreature = new System.Windows.Forms.ToolStripButton();
            this.tsBtAddAsExtractionTest = new System.Windows.Forms.ToolStripButton();
            this.copyToMultiplierTesterToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ToolStripLabelFilter = new System.Windows.Forms.ToolStripLabel();
            this.ToolStripTextBoxLibraryFilter = new System.Windows.Forms.ToolStripTextBox();
            this.ToolStripButtonLibraryFilterClear = new System.Windows.Forms.ToolStripButton();
            this.ToolStripButtonSaveFilterPreset = new System.Windows.Forms.ToolStripButton();
            this.TsSpOcrLabel = new System.Windows.Forms.ToolStripSeparator();
            this.TsLbLabelSet = new System.Windows.Forms.ToolStripLabel();
            this.TsCbbLabelSets = new System.Windows.Forms.ToolStripComboBox();
            this.panelToolBar = new System.Windows.Forms.Panel();
            this.btImportLastExported = new System.Windows.Forms.Button();
            this.pbSpecies = new System.Windows.Forms.PictureBox();
            this.tbSpeciesGlobal = new ARKBreedingStats.uiControls.TextBoxSuggest();
            this.cbGuessSpecies = new System.Windows.Forms.CheckBox();
            this.cbToggleOverlay = new System.Windows.Forms.CheckBox();
            this.lbListening = new System.Windows.Forms.Label();
            this.lbSpecies = new System.Windows.Forms.Label();
            this.TbMessageLabel = new System.Windows.Forms.TextBox();
            this.contextMenuStripLibraryHeader = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemResetLibraryColumnWidths = new System.Windows.Forms.ToolStripMenuItem();
            this.resetColumnWidthNoMutationLevelColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreMutationLevelsASAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseMutationsLevelsASEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.resetColumnOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.speciesSelector1 = new ARKBreedingStats.SpeciesSelector();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownImprintingBonusTester)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownTestingTE)).BeginInit();
            this.groupBoxPossibilities.SuspendLayout();
            this.groupBoxDetailsExtractor.SuspendLayout();
            this.panelExtrImpr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownImprintingBonusExtractor)).BeginInit();
            this.panelExtrTE.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpperTEffBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowerTEffBound)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.panelSums.SuspendLayout();
            this.panelWildTamedBred.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabPageStatTesting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColorRegionsTester)).BeginInit();
            this.gbStatChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radarChart1)).BeginInit();
            this.panelWildTamedBredTester.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanelStatIOsTester.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelStatTesterFootnote.SuspendLayout();
            this.gpPreviewEdit.SuspendLayout();
            this.tabPageExtractor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PbCreatureColorsExtractor)).BeginInit();
            this.groupBoxRadarChartExtractor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radarChartExtractor)).BeginInit();
            this.groupBoxTamingInfo.SuspendLayout();
            this.gbStatsExtractor.SuspendLayout();
            this.flowLayoutPanelStatIOsExtractor.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).BeginInit();
            this.tabPageLibrary.SuspendLayout();
            this.tableLayoutPanelLibrary.SuspendLayout();
            this.contextMenuStripLibrary.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControlLibFilter.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabPageLibRadarChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radarChartLibrary)).BeginInit();
            this.tabPageLibraryInfo.SuspendLayout();
            this.tlpLibraryInfo.SuspendLayout();
            this.tabPagePedigree.SuspendLayout();
            this.tabPageTaming.SuspendLayout();
            this.tabPageBreedingPlan.SuspendLayout();
            this.tabPageHatching.SuspendLayout();
            this.tabPageRaising.SuspendLayout();
            this.tabPageTimer.SuspendLayout();
            this.tabPagePlayerTribes.SuspendLayout();
            this.tabPageNotes.SuspendLayout();
            this.TabPageOCR.SuspendLayout();
            this.tabPageExtractionTests.SuspendLayout();
            this.tabPageMultiplierTesting.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.panelToolBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSpecies)).BeginInit();
            this.contextMenuStripLibraryHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.aboutToolStripMenuItem.Text = "about…";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.loadAndAddToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator2,
            this.recentlyUsedToolStripMenuItem,
            this.toolStripSeparator21,
            this.openFolderOfCurrentFileToolStripMenuItem,
            this.toolStripSeparator15,
            this.importingFromSavegameToolStripMenuItem,
            this.importExportedCreaturesToolStripMenuItem,
            this.importFromTabSeparatedFileToolStripMenuItem,
            this.toolStripSeparator19,
            this.copyLibrarydumpToClipboardToolStripMenuItem,
            this.toolStripSeparator10,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.newToolStripMenuItem.Text = "&New Library";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.loadToolStripMenuItem.Text = "&Load...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // loadAndAddToolStripMenuItem
            // 
            this.loadAndAddToolStripMenuItem.Name = "loadAndAddToolStripMenuItem";
            this.loadAndAddToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.loadAndAddToolStripMenuItem.Text = "Load and A&dd...";
            this.loadAndAddToolStripMenuItem.ToolTipText = "Select a library-file and add all its creatures to the currently loaded library";
            this.loadAndAddToolStripMenuItem.Click += new System.EventHandler(this.loadAndAddToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.saveAsToolStripMenuItem.Text = "Save &as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(274, 6);
            // 
            // recentlyUsedToolStripMenuItem
            // 
            this.recentlyUsedToolStripMenuItem.Name = "recentlyUsedToolStripMenuItem";
            this.recentlyUsedToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.recentlyUsedToolStripMenuItem.Text = "Recently used";
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Size = new System.Drawing.Size(274, 6);
            // 
            // openFolderOfCurrentFileToolStripMenuItem
            // 
            this.openFolderOfCurrentFileToolStripMenuItem.Enabled = false;
            this.openFolderOfCurrentFileToolStripMenuItem.Name = "openFolderOfCurrentFileToolStripMenuItem";
            this.openFolderOfCurrentFileToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.openFolderOfCurrentFileToolStripMenuItem.Text = "Open folder of current file…";
            this.openFolderOfCurrentFileToolStripMenuItem.Click += new System.EventHandler(this.openFolderOfCurrentFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(274, 6);
            // 
            // importingFromSavegameToolStripMenuItem
            // 
            this.importingFromSavegameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importingFromSavegameEmptyToolStripMenuItem});
            this.importingFromSavegameToolStripMenuItem.Name = "importingFromSavegameToolStripMenuItem";
            this.importingFromSavegameToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.importingFromSavegameToolStripMenuItem.Text = "Importing from savegame (only ASE)";
            // 
            // importingFromSavegameEmptyToolStripMenuItem
            // 
            this.importingFromSavegameEmptyToolStripMenuItem.Name = "importingFromSavegameEmptyToolStripMenuItem";
            this.importingFromSavegameEmptyToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.importingFromSavegameEmptyToolStripMenuItem.Text = "At first configure \"Import Savegame\"";
            this.importingFromSavegameEmptyToolStripMenuItem.Click += new System.EventHandler(this.importingFromSavegameEmptyToolStripMenuItem_Click);
            // 
            // importExportedCreaturesToolStripMenuItem
            // 
            this.importExportedCreaturesToolStripMenuItem.Name = "importExportedCreaturesToolStripMenuItem";
            this.importExportedCreaturesToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.importExportedCreaturesToolStripMenuItem.Text = "Import exported Creatures";
            // 
            // importFromTabSeparatedFileToolStripMenuItem
            // 
            this.importFromTabSeparatedFileToolStripMenuItem.Name = "importFromTabSeparatedFileToolStripMenuItem";
            this.importFromTabSeparatedFileToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.importFromTabSeparatedFileToolStripMenuItem.Text = "Import from tab separated values file…";
            this.importFromTabSeparatedFileToolStripMenuItem.Click += new System.EventHandler(this.importFromTabSeparatedFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(274, 6);
            // 
            // copyLibrarydumpToClipboardToolStripMenuItem
            // 
            this.copyLibrarydumpToClipboardToolStripMenuItem.Name = "copyLibrarydumpToClipboardToolStripMenuItem";
            this.copyLibrarydumpToClipboardToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.copyLibrarydumpToClipboardToolStripMenuItem.Text = "Copy library-dump to clipboard";
            this.copyLibrarydumpToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyLibrarydumpToClipboardToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(274, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.quitToolStripMenuItem.Text = "&Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbImprintedCount);
            this.groupBox1.Controls.Add(this.labelImprintingTester);
            this.groupBox1.Controls.Add(this.numericUpDownImprintingBonusTester);
            this.groupBox1.Controls.Add(this.NumericUpDownTestingTE);
            this.groupBox1.Controls.Add(this.labelTesterTE);
            this.groupBox1.Location = new System.Drawing.Point(373, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(229, 72);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Details";
            // 
            // lbImprintedCount
            // 
            this.lbImprintedCount.AutoSize = true;
            this.lbImprintedCount.Location = new System.Drawing.Point(181, 47);
            this.lbImprintedCount.Name = "lbImprintedCount";
            this.lbImprintedCount.Size = new System.Drawing.Size(25, 13);
            this.lbImprintedCount.TabIndex = 5;
            this.lbImprintedCount.Text = "(0×)";
            this.lbImprintedCount.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelImprintedCount_MouseClick);
            // 
            // labelImprintingTester
            // 
            this.labelImprintingTester.AutoSize = true;
            this.labelImprintingTester.Enabled = false;
            this.labelImprintingTester.Location = new System.Drawing.Point(87, 47);
            this.labelImprintingTester.Name = "labelImprintingTester";
            this.labelImprintingTester.Size = new System.Drawing.Size(96, 13);
            this.labelImprintingTester.TabIndex = 5;
            this.labelImprintingTester.Text = "% Imprinting Bonus";
            // 
            // numericUpDownImprintingBonusTester
            // 
            this.numericUpDownImprintingBonusTester.DecimalPlaces = 5;
            this.numericUpDownImprintingBonusTester.Enabled = false;
            this.numericUpDownImprintingBonusTester.ForeColor = System.Drawing.SystemColors.GrayText;
            this.numericUpDownImprintingBonusTester.Location = new System.Drawing.Point(6, 45);
            this.numericUpDownImprintingBonusTester.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownImprintingBonusTester.Name = "numericUpDownImprintingBonusTester";
            this.numericUpDownImprintingBonusTester.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDownImprintingBonusTester.Size = new System.Drawing.Size(75, 20);
            this.numericUpDownImprintingBonusTester.TabIndex = 4;
            this.numericUpDownImprintingBonusTester.ValueChanged += new System.EventHandler(this.numericUpDownImprintingBonusTester_ValueChanged);
            // 
            // NumericUpDownTestingTE
            // 
            this.NumericUpDownTestingTE.DecimalPlaces = 2;
            this.NumericUpDownTestingTE.ForeColor = System.Drawing.SystemColors.WindowText;
            this.NumericUpDownTestingTE.Location = new System.Drawing.Point(6, 19);
            this.NumericUpDownTestingTE.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.NumericUpDownTestingTE.Name = "NumericUpDownTestingTE";
            this.NumericUpDownTestingTE.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.NumericUpDownTestingTE.Size = new System.Drawing.Size(60, 20);
            this.NumericUpDownTestingTE.TabIndex = 0;
            this.NumericUpDownTestingTE.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.NumericUpDownTestingTE.ValueChanged += new System.EventHandler(this.NumericUpDownTestingTE_ValueChanged);
            // 
            // labelTesterTE
            // 
            this.labelTesterTE.AutoSize = true;
            this.labelTesterTE.Location = new System.Drawing.Point(72, 21);
            this.labelTesterTE.Name = "labelTesterTE";
            this.labelTesterTE.Size = new System.Drawing.Size(120, 13);
            this.labelTesterTE.TabIndex = 1;
            this.labelTesterTE.Text = "% Taming Effectiveness";
            // 
            // groupBoxPossibilities
            // 
            this.groupBoxPossibilities.Controls.Add(this.listViewPossibilities);
            this.groupBoxPossibilities.Location = new System.Drawing.Point(642, 43);
            this.groupBoxPossibilities.Name = "groupBoxPossibilities";
            this.groupBoxPossibilities.Size = new System.Drawing.Size(189, 295);
            this.groupBoxPossibilities.TabIndex = 11;
            this.groupBoxPossibilities.TabStop = false;
            this.groupBoxPossibilities.Text = "Possible Levels";
            // 
            // listViewPossibilities
            // 
            this.listViewPossibilities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderWild,
            this.columnHeaderDom,
            this.columnHeaderTE,
            this.columnHeaderLW});
            this.listViewPossibilities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPossibilities.FullRowSelect = true;
            this.listViewPossibilities.HideSelection = false;
            this.listViewPossibilities.Location = new System.Drawing.Point(3, 16);
            this.listViewPossibilities.MultiSelect = false;
            this.listViewPossibilities.Name = "listViewPossibilities";
            this.listViewPossibilities.ShowGroups = false;
            this.listViewPossibilities.Size = new System.Drawing.Size(183, 276);
            this.listViewPossibilities.TabIndex = 0;
            this.listViewPossibilities.UseCompatibleStateImageBehavior = false;
            this.listViewPossibilities.View = System.Windows.Forms.View.Details;
            this.listViewPossibilities.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            this.listViewPossibilities.SelectedIndexChanged += new System.EventHandler(this.listViewPossibilities_SelectedIndexChanged);
            // 
            // columnHeaderWild
            // 
            this.columnHeaderWild.Text = "Wild";
            this.columnHeaderWild.Width = 34;
            // 
            // columnHeaderDom
            // 
            this.columnHeaderDom.Text = "Dom";
            this.columnHeaderDom.Width = 34;
            // 
            // columnHeaderTE
            // 
            this.columnHeaderTE.Text = "TEff [%]";
            this.columnHeaderTE.Width = 49;
            // 
            // columnHeaderLW
            // 
            this.columnHeaderLW.Text = "WLvl";
            this.columnHeaderLW.Width = 37;
            // 
            // groupBoxDetailsExtractor
            // 
            this.groupBoxDetailsExtractor.Controls.Add(this.panelExtrImpr);
            this.groupBoxDetailsExtractor.Controls.Add(this.panelExtrTE);
            this.groupBoxDetailsExtractor.Location = new System.Drawing.Point(373, 6);
            this.groupBoxDetailsExtractor.Name = "groupBoxDetailsExtractor";
            this.groupBoxDetailsExtractor.Size = new System.Drawing.Size(229, 75);
            this.groupBoxDetailsExtractor.TabIndex = 4;
            this.groupBoxDetailsExtractor.TabStop = false;
            this.groupBoxDetailsExtractor.Text = "Taming-Effectiveness";
            // 
            // panelExtrImpr
            // 
            this.panelExtrImpr.Controls.Add(this.cbExactlyImprinting);
            this.panelExtrImpr.Controls.Add(this.labelImprintingBonus);
            this.panelExtrImpr.Controls.Add(this.lbImprintingCuddleCountExtractor);
            this.panelExtrImpr.Controls.Add(this.numericUpDownImprintingBonusExtractor);
            this.panelExtrImpr.Location = new System.Drawing.Point(6, 16);
            this.panelExtrImpr.Name = "panelExtrImpr";
            this.panelExtrImpr.Size = new System.Drawing.Size(220, 53);
            this.panelExtrImpr.TabIndex = 52;
            this.panelExtrImpr.Visible = false;
            // 
            // cbExactlyImprinting
            // 
            this.cbExactlyImprinting.AutoSize = true;
            this.cbExactlyImprinting.Location = new System.Drawing.Point(3, 29);
            this.cbExactlyImprinting.Name = "cbExactlyImprinting";
            this.cbExactlyImprinting.Size = new System.Drawing.Size(120, 17);
            this.cbExactlyImprinting.TabIndex = 51;
            this.cbExactlyImprinting.Text = "Exactly, don\'t adjust";
            this.cbExactlyImprinting.UseVisualStyleBackColor = true;
            // 
            // labelImprintingBonus
            // 
            this.labelImprintingBonus.AutoSize = true;
            this.labelImprintingBonus.Location = new System.Drawing.Point(86, 5);
            this.labelImprintingBonus.Name = "labelImprintingBonus";
            this.labelImprintingBonus.Size = new System.Drawing.Size(96, 13);
            this.labelImprintingBonus.TabIndex = 7;
            this.labelImprintingBonus.Text = "% Imprinting Bonus";
            // 
            // lbImprintingCuddleCountExtractor
            // 
            this.lbImprintingCuddleCountExtractor.AutoSize = true;
            this.lbImprintingCuddleCountExtractor.Location = new System.Drawing.Point(188, 5);
            this.lbImprintingCuddleCountExtractor.Name = "lbImprintingCuddleCountExtractor";
            this.lbImprintingCuddleCountExtractor.Size = new System.Drawing.Size(25, 13);
            this.lbImprintingCuddleCountExtractor.TabIndex = 50;
            this.lbImprintingCuddleCountExtractor.Text = "(0×)";
            // 
            // numericUpDownImprintingBonusExtractor
            // 
            this.numericUpDownImprintingBonusExtractor.DecimalPlaces = 5;
            this.numericUpDownImprintingBonusExtractor.ForeColor = System.Drawing.SystemColors.GrayText;
            this.numericUpDownImprintingBonusExtractor.Location = new System.Drawing.Point(3, 3);
            this.numericUpDownImprintingBonusExtractor.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownImprintingBonusExtractor.Name = "numericUpDownImprintingBonusExtractor";
            this.numericUpDownImprintingBonusExtractor.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDownImprintingBonusExtractor.Size = new System.Drawing.Size(77, 20);
            this.numericUpDownImprintingBonusExtractor.TabIndex = 6;
            this.numericUpDownImprintingBonusExtractor.ValueChanged += new System.EventHandler(this.numericUpDownImprintingBonusExtractor_ValueChanged);
            this.numericUpDownImprintingBonusExtractor.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // panelExtrTE
            // 
            this.panelExtrTE.Controls.Add(this.labelTE);
            this.panelExtrTE.Controls.Add(this.label2);
            this.panelExtrTE.Controls.Add(this.label1);
            this.panelExtrTE.Controls.Add(this.numericUpDownUpperTEffBound);
            this.panelExtrTE.Controls.Add(this.label3);
            this.panelExtrTE.Controls.Add(this.numericUpDownLowerTEffBound);
            this.panelExtrTE.Location = new System.Drawing.Point(6, 16);
            this.panelExtrTE.Name = "panelExtrTE";
            this.panelExtrTE.Size = new System.Drawing.Size(220, 53);
            this.panelExtrTE.TabIndex = 52;
            // 
            // labelTE
            // 
            this.labelTE.Location = new System.Drawing.Point(3, 31);
            this.labelTE.Name = "labelTE";
            this.labelTE.Size = new System.Drawing.Size(210, 19);
            this.labelTE.TabIndex = 4;
            this.labelTE.Text = "TE differs in chosen possibilities";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Range to test";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(198, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "%";
            // 
            // numericUpDownUpperTEffBound
            // 
            this.numericUpDownUpperTEffBound.ForeColor = System.Drawing.SystemColors.WindowText;
            this.numericUpDownUpperTEffBound.Location = new System.Drawing.Point(147, 3);
            this.numericUpDownUpperTEffBound.Name = "numericUpDownUpperTEffBound";
            this.numericUpDownUpperTEffBound.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDownUpperTEffBound.Size = new System.Drawing.Size(45, 20);
            this.numericUpDownUpperTEffBound.TabIndex = 3;
            this.numericUpDownUpperTEffBound.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownUpperTEffBound.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(131, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "-";
            // 
            // numericUpDownLowerTEffBound
            // 
            this.numericUpDownLowerTEffBound.ForeColor = System.Drawing.SystemColors.WindowText;
            this.numericUpDownLowerTEffBound.Location = new System.Drawing.Point(80, 3);
            this.numericUpDownLowerTEffBound.Name = "numericUpDownLowerTEffBound";
            this.numericUpDownLowerTEffBound.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDownLowerTEffBound.Size = new System.Drawing.Size(45, 20);
            this.numericUpDownLowerTEffBound.TabIndex = 1;
            this.numericUpDownLowerTEffBound.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.numericUpDownLowerTEffBound.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // lbLevel
            // 
            this.lbLevel.AutoSize = true;
            this.lbLevel.Location = new System.Drawing.Point(205, 11);
            this.lbLevel.Name = "lbLevel";
            this.lbLevel.Size = new System.Drawing.Size(33, 13);
            this.lbLevel.TabIndex = 1;
            this.lbLevel.Text = "Level";
            // 
            // lbBreedingValueTester
            // 
            this.lbBreedingValueTester.AutoSize = true;
            this.lbBreedingValueTester.Location = new System.Drawing.Point(270, 0);
            this.lbBreedingValueTester.Name = "lbBreedingValueTester";
            this.lbBreedingValueTester.Size = new System.Drawing.Size(79, 13);
            this.lbBreedingValueTester.TabIndex = 33;
            this.lbBreedingValueTester.Text = "Breeding Value";
            // 
            // lbTesterWildLevel
            // 
            this.lbTesterWildLevel.AutoSize = true;
            this.lbTesterWildLevel.Location = new System.Drawing.Point(18, 0);
            this.lbTesterWildLevel.Name = "lbTesterWildLevel";
            this.lbTesterWildLevel.Size = new System.Drawing.Size(45, 13);
            this.lbTesterWildLevel.TabIndex = 31;
            this.lbTesterWildLevel.Text = "Wild-Lvl";
            // 
            // lbTesterDomLevel
            // 
            this.lbTesterDomLevel.AutoSize = true;
            this.lbTesterDomLevel.Location = new System.Drawing.Point(125, 0);
            this.lbTesterDomLevel.Name = "lbTesterDomLevel";
            this.lbTesterDomLevel.Size = new System.Drawing.Size(46, 13);
            this.lbTesterDomLevel.TabIndex = 32;
            this.lbTesterDomLevel.Text = "Dom-Lvl";
            // 
            // lbInfoYellowStats
            // 
            this.lbInfoYellowStats.Location = new System.Drawing.Point(642, 341);
            this.lbInfoYellowStats.Name = "lbInfoYellowStats";
            this.lbInfoYellowStats.Size = new System.Drawing.Size(255, 126);
            this.lbInfoYellowStats.TabIndex = 15;
            this.lbInfoYellowStats.Text = resources.GetString("lbInfoYellowStats.Text");
            // 
            // labelFootnote
            // 
            this.labelFootnote.Location = new System.Drawing.Point(3, 61);
            this.labelFootnote.Name = "labelFootnote";
            this.labelFootnote.Size = new System.Drawing.Size(295, 16);
            this.labelFootnote.TabIndex = 18;
            this.labelFootnote.Text = "*Creature is not yet tamed and may get better values then.";
            // 
            // labelHBV
            // 
            this.labelHBV.AutoSize = true;
            this.labelHBV.Location = new System.Drawing.Point(270, 0);
            this.labelHBV.Name = "labelHBV";
            this.labelHBV.Size = new System.Drawing.Size(79, 13);
            this.labelHBV.TabIndex = 27;
            this.labelHBV.Text = "Breeding Value";
            // 
            // lbExtractorDomLevel
            // 
            this.lbExtractorDomLevel.AutoSize = true;
            this.lbExtractorDomLevel.Location = new System.Drawing.Point(224, 0);
            this.lbExtractorDomLevel.Name = "lbExtractorDomLevel";
            this.lbExtractorDomLevel.Size = new System.Drawing.Size(46, 13);
            this.lbExtractorDomLevel.TabIndex = 26;
            this.lbExtractorDomLevel.Text = "Dom-Lvl";
            // 
            // lbExtractorWildLevel
            // 
            this.lbExtractorWildLevel.AutoSize = true;
            this.lbExtractorWildLevel.Location = new System.Drawing.Point(123, 0);
            this.lbExtractorWildLevel.Name = "lbExtractorWildLevel";
            this.lbExtractorWildLevel.Size = new System.Drawing.Size(45, 13);
            this.lbExtractorWildLevel.TabIndex = 25;
            this.lbExtractorWildLevel.Text = "Wild-Lvl";
            // 
            // lbSum
            // 
            this.lbSum.AutoSize = true;
            this.lbSum.Location = new System.Drawing.Point(76, 2);
            this.lbSum.Name = "lbSum";
            this.lbSum.Size = new System.Drawing.Size(28, 13);
            this.lbSum.TabIndex = 29;
            this.lbSum.Text = "Sum";
            // 
            // lbSumDom
            // 
            this.lbSumDom.AutoSize = true;
            this.lbSumDom.Location = new System.Drawing.Point(241, 2);
            this.lbSumDom.Name = "lbSumDom";
            this.lbSumDom.Size = new System.Drawing.Size(25, 13);
            this.lbSumDom.TabIndex = 31;
            this.lbSumDom.Text = "100";
            this.lbSumDom.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbSumWild
            // 
            this.lbSumWild.AutoSize = true;
            this.lbSumWild.Location = new System.Drawing.Point(144, 2);
            this.lbSumWild.Name = "lbSumWild";
            this.lbSumWild.Size = new System.Drawing.Size(25, 13);
            this.lbSumWild.TabIndex = 30;
            this.lbSumWild.Text = "100";
            this.lbSumWild.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.libraryFilterToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.devToolStripMenuItem,
            this.serverToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1878, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportValuesToClipboardToolStripMenuItem,
            this.toolStripSeparator13,
            this.setStatusToolStripMenuItem,
            this.multiSetterToolStripMenuItem,
            this.toolStripSeparator5,
            this.deleteSelectedToolStripMenuItem,
            this.findDuplicatesToolStripMenuItem,
            this.toolStripSeparator7,
            this.exactSpawnCommandToolStripMenuItem,
            this.exactSpawnCommandDS2ToolStripMenuItem,
            this.toolStripSeparator25,
            this.copyCreatureToolStripMenuItem,
            this.pasteCreatureToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // exportValuesToClipboardToolStripMenuItem
            // 
            this.exportValuesToClipboardToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.plainTextcurrentValuesToolStripMenuItem,
            this.plainTextbreedingValuesToolStripMenuItem,
            this.toolStripSeparator24,
            this.forSpreadsheetToolStripMenuItem,
            this.editSpreadsheetExportFieldsToolStripMenuItem});
            this.exportValuesToClipboardToolStripMenuItem.Name = "exportValuesToClipboardToolStripMenuItem";
            this.exportValuesToClipboardToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.exportValuesToClipboardToolStripMenuItem.Text = "Export to Clipboard";
            // 
            // plainTextcurrentValuesToolStripMenuItem
            // 
            this.plainTextcurrentValuesToolStripMenuItem.Name = "plainTextcurrentValuesToolStripMenuItem";
            this.plainTextcurrentValuesToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.plainTextcurrentValuesToolStripMenuItem.Text = "Plain Text (current values)";
            this.plainTextcurrentValuesToolStripMenuItem.Click += new System.EventHandler(this.plainTextcurrentValuesToolStripMenuItem_Click);
            // 
            // plainTextbreedingValuesToolStripMenuItem
            // 
            this.plainTextbreedingValuesToolStripMenuItem.Name = "plainTextbreedingValuesToolStripMenuItem";
            this.plainTextbreedingValuesToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.plainTextbreedingValuesToolStripMenuItem.Text = "Plain Text (breeding values)";
            this.plainTextbreedingValuesToolStripMenuItem.Click += new System.EventHandler(this.plainTextbreedingValuesToolStripMenuItem_Click);
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            this.toolStripSeparator24.Size = new System.Drawing.Size(235, 6);
            // 
            // forSpreadsheetToolStripMenuItem
            // 
            this.forSpreadsheetToolStripMenuItem.Name = "forSpreadsheetToolStripMenuItem";
            this.forSpreadsheetToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.forSpreadsheetToolStripMenuItem.Text = "for Spreadsheet";
            this.forSpreadsheetToolStripMenuItem.Click += new System.EventHandler(this.forSpreadsheetToolStripMenuItem_Click);
            // 
            // editSpreadsheetExportFieldsToolStripMenuItem
            // 
            this.editSpreadsheetExportFieldsToolStripMenuItem.Name = "editSpreadsheetExportFieldsToolStripMenuItem";
            this.editSpreadsheetExportFieldsToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.editSpreadsheetExportFieldsToolStripMenuItem.Text = "Edit Spreadsheet export fields…";
            this.editSpreadsheetExportFieldsToolStripMenuItem.Click += new System.EventHandler(this.editSpreadsheetExportFieldsToolStripMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(211, 6);
            // 
            // setStatusToolStripMenuItem
            // 
            this.setStatusToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aliveToolStripMenuItem,
            this.deadToolStripMenuItem,
            this.unavailableToolStripMenuItem,
            this.obeliskToolStripMenuItem1});
            this.setStatusToolStripMenuItem.Name = "setStatusToolStripMenuItem";
            this.setStatusToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.setStatusToolStripMenuItem.Text = "Set Status";
            // 
            // aliveToolStripMenuItem
            // 
            this.aliveToolStripMenuItem.Name = "aliveToolStripMenuItem";
            this.aliveToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.aliveToolStripMenuItem.Text = "Available";
            this.aliveToolStripMenuItem.Click += new System.EventHandler(this.aliveToolStripMenuItem_Click);
            // 
            // deadToolStripMenuItem
            // 
            this.deadToolStripMenuItem.Name = "deadToolStripMenuItem";
            this.deadToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.deadToolStripMenuItem.Text = "Dead";
            this.deadToolStripMenuItem.Click += new System.EventHandler(this.deadToolStripMenuItem_Click);
            // 
            // unavailableToolStripMenuItem
            // 
            this.unavailableToolStripMenuItem.Name = "unavailableToolStripMenuItem";
            this.unavailableToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.unavailableToolStripMenuItem.Text = "Unavailable";
            this.unavailableToolStripMenuItem.Click += new System.EventHandler(this.unavailableToolStripMenuItem_Click);
            // 
            // obeliskToolStripMenuItem1
            // 
            this.obeliskToolStripMenuItem1.Name = "obeliskToolStripMenuItem1";
            this.obeliskToolStripMenuItem1.Size = new System.Drawing.Size(135, 22);
            this.obeliskToolStripMenuItem1.Text = "Obelisk";
            this.obeliskToolStripMenuItem1.Click += new System.EventHandler(this.obeliskToolStripMenuItem1_Click);
            // 
            // multiSetterToolStripMenuItem
            // 
            this.multiSetterToolStripMenuItem.Name = "multiSetterToolStripMenuItem";
            this.multiSetterToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.multiSetterToolStripMenuItem.Text = "MultiSetter…";
            this.multiSetterToolStripMenuItem.Click += new System.EventHandler(this.multiSetterToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(211, 6);
            // 
            // deleteSelectedToolStripMenuItem
            // 
            this.deleteSelectedToolStripMenuItem.Name = "deleteSelectedToolStripMenuItem";
            this.deleteSelectedToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.deleteSelectedToolStripMenuItem.Text = "Remove…";
            this.deleteSelectedToolStripMenuItem.Click += new System.EventHandler(this.deleteSelectedToolStripMenuItem_Click);
            // 
            // findDuplicatesToolStripMenuItem
            // 
            this.findDuplicatesToolStripMenuItem.Name = "findDuplicatesToolStripMenuItem";
            this.findDuplicatesToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.findDuplicatesToolStripMenuItem.Text = "Find Duplicates…";
            this.findDuplicatesToolStripMenuItem.Visible = false;
            this.findDuplicatesToolStripMenuItem.Click += new System.EventHandler(this.findDuplicatesToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(211, 6);
            // 
            // exactSpawnCommandToolStripMenuItem
            // 
            this.exactSpawnCommandToolStripMenuItem.Name = "exactSpawnCommandToolStripMenuItem";
            this.exactSpawnCommandToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.exactSpawnCommandToolStripMenuItem.Text = "ExactSpawnCommand";
            this.exactSpawnCommandToolStripMenuItem.ToolTipText = "Creates a spawn command to spawn this creature in game. This command can crash yo" +
    "ur game";
            this.exactSpawnCommandToolStripMenuItem.Click += new System.EventHandler(this.exactSpawnCommandToolStripMenuItem_Click);
            // 
            // exactSpawnCommandDS2ToolStripMenuItem
            // 
            this.exactSpawnCommandDS2ToolStripMenuItem.Name = "exactSpawnCommandDS2ToolStripMenuItem";
            this.exactSpawnCommandDS2ToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.exactSpawnCommandDS2ToolStripMenuItem.Text = "ExactSpawnCommandDS2";
            this.exactSpawnCommandDS2ToolStripMenuItem.ToolTipText = "Creates a spawn command to spawn this creature in game, used with the mod DinoSto" +
    "rageV2. This command is stable.";
            this.exactSpawnCommandDS2ToolStripMenuItem.Click += new System.EventHandler(this.exactSpawnCommandDS2ToolStripMenuItem_Click);
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            this.toolStripSeparator25.Size = new System.Drawing.Size(211, 6);
            // 
            // copyCreatureToolStripMenuItem
            // 
            this.copyCreatureToolStripMenuItem.Name = "copyCreatureToolStripMenuItem";
            this.copyCreatureToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.copyCreatureToolStripMenuItem.Text = "Copy Creature";
            this.copyCreatureToolStripMenuItem.Click += new System.EventHandler(this.copyCreatureToolStripMenuItem_Click);
            // 
            // pasteCreatureToolStripMenuItem
            // 
            this.pasteCreatureToolStripMenuItem.Name = "pasteCreatureToolStripMenuItem";
            this.pasteCreatureToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.pasteCreatureToolStripMenuItem.Text = "Paste Creature";
            this.pasteCreatureToolStripMenuItem.Click += new System.EventHandler(this.pasteCreatureToolStripMenuItem_Click);
            // 
            // libraryFilterToolStripMenuItem
            // 
            this.libraryFilterToolStripMenuItem.Name = "libraryFilterToolStripMenuItem";
            this.libraryFilterToolStripMenuItem.Size = new System.Drawing.Size(91, 20);
            this.libraryFilterToolStripMenuItem.Text = "Library filter…";
            this.libraryFilterToolStripMenuItem.Click += new System.EventHandler(this.libraryFilterToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSettingsToolStripMenuItem,
            this.toolStripSeparator18,
            this.modValueManagerToolStripMenuItem,
            this.customStatOverridesToolStripMenuItem,
            this.toolStripSeparator1,
            this.extraDataToolStripMenuItem,
            this.toolStripSeparator23,
            this.openJsonDataFolderToolStripMenuItem,
            this.speciesSortingToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // openSettingsToolStripMenuItem
            // 
            this.openSettingsToolStripMenuItem.Name = "openSettingsToolStripMenuItem";
            this.openSettingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemcomma)));
            this.openSettingsToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.openSettingsToolStripMenuItem.Text = "Settings…";
            this.openSettingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(223, 6);
            // 
            // modValueManagerToolStripMenuItem
            // 
            this.modValueManagerToolStripMenuItem.Name = "modValueManagerToolStripMenuItem";
            this.modValueManagerToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.modValueManagerToolStripMenuItem.Text = "Mod value manager…";
            this.modValueManagerToolStripMenuItem.Click += new System.EventHandler(this.loadAdditionalValuesToolStripMenuItem_Click);
            // 
            // customStatOverridesToolStripMenuItem
            // 
            this.customStatOverridesToolStripMenuItem.Name = "customStatOverridesToolStripMenuItem";
            this.customStatOverridesToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.customStatOverridesToolStripMenuItem.Text = "Custom stat overrides…";
            this.customStatOverridesToolStripMenuItem.Click += new System.EventHandler(this.customStatOverridesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(223, 6);
            // 
            // extraDataToolStripMenuItem
            // 
            this.extraDataToolStripMenuItem.Name = "extraDataToolStripMenuItem";
            this.extraDataToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.extraDataToolStripMenuItem.Text = "Extra data…";
            this.extraDataToolStripMenuItem.Click += new System.EventHandler(this.extraDataToolStripMenuItem_Click);
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            this.toolStripSeparator23.Size = new System.Drawing.Size(223, 6);
            // 
            // openJsonDataFolderToolStripMenuItem
            // 
            this.openJsonDataFolderToolStripMenuItem.Name = "openJsonDataFolderToolStripMenuItem";
            this.openJsonDataFolderToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.openJsonDataFolderToolStripMenuItem.Text = "Open json data folder…";
            this.openJsonDataFolderToolStripMenuItem.Click += new System.EventHandler(this.openJsonDataFolderToolStripMenuItem_Click);
            // 
            // speciesSortingToolStripMenuItem
            // 
            this.speciesSortingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetSortingToolStripMenuItem,
            this.resetSortingToolStripMenuItem1,
            this.toolStripSeparator26,
            this.editSortingToolStripMenuItem,
            this.applyChangedSortingToolStripMenuItem,
            this.helpAboutSpeciesSortingToolStripMenuItem});
            this.speciesSortingToolStripMenuItem.Name = "speciesSortingToolStripMenuItem";
            this.speciesSortingToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.speciesSortingToolStripMenuItem.Text = "Species sorting";
            // 
            // resetSortingToolStripMenuItem
            // 
            this.resetSortingToolStripMenuItem.Name = "resetSortingToolStripMenuItem";
            this.resetSortingToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.resetSortingToolStripMenuItem.Text = "Reset sorting to default";
            this.resetSortingToolStripMenuItem.Click += new System.EventHandler(this.resetSortingToolStripMenuItem_Click);
            // 
            // resetSortingToolStripMenuItem1
            // 
            this.resetSortingToolStripMenuItem1.Name = "resetSortingToolStripMenuItem1";
            this.resetSortingToolStripMenuItem1.Size = new System.Drawing.Size(214, 22);
            this.resetSortingToolStripMenuItem1.Text = "Reset sorting";
            this.resetSortingToolStripMenuItem1.Click += new System.EventHandler(this.resetSortingToolStripMenuItem1_Click);
            // 
            // toolStripSeparator26
            // 
            this.toolStripSeparator26.Name = "toolStripSeparator26";
            this.toolStripSeparator26.Size = new System.Drawing.Size(211, 6);
            // 
            // editSortingToolStripMenuItem
            // 
            this.editSortingToolStripMenuItem.Name = "editSortingToolStripMenuItem";
            this.editSortingToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.editSortingToolStripMenuItem.Text = "Edit sorting";
            this.editSortingToolStripMenuItem.Click += new System.EventHandler(this.editSortingToolStripMenuItem_Click);
            // 
            // applyChangedSortingToolStripMenuItem
            // 
            this.applyChangedSortingToolStripMenuItem.Name = "applyChangedSortingToolStripMenuItem";
            this.applyChangedSortingToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.applyChangedSortingToolStripMenuItem.Text = "Apply changed sorting";
            this.applyChangedSortingToolStripMenuItem.Click += new System.EventHandler(this.applyChangedSortingToolStripMenuItem_Click);
            // 
            // helpAboutSpeciesSortingToolStripMenuItem
            // 
            this.helpAboutSpeciesSortingToolStripMenuItem.Name = "helpAboutSpeciesSortingToolStripMenuItem";
            this.helpAboutSpeciesSortingToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.helpAboutSpeciesSortingToolStripMenuItem.Text = "Help about species sorting";
            this.helpAboutSpeciesSortingToolStripMenuItem.Click += new System.EventHandler(this.helpAboutSpeciesSortingToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.toolStripSeparator11,
            this.discordServerToolStripMenuItem,
            this.onlinehelpToolStripMenuItem,
            this.BreedingPlanHelpToolStripMenuItem,
            this.extractionIssuesToolStripMenuItem,
            this.toolStripSeparator12,
            this.checkForUpdatedStatsToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(24, 20);
            this.helpToolStripMenuItem.Text = "?";
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(186, 6);
            // 
            // discordServerToolStripMenuItem
            // 
            this.discordServerToolStripMenuItem.Name = "discordServerToolStripMenuItem";
            this.discordServerToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.discordServerToolStripMenuItem.Text = "Go to Discord server…";
            this.discordServerToolStripMenuItem.Click += new System.EventHandler(this.discordServerToolStripMenuItem_Click);
            // 
            // onlinehelpToolStripMenuItem
            // 
            this.onlinehelpToolStripMenuItem.Name = "onlinehelpToolStripMenuItem";
            this.onlinehelpToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.onlinehelpToolStripMenuItem.Text = "Online Manual…";
            this.onlinehelpToolStripMenuItem.Click += new System.EventHandler(this.onlinehelpToolStripMenuItem_Click);
            // 
            // BreedingPlanHelpToolStripMenuItem
            // 
            this.BreedingPlanHelpToolStripMenuItem.Name = "BreedingPlanHelpToolStripMenuItem";
            this.BreedingPlanHelpToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.BreedingPlanHelpToolStripMenuItem.Text = "Breeding Plan…";
            this.BreedingPlanHelpToolStripMenuItem.Click += new System.EventHandler(this.breedingPlanToolStripMenuItem_Click);
            // 
            // extractionIssuesToolStripMenuItem
            // 
            this.extractionIssuesToolStripMenuItem.Name = "extractionIssuesToolStripMenuItem";
            this.extractionIssuesToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.extractionIssuesToolStripMenuItem.Text = "Extraction Issues…";
            this.extractionIssuesToolStripMenuItem.Click += new System.EventHandler(this.extractionIssuesToolStripMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(186, 6);
            // 
            // checkForUpdatedStatsToolStripMenuItem
            // 
            this.checkForUpdatedStatsToolStripMenuItem.Name = "checkForUpdatedStatsToolStripMenuItem";
            this.checkForUpdatedStatsToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.checkForUpdatedStatsToolStripMenuItem.Text = "Check for Updates";
            this.checkForUpdatedStatsToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatedStatsToolStripMenuItem_Click);
            // 
            // devToolStripMenuItem
            // 
            this.devToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addRandomCreaturesToolStripMenuItem,
            this.colorDefinitionsToClipboardToolStripMenuItem});
            this.devToolStripMenuItem.Name = "devToolStripMenuItem";
            this.devToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.devToolStripMenuItem.Text = "Dev";
            // 
            // addRandomCreaturesToolStripMenuItem
            // 
            this.addRandomCreaturesToolStripMenuItem.Name = "addRandomCreaturesToolStripMenuItem";
            this.addRandomCreaturesToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.addRandomCreaturesToolStripMenuItem.Text = "Add random creatures…";
            this.addRandomCreaturesToolStripMenuItem.Click += new System.EventHandler(this.addRandomCreaturesToolStripMenuItem_Click);
            // 
            // colorDefinitionsToClipboardToolStripMenuItem
            // 
            this.colorDefinitionsToClipboardToolStripMenuItem.Name = "colorDefinitionsToClipboardToolStripMenuItem";
            this.colorDefinitionsToClipboardToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.colorDefinitionsToClipboardToolStripMenuItem.Text = "Color definitions to clipboard";
            this.colorDefinitionsToClipboardToolStripMenuItem.Click += new System.EventHandler(this.colorDefinitionsToClipboardToolStripMenuItem_Click);
            // 
            // serverToolStripMenuItem
            // 
            this.serverToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listenToolStripMenuItem,
            this.listenWithNewTokenToolStripMenuItem,
            this.sendExampleCreatureToolStripMenuItem});
            this.serverToolStripMenuItem.Name = "serverToolStripMenuItem";
            this.serverToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.serverToolStripMenuItem.Text = "Server";
            // 
            // listenToolStripMenuItem
            // 
            this.listenToolStripMenuItem.CheckOnClick = true;
            this.listenToolStripMenuItem.Name = "listenToolStripMenuItem";
            this.listenToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.listenToolStripMenuItem.Text = "Listen";
            this.listenToolStripMenuItem.CheckedChanged += new System.EventHandler(this.listenToolStripMenuItem_CheckedChanged);
            // 
            // listenWithNewTokenToolStripMenuItem
            // 
            this.listenWithNewTokenToolStripMenuItem.Name = "listenWithNewTokenToolStripMenuItem";
            this.listenWithNewTokenToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.listenWithNewTokenToolStripMenuItem.Text = "Listen with new token";
            this.listenWithNewTokenToolStripMenuItem.Click += new System.EventHandler(this.listenWithNewTokenToolStripMenuItem_Click);
            // 
            // sendExampleCreatureToolStripMenuItem
            // 
            this.sendExampleCreatureToolStripMenuItem.Name = "sendExampleCreatureToolStripMenuItem";
            this.sendExampleCreatureToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.sendExampleCreatureToolStripMenuItem.Text = "Send example creature";
            this.sendExampleCreatureToolStripMenuItem.Click += new System.EventHandler(this.sendExampleCreatureToolStripMenuItem_Click);
            // 
            // panelSums
            // 
            this.panelSums.Controls.Add(this.lbShouldBe);
            this.panelSums.Controls.Add(this.lbSumDomSB);
            this.panelSums.Controls.Add(this.lbSum);
            this.panelSums.Controls.Add(this.lbSumWild);
            this.panelSums.Controls.Add(this.lbSumDom);
            this.panelSums.Location = new System.Drawing.Point(3, 26);
            this.panelSums.Name = "panelSums";
            this.panelSums.Size = new System.Drawing.Size(344, 32);
            this.panelSums.TabIndex = 8;
            // 
            // lbShouldBe
            // 
            this.lbShouldBe.AutoSize = true;
            this.lbShouldBe.Location = new System.Drawing.Point(49, 15);
            this.lbShouldBe.Name = "lbShouldBe";
            this.lbShouldBe.Size = new System.Drawing.Size(55, 13);
            this.lbShouldBe.TabIndex = 52;
            this.lbShouldBe.Text = "Should be";
            // 
            // lbSumDomSB
            // 
            this.lbSumDomSB.AutoSize = true;
            this.lbSumDomSB.Location = new System.Drawing.Point(241, 15);
            this.lbSumDomSB.Name = "lbSumDomSB";
            this.lbSumDomSB.Size = new System.Drawing.Size(25, 13);
            this.lbSumDomSB.TabIndex = 51;
            this.lbSumDomSB.Text = "100";
            this.lbSumDomSB.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panelWildTamedBred
            // 
            this.panelWildTamedBred.Controls.Add(this.rbBredExtractor);
            this.panelWildTamedBred.Controls.Add(this.rbTamedExtractor);
            this.panelWildTamedBred.Controls.Add(this.rbWildExtractor);
            this.panelWildTamedBred.Location = new System.Drawing.Point(8, 6);
            this.panelWildTamedBred.Name = "panelWildTamedBred";
            this.panelWildTamedBred.Size = new System.Drawing.Size(191, 25);
            this.panelWildTamedBred.TabIndex = 0;
            // 
            // rbBredExtractor
            // 
            this.rbBredExtractor.AutoSize = true;
            this.rbBredExtractor.Location = new System.Drawing.Point(119, 3);
            this.rbBredExtractor.Name = "rbBredExtractor";
            this.rbBredExtractor.Size = new System.Drawing.Size(47, 17);
            this.rbBredExtractor.TabIndex = 3;
            this.rbBredExtractor.Text = "Bred";
            this.rbBredExtractor.UseVisualStyleBackColor = true;
            this.rbBredExtractor.CheckedChanged += new System.EventHandler(this.radioButtonBred_CheckedChanged);
            // 
            // rbTamedExtractor
            // 
            this.rbTamedExtractor.AutoSize = true;
            this.rbTamedExtractor.Checked = true;
            this.rbTamedExtractor.Location = new System.Drawing.Point(55, 3);
            this.rbTamedExtractor.Name = "rbTamedExtractor";
            this.rbTamedExtractor.Size = new System.Drawing.Size(58, 17);
            this.rbTamedExtractor.TabIndex = 2;
            this.rbTamedExtractor.TabStop = true;
            this.rbTamedExtractor.Text = "Tamed";
            this.rbTamedExtractor.UseVisualStyleBackColor = true;
            this.rbTamedExtractor.CheckedChanged += new System.EventHandler(this.radioButtonTamed_CheckedChanged);
            // 
            // rbWildExtractor
            // 
            this.rbWildExtractor.AutoSize = true;
            this.rbWildExtractor.Location = new System.Drawing.Point(3, 3);
            this.rbWildExtractor.Name = "rbWildExtractor";
            this.rbWildExtractor.Size = new System.Drawing.Size(46, 17);
            this.rbWildExtractor.TabIndex = 1;
            this.rbWildExtractor.Text = "Wild";
            this.rbWildExtractor.UseVisualStyleBackColor = true;
            this.rbWildExtractor.CheckedChanged += new System.EventHandler(this.radioButtonWild_CheckedChanged);
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageStatTesting);
            this.tabControlMain.Controls.Add(this.tabPageExtractor);
            this.tabControlMain.Controls.Add(this.tabPageLibrary);
            this.tabControlMain.Controls.Add(this.tabPageLibraryInfo);
            this.tabControlMain.Controls.Add(this.tabPagePedigree);
            this.tabControlMain.Controls.Add(this.tabPageTaming);
            this.tabControlMain.Controls.Add(this.tabPageBreedingPlan);
            this.tabControlMain.Controls.Add(this.tabPageHatching);
            this.tabControlMain.Controls.Add(this.tabPageRaising);
            this.tabControlMain.Controls.Add(this.tabPageTimer);
            this.tabControlMain.Controls.Add(this.tabPagePlayerTribes);
            this.tabControlMain.Controls.Add(this.tabPageNotes);
            this.tabControlMain.Controls.Add(this.TabPageOCR);
            this.tabControlMain.Controls.Add(this.tabPageExtractionTests);
            this.tabControlMain.Controls.Add(this.tabPageMultiplierTesting);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 103);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 1;
            this.tabControlMain.Size = new System.Drawing.Size(1878, 810);
            this.tabControlMain.TabIndex = 3;
            this.tabControlMain.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageStatTesting
            // 
            this.tabPageStatTesting.AutoScroll = true;
            this.tabPageStatTesting.Controls.Add(this.pictureBoxColorRegionsTester);
            this.tabPageStatTesting.Controls.Add(this.statPotentials1);
            this.tabPageStatTesting.Controls.Add(this.gbStatChart);
            this.tabPageStatTesting.Controls.Add(this.panelWildTamedBredTester);
            this.tabPageStatTesting.Controls.Add(this.groupBox2);
            this.tabPageStatTesting.Controls.Add(this.gpPreviewEdit);
            this.tabPageStatTesting.Controls.Add(this.groupBox1);
            this.tabPageStatTesting.Controls.Add(this.creatureInfoInputTester);
            this.tabPageStatTesting.Location = new System.Drawing.Point(4, 22);
            this.tabPageStatTesting.Name = "tabPageStatTesting";
            this.tabPageStatTesting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageStatTesting.Size = new System.Drawing.Size(1870, 784);
            this.tabPageStatTesting.TabIndex = 1;
            this.tabPageStatTesting.Text = "Stat Testing";
            this.tabPageStatTesting.UseVisualStyleBackColor = true;
            // 
            // pictureBoxColorRegionsTester
            // 
            this.pictureBoxColorRegionsTester.Location = new System.Drawing.Point(641, 489);
            this.pictureBoxColorRegionsTester.Name = "pictureBoxColorRegionsTester";
            this.pictureBoxColorRegionsTester.Size = new System.Drawing.Size(256, 256);
            this.pictureBoxColorRegionsTester.TabIndex = 13;
            this.pictureBoxColorRegionsTester.TabStop = false;
            this.pictureBoxColorRegionsTester.Click += new System.EventHandler(this.pictureBoxColorRegionsTester_Click);
            // 
            // statPotentials1
            // 
            this.statPotentials1.Location = new System.Drawing.Point(860, 9);
            this.statPotentials1.Name = "statPotentials1";
            this.statPotentials1.Size = new System.Drawing.Size(293, 433);
            this.statPotentials1.TabIndex = 12;
            // 
            // gbStatChart
            // 
            this.gbStatChart.Controls.Add(this.radarChart1);
            this.gbStatChart.Location = new System.Drawing.Point(641, 9);
            this.gbStatChart.Name = "gbStatChart";
            this.gbStatChart.Size = new System.Drawing.Size(213, 228);
            this.gbStatChart.TabIndex = 11;
            this.gbStatChart.TabStop = false;
            this.gbStatChart.Text = "Stat-Chart";
            // 
            // radarChart1
            // 
            this.radarChart1.Image = ((System.Drawing.Image)(resources.GetObject("radarChart1.Image")));
            this.radarChart1.Location = new System.Drawing.Point(6, 19);
            this.radarChart1.Name = "radarChart1";
            this.radarChart1.Size = new System.Drawing.Size(200, 200);
            this.radarChart1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.radarChart1.TabIndex = 10;
            this.radarChart1.TabStop = false;
            // 
            // panelWildTamedBredTester
            // 
            this.panelWildTamedBredTester.Controls.Add(this.rbBredTester);
            this.panelWildTamedBredTester.Controls.Add(this.rbTamedTester);
            this.panelWildTamedBredTester.Controls.Add(this.rbWildTester);
            this.panelWildTamedBredTester.Location = new System.Drawing.Point(8, 6);
            this.panelWildTamedBredTester.Name = "panelWildTamedBredTester";
            this.panelWildTamedBredTester.Size = new System.Drawing.Size(191, 25);
            this.panelWildTamedBredTester.TabIndex = 0;
            // 
            // rbBredTester
            // 
            this.rbBredTester.AutoSize = true;
            this.rbBredTester.Location = new System.Drawing.Point(119, 3);
            this.rbBredTester.Name = "rbBredTester";
            this.rbBredTester.Size = new System.Drawing.Size(47, 17);
            this.rbBredTester.TabIndex = 3;
            this.rbBredTester.Text = "Bred";
            this.rbBredTester.UseVisualStyleBackColor = true;
            this.rbBredTester.CheckedChanged += new System.EventHandler(this.radioButtonTesterBred_CheckedChanged);
            // 
            // rbTamedTester
            // 
            this.rbTamedTester.AutoSize = true;
            this.rbTamedTester.Checked = true;
            this.rbTamedTester.Location = new System.Drawing.Point(55, 3);
            this.rbTamedTester.Name = "rbTamedTester";
            this.rbTamedTester.Size = new System.Drawing.Size(58, 17);
            this.rbTamedTester.TabIndex = 2;
            this.rbTamedTester.TabStop = true;
            this.rbTamedTester.Text = "Tamed";
            this.rbTamedTester.UseVisualStyleBackColor = true;
            this.rbTamedTester.CheckedChanged += new System.EventHandler(this.radioButtonTesterTamed_CheckedChanged);
            // 
            // rbWildTester
            // 
            this.rbWildTester.AutoSize = true;
            this.rbWildTester.Location = new System.Drawing.Point(3, 3);
            this.rbWildTester.Name = "rbWildTester";
            this.rbWildTester.Size = new System.Drawing.Size(46, 17);
            this.rbWildTester.TabIndex = 1;
            this.rbWildTester.Text = "Wild";
            this.rbWildTester.UseVisualStyleBackColor = true;
            this.rbWildTester.CheckedChanged += new System.EventHandler(this.radioButtonTesterWild_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.flowLayoutPanelStatIOsTester);
            this.groupBox2.Location = new System.Drawing.Point(8, 37);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(359, 639);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Stats";
            // 
            // flowLayoutPanelStatIOsTester
            // 
            this.flowLayoutPanelStatIOsTester.AutoScroll = true;
            this.flowLayoutPanelStatIOsTester.Controls.Add(this.panel2);
            this.flowLayoutPanelStatIOsTester.Controls.Add(this.panelStatTesterFootnote);
            this.flowLayoutPanelStatIOsTester.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelStatIOsTester.Location = new System.Drawing.Point(6, 19);
            this.flowLayoutPanelStatIOsTester.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanelStatIOsTester.Name = "flowLayoutPanelStatIOsTester";
            this.flowLayoutPanelStatIOsTester.Size = new System.Drawing.Size(353, 617);
            this.flowLayoutPanelStatIOsTester.TabIndex = 53;
            this.flowLayoutPanelStatIOsTester.WrapContents = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.lbTesterWildLevel);
            this.panel2.Controls.Add(this.lbTesterDomLevel);
            this.panel2.Controls.Add(this.lbBreedingValueTester);
            this.panel2.Controls.Add(this.lbCurrentValue);
            this.panel2.Location = new System.Drawing.Point(0, 3);
            this.panel2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(350, 17);
            this.panel2.TabIndex = 54;
            // 
            // lbCurrentValue
            // 
            this.lbCurrentValue.AutoSize = true;
            this.lbCurrentValue.Location = new System.Drawing.Point(178, 0);
            this.lbCurrentValue.Name = "lbCurrentValue";
            this.lbCurrentValue.Size = new System.Drawing.Size(71, 13);
            this.lbCurrentValue.TabIndex = 36;
            this.lbCurrentValue.Text = "Current Value";
            // 
            // panelStatTesterFootnote
            // 
            this.panelStatTesterFootnote.Controls.Add(this.LbWarningLevel255);
            this.panelStatTesterFootnote.Controls.Add(this.lbWildLevelTester);
            this.panelStatTesterFootnote.Controls.Add(this.labelDomLevelSum);
            this.panelStatTesterFootnote.Controls.Add(this.labelTesterTotalLevel);
            this.panelStatTesterFootnote.Controls.Add(this.lbNotYetTamed);
            this.panelStatTesterFootnote.Location = new System.Drawing.Point(3, 26);
            this.panelStatTesterFootnote.Name = "panelStatTesterFootnote";
            this.panelStatTesterFootnote.Size = new System.Drawing.Size(344, 128);
            this.panelStatTesterFootnote.TabIndex = 54;
            // 
            // LbWarningLevel255
            // 
            this.LbWarningLevel255.BackColor = System.Drawing.Color.Firebrick;
            this.LbWarningLevel255.ForeColor = System.Drawing.Color.White;
            this.LbWarningLevel255.Location = new System.Drawing.Point(8, 52);
            this.LbWarningLevel255.Name = "LbWarningLevel255";
            this.LbWarningLevel255.Padding = new System.Windows.Forms.Padding(3);
            this.LbWarningLevel255.Size = new System.Drawing.Size(333, 70);
            this.LbWarningLevel255.TabIndex = 50;
            // 
            // lbWildLevelTester
            // 
            this.lbWildLevelTester.AutoSize = true;
            this.lbWildLevelTester.Location = new System.Drawing.Point(8, 16);
            this.lbWildLevelTester.Name = "lbWildLevelTester";
            this.lbWildLevelTester.Size = new System.Drawing.Size(79, 13);
            this.lbWildLevelTester.TabIndex = 13;
            this.lbWildLevelTester.Text = "PreTame Level";
            // 
            // labelDomLevelSum
            // 
            this.labelDomLevelSum.AutoSize = true;
            this.labelDomLevelSum.Location = new System.Drawing.Point(8, 0);
            this.labelDomLevelSum.Name = "labelDomLevelSum";
            this.labelDomLevelSum.Size = new System.Drawing.Size(63, 13);
            this.labelDomLevelSum.TabIndex = 46;
            this.labelDomLevelSum.Text = "Dom Levels";
            // 
            // labelTesterTotalLevel
            // 
            this.labelTesterTotalLevel.Location = new System.Drawing.Point(147, 0);
            this.labelTesterTotalLevel.Name = "labelTesterTotalLevel";
            this.labelTesterTotalLevel.Size = new System.Drawing.Size(143, 13);
            this.labelTesterTotalLevel.TabIndex = 49;
            this.labelTesterTotalLevel.Text = "Total Level";
            this.labelTesterTotalLevel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbNotYetTamed
            // 
            this.lbNotYetTamed.Location = new System.Drawing.Point(6, 34);
            this.lbNotYetTamed.Name = "lbNotYetTamed";
            this.lbNotYetTamed.Size = new System.Drawing.Size(285, 16);
            this.lbNotYetTamed.TabIndex = 41;
            this.lbNotYetTamed.Text = "*Creature is not yet tamed and may get better values then.";
            this.lbNotYetTamed.Visible = false;
            // 
            // gpPreviewEdit
            // 
            this.gpPreviewEdit.Controls.Add(this.lbCurrentCreature);
            this.gpPreviewEdit.Controls.Add(this.labelCurrentTesterCreature);
            this.gpPreviewEdit.Controls.Add(this.lbTestingInfo);
            this.gpPreviewEdit.Location = new System.Drawing.Point(373, 84);
            this.gpPreviewEdit.Name = "gpPreviewEdit";
            this.gpPreviewEdit.Size = new System.Drawing.Size(229, 91);
            this.gpPreviewEdit.TabIndex = 3;
            this.gpPreviewEdit.TabStop = false;
            this.gpPreviewEdit.Text = "Preview / Edit";
            // 
            // lbCurrentCreature
            // 
            this.lbCurrentCreature.AutoSize = true;
            this.lbCurrentCreature.Location = new System.Drawing.Point(6, 35);
            this.lbCurrentCreature.Name = "lbCurrentCreature";
            this.lbCurrentCreature.Size = new System.Drawing.Size(84, 13);
            this.lbCurrentCreature.TabIndex = 39;
            this.lbCurrentCreature.Text = "Current Creature";
            // 
            // labelCurrentTesterCreature
            // 
            this.labelCurrentTesterCreature.AutoSize = true;
            this.labelCurrentTesterCreature.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentTesterCreature.Location = new System.Drawing.Point(6, 52);
            this.labelCurrentTesterCreature.Name = "labelCurrentTesterCreature";
            this.labelCurrentTesterCreature.Size = new System.Drawing.Size(87, 13);
            this.labelCurrentTesterCreature.TabIndex = 38;
            this.labelCurrentTesterCreature.Text = "CreatureName";
            // 
            // lbTestingInfo
            // 
            this.lbTestingInfo.Location = new System.Drawing.Point(6, 16);
            this.lbTestingInfo.Name = "lbTestingInfo";
            this.lbTestingInfo.Size = new System.Drawing.Size(217, 25);
            this.lbTestingInfo.TabIndex = 37;
            this.lbTestingInfo.Text = "Preview or edit levels of a creature.";
            // 
            // creatureInfoInputTester
            // 
            this.creatureInfoInputTester.ColorIdsAlsoPossible = null;
            this.creatureInfoInputTester.CooldownUntil = null;
            this.creatureInfoInputTester.CreatureFlags = ARKBreedingStats.Library.CreatureFlags.None;
            this.creatureInfoInputTester.CreatureName = "";
            this.creatureInfoInputTester.CreatureNote = "";
            this.creatureInfoInputTester.CreatureOwner = "";
            this.creatureInfoInputTester.CreatureServer = "";
            this.creatureInfoInputTester.CreatureSex = ARKBreedingStats.Library.Sex.Unknown;
            this.creatureInfoInputTester.CreatureStatus = ARKBreedingStats.Library.CreatureStatus.Available;
            this.creatureInfoInputTester.CreatureTribe = "";
            this.creatureInfoInputTester.DomesticatedAt = new System.DateTime(2014, 12, 31, 0, 0, 0, 0);
            this.creatureInfoInputTester.Father = null;
            this.creatureInfoInputTester.GrowingUntil = null;
            this.creatureInfoInputTester.Location = new System.Drawing.Point(373, 184);
            this.creatureInfoInputTester.LockServer = false;
            this.creatureInfoInputTester.Mother = null;
            this.creatureInfoInputTester.MutationCounterFather = 0;
            this.creatureInfoInputTester.MutationCounterMother = 0;
            this.creatureInfoInputTester.Name = "creatureInfoInputTester";
            this.creatureInfoInputTester.OwnerLock = false;
            this.creatureInfoInputTester.RegionColors = new byte[] {
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0))};
            this.creatureInfoInputTester.Size = new System.Drawing.Size(262, 590);
            this.creatureInfoInputTester.TabIndex = 4;
            this.creatureInfoInputTester.TribeLock = false;
            this.creatureInfoInputTester.Add2LibraryClicked += new System.Action<ARKBreedingStats.CreatureInfoInput>(this.creatureInfoInputTester_Add2Library_Clicked);
            this.creatureInfoInputTester.Save2LibraryClicked += new System.Action<ARKBreedingStats.CreatureInfoInput>(this.creatureInfoInputTester_Save2Library_Clicked);
            this.creatureInfoInputTester.ParentListRequested += new System.Action<ARKBreedingStats.CreatureInfoInput>(this.CreatureInfoInput_ParentListRequested);
            // 
            // tabPageExtractor
            // 
            this.tabPageExtractor.AutoScroll = true;
            this.tabPageExtractor.Controls.Add(this.LbAsa);
            this.tabPageExtractor.Controls.Add(this.LbBlueprintPath);
            this.tabPageExtractor.Controls.Add(this.BtCopyIssueDumpToClipboard);
            this.tabPageExtractor.Controls.Add(this.llOnlineHelpExtractionIssues);
            this.tabPageExtractor.Controls.Add(this.PbCreatureColorsExtractor);
            this.tabPageExtractor.Controls.Add(this.groupBoxRadarChartExtractor);
            this.tabPageExtractor.Controls.Add(this.lbImprintingFailInfo);
            this.tabPageExtractor.Controls.Add(this.groupBoxTamingInfo);
            this.tabPageExtractor.Controls.Add(this.button2TamingCalc);
            this.tabPageExtractor.Controls.Add(this.gbStatsExtractor);
            this.tabPageExtractor.Controls.Add(this.btExtractLevels);
            this.tabPageExtractor.Controls.Add(this.cbQuickWildCheck);
            this.tabPageExtractor.Controls.Add(this.panelWildTamedBred);
            this.tabPageExtractor.Controls.Add(this.lbInfoYellowStats);
            this.tabPageExtractor.Controls.Add(this.groupBoxDetailsExtractor);
            this.tabPageExtractor.Controls.Add(this.groupBoxPossibilities);
            this.tabPageExtractor.Controls.Add(this.lbLevel);
            this.tabPageExtractor.Controls.Add(this.labelErrorHelp);
            this.tabPageExtractor.Controls.Add(this.creatureAnalysis1);
            this.tabPageExtractor.Controls.Add(this.parentInheritanceExtractor);
            this.tabPageExtractor.Controls.Add(this.numericUpDownLevel);
            this.tabPageExtractor.Controls.Add(this.creatureInfoInputExtractor);
            this.tabPageExtractor.Location = new System.Drawing.Point(4, 22);
            this.tabPageExtractor.Name = "tabPageExtractor";
            this.tabPageExtractor.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExtractor.Size = new System.Drawing.Size(1870, 784);
            this.tabPageExtractor.TabIndex = 0;
            this.tabPageExtractor.Text = "Extractor";
            this.tabPageExtractor.UseVisualStyleBackColor = true;
            // 
            // LbAsa
            // 
            this.LbAsa.AutoSize = true;
            this.LbAsa.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbAsa.ForeColor = System.Drawing.Color.DarkGray;
            this.LbAsa.Location = new System.Drawing.Point(295, 66);
            this.LbAsa.Name = "LbAsa";
            this.LbAsa.Size = new System.Drawing.Size(20, 9);
            this.LbAsa.TabIndex = 56;
            this.LbAsa.Text = "ASA";
            // 
            // LbBlueprintPath
            // 
            this.LbBlueprintPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbBlueprintPath.ForeColor = System.Drawing.Color.DarkGray;
            this.LbBlueprintPath.Location = new System.Drawing.Point(8, 34);
            this.LbBlueprintPath.Name = "LbBlueprintPath";
            this.LbBlueprintPath.Size = new System.Drawing.Size(307, 41);
            this.LbBlueprintPath.TabIndex = 54;
            this.LbBlueprintPath.Text = "/Game/​PrimalEarth/​Dinos/​Direwolf/ ​Direwolf_Character_BP.Direwolf_Character_BP" +
    "";
            this.LbBlueprintPath.Click += new System.EventHandler(this.LbBlueprintPath_Click);
            // 
            // BtCopyIssueDumpToClipboard
            // 
            this.BtCopyIssueDumpToClipboard.Location = new System.Drawing.Point(642, 703);
            this.BtCopyIssueDumpToClipboard.Name = "BtCopyIssueDumpToClipboard";
            this.BtCopyIssueDumpToClipboard.Size = new System.Drawing.Size(345, 23);
            this.BtCopyIssueDumpToClipboard.TabIndex = 53;
            this.BtCopyIssueDumpToClipboard.Text = "Copy library dump to clipboard for a report";
            this.BtCopyIssueDumpToClipboard.UseVisualStyleBackColor = true;
            this.BtCopyIssueDumpToClipboard.Click += new System.EventHandler(this.BtCopyIssueDumpToClipboard_Click);
            // 
            // llOnlineHelpExtractionIssues
            // 
            this.llOnlineHelpExtractionIssues.AutoSize = true;
            this.llOnlineHelpExtractionIssues.Location = new System.Drawing.Point(645, 599);
            this.llOnlineHelpExtractionIssues.Name = "llOnlineHelpExtractionIssues";
            this.llOnlineHelpExtractionIssues.Size = new System.Drawing.Size(141, 13);
            this.llOnlineHelpExtractionIssues.TabIndex = 50;
            this.llOnlineHelpExtractionIssues.TabStop = true;
            this.llOnlineHelpExtractionIssues.Text = "Red Stat-boxes: Online-Help";
            this.llOnlineHelpExtractionIssues.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llOnlineHelpExtractionIssues_LinkClicked);
            // 
            // PbCreatureColorsExtractor
            // 
            this.PbCreatureColorsExtractor.Location = new System.Drawing.Point(641, 489);
            this.PbCreatureColorsExtractor.Name = "PbCreatureColorsExtractor";
            this.PbCreatureColorsExtractor.Size = new System.Drawing.Size(256, 256);
            this.PbCreatureColorsExtractor.TabIndex = 51;
            this.PbCreatureColorsExtractor.TabStop = false;
            this.PbCreatureColorsExtractor.Click += new System.EventHandler(this.PbCreatureColorsExtractor_Click);
            // 
            // groupBoxRadarChartExtractor
            // 
            this.groupBoxRadarChartExtractor.Controls.Add(this.radarChartExtractor);
            this.groupBoxRadarChartExtractor.Location = new System.Drawing.Point(837, 6);
            this.groupBoxRadarChartExtractor.Name = "groupBoxRadarChartExtractor";
            this.groupBoxRadarChartExtractor.Size = new System.Drawing.Size(150, 163);
            this.groupBoxRadarChartExtractor.TabIndex = 11;
            this.groupBoxRadarChartExtractor.TabStop = false;
            this.groupBoxRadarChartExtractor.Text = "Stat-Chart";
            // 
            // radarChartExtractor
            // 
            this.radarChartExtractor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radarChartExtractor.Image = ((System.Drawing.Image)(resources.GetObject("radarChartExtractor.Image")));
            this.radarChartExtractor.Location = new System.Drawing.Point(3, 16);
            this.radarChartExtractor.Name = "radarChartExtractor";
            this.radarChartExtractor.Size = new System.Drawing.Size(144, 144);
            this.radarChartExtractor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.radarChartExtractor.TabIndex = 10;
            this.radarChartExtractor.TabStop = false;
            // 
            // lbImprintingFailInfo
            // 
            this.lbImprintingFailInfo.BackColor = System.Drawing.Color.MistyRose;
            this.lbImprintingFailInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbImprintingFailInfo.ForeColor = System.Drawing.Color.Maroon;
            this.lbImprintingFailInfo.Location = new System.Drawing.Point(641, 612);
            this.lbImprintingFailInfo.Name = "lbImprintingFailInfo";
            this.lbImprintingFailInfo.Size = new System.Drawing.Size(443, 88);
            this.lbImprintingFailInfo.TabIndex = 49;
            this.lbImprintingFailInfo.Text = "If the creature is imprinted the extraction may fail because the game sometimes \"" +
    "forgets\" to increase some stat-values during the imprinting-process. Usually it " +
    "works after a server-restart.";
            this.lbImprintingFailInfo.Visible = false;
            // 
            // groupBoxTamingInfo
            // 
            this.groupBoxTamingInfo.Controls.Add(this.labelTamingInfo);
            this.groupBoxTamingInfo.Location = new System.Drawing.Point(642, 60);
            this.groupBoxTamingInfo.Name = "groupBoxTamingInfo";
            this.groupBoxTamingInfo.Size = new System.Drawing.Size(174, 423);
            this.groupBoxTamingInfo.TabIndex = 48;
            this.groupBoxTamingInfo.TabStop = false;
            this.groupBoxTamingInfo.Text = "Taming Info";
            // 
            // labelTamingInfo
            // 
            this.labelTamingInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTamingInfo.Location = new System.Drawing.Point(3, 16);
            this.labelTamingInfo.Name = "labelTamingInfo";
            this.labelTamingInfo.Size = new System.Drawing.Size(168, 404);
            this.labelTamingInfo.TabIndex = 0;
            // 
            // button2TamingCalc
            // 
            this.button2TamingCalc.Location = new System.Drawing.Point(642, 32);
            this.button2TamingCalc.Name = "button2TamingCalc";
            this.button2TamingCalc.Size = new System.Drawing.Size(177, 23);
            this.button2TamingCalc.TabIndex = 9;
            this.button2TamingCalc.Text = "Taming Calculator";
            this.button2TamingCalc.UseVisualStyleBackColor = true;
            this.button2TamingCalc.Visible = false;
            this.button2TamingCalc.Click += new System.EventHandler(this.button2TamingCalc_Click);
            // 
            // gbStatsExtractor
            // 
            this.gbStatsExtractor.Controls.Add(this.flowLayoutPanelStatIOsExtractor);
            this.gbStatsExtractor.Location = new System.Drawing.Point(8, 76);
            this.gbStatsExtractor.Name = "gbStatsExtractor";
            this.gbStatsExtractor.Size = new System.Drawing.Size(359, 639);
            this.gbStatsExtractor.TabIndex = 3;
            this.gbStatsExtractor.TabStop = false;
            this.gbStatsExtractor.Text = "Stats";
            // 
            // flowLayoutPanelStatIOsExtractor
            // 
            this.flowLayoutPanelStatIOsExtractor.AutoScroll = true;
            this.flowLayoutPanelStatIOsExtractor.Controls.Add(this.panel1);
            this.flowLayoutPanelStatIOsExtractor.Controls.Add(this.panelSums);
            this.flowLayoutPanelStatIOsExtractor.Controls.Add(this.labelFootnote);
            this.flowLayoutPanelStatIOsExtractor.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelStatIOsExtractor.Location = new System.Drawing.Point(6, 19);
            this.flowLayoutPanelStatIOsExtractor.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanelStatIOsExtractor.Name = "flowLayoutPanelStatIOsExtractor";
            this.flowLayoutPanelStatIOsExtractor.Size = new System.Drawing.Size(353, 617);
            this.flowLayoutPanelStatIOsExtractor.TabIndex = 52;
            this.flowLayoutPanelStatIOsExtractor.WrapContents = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.lbCurrentStatEx);
            this.panel1.Controls.Add(this.lbExtractorWildLevel);
            this.panel1.Controls.Add(this.labelHBV);
            this.panel1.Controls.Add(this.lbExtractorDomLevel);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(344, 17);
            this.panel1.TabIndex = 53;
            // 
            // lbCurrentStatEx
            // 
            this.lbCurrentStatEx.AutoSize = true;
            this.lbCurrentStatEx.Location = new System.Drawing.Point(3, 0);
            this.lbCurrentStatEx.Name = "lbCurrentStatEx";
            this.lbCurrentStatEx.Size = new System.Drawing.Size(90, 13);
            this.lbCurrentStatEx.TabIndex = 50;
            this.lbCurrentStatEx.Text = "Current stat-value";
            // 
            // btExtractLevels
            // 
            this.btExtractLevels.Location = new System.Drawing.Point(373, 110);
            this.btExtractLevels.Name = "btExtractLevels";
            this.btExtractLevels.Size = new System.Drawing.Size(229, 68);
            this.btExtractLevels.TabIndex = 6;
            this.btExtractLevels.Text = "Extract Level Distribution";
            this.btExtractLevels.UseVisualStyleBackColor = true;
            this.btExtractLevels.Click += new System.EventHandler(this.buttonExtract_Click);
            // 
            // cbQuickWildCheck
            // 
            this.cbQuickWildCheck.AutoSize = true;
            this.cbQuickWildCheck.Location = new System.Drawing.Point(608, 8);
            this.cbQuickWildCheck.Name = "cbQuickWildCheck";
            this.cbQuickWildCheck.Size = new System.Drawing.Size(155, 17);
            this.cbQuickWildCheck.TabIndex = 8;
            this.cbQuickWildCheck.Text = "Quick Wild-Creature Check";
            this.cbQuickWildCheck.UseVisualStyleBackColor = true;
            this.cbQuickWildCheck.CheckedChanged += new System.EventHandler(this.checkBoxQuickWildCheck_CheckedChanged);
            // 
            // labelErrorHelp
            // 
            this.labelErrorHelp.Location = new System.Drawing.Point(642, 43);
            this.labelErrorHelp.Name = "labelErrorHelp";
            this.labelErrorHelp.Size = new System.Drawing.Size(239, 569);
            this.labelErrorHelp.TabIndex = 40;
            this.labelErrorHelp.Text = resources.GetString("labelErrorHelp.Text");
            // 
            // creatureAnalysis1
            // 
            this.creatureAnalysis1.Location = new System.Drawing.Point(903, 265);
            this.creatureAnalysis1.Name = "creatureAnalysis1";
            this.creatureAnalysis1.Size = new System.Drawing.Size(346, 199);
            this.creatureAnalysis1.TabIndex = 55;
            // 
            // parentInheritanceExtractor
            // 
            this.parentInheritanceExtractor.Location = new System.Drawing.Point(903, 470);
            this.parentInheritanceExtractor.Name = "parentInheritanceExtractor";
            this.parentInheritanceExtractor.Size = new System.Drawing.Size(337, 182);
            this.parentInheritanceExtractor.TabIndex = 52;
            // 
            // numericUpDownLevel
            // 
            this.numericUpDownLevel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.numericUpDownLevel.Location = new System.Drawing.Point(244, 9);
            this.numericUpDownLevel.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownLevel.Name = "numericUpDownLevel";
            this.numericUpDownLevel.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDownLevel.Size = new System.Drawing.Size(56, 20);
            this.numericUpDownLevel.TabIndex = 2;
            this.numericUpDownLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLevel.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // creatureInfoInputExtractor
            // 
            this.creatureInfoInputExtractor.ColorIdsAlsoPossible = null;
            this.creatureInfoInputExtractor.CooldownUntil = null;
            this.creatureInfoInputExtractor.CreatureFlags = ARKBreedingStats.Library.CreatureFlags.None;
            this.creatureInfoInputExtractor.CreatureName = "";
            this.creatureInfoInputExtractor.CreatureNote = "";
            this.creatureInfoInputExtractor.CreatureOwner = "";
            this.creatureInfoInputExtractor.CreatureServer = "";
            this.creatureInfoInputExtractor.CreatureSex = ARKBreedingStats.Library.Sex.Unknown;
            this.creatureInfoInputExtractor.CreatureStatus = ARKBreedingStats.Library.CreatureStatus.Available;
            this.creatureInfoInputExtractor.CreatureTribe = "";
            this.creatureInfoInputExtractor.DomesticatedAt = new System.DateTime(2014, 12, 31, 0, 0, 0, 0);
            this.creatureInfoInputExtractor.Father = null;
            this.creatureInfoInputExtractor.GrowingUntil = null;
            this.creatureInfoInputExtractor.Location = new System.Drawing.Point(373, 184);
            this.creatureInfoInputExtractor.LockServer = false;
            this.creatureInfoInputExtractor.Mother = null;
            this.creatureInfoInputExtractor.MutationCounterFather = 0;
            this.creatureInfoInputExtractor.MutationCounterMother = 0;
            this.creatureInfoInputExtractor.Name = "creatureInfoInputExtractor";
            this.creatureInfoInputExtractor.OwnerLock = false;
            this.creatureInfoInputExtractor.RegionColors = new byte[] {
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0))};
            this.creatureInfoInputExtractor.Size = new System.Drawing.Size(262, 590);
            this.creatureInfoInputExtractor.TabIndex = 7;
            this.creatureInfoInputExtractor.TribeLock = false;
            this.creatureInfoInputExtractor.Add2LibraryClicked += new System.Action<ARKBreedingStats.CreatureInfoInput>(this.creatureInfoInputExtractor_Add2Library_Clicked);
            this.creatureInfoInputExtractor.ParentListRequested += new System.Action<ARKBreedingStats.CreatureInfoInput>(this.CreatureInfoInput_ParentListRequested);
            // 
            // tabPageLibrary
            // 
            this.tabPageLibrary.Controls.Add(this.tableLayoutPanelLibrary);
            this.tabPageLibrary.Location = new System.Drawing.Point(4, 22);
            this.tabPageLibrary.Name = "tabPageLibrary";
            this.tabPageLibrary.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLibrary.Size = new System.Drawing.Size(1870, 749);
            this.tabPageLibrary.TabIndex = 2;
            this.tabPageLibrary.Text = "Library";
            this.tabPageLibrary.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelLibrary
            // 
            this.tableLayoutPanelLibrary.ColumnCount = 2;
            this.tableLayoutPanelLibrary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelLibrary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLibrary.Controls.Add(this.listViewLibrary, 1, 0);
            this.tableLayoutPanelLibrary.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanelLibrary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelLibrary.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelLibrary.Name = "tableLayoutPanelLibrary";
            this.tableLayoutPanelLibrary.RowCount = 1;
            this.tableLayoutPanelLibrary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLibrary.Size = new System.Drawing.Size(1864, 743);
            this.tableLayoutPanelLibrary.TabIndex = 4;
            // 
            // listViewLibrary
            // 
            this.listViewLibrary.AllowColumnReorder = true;
            this.listViewLibrary.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderOwner,
            this.columnHeaderNote,
            this.columnHeaderServer,
            this.columnHeaderSex,
            this.columnHeaderDomesticated,
            this.columnHeaderTopness,
            this.columnHeaderTopStatsNr,
            this.columnHeaderGen,
            this.columnHeaderFound,
            this.columnHeaderMutations,
            this.columnHeaderCooldown,
            this.columnHeaderHP,
            this.columnHeaderSt,
            this.columnHeaderTo,
            this.columnHeaderOx,
            this.columnHeaderFo,
            this.columnHeaderWa,
            this.columnHeaderTm,
            this.columnHeaderWe,
            this.columnHeaderDm,
            this.columnHeaderSp,
            this.columnHeaderFr,
            this.columnHeaderCr,
            this.columnHeaderHPM,
            this.columnHeaderStM,
            this.columnHeaderToM,
            this.columnHeaderOxM,
            this.columnHeaderFoM,
            this.columnHeaderWaM,
            this.columnHeaderTmM,
            this.columnHeaderWeM,
            this.columnHeaderDmM,
            this.columnHeaderSpM,
            this.columnHeaderFrM,
            this.columnHeaderCrM,
            this.columnHeaderColor0,
            this.columnHeaderColor1,
            this.columnHeaderColor2,
            this.columnHeaderColor3,
            this.columnHeaderColor4,
            this.columnHeaderColor5,
            this.columnHeaderSpecies,
            this.columnHeaderStatus,
            this.columnHeaderTribe,
            this.columnHeaderStatusIcon,
            this.columnHeaderMutagen});
            this.listViewLibrary.ContextMenuStrip = this.contextMenuStripLibrary;
            this.listViewLibrary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewLibrary.FullRowSelect = true;
            this.listViewLibrary.HideSelection = false;
            this.listViewLibrary.Location = new System.Drawing.Point(204, 3);
            this.listViewLibrary.Name = "listViewLibrary";
            this.listViewLibrary.ShowItemToolTips = true;
            this.listViewLibrary.Size = new System.Drawing.Size(1657, 737);
            this.listViewLibrary.TabIndex = 2;
            this.listViewLibrary.UseCompatibleStateImageBehavior = false;
            this.listViewLibrary.View = System.Windows.Forms.View.Details;
            this.listViewLibrary.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.libraryListView_ColumnClick);
            this.listViewLibrary.SelectedIndexChanged += new System.EventHandler(this.listViewLibrary_SelectedIndexChanged);
            this.listViewLibrary.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewLibrary_KeyUp);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.DisplayIndex = 1;
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 97;
            // 
            // columnHeaderOwner
            // 
            this.columnHeaderOwner.DisplayIndex = 2;
            this.columnHeaderOwner.Text = "Owner";
            this.columnHeaderOwner.Width = 48;
            // 
            // columnHeaderNote
            // 
            this.columnHeaderNote.DisplayIndex = 3;
            this.columnHeaderNote.Text = "Notes";
            this.columnHeaderNote.Width = 48;
            // 
            // columnHeaderServer
            // 
            this.columnHeaderServer.DisplayIndex = 4;
            this.columnHeaderServer.Text = "Server";
            // 
            // columnHeaderSex
            // 
            this.columnHeaderSex.DisplayIndex = 5;
            this.columnHeaderSex.Text = "S";
            this.columnHeaderSex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderSex.Width = 22;
            // 
            // columnHeaderDomesticated
            // 
            this.columnHeaderDomesticated.DisplayIndex = 36;
            this.columnHeaderDomesticated.Text = "Domesticated";
            // 
            // columnHeaderTopness
            // 
            this.columnHeaderTopness.DisplayIndex = 30;
            this.columnHeaderTopness.Text = "Tp%";
            this.columnHeaderTopness.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderTopness.Width = 33;
            // 
            // columnHeaderTopStatsNr
            // 
            this.columnHeaderTopStatsNr.DisplayIndex = 31;
            this.columnHeaderTopStatsNr.Text = "Top";
            this.columnHeaderTopStatsNr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderTopStatsNr.Width = 31;
            // 
            // columnHeaderGen
            // 
            this.columnHeaderGen.DisplayIndex = 32;
            this.columnHeaderGen.Text = "Gen";
            this.columnHeaderGen.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderGen.Width = 34;
            // 
            // columnHeaderFound
            // 
            this.columnHeaderFound.DisplayIndex = 33;
            this.columnHeaderFound.Text = "LW";
            this.columnHeaderFound.Width = 30;
            // 
            // columnHeaderMutations
            // 
            this.columnHeaderMutations.DisplayIndex = 34;
            this.columnHeaderMutations.Text = "Mu";
            this.columnHeaderMutations.Width = 30;
            // 
            // columnHeaderCooldown
            // 
            this.columnHeaderCooldown.DisplayIndex = 37;
            this.columnHeaderCooldown.Text = "Cooldown/Growing";
            // 
            // columnHeaderHP
            // 
            this.columnHeaderHP.DisplayIndex = 6;
            this.columnHeaderHP.Text = "HP";
            this.columnHeaderHP.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderHP.Width = 30;
            // 
            // columnHeaderSt
            // 
            this.columnHeaderSt.DisplayIndex = 8;
            this.columnHeaderSt.Text = "St";
            this.columnHeaderSt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderSt.Width = 30;
            // 
            // columnHeaderTo
            // 
            this.columnHeaderTo.DisplayIndex = 28;
            this.columnHeaderTo.Text = "To";
            this.columnHeaderTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderTo.Width = 30;
            // 
            // columnHeaderOx
            // 
            this.columnHeaderOx.DisplayIndex = 10;
            this.columnHeaderOx.Text = "Ox";
            this.columnHeaderOx.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderOx.Width = 30;
            // 
            // columnHeaderFo
            // 
            this.columnHeaderFo.DisplayIndex = 12;
            this.columnHeaderFo.Text = "Fo";
            this.columnHeaderFo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderFo.Width = 30;
            // 
            // columnHeaderWa
            // 
            this.columnHeaderWa.DisplayIndex = 14;
            this.columnHeaderWa.Text = "Wa";
            this.columnHeaderWa.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderWa.Width = 30;
            // 
            // columnHeaderTm
            // 
            this.columnHeaderTm.DisplayIndex = 16;
            this.columnHeaderTm.Text = "Te";
            this.columnHeaderTm.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderTm.Width = 30;
            // 
            // columnHeaderWe
            // 
            this.columnHeaderWe.DisplayIndex = 18;
            this.columnHeaderWe.Text = "We";
            this.columnHeaderWe.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderWe.Width = 30;
            // 
            // columnHeaderDm
            // 
            this.columnHeaderDm.Text = "Dm";
            this.columnHeaderDm.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderDm.Width = 30;
            // 
            // columnHeaderSp
            // 
            this.columnHeaderSp.DisplayIndex = 22;
            this.columnHeaderSp.Text = "Sp";
            this.columnHeaderSp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderSp.Width = 30;
            // 
            // columnHeaderFr
            // 
            this.columnHeaderFr.DisplayIndex = 24;
            this.columnHeaderFr.Text = "Fr";
            this.columnHeaderFr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderFr.Width = 30;
            // 
            // columnHeaderCr
            // 
            this.columnHeaderCr.DisplayIndex = 26;
            this.columnHeaderCr.Text = "Cr";
            this.columnHeaderCr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderCr.Width = 30;
            // 
            // columnHeaderHPM
            // 
            this.columnHeaderHPM.DisplayIndex = 7;
            this.columnHeaderHPM.Text = "HPM";
            this.columnHeaderHPM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderHPM.Width = 35;
            // 
            // columnHeaderStM
            // 
            this.columnHeaderStM.DisplayIndex = 9;
            this.columnHeaderStM.Text = "StM";
            this.columnHeaderStM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderStM.Width = 35;
            // 
            // columnHeaderToM
            // 
            this.columnHeaderToM.DisplayIndex = 29;
            this.columnHeaderToM.Text = "ToM";
            this.columnHeaderToM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderToM.Width = 35;
            // 
            // columnHeaderOxM
            // 
            this.columnHeaderOxM.DisplayIndex = 11;
            this.columnHeaderOxM.Text = "OxM";
            this.columnHeaderOxM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderOxM.Width = 35;
            // 
            // columnHeaderFoM
            // 
            this.columnHeaderFoM.DisplayIndex = 13;
            this.columnHeaderFoM.Text = "FoM";
            this.columnHeaderFoM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderFoM.Width = 35;
            // 
            // columnHeaderWaM
            // 
            this.columnHeaderWaM.DisplayIndex = 15;
            this.columnHeaderWaM.Text = "WaM";
            this.columnHeaderWaM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderWaM.Width = 0;
            // 
            // columnHeaderTmM
            // 
            this.columnHeaderTmM.DisplayIndex = 17;
            this.columnHeaderTmM.Text = "TeM";
            this.columnHeaderTmM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderTmM.Width = 0;
            // 
            // columnHeaderWeM
            // 
            this.columnHeaderWeM.DisplayIndex = 19;
            this.columnHeaderWeM.Text = "WeM";
            this.columnHeaderWeM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderWeM.Width = 35;
            // 
            // columnHeaderDmM
            // 
            this.columnHeaderDmM.DisplayIndex = 21;
            this.columnHeaderDmM.Text = "DmM";
            this.columnHeaderDmM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderDmM.Width = 35;
            // 
            // columnHeaderSpM
            // 
            this.columnHeaderSpM.DisplayIndex = 23;
            this.columnHeaderSpM.Text = "SpM";
            this.columnHeaderSpM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderSpM.Width = 35;
            // 
            // columnHeaderFrM
            // 
            this.columnHeaderFrM.DisplayIndex = 25;
            this.columnHeaderFrM.Text = "FrM";
            this.columnHeaderFrM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderFrM.Width = 0;
            // 
            // columnHeaderCrM
            // 
            this.columnHeaderCrM.DisplayIndex = 27;
            this.columnHeaderCrM.Text = "CrM";
            this.columnHeaderCrM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderCrM.Width = 0;
            // 
            // columnHeaderColor0
            // 
            this.columnHeaderColor0.DisplayIndex = 38;
            this.columnHeaderColor0.Text = "C0";
            this.columnHeaderColor0.Width = 25;
            // 
            // columnHeaderColor1
            // 
            this.columnHeaderColor1.DisplayIndex = 39;
            this.columnHeaderColor1.Text = "C1";
            this.columnHeaderColor1.Width = 25;
            // 
            // columnHeaderColor2
            // 
            this.columnHeaderColor2.DisplayIndex = 40;
            this.columnHeaderColor2.Text = "C2";
            this.columnHeaderColor2.Width = 25;
            // 
            // columnHeaderColor3
            // 
            this.columnHeaderColor3.DisplayIndex = 41;
            this.columnHeaderColor3.Text = "C3";
            this.columnHeaderColor3.Width = 25;
            // 
            // columnHeaderColor4
            // 
            this.columnHeaderColor4.DisplayIndex = 42;
            this.columnHeaderColor4.Text = "C4";
            this.columnHeaderColor4.Width = 25;
            // 
            // columnHeaderColor5
            // 
            this.columnHeaderColor5.DisplayIndex = 43;
            this.columnHeaderColor5.Text = "C5";
            this.columnHeaderColor5.Width = 25;
            // 
            // columnHeaderSpecies
            // 
            this.columnHeaderSpecies.DisplayIndex = 44;
            this.columnHeaderSpecies.Text = "Species";
            // 
            // columnHeaderStatus
            // 
            this.columnHeaderStatus.DisplayIndex = 0;
            this.columnHeaderStatus.Text = "Status";
            // 
            // columnHeaderTribe
            // 
            this.columnHeaderTribe.DisplayIndex = 45;
            this.columnHeaderTribe.Text = "Tribe";
            // 
            // columnHeaderStatusIcon
            // 
            this.columnHeaderStatusIcon.DisplayIndex = 46;
            this.columnHeaderStatusIcon.Text = "Status";
            this.columnHeaderStatusIcon.Width = 35;
            // 
            // columnHeaderMutagen
            // 
            this.columnHeaderMutagen.DisplayIndex = 35;
            this.columnHeaderMutagen.Text = "Mutagen";
            this.columnHeaderMutagen.Width = 30;
            // 
            // contextMenuStripLibrary
            // 
            this.contextMenuStripLibrary.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemEdit,
            this.editAllSelectedToolStripMenuItem,
            this.toolStripSeparator17,
            this.toolStripMenuItemGenerateCreatureName,
            this.toolStripMenuItemCopyCreatureName,
            this.toolStripSeparator9,
            this.copyValuesToExtractorToolStripMenuItem,
            this.exportToClipboardToolStripMenuItem1,
            this.copyInfographicToClipboardToolStripMenuItem,
            this.saveInfographicsToFolderToolStripMenuItem,
            this.toolStripSeparator22,
            this.removeCooldownGrowingToolStripMenuItem,
            this.bestBreedingPartnersToolStripMenuItem,
            this.breedingPlanForSelectedCreaturesToolStripMenuItem,
            this.toolStripMenuItemStatus,
            this.applyMutagenToolStripMenuItem,
            this.toolStripSeparator16,
            this.adminCommandToSetColorsToolStripMenuItem,
            this.adminCommandToSpawnExactDinoToolStripMenuItem,
            this.adminCommandToSpawnExactDinoDS2ToolStripMenuItem,
            this.fixColorsToolStripMenuItem,
            this.toolStripSeparator6,
            this.toolStripMenuItemOpenWiki,
            this.toolStripSeparator14,
            this.toolStripMenuItemRemove});
            this.contextMenuStripLibrary.Name = "contextMenuStripLibrary";
            this.contextMenuStripLibrary.Size = new System.Drawing.Size(303, 458);
            this.contextMenuStripLibrary.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripLibrary_Opening);
            // 
            // toolStripMenuItemEdit
            // 
            this.toolStripMenuItemEdit.Name = "toolStripMenuItemEdit";
            this.toolStripMenuItemEdit.ShortcutKeyDisplayString = "F2";
            this.toolStripMenuItemEdit.Size = new System.Drawing.Size(302, 22);
            this.toolStripMenuItemEdit.Text = "Edit";
            this.toolStripMenuItemEdit.Click += new System.EventHandler(this.toolStripMenuItemEdit_Click);
            // 
            // editAllSelectedToolStripMenuItem
            // 
            this.editAllSelectedToolStripMenuItem.Name = "editAllSelectedToolStripMenuItem";
            this.editAllSelectedToolStripMenuItem.ShortcutKeyDisplayString = "F3";
            this.editAllSelectedToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.editAllSelectedToolStripMenuItem.Text = "Edit all Selected...";
            this.editAllSelectedToolStripMenuItem.Click += new System.EventHandler(this.editAllSelectedToolStripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(299, 6);
            // 
            // toolStripMenuItemGenerateCreatureName
            // 
            this.toolStripMenuItemGenerateCreatureName.Name = "toolStripMenuItemGenerateCreatureName";
            this.toolStripMenuItemGenerateCreatureName.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.toolStripMenuItemGenerateCreatureName.Size = new System.Drawing.Size(302, 22);
            this.toolStripMenuItemGenerateCreatureName.Text = "Generate creature Name";
            this.toolStripMenuItemGenerateCreatureName.ToolTipText = "Applies the naming pattern on all selected creatures";
            this.toolStripMenuItemGenerateCreatureName.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripMenuItemCopyCreatureName
            // 
            this.toolStripMenuItemCopyCreatureName.Name = "toolStripMenuItemCopyCreatureName";
            this.toolStripMenuItemCopyCreatureName.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.toolStripMenuItemCopyCreatureName.Size = new System.Drawing.Size(302, 22);
            this.toolStripMenuItemCopyCreatureName.Text = "Copy creature Name";
            this.toolStripMenuItemCopyCreatureName.Click += new System.EventHandler(this.toolStripMenuItemCopyCreatureName_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(299, 6);
            // 
            // copyValuesToExtractorToolStripMenuItem
            // 
            this.copyValuesToExtractorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentValuesToolStripMenuItem,
            this.wildValuesToolStripMenuItem});
            this.copyValuesToExtractorToolStripMenuItem.Name = "copyValuesToExtractorToolStripMenuItem";
            this.copyValuesToExtractorToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.copyValuesToExtractorToolStripMenuItem.Text = "Copy Values to Extractor";
            // 
            // currentValuesToolStripMenuItem
            // 
            this.currentValuesToolStripMenuItem.Name = "currentValuesToolStripMenuItem";
            this.currentValuesToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.currentValuesToolStripMenuItem.Text = "Current Values";
            this.currentValuesToolStripMenuItem.Click += new System.EventHandler(this.currentValuesToolStripMenuItem_Click);
            // 
            // wildValuesToolStripMenuItem
            // 
            this.wildValuesToolStripMenuItem.Name = "wildValuesToolStripMenuItem";
            this.wildValuesToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.wildValuesToolStripMenuItem.Text = "Only Wild Values";
            this.wildValuesToolStripMenuItem.Click += new System.EventHandler(this.wildValuesToolStripMenuItem_Click);
            // 
            // exportToClipboardToolStripMenuItem1
            // 
            this.exportToClipboardToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.plainTextcurrentValuesToolStripMenuItem1,
            this.plainTextbreedingValuesToolStripMenuItem1,
            this.forSpreadsheetToolStripMenuItem1});
            this.exportToClipboardToolStripMenuItem1.Name = "exportToClipboardToolStripMenuItem1";
            this.exportToClipboardToolStripMenuItem1.Size = new System.Drawing.Size(302, 22);
            this.exportToClipboardToolStripMenuItem1.Text = "Export to Clipboard";
            // 
            // plainTextcurrentValuesToolStripMenuItem1
            // 
            this.plainTextcurrentValuesToolStripMenuItem1.Name = "plainTextcurrentValuesToolStripMenuItem1";
            this.plainTextcurrentValuesToolStripMenuItem1.Size = new System.Drawing.Size(218, 22);
            this.plainTextcurrentValuesToolStripMenuItem1.Text = "Plain Text (current values)";
            this.plainTextcurrentValuesToolStripMenuItem1.Click += new System.EventHandler(this.plainTextcurrentValuesToolStripMenuItem1_Click);
            // 
            // plainTextbreedingValuesToolStripMenuItem1
            // 
            this.plainTextbreedingValuesToolStripMenuItem1.Name = "plainTextbreedingValuesToolStripMenuItem1";
            this.plainTextbreedingValuesToolStripMenuItem1.Size = new System.Drawing.Size(218, 22);
            this.plainTextbreedingValuesToolStripMenuItem1.Text = "Plain Text (breeding values)";
            this.plainTextbreedingValuesToolStripMenuItem1.Click += new System.EventHandler(this.plainTextbreedingValuesToolStripMenuItem1_Click);
            // 
            // forSpreadsheetToolStripMenuItem1
            // 
            this.forSpreadsheetToolStripMenuItem1.Name = "forSpreadsheetToolStripMenuItem1";
            this.forSpreadsheetToolStripMenuItem1.Size = new System.Drawing.Size(218, 22);
            this.forSpreadsheetToolStripMenuItem1.Text = "for Spreadsheet";
            this.forSpreadsheetToolStripMenuItem1.Click += new System.EventHandler(this.forSpreadsheetToolStripMenuItem_Click);
            // 
            // copyInfographicToClipboardToolStripMenuItem
            // 
            this.copyInfographicToClipboardToolStripMenuItem.Name = "copyInfographicToClipboardToolStripMenuItem";
            this.copyInfographicToClipboardToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.copyInfographicToClipboardToolStripMenuItem.Text = "Copy Infographic to Clipboard";
            this.copyInfographicToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyInfographicToClipboardToolStripMenuItem_Click);
            // 
            // saveInfographicsToFolderToolStripMenuItem
            // 
            this.saveInfographicsToFolderToolStripMenuItem.Name = "saveInfographicsToFolderToolStripMenuItem";
            this.saveInfographicsToFolderToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.saveInfographicsToFolderToolStripMenuItem.Text = "Save Infographics to folder…";
            this.saveInfographicsToFolderToolStripMenuItem.Click += new System.EventHandler(this.saveInfographicsToFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            this.toolStripSeparator22.Size = new System.Drawing.Size(299, 6);
            // 
            // removeCooldownGrowingToolStripMenuItem
            // 
            this.removeCooldownGrowingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setToMatureToolStripMenuItem,
            this.clearMatingCooldownToolStripMenuItem,
            this.justMatedToolStripMenuItem});
            this.removeCooldownGrowingToolStripMenuItem.Name = "removeCooldownGrowingToolStripMenuItem";
            this.removeCooldownGrowingToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.removeCooldownGrowingToolStripMenuItem.Text = "Set maturity / breeding state";
            // 
            // setToMatureToolStripMenuItem
            // 
            this.setToMatureToolStripMenuItem.Name = "setToMatureToolStripMenuItem";
            this.setToMatureToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.setToMatureToolStripMenuItem.Text = "Set to mature";
            this.setToMatureToolStripMenuItem.Click += new System.EventHandler(this.setToMatureToolStripMenuItem_Click);
            // 
            // clearMatingCooldownToolStripMenuItem
            // 
            this.clearMatingCooldownToolStripMenuItem.Name = "clearMatingCooldownToolStripMenuItem";
            this.clearMatingCooldownToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.clearMatingCooldownToolStripMenuItem.Text = "Clear mating cooldown";
            this.clearMatingCooldownToolStripMenuItem.Click += new System.EventHandler(this.clearMatingCooldownToolStripMenuItem_Click);
            // 
            // justMatedToolStripMenuItem
            // 
            this.justMatedToolStripMenuItem.Name = "justMatedToolStripMenuItem";
            this.justMatedToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.justMatedToolStripMenuItem.Text = "Just mated";
            this.justMatedToolStripMenuItem.Click += new System.EventHandler(this.justMatedToolStripMenuItem_Click);
            // 
            // bestBreedingPartnersToolStripMenuItem
            // 
            this.bestBreedingPartnersToolStripMenuItem.Name = "bestBreedingPartnersToolStripMenuItem";
            this.bestBreedingPartnersToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.bestBreedingPartnersToolStripMenuItem.Text = "Best Breeding Partners...";
            this.bestBreedingPartnersToolStripMenuItem.Click += new System.EventHandler(this.bestBreedingPartnersToolStripMenuItem_Click);
            // 
            // breedingPlanForSelectedCreaturesToolStripMenuItem
            // 
            this.breedingPlanForSelectedCreaturesToolStripMenuItem.Name = "breedingPlanForSelectedCreaturesToolStripMenuItem";
            this.breedingPlanForSelectedCreaturesToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.breedingPlanForSelectedCreaturesToolStripMenuItem.Text = "Breeding Plan for selected Creatures…";
            this.breedingPlanForSelectedCreaturesToolStripMenuItem.Click += new System.EventHandler(this.breedingPlanForSelectedCreaturesToolStripMenuItem_Click);
            // 
            // toolStripMenuItemStatus
            // 
            this.toolStripMenuItemStatus.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.obeliskToolStripMenuItem,
            this.cryopodToolStripMenuItem});
            this.toolStripMenuItemStatus.Name = "toolStripMenuItemStatus";
            this.toolStripMenuItemStatus.Size = new System.Drawing.Size(302, 22);
            this.toolStripMenuItemStatus.Text = "Set Status";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem2.Text = "Available";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem3.Text = "Unavailable";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem4.Text = "Dead";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // obeliskToolStripMenuItem
            // 
            this.obeliskToolStripMenuItem.Name = "obeliskToolStripMenuItem";
            this.obeliskToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.obeliskToolStripMenuItem.Text = "Obelisk";
            this.obeliskToolStripMenuItem.Click += new System.EventHandler(this.obeliskToolStripMenuItem_Click);
            // 
            // cryopodToolStripMenuItem
            // 
            this.cryopodToolStripMenuItem.Name = "cryopodToolStripMenuItem";
            this.cryopodToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.cryopodToolStripMenuItem.Text = "Cryopod";
            this.cryopodToolStripMenuItem.Click += new System.EventHandler(this.cryopodToolStripMenuItem_Click);
            // 
            // applyMutagenToolStripMenuItem
            // 
            this.applyMutagenToolStripMenuItem.Name = "applyMutagenToolStripMenuItem";
            this.applyMutagenToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.applyMutagenToolStripMenuItem.Text = "Apply Mutagen";
            this.applyMutagenToolStripMenuItem.Click += new System.EventHandler(this.applyMutagenToolStripMenuItem_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(299, 6);
            // 
            // adminCommandToSetColorsToolStripMenuItem
            // 
            this.adminCommandToSetColorsToolStripMenuItem.Name = "adminCommandToSetColorsToolStripMenuItem";
            this.adminCommandToSetColorsToolStripMenuItem.ShortcutKeyDisplayString = "F5";
            this.adminCommandToSetColorsToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.adminCommandToSetColorsToolStripMenuItem.Text = "Admin Command to set Colors";
            this.adminCommandToSetColorsToolStripMenuItem.Click += new System.EventHandler(this.adminCommandToSetColorsToolStripMenuItem_Click);
            // 
            // adminCommandToSpawnExactDinoToolStripMenuItem
            // 
            this.adminCommandToSpawnExactDinoToolStripMenuItem.Name = "adminCommandToSpawnExactDinoToolStripMenuItem";
            this.adminCommandToSpawnExactDinoToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.adminCommandToSpawnExactDinoToolStripMenuItem.Text = "Admin Command to spawn exact dino";
            this.adminCommandToSpawnExactDinoToolStripMenuItem.Click += new System.EventHandler(this.adminCommandToSpawnExactDinoToolStripMenuItem_Click);
            // 
            // adminCommandToSpawnExactDinoDS2ToolStripMenuItem
            // 
            this.adminCommandToSpawnExactDinoDS2ToolStripMenuItem.Name = "adminCommandToSpawnExactDinoDS2ToolStripMenuItem";
            this.adminCommandToSpawnExactDinoDS2ToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.adminCommandToSpawnExactDinoDS2ToolStripMenuItem.Text = "Admin Command to spawn exact dino DS2";
            this.adminCommandToSpawnExactDinoDS2ToolStripMenuItem.Click += new System.EventHandler(this.adminCommandToSpawnExactDinoDS2ToolStripMenuItem_Click);
            // 
            // fixColorsToolStripMenuItem
            // 
            this.fixColorsToolStripMenuItem.Name = "fixColorsToolStripMenuItem";
            this.fixColorsToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.fixColorsToolStripMenuItem.Text = "Fix colors";
            this.fixColorsToolStripMenuItem.ToolTipText = resources.GetString("fixColorsToolStripMenuItem.ToolTipText");
            this.fixColorsToolStripMenuItem.Click += new System.EventHandler(this.fixColorsToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(299, 6);
            // 
            // toolStripMenuItemOpenWiki
            // 
            this.toolStripMenuItemOpenWiki.Name = "toolStripMenuItemOpenWiki";
            this.toolStripMenuItemOpenWiki.Size = new System.Drawing.Size(302, 22);
            this.toolStripMenuItemOpenWiki.Text = "Open Wiki-page in Browser";
            this.toolStripMenuItemOpenWiki.Click += new System.EventHandler(this.ToolStripMenuItemOpenWiki_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(299, 6);
            // 
            // toolStripMenuItemRemove
            // 
            this.toolStripMenuItemRemove.Name = "toolStripMenuItemRemove";
            this.toolStripMenuItemRemove.Size = new System.Drawing.Size(302, 22);
            this.toolStripMenuItemRemove.Text = "Delete creature...";
            this.toolStripMenuItemRemove.Click += new System.EventHandler(this.toolStripMenuItemRemove_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.AutoScrollMinSize = new System.Drawing.Size(0, 620);
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tabControlLibFilter, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.creatureBoxListView, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(195, 737);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // tabControlLibFilter
            // 
            this.tabControlLibFilter.Controls.Add(this.tabPage1);
            this.tabControlLibFilter.Controls.Add(this.tabPage3);
            this.tabControlLibFilter.Controls.Add(this.tabPageLibRadarChart);
            this.tabControlLibFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlLibFilter.Location = new System.Drawing.Point(3, 415);
            this.tabControlLibFilter.Name = "tabControlLibFilter";
            this.tabControlLibFilter.SelectedIndex = 0;
            this.tabControlLibFilter.Size = new System.Drawing.Size(189, 319);
            this.tabControlLibFilter.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listBoxSpeciesLib);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(181, 293);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Species";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listBoxSpeciesLib
            // 
            this.listBoxSpeciesLib.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxSpeciesLib.FormattingEnabled = true;
            this.listBoxSpeciesLib.Location = new System.Drawing.Point(3, 3);
            this.listBoxSpeciesLib.Name = "listBoxSpeciesLib";
            this.listBoxSpeciesLib.Size = new System.Drawing.Size(175, 287);
            this.listBoxSpeciesLib.TabIndex = 0;
            this.listBoxSpeciesLib.Click += new System.EventHandler(this.listBoxSpeciesLib_Click);
            this.listBoxSpeciesLib.SelectedIndexChanged += new System.EventHandler(this.listBoxSpeciesLib_SelectedIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tableLayoutPanel2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(181, 293);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Stats";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.checkedListBoxConsiderStatTop, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.buttonRecalculateTops, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label17, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(175, 287);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // checkedListBoxConsiderStatTop
            // 
            this.checkedListBoxConsiderStatTop.CheckOnClick = true;
            this.checkedListBoxConsiderStatTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxConsiderStatTop.FormattingEnabled = true;
            this.checkedListBoxConsiderStatTop.Location = new System.Drawing.Point(3, 35);
            this.checkedListBoxConsiderStatTop.Name = "checkedListBoxConsiderStatTop";
            this.checkedListBoxConsiderStatTop.Size = new System.Drawing.Size(169, 220);
            this.checkedListBoxConsiderStatTop.TabIndex = 3;
            // 
            // buttonRecalculateTops
            // 
            this.buttonRecalculateTops.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRecalculateTops.Location = new System.Drawing.Point(3, 261);
            this.buttonRecalculateTops.Name = "buttonRecalculateTops";
            this.buttonRecalculateTops.Size = new System.Drawing.Size(169, 23);
            this.buttonRecalculateTops.TabIndex = 2;
            this.buttonRecalculateTops.Text = "Apply";
            this.buttonRecalculateTops.UseVisualStyleBackColor = true;
            this.buttonRecalculateTops.Click += new System.EventHandler(this.buttonRecalculateTops_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(3, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(168, 26);
            this.label17.TabIndex = 4;
            this.label17.Text = "Select the stats considered for the TopStat-Calculation and Coloring";
            // 
            // tabPageLibRadarChart
            // 
            this.tabPageLibRadarChart.Controls.Add(this.radarChartLibrary);
            this.tabPageLibRadarChart.Location = new System.Drawing.Point(4, 22);
            this.tabPageLibRadarChart.Name = "tabPageLibRadarChart";
            this.tabPageLibRadarChart.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLibRadarChart.Size = new System.Drawing.Size(181, 293);
            this.tabPageLibRadarChart.TabIndex = 4;
            this.tabPageLibRadarChart.Text = "Chart";
            this.tabPageLibRadarChart.UseVisualStyleBackColor = true;
            // 
            // radarChartLibrary
            // 
            this.radarChartLibrary.Dock = System.Windows.Forms.DockStyle.Top;
            this.radarChartLibrary.Image = ((System.Drawing.Image)(resources.GetObject("radarChartLibrary.Image")));
            this.radarChartLibrary.Location = new System.Drawing.Point(3, 3);
            this.radarChartLibrary.Name = "radarChartLibrary";
            this.radarChartLibrary.Size = new System.Drawing.Size(175, 287);
            this.radarChartLibrary.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.radarChartLibrary.TabIndex = 0;
            this.radarChartLibrary.TabStop = false;
            // 
            // creatureBoxListView
            // 
            this.creatureBoxListView.Location = new System.Drawing.Point(3, 3);
            this.creatureBoxListView.Name = "creatureBoxListView";
            this.creatureBoxListView.Size = new System.Drawing.Size(189, 406);
            this.creatureBoxListView.TabIndex = 0;
            this.creatureBoxListView.Changed += new System.Action<ARKBreedingStats.Library.Creature, bool, bool>(this.UpdateDisplayedCreatureValues);
            this.creatureBoxListView.GiveParents += new System.Action<ARKBreedingStats.Library.Creature>(this.CreatureBoxListView_FindParents);
            this.creatureBoxListView.SelectCreature += new System.Action<ARKBreedingStats.Library.Creature>(this.SelectCreatureInLibrary);
            // 
            // tabPageLibraryInfo
            // 
            this.tabPageLibraryInfo.Controls.Add(this.tlpLibraryInfo);
            this.tabPageLibraryInfo.Location = new System.Drawing.Point(4, 22);
            this.tabPageLibraryInfo.Name = "tabPageLibraryInfo";
            this.tabPageLibraryInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLibraryInfo.Size = new System.Drawing.Size(1870, 749);
            this.tabPageLibraryInfo.TabIndex = 14;
            this.tabPageLibraryInfo.Text = "Library Info";
            this.tabPageLibraryInfo.UseVisualStyleBackColor = true;
            // 
            // tlpLibraryInfo
            // 
            this.tlpLibraryInfo.AutoScroll = true;
            this.tlpLibraryInfo.ColumnCount = 2;
            this.tlpLibraryInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpLibraryInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpLibraryInfo.Controls.Add(this.CbLibraryInfoUseFilter, 1, 0);
            this.tlpLibraryInfo.Controls.Add(this.BtCopyLibraryColorToClipboard, 0, 0);
            this.tlpLibraryInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpLibraryInfo.Location = new System.Drawing.Point(3, 3);
            this.tlpLibraryInfo.Name = "tlpLibraryInfo";
            this.tlpLibraryInfo.RowCount = 1;
            this.tlpLibraryInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpLibraryInfo.Size = new System.Drawing.Size(1864, 743);
            this.tlpLibraryInfo.TabIndex = 0;
            // 
            // CbLibraryInfoUseFilter
            // 
            this.CbLibraryInfoUseFilter.AutoSize = true;
            this.CbLibraryInfoUseFilter.Location = new System.Drawing.Point(223, 3);
            this.CbLibraryInfoUseFilter.Name = "CbLibraryInfoUseFilter";
            this.CbLibraryInfoUseFilter.Size = new System.Drawing.Size(97, 17);
            this.CbLibraryInfoUseFilter.TabIndex = 1;
            this.CbLibraryInfoUseFilter.Text = "Use library filter";
            this.CbLibraryInfoUseFilter.UseVisualStyleBackColor = true;
            this.CbLibraryInfoUseFilter.CheckedChanged += new System.EventHandler(this.CbLibraryInfoUseFilter_CheckedChanged);
            // 
            // BtCopyLibraryColorToClipboard
            // 
            this.BtCopyLibraryColorToClipboard.Location = new System.Drawing.Point(3, 3);
            this.BtCopyLibraryColorToClipboard.Name = "BtCopyLibraryColorToClipboard";
            this.BtCopyLibraryColorToClipboard.Size = new System.Drawing.Size(214, 23);
            this.BtCopyLibraryColorToClipboard.TabIndex = 0;
            this.BtCopyLibraryColorToClipboard.Text = "Copy this text to the clipboard";
            this.BtCopyLibraryColorToClipboard.UseVisualStyleBackColor = true;
            this.BtCopyLibraryColorToClipboard.Click += new System.EventHandler(this.BtCopyLibraryColorToClipboard_Click);
            // 
            // tabPagePedigree
            // 
            this.tabPagePedigree.Controls.Add(this.pedigree1);
            this.tabPagePedigree.Location = new System.Drawing.Point(4, 22);
            this.tabPagePedigree.Name = "tabPagePedigree";
            this.tabPagePedigree.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePedigree.Size = new System.Drawing.Size(1870, 749);
            this.tabPagePedigree.TabIndex = 3;
            this.tabPagePedigree.Text = "Pedigree";
            this.tabPagePedigree.UseVisualStyleBackColor = true;
            // 
            // pedigree1
            // 
            this.pedigree1.AutoScroll = true;
            this.pedigree1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pedigree1.LeftColumnWidth = 203;
            this.pedigree1.Location = new System.Drawing.Point(3, 3);
            this.pedigree1.Name = "pedigree1";
            this.pedigree1.Size = new System.Drawing.Size(1864, 743);
            this.pedigree1.TabIndex = 0;
            // 
            // tabPageTaming
            // 
            this.tabPageTaming.Controls.Add(this.tamingControl1);
            this.tabPageTaming.Location = new System.Drawing.Point(4, 22);
            this.tabPageTaming.Name = "tabPageTaming";
            this.tabPageTaming.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTaming.Size = new System.Drawing.Size(1870, 749);
            this.tabPageTaming.TabIndex = 8;
            this.tabPageTaming.Text = "Taming";
            this.tabPageTaming.UseVisualStyleBackColor = true;
            // 
            // tamingControl1
            // 
            this.tamingControl1.AutoScroll = true;
            this.tamingControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tamingControl1.Location = new System.Drawing.Point(3, 3);
            this.tamingControl1.Name = "tamingControl1";
            this.tamingControl1.Size = new System.Drawing.Size(1864, 743);
            this.tamingControl1.TabIndex = 0;
            this.tamingControl1.WeaponDamages = new double[] {
        100D,
        100D,
        100D,
        100D,
        100D,
        100D,
        100D};
            this.tamingControl1.WeaponDamagesEnabled = 3;
            // 
            // tabPageBreedingPlan
            // 
            this.tabPageBreedingPlan.Controls.Add(this.breedingPlan1);
            this.tabPageBreedingPlan.Location = new System.Drawing.Point(4, 22);
            this.tabPageBreedingPlan.Name = "tabPageBreedingPlan";
            this.tabPageBreedingPlan.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBreedingPlan.Size = new System.Drawing.Size(1870, 749);
            this.tabPageBreedingPlan.TabIndex = 4;
            this.tabPageBreedingPlan.Text = "Breeding Plan";
            this.tabPageBreedingPlan.UseVisualStyleBackColor = true;
            // 
            // breedingPlan1
            // 
            this.breedingPlan1.AutoScroll = true;
            this.breedingPlan1.CurrentSpecies = null;
            this.breedingPlan1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.breedingPlan1.Location = new System.Drawing.Point(3, 3);
            this.breedingPlan1.MutationLimit = 0;
            this.breedingPlan1.Name = "breedingPlan1";
            this.breedingPlan1.Size = new System.Drawing.Size(1864, 743);
            this.breedingPlan1.TabIndex = 0;
            // 
            // tabPageHatching
            // 
            this.tabPageHatching.Controls.Add(this.hatching1);
            this.tabPageHatching.Location = new System.Drawing.Point(4, 22);
            this.tabPageHatching.Name = "tabPageHatching";
            this.tabPageHatching.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHatching.Size = new System.Drawing.Size(1870, 749);
            this.tabPageHatching.TabIndex = 13;
            this.tabPageHatching.Text = "Hatching";
            this.tabPageHatching.UseVisualStyleBackColor = true;
            // 
            // hatching1
            // 
            this.hatching1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hatching1.Location = new System.Drawing.Point(3, 3);
            this.hatching1.Name = "hatching1";
            this.hatching1.Size = new System.Drawing.Size(1864, 743);
            this.hatching1.TabIndex = 0;
            // 
            // tabPageRaising
            // 
            this.tabPageRaising.Controls.Add(this.raisingControl1);
            this.tabPageRaising.Location = new System.Drawing.Point(4, 22);
            this.tabPageRaising.Name = "tabPageRaising";
            this.tabPageRaising.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRaising.Size = new System.Drawing.Size(1870, 749);
            this.tabPageRaising.TabIndex = 9;
            this.tabPageRaising.Text = "Raising";
            this.tabPageRaising.UseVisualStyleBackColor = true;
            // 
            // raisingControl1
            // 
            this.raisingControl1.AutoScroll = true;
            this.raisingControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.raisingControl1.Location = new System.Drawing.Point(3, 3);
            this.raisingControl1.Name = "raisingControl1";
            this.raisingControl1.Size = new System.Drawing.Size(1864, 743);
            this.raisingControl1.TabIndex = 0;
            // 
            // tabPageTimer
            // 
            this.tabPageTimer.Controls.Add(this.timerList1);
            this.tabPageTimer.Location = new System.Drawing.Point(4, 22);
            this.tabPageTimer.Name = "tabPageTimer";
            this.tabPageTimer.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTimer.Size = new System.Drawing.Size(1870, 749);
            this.tabPageTimer.TabIndex = 6;
            this.tabPageTimer.Text = "Timer";
            this.tabPageTimer.UseVisualStyleBackColor = true;
            // 
            // timerList1
            // 
            this.timerList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timerList1.Location = new System.Drawing.Point(3, 3);
            this.timerList1.Name = "timerList1";
            this.timerList1.Size = new System.Drawing.Size(1864, 743);
            this.timerList1.TabIndex = 0;
            this.timerList1.TimerAlertsCSV = "";
            // 
            // tabPagePlayerTribes
            // 
            this.tabPagePlayerTribes.Controls.Add(this.tribesControl1);
            this.tabPagePlayerTribes.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlayerTribes.Name = "tabPagePlayerTribes";
            this.tabPagePlayerTribes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePlayerTribes.Size = new System.Drawing.Size(1870, 749);
            this.tabPagePlayerTribes.TabIndex = 7;
            this.tabPagePlayerTribes.Text = "Player";
            this.tabPagePlayerTribes.UseVisualStyleBackColor = true;
            // 
            // tribesControl1
            // 
            this.tribesControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tribesControl1.Location = new System.Drawing.Point(3, 3);
            this.tribesControl1.Name = "tribesControl1";
            this.tribesControl1.Size = new System.Drawing.Size(1864, 743);
            this.tribesControl1.TabIndex = 0;
            // 
            // tabPageNotes
            // 
            this.tabPageNotes.Controls.Add(this.notesControl1);
            this.tabPageNotes.Location = new System.Drawing.Point(4, 22);
            this.tabPageNotes.Name = "tabPageNotes";
            this.tabPageNotes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNotes.Size = new System.Drawing.Size(1870, 749);
            this.tabPageNotes.TabIndex = 10;
            this.tabPageNotes.Text = "Notes";
            this.tabPageNotes.UseVisualStyleBackColor = true;
            // 
            // notesControl1
            // 
            this.notesControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.notesControl1.Location = new System.Drawing.Point(3, 3);
            this.notesControl1.Name = "notesControl1";
            this.notesControl1.Size = new System.Drawing.Size(1864, 743);
            this.notesControl1.TabIndex = 0;
            // 
            // TabPageOCR
            // 
            this.TabPageOCR.Controls.Add(this.ocrControl1);
            this.TabPageOCR.Location = new System.Drawing.Point(4, 22);
            this.TabPageOCR.Name = "TabPageOCR";
            this.TabPageOCR.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageOCR.Size = new System.Drawing.Size(1870, 749);
            this.TabPageOCR.TabIndex = 5;
            this.TabPageOCR.Text = "Experimental OCR";
            this.TabPageOCR.UseVisualStyleBackColor = true;
            // 
            // ocrControl1
            // 
            this.ocrControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ocrControl1.Location = new System.Drawing.Point(3, 3);
            this.ocrControl1.Name = "ocrControl1";
            this.ocrControl1.Size = new System.Drawing.Size(1864, 743);
            this.ocrControl1.TabIndex = 2;
            // 
            // tabPageExtractionTests
            // 
            this.tabPageExtractionTests.Controls.Add(this.extractionTestControl1);
            this.tabPageExtractionTests.Location = new System.Drawing.Point(4, 22);
            this.tabPageExtractionTests.Name = "tabPageExtractionTests";
            this.tabPageExtractionTests.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExtractionTests.Size = new System.Drawing.Size(1870, 749);
            this.tabPageExtractionTests.TabIndex = 11;
            this.tabPageExtractionTests.Text = "Extraction Tests";
            this.tabPageExtractionTests.UseVisualStyleBackColor = true;
            // 
            // extractionTestControl1
            // 
            this.extractionTestControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extractionTestControl1.Location = new System.Drawing.Point(3, 3);
            this.extractionTestControl1.Name = "extractionTestControl1";
            this.extractionTestControl1.Size = new System.Drawing.Size(1864, 743);
            this.extractionTestControl1.TabIndex = 0;
            // 
            // tabPageMultiplierTesting
            // 
            this.tabPageMultiplierTesting.Controls.Add(this.statsMultiplierTesting1);
            this.tabPageMultiplierTesting.Location = new System.Drawing.Point(4, 22);
            this.tabPageMultiplierTesting.Name = "tabPageMultiplierTesting";
            this.tabPageMultiplierTesting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMultiplierTesting.Size = new System.Drawing.Size(1870, 749);
            this.tabPageMultiplierTesting.TabIndex = 12;
            this.tabPageMultiplierTesting.Text = "Multiplier Testing";
            this.tabPageMultiplierTesting.UseVisualStyleBackColor = true;
            // 
            // statsMultiplierTesting1
            // 
            this.statsMultiplierTesting1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsMultiplierTesting1.Location = new System.Drawing.Point(3, 3);
            this.statsMultiplierTesting1.Name = "statsMultiplierTesting1";
            this.statsMultiplierTesting1.Size = new System.Drawing.Size(1864, 743);
            this.statsMultiplierTesting1.TabIndex = 0;
            // 
            // btReadValuesFromArk
            // 
            this.btReadValuesFromArk.Location = new System.Drawing.Point(262, 3);
            this.btReadValuesFromArk.Name = "btReadValuesFromArk";
            this.btReadValuesFromArk.Size = new System.Drawing.Size(111, 45);
            this.btReadValuesFromArk.TabIndex = 3;
            this.btReadValuesFromArk.Text = "Read Values From ARK Window";
            this.btReadValuesFromArk.UseVisualStyleBackColor = true;
            this.btReadValuesFromArk.Click += new System.EventHandler(this.btnReadValuesFromArk_Click);
            // 
            // cbEventMultipliers
            // 
            this.cbEventMultipliers.AutoSize = true;
            this.cbEventMultipliers.Location = new System.Drawing.Point(53, 29);
            this.cbEventMultipliers.Name = "cbEventMultipliers";
            this.cbEventMultipliers.Size = new System.Drawing.Size(54, 17);
            this.cbEventMultipliers.TabIndex = 1;
            this.cbEventMultipliers.Text = "Event";
            this.cbEventMultipliers.UseVisualStyleBackColor = true;
            this.cbEventMultipliers.CheckedChanged += new System.EventHandler(this.cbEvolutionEvent_CheckedChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel,
            this.ToolStripStatusLabelImport});
            this.statusStrip1.Location = new System.Drawing.Point(0, 913);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1878, 22);
            this.statusStrip1.TabIndex = 44;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(119, 17);
            this.toolStripStatusLabel.Text = "ToolStripStatusLabel1";
            // 
            // ToolStripStatusLabelImport
            // 
            this.ToolStripStatusLabelImport.BackColor = System.Drawing.Color.Yellow;
            this.ToolStripStatusLabelImport.Name = "ToolStripStatusLabelImport";
            this.ToolStripStatusLabelImport.Size = new System.Drawing.Size(125, 17);
            this.ToolStripStatusLabelImport.Text = "Importing savegame…";
            this.ToolStripStatusLabelImport.Visible = false;
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton1,
            this.openToolStripButton1,
            this.saveToolStripButton1,
            this.toolStripSeparator20,
            this.TsbQuickSaveGameImport,
            this.toolStripSeparator3,
            this.toolStripButtonSettings,
            this.toolStripSeparator4,
            this.toolStripButtonCopy2Tester,
            this.toolStripButtonCopy2Extractor,
            this.toolStripButtonClear,
            this.toolStripButtonAddPlayer,
            this.toolStripButtonAddTribe,
            this.toolStripButtonDeleteExpiredIncubationTimers,
            this.toolStripSeparator8,
            this.toolStripButtonSaveCreatureValuesTemp,
            this.toolStripCBTempCreatures,
            this.toolStripButtonDeleteTempCreature,
            this.tsBtAddAsExtractionTest,
            this.copyToMultiplierTesterToolStripButton,
            this.ToolStripLabelFilter,
            this.ToolStripTextBoxLibraryFilter,
            this.ToolStripButtonLibraryFilterClear,
            this.ToolStripButtonSaveFilterPreset,
            this.TsSpOcrLabel,
            this.TsLbLabelSet,
            this.TsCbbLabelSets});
            this.toolStrip2.Location = new System.Drawing.Point(0, 24);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1878, 25);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // newToolStripButton1
            // 
            this.newToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newToolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripButton1.Image")));
            this.newToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripButton1.Name = "newToolStripButton1";
            this.newToolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.newToolStripButton1.Text = "&New";
            this.newToolStripButton1.Click += new System.EventHandler(this.newToolStripButton1_Click);
            // 
            // openToolStripButton1
            // 
            this.openToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton1.Image")));
            this.openToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton1.Name = "openToolStripButton1";
            this.openToolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton1.Text = "&Open";
            this.openToolStripButton1.Click += new System.EventHandler(this.openToolStripButton1_Click);
            // 
            // saveToolStripButton1
            // 
            this.saveToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton1.Image")));
            this.saveToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton1.Name = "saveToolStripButton1";
            this.saveToolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton1.Text = "&Save";
            this.saveToolStripButton1.Click += new System.EventHandler(this.saveToolStripButton1_Click);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(6, 25);
            // 
            // TsbQuickSaveGameImport
            // 
            this.TsbQuickSaveGameImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TsbQuickSaveGameImport.Image = ((System.Drawing.Image)(resources.GetObject("TsbQuickSaveGameImport.Image")));
            this.TsbQuickSaveGameImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbQuickSaveGameImport.Name = "TsbQuickSaveGameImport";
            this.TsbQuickSaveGameImport.Size = new System.Drawing.Size(45, 22);
            this.TsbQuickSaveGameImport.Text = "ImpSg";
            this.TsbQuickSaveGameImport.ToolTipText = "Import Savegame";
            this.TsbQuickSaveGameImport.Click += new System.EventHandler(this.TsbQuickSaveGameImport_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSettings
            // 
            this.toolStripButtonSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSettings.Image = global::ARKBreedingStats.Properties.Resources.settings;
            this.toolStripButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSettings.Name = "toolStripButtonSettings";
            this.toolStripButtonSettings.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSettings.Text = "Settings";
            this.toolStripButtonSettings.Click += new System.EventHandler(this.toolStripButtonSettings_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonCopy2Tester
            // 
            this.toolStripButtonCopy2Tester.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonCopy2Tester.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCopy2Tester.Image")));
            this.toolStripButtonCopy2Tester.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCopy2Tester.Name = "toolStripButtonCopy2Tester";
            this.toolStripButtonCopy2Tester.Size = new System.Drawing.Size(86, 22);
            this.toolStripButtonCopy2Tester.Text = "Copy to Tester";
            this.toolStripButtonCopy2Tester.Click += new System.EventHandler(this.toolStripButtonCopy2Tester_Click);
            // 
            // toolStripButtonCopy2Extractor
            // 
            this.toolStripButtonCopy2Extractor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonCopy2Extractor.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCopy2Extractor.Image")));
            this.toolStripButtonCopy2Extractor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCopy2Extractor.Name = "toolStripButtonCopy2Extractor";
            this.toolStripButtonCopy2Extractor.Size = new System.Drawing.Size(103, 22);
            this.toolStripButtonCopy2Extractor.Text = "Copy to Extractor";
            this.toolStripButtonCopy2Extractor.Visible = false;
            this.toolStripButtonCopy2Extractor.Click += new System.EventHandler(this.toolStripButtonCopy2Extractor_Click);
            // 
            // toolStripButtonClear
            // 
            this.toolStripButtonClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonClear.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClear.Image")));
            this.toolStripButtonClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClear.Name = "toolStripButtonClear";
            this.toolStripButtonClear.Size = new System.Drawing.Size(38, 22);
            this.toolStripButtonClear.Text = "Clear";
            this.toolStripButtonClear.Click += new System.EventHandler(this.toolStripButtonClear_Click);
            // 
            // toolStripButtonAddPlayer
            // 
            this.toolStripButtonAddPlayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddPlayer.Image = global::ARKBreedingStats.Properties.Resources.newPlayer;
            this.toolStripButtonAddPlayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddPlayer.Name = "toolStripButtonAddPlayer";
            this.toolStripButtonAddPlayer.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddPlayer.Text = "Add Player";
            this.toolStripButtonAddPlayer.Visible = false;
            this.toolStripButtonAddPlayer.Click += new System.EventHandler(this.toolStripButtonAddPlayer_Click);
            // 
            // toolStripButtonAddTribe
            // 
            this.toolStripButtonAddTribe.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddTribe.Image = global::ARKBreedingStats.Properties.Resources.newTribe;
            this.toolStripButtonAddTribe.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddTribe.Name = "toolStripButtonAddTribe";
            this.toolStripButtonAddTribe.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddTribe.Text = "Add Tribe";
            this.toolStripButtonAddTribe.Visible = false;
            this.toolStripButtonAddTribe.Click += new System.EventHandler(this.toolStripButtonAddTribe_Click);
            // 
            // toolStripButtonDeleteExpiredIncubationTimers
            // 
            this.toolStripButtonDeleteExpiredIncubationTimers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonDeleteExpiredIncubationTimers.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDeleteExpiredIncubationTimers.Image")));
            this.toolStripButtonDeleteExpiredIncubationTimers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeleteExpiredIncubationTimers.Name = "toolStripButtonDeleteExpiredIncubationTimers";
            this.toolStripButtonDeleteExpiredIncubationTimers.Size = new System.Drawing.Size(103, 22);
            this.toolStripButtonDeleteExpiredIncubationTimers.Text = "Delete All Expired";
            this.toolStripButtonDeleteExpiredIncubationTimers.Visible = false;
            this.toolStripButtonDeleteExpiredIncubationTimers.Click += new System.EventHandler(this.toolStripButtonDeleteExpiredIncubationTimers_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSaveCreatureValuesTemp
            // 
            this.toolStripButtonSaveCreatureValuesTemp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonSaveCreatureValuesTemp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveCreatureValuesTemp.Image")));
            this.toolStripButtonSaveCreatureValuesTemp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveCreatureValuesTemp.Name = "toolStripButtonSaveCreatureValuesTemp";
            this.toolStripButtonSaveCreatureValuesTemp.Size = new System.Drawing.Size(71, 22);
            this.toolStripButtonSaveCreatureValuesTemp.Text = "Save values";
            this.toolStripButtonSaveCreatureValuesTemp.ToolTipText = "Save entered values until extraction-issue is resolved. This creature cannot be u" +
    "sed in other parts of this application until it is properly extracted.";
            this.toolStripButtonSaveCreatureValuesTemp.Click += new System.EventHandler(this.toolStripButtonSaveCreatureValuesTemp_Click);
            // 
            // toolStripCBTempCreatures
            // 
            this.toolStripCBTempCreatures.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripCBTempCreatures.Name = "toolStripCBTempCreatures";
            this.toolStripCBTempCreatures.Size = new System.Drawing.Size(180, 25);
            this.toolStripCBTempCreatures.SelectedIndexChanged += new System.EventHandler(this.toolStripCBTempCreatures_SelectedIndexChanged);
            // 
            // toolStripButtonDeleteTempCreature
            // 
            this.toolStripButtonDeleteTempCreature.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonDeleteTempCreature.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDeleteTempCreature.Image")));
            this.toolStripButtonDeleteTempCreature.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeleteTempCreature.Name = "toolStripButtonDeleteTempCreature";
            this.toolStripButtonDeleteTempCreature.Size = new System.Drawing.Size(90, 22);
            this.toolStripButtonDeleteTempCreature.Text = "Delete temp Cr";
            this.toolStripButtonDeleteTempCreature.ToolTipText = "Delete currently selected data of the temporary creature";
            this.toolStripButtonDeleteTempCreature.Visible = false;
            this.toolStripButtonDeleteTempCreature.Click += new System.EventHandler(this.toolStripButtonDeleteTempCreature_Click);
            // 
            // tsBtAddAsExtractionTest
            // 
            this.tsBtAddAsExtractionTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtAddAsExtractionTest.Image = ((System.Drawing.Image)(resources.GetObject("tsBtAddAsExtractionTest.Image")));
            this.tsBtAddAsExtractionTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtAddAsExtractionTest.Name = "tsBtAddAsExtractionTest";
            this.tsBtAddAsExtractionTest.Size = new System.Drawing.Size(70, 22);
            this.tsBtAddAsExtractionTest.Text = "Add as Test";
            this.tsBtAddAsExtractionTest.Click += new System.EventHandler(this.tsBtAddAsExtractionTest_Click);
            // 
            // copyToMultiplierTesterToolStripButton
            // 
            this.copyToMultiplierTesterToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.copyToMultiplierTesterToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("copyToMultiplierTesterToolStripButton.Image")));
            this.copyToMultiplierTesterToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToMultiplierTesterToolStripButton.Name = "copyToMultiplierTesterToolStripButton";
            this.copyToMultiplierTesterToolStripButton.Size = new System.Drawing.Size(137, 22);
            this.copyToMultiplierTesterToolStripButton.Text = "Copy to MultiplierTester";
            this.copyToMultiplierTesterToolStripButton.Click += new System.EventHandler(this.copyToMultiplierTesterToolStripButton_Click);
            // 
            // ToolStripLabelFilter
            // 
            this.ToolStripLabelFilter.Name = "ToolStripLabelFilter";
            this.ToolStripLabelFilter.Size = new System.Drawing.Size(33, 22);
            this.ToolStripLabelFilter.Text = "Filter";
            this.ToolStripLabelFilter.Visible = false;
            // 
            // ToolStripTextBoxLibraryFilter
            // 
            this.ToolStripTextBoxLibraryFilter.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ToolStripTextBoxLibraryFilter.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.ToolStripTextBoxLibraryFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ToolStripTextBoxLibraryFilter.Name = "ToolStripTextBoxLibraryFilter";
            this.ToolStripTextBoxLibraryFilter.Size = new System.Drawing.Size(200, 25);
            this.ToolStripTextBoxLibraryFilter.Visible = false;
            this.ToolStripTextBoxLibraryFilter.Click += new System.EventHandler(this.ToolStripTextBoxLibraryFilter_Click);
            this.ToolStripTextBoxLibraryFilter.TextChanged += new System.EventHandler(this.ToolStripTextBoxLibraryFilter_TextChanged);
            // 
            // ToolStripButtonLibraryFilterClear
            // 
            this.ToolStripButtonLibraryFilterClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ToolStripButtonLibraryFilterClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripButtonLibraryFilterClear.Name = "ToolStripButtonLibraryFilterClear";
            this.ToolStripButtonLibraryFilterClear.Size = new System.Drawing.Size(23, 22);
            this.ToolStripButtonLibraryFilterClear.Text = "×";
            this.ToolStripButtonLibraryFilterClear.Visible = false;
            this.ToolStripButtonLibraryFilterClear.Click += new System.EventHandler(this.ToolStripButtonLibraryFilterClear_Click);
            // 
            // ToolStripButtonSaveFilterPreset
            // 
            this.ToolStripButtonSaveFilterPreset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ToolStripButtonSaveFilterPreset.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripButtonSaveFilterPreset.Image")));
            this.ToolStripButtonSaveFilterPreset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripButtonSaveFilterPreset.Name = "ToolStripButtonSaveFilterPreset";
            this.ToolStripButtonSaveFilterPreset.Size = new System.Drawing.Size(23, 22);
            this.ToolStripButtonSaveFilterPreset.Text = "▼";
            this.ToolStripButtonSaveFilterPreset.ToolTipText = "Save filter as preset";
            this.ToolStripButtonSaveFilterPreset.Visible = false;
            this.ToolStripButtonSaveFilterPreset.Click += new System.EventHandler(this.ToolStripButtonSaveFilterPresetClick);
            // 
            // TsSpOcrLabel
            // 
            this.TsSpOcrLabel.Name = "TsSpOcrLabel";
            this.TsSpOcrLabel.Size = new System.Drawing.Size(6, 25);
            // 
            // TsLbLabelSet
            // 
            this.TsLbLabelSet.Name = "TsLbLabelSet";
            this.TsLbLabelSet.Size = new System.Drawing.Size(77, 22);
            this.TsLbLabelSet.Text = "OCR label set";
            // 
            // TsCbbLabelSets
            // 
            this.TsCbbLabelSets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TsCbbLabelSets.Name = "TsCbbLabelSets";
            this.TsCbbLabelSets.Size = new System.Drawing.Size(121, 25);
            this.TsCbbLabelSets.SelectedIndexChanged += new System.EventHandler(this.TsCbbLabelSets_SelectedIndexChanged);
            // 
            // panelToolBar
            // 
            this.panelToolBar.Controls.Add(this.btReadValuesFromArk);
            this.panelToolBar.Controls.Add(this.btImportLastExported);
            this.panelToolBar.Controls.Add(this.pbSpecies);
            this.panelToolBar.Controls.Add(this.tbSpeciesGlobal);
            this.panelToolBar.Controls.Add(this.cbGuessSpecies);
            this.panelToolBar.Controls.Add(this.cbToggleOverlay);
            this.panelToolBar.Controls.Add(this.lbListening);
            this.panelToolBar.Controls.Add(this.cbEventMultipliers);
            this.panelToolBar.Controls.Add(this.lbSpecies);
            this.panelToolBar.Controls.Add(this.TbMessageLabel);
            this.panelToolBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelToolBar.Location = new System.Drawing.Point(0, 49);
            this.panelToolBar.Name = "panelToolBar";
            this.panelToolBar.Size = new System.Drawing.Size(1878, 54);
            this.panelToolBar.TabIndex = 2;
            // 
            // btImportLastExported
            // 
            this.btImportLastExported.Location = new System.Drawing.Point(379, 3);
            this.btImportLastExported.Name = "btImportLastExported";
            this.btImportLastExported.Size = new System.Drawing.Size(85, 44);
            this.btImportLastExported.TabIndex = 4;
            this.btImportLastExported.Text = "Last Export";
            this.btImportLastExported.UseVisualStyleBackColor = true;
            this.btImportLastExported.Click += new System.EventHandler(this.btImportLastExported_Click);
            // 
            // pbSpecies
            // 
            this.pbSpecies.Location = new System.Drawing.Point(3, 3);
            this.pbSpecies.Name = "pbSpecies";
            this.pbSpecies.Size = new System.Drawing.Size(44, 44);
            this.pbSpecies.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbSpecies.TabIndex = 13;
            this.pbSpecies.TabStop = false;
            this.pbSpecies.Click += new System.EventHandler(this.pbSpecies_Click);
            // 
            // tbSpeciesGlobal
            // 
            this.tbSpeciesGlobal.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.tbSpeciesGlobal.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbSpeciesGlobal.Location = new System.Drawing.Point(104, 3);
            this.tbSpeciesGlobal.Name = "tbSpeciesGlobal";
            this.tbSpeciesGlobal.Size = new System.Drawing.Size(152, 20);
            this.tbSpeciesGlobal.TabIndex = 8;
            this.tbSpeciesGlobal.Click += new System.EventHandler(this.tbSpeciesGlobal_Click);
            this.tbSpeciesGlobal.Enter += new System.EventHandler(this.tbSpeciesGlobal_Enter);
            this.tbSpeciesGlobal.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TbSpeciesGlobal_KeyUp);
            // 
            // cbGuessSpecies
            // 
            this.cbGuessSpecies.AutoSize = true;
            this.cbGuessSpecies.Location = new System.Drawing.Point(127, 29);
            this.cbGuessSpecies.Name = "cbGuessSpecies";
            this.cbGuessSpecies.Size = new System.Drawing.Size(97, 17);
            this.cbGuessSpecies.TabIndex = 2;
            this.cbGuessSpecies.Text = "Guess Species";
            this.cbGuessSpecies.UseVisualStyleBackColor = true;
            // 
            // cbToggleOverlay
            // 
            this.cbToggleOverlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbToggleOverlay.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbToggleOverlay.AutoSize = true;
            this.cbToggleOverlay.Location = new System.Drawing.Point(1813, 28);
            this.cbToggleOverlay.Name = "cbToggleOverlay";
            this.cbToggleOverlay.Size = new System.Drawing.Size(53, 23);
            this.cbToggleOverlay.TabIndex = 7;
            this.cbToggleOverlay.Text = "Overlay";
            this.cbToggleOverlay.UseVisualStyleBackColor = true;
            this.cbToggleOverlay.CheckedChanged += new System.EventHandler(this.chkbToggleOverlay_CheckedChanged);
            // 
            // lbListening
            // 
            this.lbListening.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbListening.AutoSize = true;
            this.lbListening.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbListening.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lbListening.Location = new System.Drawing.Point(1845, 3);
            this.lbListening.Name = "lbListening";
            this.lbListening.Size = new System.Drawing.Size(25, 20);
            this.lbListening.TabIndex = 6;
            this.lbListening.Text = "🎤";
            this.lbListening.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lbListening.Click += new System.EventHandler(this.labelListening_Click);
            // 
            // lbSpecies
            // 
            this.lbSpecies.AutoSize = true;
            this.lbSpecies.Location = new System.Drawing.Point(53, 6);
            this.lbSpecies.Name = "lbSpecies";
            this.lbSpecies.Size = new System.Drawing.Size(45, 13);
            this.lbSpecies.TabIndex = 0;
            this.lbSpecies.Text = "Species";
            // 
            // TbMessageLabel
            // 
            this.TbMessageLabel.AcceptsReturn = true;
            this.TbMessageLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TbMessageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TbMessageLabel.Location = new System.Drawing.Point(470, 3);
            this.TbMessageLabel.Multiline = true;
            this.TbMessageLabel.Name = "TbMessageLabel";
            this.TbMessageLabel.ReadOnly = true;
            this.TbMessageLabel.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TbMessageLabel.Size = new System.Drawing.Size(889, 48);
            this.TbMessageLabel.TabIndex = 14;
            this.TbMessageLabel.Click += new System.EventHandler(this.TbMessageLabel_Click);
            // 
            // contextMenuStripLibraryHeader
            // 
            this.contextMenuStripLibraryHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemResetLibraryColumnWidths,
            this.resetColumnWidthNoMutationLevelColumnsToolStripMenuItem,
            this.restoreMutationLevelsASAToolStripMenuItem,
            this.collapseMutationsLevelsASEToolStripMenuItem,
            this.toolStripSeparator27,
            this.resetColumnOrderToolStripMenuItem});
            this.contextMenuStripLibraryHeader.Name = "contextMenuStrip1";
            this.contextMenuStripLibraryHeader.Size = new System.Drawing.Size(333, 120);
            // 
            // toolStripMenuItemResetLibraryColumnWidths
            // 
            this.toolStripMenuItemResetLibraryColumnWidths.Name = "toolStripMenuItemResetLibraryColumnWidths";
            this.toolStripMenuItemResetLibraryColumnWidths.Size = new System.Drawing.Size(332, 22);
            this.toolStripMenuItemResetLibraryColumnWidths.Text = "Reset all column widths";
            this.toolStripMenuItemResetLibraryColumnWidths.Click += new System.EventHandler(this.toolStripMenuItemResetLibraryColumnWidths_Click);
            // 
            // resetColumnWidthNoMutationLevelColumnsToolStripMenuItem
            // 
            this.resetColumnWidthNoMutationLevelColumnsToolStripMenuItem.Name = "resetColumnWidthNoMutationLevelColumnsToolStripMenuItem";
            this.resetColumnWidthNoMutationLevelColumnsToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.resetColumnWidthNoMutationLevelColumnsToolStripMenuItem.Text = "Reset all column widths, collapse mutation levels";
            this.resetColumnWidthNoMutationLevelColumnsToolStripMenuItem.Click += new System.EventHandler(this.resetColumnWidthNoMutationLevelColumnsToolStripMenuItem_Click);
            // 
            // restoreMutationLevelsASAToolStripMenuItem
            // 
            this.restoreMutationLevelsASAToolStripMenuItem.Name = "restoreMutationLevelsASAToolStripMenuItem";
            this.restoreMutationLevelsASAToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.restoreMutationLevelsASAToolStripMenuItem.Text = "Restore mutation levels (ASA)";
            this.restoreMutationLevelsASAToolStripMenuItem.Click += new System.EventHandler(this.restoreMutationLevelsASAToolStripMenuItem_Click);
            // 
            // collapseMutationsLevelsASEToolStripMenuItem
            // 
            this.collapseMutationsLevelsASEToolStripMenuItem.Name = "collapseMutationsLevelsASEToolStripMenuItem";
            this.collapseMutationsLevelsASEToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.collapseMutationsLevelsASEToolStripMenuItem.Text = "Collapse mutations levels (ASE)";
            this.collapseMutationsLevelsASEToolStripMenuItem.Click += new System.EventHandler(this.collapseMutationsLevelsASEToolStripMenuItem_Click);
            // 
            // toolStripSeparator27
            // 
            this.toolStripSeparator27.Name = "toolStripSeparator27";
            this.toolStripSeparator27.Size = new System.Drawing.Size(329, 6);
            // 
            // resetColumnOrderToolStripMenuItem
            // 
            this.resetColumnOrderToolStripMenuItem.Name = "resetColumnOrderToolStripMenuItem";
            this.resetColumnOrderToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            this.resetColumnOrderToolStripMenuItem.Text = "Reset column order";
            this.resetColumnOrderToolStripMenuItem.Click += new System.EventHandler(this.resetColumnOrderToolStripMenuItem_Click);
            // 
            // speciesSelector1
            // 
            this.speciesSelector1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.speciesSelector1.LastSpecies = new string[0];
            this.speciesSelector1.Location = new System.Drawing.Point(0, 103);
            this.speciesSelector1.Name = "speciesSelector1";
            this.speciesSelector1.Size = new System.Drawing.Size(1878, 810);
            this.speciesSelector1.SplitterDistance = 500;
            this.speciesSelector1.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(69, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 37;
            this.label4.Text = "Mutations";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(174, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 51;
            this.label5.Text = "Mutations";
            // 
            // Form1
            // 
            this.AcceptButton = this.btExtractLevels;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1878, 935);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.speciesSelector1);
            this.Controls.Add(this.panelToolBar);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "ARK Smart Breeding";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownImprintingBonusTester)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownTestingTE)).EndInit();
            this.groupBoxPossibilities.ResumeLayout(false);
            this.groupBoxDetailsExtractor.ResumeLayout(false);
            this.panelExtrImpr.ResumeLayout(false);
            this.panelExtrImpr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownImprintingBonusExtractor)).EndInit();
            this.panelExtrTE.ResumeLayout(false);
            this.panelExtrTE.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpperTEffBound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowerTEffBound)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelSums.ResumeLayout(false);
            this.panelSums.PerformLayout();
            this.panelWildTamedBred.ResumeLayout(false);
            this.panelWildTamedBred.PerformLayout();
            this.tabControlMain.ResumeLayout(false);
            this.tabPageStatTesting.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColorRegionsTester)).EndInit();
            this.gbStatChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radarChart1)).EndInit();
            this.panelWildTamedBredTester.ResumeLayout(false);
            this.panelWildTamedBredTester.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.flowLayoutPanelStatIOsTester.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelStatTesterFootnote.ResumeLayout(false);
            this.panelStatTesterFootnote.PerformLayout();
            this.gpPreviewEdit.ResumeLayout(false);
            this.gpPreviewEdit.PerformLayout();
            this.tabPageExtractor.ResumeLayout(false);
            this.tabPageExtractor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PbCreatureColorsExtractor)).EndInit();
            this.groupBoxRadarChartExtractor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radarChartExtractor)).EndInit();
            this.groupBoxTamingInfo.ResumeLayout(false);
            this.gbStatsExtractor.ResumeLayout(false);
            this.flowLayoutPanelStatIOsExtractor.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).EndInit();
            this.tabPageLibrary.ResumeLayout(false);
            this.tableLayoutPanelLibrary.ResumeLayout(false);
            this.contextMenuStripLibrary.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabControlLibFilter.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tabPageLibRadarChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radarChartLibrary)).EndInit();
            this.tabPageLibraryInfo.ResumeLayout(false);
            this.tlpLibraryInfo.ResumeLayout(false);
            this.tlpLibraryInfo.PerformLayout();
            this.tabPagePedigree.ResumeLayout(false);
            this.tabPageTaming.ResumeLayout(false);
            this.tabPageBreedingPlan.ResumeLayout(false);
            this.tabPageHatching.ResumeLayout(false);
            this.tabPageRaising.ResumeLayout(false);
            this.tabPageTimer.ResumeLayout(false);
            this.tabPagePlayerTribes.ResumeLayout(false);
            this.tabPageNotes.ResumeLayout(false);
            this.TabPageOCR.ResumeLayout(false);
            this.tabPageExtractionTests.ResumeLayout(false);
            this.tabPageMultiplierTesting.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.panelToolBar.ResumeLayout(false);
            this.panelToolBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSpecies)).EndInit();
            this.contextMenuStripLibraryHeader.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbExtractorWildLevel;
        private System.Windows.Forms.Label lbExtractorDomLevel;
        private uiControls.Nud numericUpDownLowerTEffBound;
        private System.Windows.Forms.GroupBox groupBoxDetailsExtractor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelHBV;
        private uiControls.Nud numericUpDownLevel;
        private System.Windows.Forms.GroupBox groupBoxPossibilities;
        private System.Windows.Forms.Label lbInfoYellowStats;
        private System.Windows.Forms.Label labelFootnote;
        private System.Windows.Forms.Label label3;
        private uiControls.Nud numericUpDownUpperTEffBound;
        private System.Windows.Forms.Label lbLevel;
        private System.Windows.Forms.Label labelTE;
        private System.Windows.Forms.Label lbSum;
        private System.Windows.Forms.Label lbSumWild;
        private System.Windows.Forms.Label lbSumDom;
        private System.Windows.Forms.Panel panelSums;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageStatTesting;
        private System.Windows.Forms.GroupBox groupBox1;
        private uiControls.Nud NumericUpDownTestingTE;
        private System.Windows.Forms.Label labelTesterTE;
        private System.Windows.Forms.Label lbBreedingValueTester;
        private System.Windows.Forms.Label lbTesterWildLevel;
        private System.Windows.Forms.Label lbTesterDomLevel;
        private System.Windows.Forms.RadioButton rbWildExtractor;
        private System.Windows.Forms.RadioButton rbTamedExtractor;
        private System.Windows.Forms.Panel panelWildTamedBred;
        private System.Windows.Forms.TabPage tabPageExtractor;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatedStatsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageLibrary;
        private System.Windows.Forms.ListView listViewLibrary;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderSex;
        private System.Windows.Forms.ColumnHeader columnHeaderHP;
        private System.Windows.Forms.ColumnHeader columnHeaderSt;
        private System.Windows.Forms.ColumnHeader columnHeaderTo;
        private System.Windows.Forms.ColumnHeader columnHeaderOx;
        private System.Windows.Forms.ColumnHeader columnHeaderFo;
        private System.Windows.Forms.ColumnHeader columnHeaderWa;
        private System.Windows.Forms.ColumnHeader columnHeaderWe;
        private System.Windows.Forms.ColumnHeader columnHeaderTm;
        private System.Windows.Forms.ColumnHeader columnHeaderDm;
        private System.Windows.Forms.ColumnHeader columnHeaderSp;
        private System.Windows.Forms.ColumnHeader columnHeaderFr;
        private System.Windows.Forms.ColumnHeader columnHeaderCr;
        private System.Windows.Forms.ColumnHeader columnHeaderOwner;
        private System.Windows.Forms.ToolStripMenuItem loadAndAddToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private CreatureBox creatureBoxListView;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectedToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeaderTopStatsNr;
        private System.Windows.Forms.Label lbCurrentValue;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPagePedigree;
        private PedigreeControl pedigree1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelLibrary;
        private System.Windows.Forms.Label lbTestingInfo;
        private System.Windows.Forms.ColumnHeader columnHeaderGen;
        private System.Windows.Forms.Label lbNotYetTamed;
        private System.Windows.Forms.CheckBox cbQuickWildCheck;
        private System.Windows.Forms.ToolStripMenuItem onlinehelpToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControlLibFilter;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ColumnHeader columnHeaderTopness;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckedListBox checkedListBoxConsiderStatTop;
        private System.Windows.Forms.Button buttonRecalculateTops;
        private System.Windows.Forms.Label label17;
        private CreatureInfoInput creatureInfoInputExtractor;
        private CreatureInfoInput creatureInfoInputTester;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem setStatusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aliveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unavailableToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeaderFound;
        private System.Windows.Forms.TabPage tabPageBreedingPlan;
        private System.Windows.Forms.ToolStripMenuItem multiSetterToolStripMenuItem;
        private System.Windows.Forms.GroupBox gpPreviewEdit;
        private System.Windows.Forms.ListBox listBoxSpeciesLib;
        private System.Windows.Forms.Label labelDomLevelSum;
        private System.Windows.Forms.Label labelTesterTotalLevel;
        private System.Windows.Forms.Label labelCurrentTesterCreature;
        private System.Windows.Forms.TabPage TabPageOCR;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripLibrary;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEdit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRemove;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStatus;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.TabPage tabPageTimer;
        private TimerControl timerList1;
        private System.Windows.Forms.ToolStripMenuItem copyValuesToExtractorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wildValuesToolStripMenuItem;
        private System.Windows.Forms.ListView listViewPossibilities;
        private System.Windows.Forms.ColumnHeader columnHeaderWild;
        private System.Windows.Forms.ColumnHeader columnHeaderDom;
        private System.Windows.Forms.ColumnHeader columnHeaderTE;
        private System.Windows.Forms.ColumnHeader columnHeaderLW;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton newToolStripButton1;
        private System.Windows.Forms.ToolStripButton openToolStripButton1;
        private System.Windows.Forms.ToolStripButton saveToolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonCopy2Tester;
        private System.Windows.Forms.ToolStripButton toolStripButtonClear;
        private System.Windows.Forms.ToolStripButton toolStripButtonCopy2Extractor;
        private System.Windows.Forms.Button btExtractLevels;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox gbStatsExtractor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelErrorHelp;
        private System.Windows.Forms.Button btReadValuesFromArk;
        private System.Windows.Forms.ToolStripButton toolStripButtonSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem editAllSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findDuplicatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bestBreedingPartnersToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPagePlayerTribes;
        private TribesControl tribesControl1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddPlayer;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddTribe;
        private System.Windows.Forms.Label labelImprintingBonus;
        private uiControls.Nud numericUpDownImprintingBonusExtractor;
        private System.Windows.Forms.Label labelImprintingTester;
        private uiControls.Nud numericUpDownImprintingBonusTester;
        private System.Windows.Forms.ToolStripMenuItem exportValuesToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forSpreadsheetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToClipboardToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem forSpreadsheetToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.TabPage tabPageTaming;
        private TamingControl tamingControl1;
        private System.Windows.Forms.Button button2TamingCalc;
        private System.Windows.Forms.Label labelTamingInfo;
        private System.Windows.Forms.GroupBox groupBoxTamingInfo;
        private System.Windows.Forms.ColumnHeader columnHeaderDomesticated;
        private System.Windows.Forms.Label lbImprintingFailInfo;
        private System.Windows.Forms.Label lbImprintedCount;
        private System.Windows.Forms.Label lbImprintingCuddleCountExtractor;
        private System.Windows.Forms.ColumnHeader columnHeaderMutations;
        private System.Windows.Forms.Label lbShouldBe;
        private System.Windows.Forms.Label lbSumDomSB;
        private System.Windows.Forms.ToolStripMenuItem plainTextbreedingValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plainTextcurrentValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plainTextbreedingValuesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem plainTextcurrentValuesToolStripMenuItem1;
        private System.Windows.Forms.RadioButton rbBredExtractor;
        private System.Windows.Forms.Panel panelExtrTE;
        private System.Windows.Forms.Panel panelWildTamedBredTester;
        private System.Windows.Forms.RadioButton rbBredTester;
        private System.Windows.Forms.RadioButton rbTamedTester;
        private System.Windows.Forms.RadioButton rbWildTester;
        private System.Windows.Forms.Panel panelExtrImpr;
        private System.Windows.Forms.ToolStripMenuItem copyCreatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteCreatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private RadarChart radarChart1;
        private System.Windows.Forms.GroupBox gbStatChart;
        private uiControls.StatPotentials statPotentials1;
        private System.Windows.Forms.ColumnHeader columnHeaderCooldown;
        private System.Windows.Forms.GroupBox groupBoxRadarChartExtractor;
        private RadarChart radarChartExtractor;
        private System.Windows.Forms.CheckBox cbEventMultipliers;
        private System.Windows.Forms.TabPage tabPageRaising;
        private RaisingControl raisingControl1;
        private System.Windows.Forms.TabPage tabPageNotes;
        private NotesControl notesControl1;
        private System.Windows.Forms.ToolStripMenuItem removeCooldownGrowingToolStripMenuItem;
        private System.Windows.Forms.Panel panelToolBar;
        private System.Windows.Forms.Label lbSpecies;
        private BreedingPlan breedingPlan1;
        private System.Windows.Forms.Label lbListening;
        private System.Windows.Forms.CheckBox cbToggleOverlay;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeleteExpiredIncubationTimers;
        private ocr.OCRControl ocrControl1;
        private System.Windows.Forms.TabPage tabPageLibRadarChart;
        private RadarChart radarChartLibrary;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripComboBox toolStripCBTempCreatures;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveCreatureValuesTemp;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeleteTempCreature;
        private System.Windows.Forms.CheckBox cbExactlyImprinting;
        private System.Windows.Forms.Label lbCurrentStatEx;
        private System.Windows.Forms.ColumnHeader columnHeaderNote;
        private System.Windows.Forms.ToolStripMenuItem obeliskToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem obeliskToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.CheckBox cbGuessSpecies;
        private System.Windows.Forms.ColumnHeader columnHeaderColor0;
        private System.Windows.Forms.ColumnHeader columnHeaderColor1;
        private System.Windows.Forms.ColumnHeader columnHeaderColor2;
        private System.Windows.Forms.ColumnHeader columnHeaderColor3;
        private System.Windows.Forms.ColumnHeader columnHeaderColor4;
        private System.Windows.Forms.ColumnHeader columnHeaderColor5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ColumnHeader columnHeaderServer;
        private SpeciesSelector speciesSelector1;
        private uiControls.TextBoxSuggest tbSpeciesGlobal;
        private System.Windows.Forms.PictureBox pbSpecies;
        private System.Windows.Forms.TabPage tabPageExtractionTests;
        private System.Windows.Forms.TabPage tabPageMultiplierTesting;
        private testCases.ExtractionTestControl extractionTestControl1;
        private System.Windows.Forms.ToolStripButton tsBtAddAsExtractionTest;
        private System.Windows.Forms.ToolStripMenuItem importExportedCreaturesToolStripMenuItem;
        private System.Windows.Forms.Button btImportLastExported;
        private System.Windows.Forms.LinkLabel llOnlineHelpExtractionIssues;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem BreedingPlanHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractionIssuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private StatsMultiplierTesting statsMultiplierTesting1;
        private System.Windows.Forms.ToolStripButton copyToMultiplierTesterToolStripButton;
        private System.Windows.Forms.Label lbWildLevelTester;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem importingFromSavegameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importingFromSavegameEmptyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cryopodToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelStatIOsExtractor;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelStatIOsTester;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panelStatTesterFootnote;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpenWiki;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripMenuItem openFolderOfCurrentFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.Label lbCurrentCreature;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopyCreatureName;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemGenerateCreatureName;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem adminCommandToSetColorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixColorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem modValueManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customStatOverridesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openJsonDataFolderToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeaderSpecies;
        private System.Windows.Forms.ColumnHeader columnHeaderStatus;
        private System.Windows.Forms.ColumnHeader columnHeaderTribe;
        private System.Windows.Forms.ToolStripMenuItem copyInfographicToClipboardToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeaderStatusIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripLibraryHeader;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemResetLibraryColumnWidths;
        private System.Windows.Forms.ToolStripMenuItem setToMatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearMatingCooldownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem justMatedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem libraryFilterToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBoxColorRegionsTester;
        private System.Windows.Forms.PictureBox PbCreatureColorsExtractor;
        private System.Windows.Forms.ToolStripTextBox ToolStripTextBoxLibraryFilter;
        private System.Windows.Forms.ToolStripButton ToolStripButtonLibraryFilterClear;
        private System.Windows.Forms.ToolStripLabel ToolStripLabelFilter;
        private uiControls.ParentInheritance parentInheritanceExtractor;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
        private System.Windows.Forms.ToolStripMenuItem copyLibrarydumpToClipboardToolStripMenuItem;
        private System.Windows.Forms.Button BtCopyIssueDumpToClipboard;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripButton TsbQuickSaveGameImport;
        private System.Windows.Forms.Label LbBlueprintPath;
        private System.Windows.Forms.ToolStripMenuItem recentlyUsedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabelImport;
        private System.Windows.Forms.ToolStripMenuItem saveInfographicsToFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator22;
        private System.Windows.Forms.ToolStripMenuItem extraDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator23;
        private System.Windows.Forms.ToolStripButton ToolStripButtonSaveFilterPreset;
        private uiControls.CreatureAnalysis creatureAnalysis1;
        private System.Windows.Forms.ToolStripMenuItem devToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRandomCreaturesToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem breedingPlanForSelectedCreaturesToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageHatching;
        private uiControls.Hatching hatching1;
        private System.Windows.Forms.ToolStripMenuItem speciesSortingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetSortingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editSortingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpAboutSpeciesSortingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetSortingToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem applyMutagenToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator24;
        private System.Windows.Forms.ToolStripMenuItem editSpreadsheetExportFieldsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem adminCommandToSpawnExactDinoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importFromTabSeparatedFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exactSpawnCommandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exactSpawnCommandDS2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
        private System.Windows.Forms.ToolStripMenuItem adminCommandToSpawnExactDinoDS2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorDefinitionsToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator26;
        private System.Windows.Forms.ToolStripMenuItem applyChangedSortingToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator TsSpOcrLabel;
        private System.Windows.Forms.ToolStripLabel TsLbLabelSet;
        private System.Windows.Forms.ToolStripComboBox TsCbbLabelSets;
        private System.Windows.Forms.Label LbWarningLevel255;
        private System.Windows.Forms.TabPage tabPageLibraryInfo;
        private System.Windows.Forms.TableLayoutPanel tlpLibraryInfo;
        private System.Windows.Forms.Button BtCopyLibraryColorToClipboard;
        private System.Windows.Forms.CheckBox CbLibraryInfoUseFilter;
        private System.Windows.Forms.TextBox TbMessageLabel;
        private System.Windows.Forms.ColumnHeader columnHeaderMutagen;
        private System.Windows.Forms.Label LbAsa;
        private System.Windows.Forms.ToolStripMenuItem discordServerToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeaderHPM;
        private System.Windows.Forms.ColumnHeader columnHeaderStM;
        private System.Windows.Forms.ColumnHeader columnHeaderToM;
        private System.Windows.Forms.ColumnHeader columnHeaderOxM;
        private System.Windows.Forms.ColumnHeader columnHeaderFoM;
        private System.Windows.Forms.ColumnHeader columnHeaderWaM;
        private System.Windows.Forms.ColumnHeader columnHeaderTmM;
        private System.Windows.Forms.ColumnHeader columnHeaderWeM;
        private System.Windows.Forms.ColumnHeader columnHeaderDmM;
        private System.Windows.Forms.ColumnHeader columnHeaderSpM;
        private System.Windows.Forms.ColumnHeader columnHeaderFrM;
        private System.Windows.Forms.ColumnHeader columnHeaderCrM;
        private System.Windows.Forms.ToolStripMenuItem resetColumnOrderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetColumnWidthNoMutationLevelColumnsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreMutationLevelsASAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseMutationsLevelsASEToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
        private System.Windows.Forms.ToolStripMenuItem serverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendExampleCreatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listenWithNewTokenToolStripMenuItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}
