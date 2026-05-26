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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            loadAndAddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            recentlyUsedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            openFolderOfCurrentFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            importingFromSavegameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            selectSavegameFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            configureSavegameImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            importExportedCreaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            importFromTabSeparatedFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            copyLibrarydumpToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            groupBox1 = new System.Windows.Forms.GroupBox();
            lbImprintedCount = new System.Windows.Forms.Label();
            BtSetImprinting100Tester = new System.Windows.Forms.Button();
            labelImprintingTester = new System.Windows.Forms.Label();
            numericUpDownImprintingBonusTester = new ARKBreedingStats.uiControls.Nud();
            NumericUpDownTestingTE = new ARKBreedingStats.uiControls.Nud();
            labelTesterTE = new System.Windows.Forms.Label();
            groupBoxPossibilities = new System.Windows.Forms.GroupBox();
            listViewPossibilities = new System.Windows.Forms.ListView();
            columnHeaderWild = new System.Windows.Forms.ColumnHeader();
            columnHeaderMutated = new System.Windows.Forms.ColumnHeader();
            columnHeaderDom = new System.Windows.Forms.ColumnHeader();
            columnHeaderTE = new System.Windows.Forms.ColumnHeader();
            columnHeaderLW = new System.Windows.Forms.ColumnHeader();
            groupBoxDetailsExtractor = new System.Windows.Forms.GroupBox();
            panelExtrImpr = new System.Windows.Forms.Panel();
            BtSetImprinting0Extractor = new System.Windows.Forms.Button();
            BtSetImprinting100Extractor = new System.Windows.Forms.Button();
            cbExactlyImprinting = new System.Windows.Forms.CheckBox();
            labelImprintingBonus = new System.Windows.Forms.Label();
            lbImprintingCuddleCountExtractor = new System.Windows.Forms.Label();
            numericUpDownImprintingBonusExtractor = new ARKBreedingStats.uiControls.Nud();
            panelExtrTE = new System.Windows.Forms.Panel();
            labelTE = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            numericUpDownUpperTEffBound = new ARKBreedingStats.uiControls.Nud();
            label3 = new System.Windows.Forms.Label();
            numericUpDownLowerTEffBound = new ARKBreedingStats.uiControls.Nud();
            lbLevel = new System.Windows.Forms.Label();
            lbBreedingValueTester = new System.Windows.Forms.Label();
            lbTesterWildLevel = new System.Windows.Forms.Label();
            lbTesterDomLevel = new System.Windows.Forms.Label();
            lbInfoYellowStats = new System.Windows.Forms.Label();
            labelFootnote = new System.Windows.Forms.Label();
            labelHBV = new System.Windows.Forms.Label();
            lbExtractorDomLevel = new System.Windows.Forms.Label();
            lbExtractorWildLevel = new System.Windows.Forms.Label();
            lbSum = new System.Windows.Forms.Label();
            lbSumDom = new System.Windows.Forms.Label();
            lbSumWild = new System.Windows.Forms.Label();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exportValuesToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            plainTextcurrentValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            plainTextbreedingValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            forSpreadsheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editSpreadsheetExportFieldsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            setStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aliveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            unavailableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            obeliskToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            multiSetterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            deleteSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            findDuplicatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            spawnWildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exactSpawnCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exactSpawnCommandDS2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            commandMutationLevelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyCoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            copyCreatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pasteCreatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            libraryFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemMutationColumns = new System.Windows.Forms.ToolStripMenuItem();
            nameGeneratorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            statsOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            modValueManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            customStatOverridesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            speciesImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            extraDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            openJsonDataFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            speciesSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            resetSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            resetSortingToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            editSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            applyChangedSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpAboutSpeciesSortingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editVariantTagsToHideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            appSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            showSettingsFileInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            loadAppSettingsFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveAppSettingsTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator29 = new System.Windows.Forms.ToolStripSeparator();
            showStatsOptionsFileInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            serverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            listenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            currentTokenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            listenWithNewTokenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            showTokenPopupOnListeningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            beginListeningToExportGunOnLaunchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
            openModPageInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            sendExampleCreatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveExportFileLocallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            sendServerCreatureStatusNeuterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            sendServerCreatureStatusDeadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            arkUtilsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            howManyFemalesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            howGoodAreMyStatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            discordServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            onlinehelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            BreedingPlanHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            extractionIssuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            uIScalingIssueFixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            checkForUpdatedStatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            devToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            addRandomCreaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            colorDefinitionsToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            panelSums = new System.Windows.Forms.Panel();
            lbShouldBe = new System.Windows.Forms.Label();
            lbSumDomSB = new System.Windows.Forms.Label();
            panelWildTamedBred = new System.Windows.Forms.Panel();
            rbBredExtractor = new System.Windows.Forms.RadioButton();
            rbTamedExtractor = new System.Windows.Forms.RadioButton();
            rbWildExtractor = new System.Windows.Forms.RadioButton();
            tabControlMain = new System.Windows.Forms.TabControl();
            tabPageStatTesting = new System.Windows.Forms.TabPage();
            ColoredCreatureImageDisplayTester = new ARKBreedingStats.uiControls.ColoredCreatureImageWithPose();
            CbLinkWildMutatedLevelsTester = new System.Windows.Forms.CheckBox();
            statPotentials1 = new ARKBreedingStats.uiControls.StatPotentials();
            gbStatChart = new System.Windows.Forms.GroupBox();
            radarChart1 = new RadarChart();
            panelWildTamedBredTester = new System.Windows.Forms.Panel();
            rbBredTester = new System.Windows.Forms.RadioButton();
            rbTamedTester = new System.Windows.Forms.RadioButton();
            rbWildTester = new System.Windows.Forms.RadioButton();
            groupBox2 = new System.Windows.Forms.GroupBox();
            flowLayoutPanelStatIOsTester = new System.Windows.Forms.FlowLayoutPanel();
            panel2 = new System.Windows.Forms.Panel();
            label4 = new System.Windows.Forms.Label();
            lbCurrentValue = new System.Windows.Forms.Label();
            panelStatTesterFootnote = new System.Windows.Forms.Panel();
            LbWarningLevel255 = new System.Windows.Forms.Label();
            lbWildLevelTester = new System.Windows.Forms.Label();
            labelDomLevelSum = new System.Windows.Forms.Label();
            labelTesterTotalLevel = new System.Windows.Forms.Label();
            lbNotYetTamed = new System.Windows.Forms.Label();
            gpPreviewEdit = new System.Windows.Forms.GroupBox();
            lbCurrentCreature = new System.Windows.Forms.Label();
            labelCurrentTesterCreature = new System.Windows.Forms.Label();
            lbTestingInfo = new System.Windows.Forms.Label();
            creatureInfoInputTester = new CreatureInfoInput();
            tabPageExtractor = new System.Windows.Forms.TabPage();
            ColoredCreatureImageDisplayExtractor = new ARKBreedingStats.uiControls.ColoredCreatureImageWithPose();
            pBondedTamingExtractor = new System.Windows.Forms.Panel();
            RbBondedTaming3 = new System.Windows.Forms.RadioButton();
            RbBondedTaming2 = new System.Windows.Forms.RadioButton();
            RbBondedTaming1 = new System.Windows.Forms.RadioButton();
            RbBondedTaming0 = new System.Windows.Forms.RadioButton();
            LbBondedTaming = new System.Windows.Forms.Label();
            LbAsa = new System.Windows.Forms.Label();
            LbBlueprintPath = new System.Windows.Forms.Label();
            BtCopyIssueDumpToClipboard = new System.Windows.Forms.Button();
            llOnlineHelpExtractionIssues = new System.Windows.Forms.LinkLabel();
            groupBoxRadarChartExtractor = new System.Windows.Forms.GroupBox();
            radarChartExtractor = new RadarChart();
            lbImprintingFailInfo = new System.Windows.Forms.Label();
            groupBoxTamingInfo = new System.Windows.Forms.GroupBox();
            labelTamingInfo = new System.Windows.Forms.Label();
            button2TamingCalc = new System.Windows.Forms.Button();
            gbStatsExtractor = new System.Windows.Forms.GroupBox();
            flowLayoutPanelStatIOsExtractor = new System.Windows.Forms.FlowLayoutPanel();
            panel1 = new System.Windows.Forms.Panel();
            label5 = new System.Windows.Forms.Label();
            lbCurrentStatEx = new System.Windows.Forms.Label();
            btExtractLevels = new System.Windows.Forms.Button();
            cbQuickWildCheck = new System.Windows.Forms.CheckBox();
            labelErrorHelp = new System.Windows.Forms.Label();
            creatureAnalysis1 = new ARKBreedingStats.uiControls.CreatureAnalysis();
            parentInheritanceExtractor = new ARKBreedingStats.uiControls.ParentInheritance();
            numericUpDownLevel = new ARKBreedingStats.uiControls.Nud();
            creatureInfoInputExtractor = new CreatureInfoInput();
            tabPageLibrary = new System.Windows.Forms.TabPage();
            tableLayoutPanelLibrary = new System.Windows.Forms.TableLayoutPanel();
            listViewLibrary = new System.Windows.Forms.ListView();
            columnHeaderName = new System.Windows.Forms.ColumnHeader();
            columnHeaderOwner = new System.Windows.Forms.ColumnHeader();
            columnHeaderNote = new System.Windows.Forms.ColumnHeader();
            columnHeaderServer = new System.Windows.Forms.ColumnHeader();
            columnHeaderSex = new System.Windows.Forms.ColumnHeader();
            columnHeaderDomesticated = new System.Windows.Forms.ColumnHeader();
            columnHeaderTopness = new System.Windows.Forms.ColumnHeader();
            columnHeaderTopStatsNr = new System.Windows.Forms.ColumnHeader();
            columnHeaderGen = new System.Windows.Forms.ColumnHeader();
            columnHeaderFound = new System.Windows.Forms.ColumnHeader();
            columnHeaderMutations = new System.Windows.Forms.ColumnHeader();
            columnHeaderCooldown = new System.Windows.Forms.ColumnHeader();
            columnHeaderHP = new System.Windows.Forms.ColumnHeader();
            columnHeaderSt = new System.Windows.Forms.ColumnHeader();
            columnHeaderTo = new System.Windows.Forms.ColumnHeader();
            columnHeaderOx = new System.Windows.Forms.ColumnHeader();
            columnHeaderFo = new System.Windows.Forms.ColumnHeader();
            columnHeaderWa = new System.Windows.Forms.ColumnHeader();
            columnHeaderTm = new System.Windows.Forms.ColumnHeader();
            columnHeaderWe = new System.Windows.Forms.ColumnHeader();
            columnHeaderDm = new System.Windows.Forms.ColumnHeader();
            columnHeaderSp = new System.Windows.Forms.ColumnHeader();
            columnHeaderFr = new System.Windows.Forms.ColumnHeader();
            columnHeaderCr = new System.Windows.Forms.ColumnHeader();
            columnHeaderHPM = new System.Windows.Forms.ColumnHeader();
            columnHeaderStM = new System.Windows.Forms.ColumnHeader();
            columnHeaderToM = new System.Windows.Forms.ColumnHeader();
            columnHeaderOxM = new System.Windows.Forms.ColumnHeader();
            columnHeaderFoM = new System.Windows.Forms.ColumnHeader();
            columnHeaderWaM = new System.Windows.Forms.ColumnHeader();
            columnHeaderTmM = new System.Windows.Forms.ColumnHeader();
            columnHeaderWeM = new System.Windows.Forms.ColumnHeader();
            columnHeaderDmM = new System.Windows.Forms.ColumnHeader();
            columnHeaderSpM = new System.Windows.Forms.ColumnHeader();
            columnHeaderFrM = new System.Windows.Forms.ColumnHeader();
            columnHeaderCrM = new System.Windows.Forms.ColumnHeader();
            columnHeaderColor0 = new System.Windows.Forms.ColumnHeader();
            columnHeaderColor1 = new System.Windows.Forms.ColumnHeader();
            columnHeaderColor2 = new System.Windows.Forms.ColumnHeader();
            columnHeaderColor3 = new System.Windows.Forms.ColumnHeader();
            columnHeaderColor4 = new System.Windows.Forms.ColumnHeader();
            columnHeaderColor5 = new System.Windows.Forms.ColumnHeader();
            columnHeaderSpecies = new System.Windows.Forms.ColumnHeader();
            columnHeaderStatus = new System.Windows.Forms.ColumnHeader();
            columnHeaderTribe = new System.Windows.Forms.ColumnHeader();
            columnHeaderStatusIcon = new System.Windows.Forms.ColumnHeader();
            columnHeaderMutagen = new System.Windows.Forms.ColumnHeader();
            columnHeaderCurrentLevel = new System.Windows.Forms.ColumnHeader();
            columnHeaderMaxPossibleLevel = new System.Windows.Forms.ColumnHeader();
            columnHeaderTraits = new System.Windows.Forms.ColumnHeader();
            contextMenuStripLibrary = new System.Windows.Forms.ContextMenuStrip(components);
            toolStripMenuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
            editAllSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItemGenerateCreatureName = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemCopyGeneratedCreatureName = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemCopyCreatureName = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            copyValuesToExtractorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            currentValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            wildValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exportToClipboardToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            plainTextcurrentValuesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            plainTextbreedingValuesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            forSpreadsheetToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            copyInfographicToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveInfographicsToFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            viewColorsInLibraryInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            SetMaturityCooldownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            clearMatingCooldownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            justMatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            maturationSeparator = new System.Windows.Forms.ToolStripSeparator();
            bestBreedingPartnersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            breedingPlanForSelectedCreaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemStatus = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            obeliskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            cryopodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            applyMutagenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editTraitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            adminCommandToSetColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            adminCommandToSpawnExactDinoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            adminCommandToSpawnExactDinoDS2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            adminCommandSetMutationLevelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            fixColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItemOpenWiki = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItemRemove = new System.Windows.Forms.ToolStripMenuItem();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tabControlLibFilter = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            listBoxSpeciesLib = new System.Windows.Forms.ListBox();
            tabPage3 = new System.Windows.Forms.TabPage();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            BtRecalculateTopStatsAfterChange = new System.Windows.Forms.Button();
            label17 = new System.Windows.Forms.Label();
            buttonRecalculateTops = new System.Windows.Forms.Button();
            tabPageLibRadarChart = new System.Windows.Forms.TabPage();
            radarChartLibrary = new RadarChart();
            creatureBoxListView = new CreatureBox();
            tabPageLibraryInfo = new System.Windows.Forms.TabPage();
            tlpLibraryInfo = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            CbLibraryInfoUseFilter = new System.Windows.Forms.CheckBox();
            BtCopyLibraryColorToClipboard = new System.Windows.Forms.Button();
            libraryInfoControl1 = new ARKBreedingStats.uiControls.LibraryInfoControl();
            tabPagePedigree = new System.Windows.Forms.TabPage();
            pedigree1 = new PedigreeControl();
            tabPageTaming = new System.Windows.Forms.TabPage();
            tamingControl1 = new TamingControl();
            tabPageBreedingPlan = new System.Windows.Forms.TabPage();
            breedingPlan1 = new BreedingPlan();
            tabPageCurrentBreeds = new System.Windows.Forms.TabPage();
            currentBreeds1 = new ARKBreedingStats.uiControls.CurrentBreeds();
            hatching1 = new ARKBreedingStats.uiControls.Hatching();
            tabPageRaising = new System.Windows.Forms.TabPage();
            raisingControl1 = new RaisingControl();
            tabPageTimer = new System.Windows.Forms.TabPage();
            timerList1 = new TimerControl();
            tabPagePlayerTribes = new System.Windows.Forms.TabPage();
            tribesControl1 = new TribesControl();
            tabPageNotes = new System.Windows.Forms.TabPage();
            notesControl1 = new NotesControl();
            TabPageOCR = new System.Windows.Forms.TabPage();
            ocrControl1 = new ARKBreedingStats.ocr.OCRControl();
            tabPageExtractionTests = new System.Windows.Forms.TabPage();
            extractionTestControl1 = new ARKBreedingStats.testCases.ExtractionTestControl();
            tabPageMultiplierTesting = new System.Windows.Forms.TabPage();
            statsMultiplierTesting1 = new StatsMultiplierTesting();
            btReadValuesFromArk = new System.Windows.Forms.Button();
            cbEventMultipliers = new System.Windows.Forms.CheckBox();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ToolStripStatusLabelImport = new System.Windows.Forms.ToolStripStatusLabel();
            toolStrip2 = new System.Windows.Forms.ToolStrip();
            newToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            openToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            saveToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            TsbQuickSaveGameImport = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripButtonSettings = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            toolStripButtonCopy2Tester = new System.Windows.Forms.ToolStripButton();
            toolStripButtonCopy2Extractor = new System.Windows.Forms.ToolStripButton();
            toolStripButtonClear = new System.Windows.Forms.ToolStripButton();
            toolStripButtonAddPlayer = new System.Windows.Forms.ToolStripButton();
            toolStripButtonAddTribe = new System.Windows.Forms.ToolStripButton();
            toolStripButtonDeleteExpiredIncubationTimers = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            toolStripButtonSaveCreatureValuesTemp = new System.Windows.Forms.ToolStripButton();
            toolStripCBTempCreatures = new System.Windows.Forms.ToolStripComboBox();
            toolStripButtonDeleteTempCreature = new System.Windows.Forms.ToolStripButton();
            tsBtAddAsExtractionTest = new System.Windows.Forms.ToolStripButton();
            copyToMultiplierTesterToolStripButton = new System.Windows.Forms.ToolStripButton();
            ToolStripLabelFilter = new System.Windows.Forms.ToolStripLabel();
            ToolStripTextBoxLibraryFilter = new System.Windows.Forms.ToolStripTextBox();
            ToolStripButtonLibraryFilterClear = new System.Windows.Forms.ToolStripButton();
            ToolStripButtonSaveFilterPreset = new System.Windows.Forms.ToolStripButton();
            TsSpOcrLabel = new System.Windows.Forms.ToolStripSeparator();
            TsLbLabelSet = new System.Windows.Forms.ToolStripLabel();
            TsCbbLabelSets = new System.Windows.Forms.ToolStripComboBox();
            panelToolBar = new System.Windows.Forms.Panel();
            btImportLastExported = new System.Windows.Forms.Button();
            pbSpecies = new System.Windows.Forms.PictureBox();
            tbSpeciesGlobal = new ARKBreedingStats.uiControls.TextBoxSuggest();
            cbGuessSpecies = new System.Windows.Forms.CheckBox();
            cbToggleOverlay = new System.Windows.Forms.CheckBox();
            lbListening = new System.Windows.Forms.Label();
            lbSpecies = new System.Windows.Forms.Label();
            TbMessageLabel = new System.Windows.Forms.TextBox();
            contextMenuStripLibraryHeader = new System.Windows.Forms.ContextMenuStrip(components);
            toolStripMenuItemResetLibraryColumnWidths = new System.Windows.Forms.ToolStripMenuItem();
            resetColumnWidthNoMutationLevelColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            restoreMutationLevelsASAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            collapseMutationsLevelsASEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            resetColumnOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            speciesSelector1 = new SpeciesSelector();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownImprintingBonusTester).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDownTestingTE).BeginInit();
            groupBoxPossibilities.SuspendLayout();
            groupBoxDetailsExtractor.SuspendLayout();
            panelExtrImpr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownImprintingBonusExtractor).BeginInit();
            panelExtrTE.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownUpperTEffBound).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownLowerTEffBound).BeginInit();
            menuStrip1.SuspendLayout();
            panelSums.SuspendLayout();
            panelWildTamedBred.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabPageStatTesting.SuspendLayout();
            gbStatChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radarChart1).BeginInit();
            panelWildTamedBredTester.SuspendLayout();
            groupBox2.SuspendLayout();
            flowLayoutPanelStatIOsTester.SuspendLayout();
            panel2.SuspendLayout();
            panelStatTesterFootnote.SuspendLayout();
            gpPreviewEdit.SuspendLayout();
            tabPageExtractor.SuspendLayout();
            pBondedTamingExtractor.SuspendLayout();
            groupBoxRadarChartExtractor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radarChartExtractor).BeginInit();
            groupBoxTamingInfo.SuspendLayout();
            gbStatsExtractor.SuspendLayout();
            flowLayoutPanelStatIOsExtractor.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownLevel).BeginInit();
            tabPageLibrary.SuspendLayout();
            tableLayoutPanelLibrary.SuspendLayout();
            contextMenuStripLibrary.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tabControlLibFilter.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage3.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tabPageLibRadarChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radarChartLibrary).BeginInit();
            tabPageLibraryInfo.SuspendLayout();
            tlpLibraryInfo.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tabPagePedigree.SuspendLayout();
            tabPageTaming.SuspendLayout();
            tabPageBreedingPlan.SuspendLayout();
            tabPageCurrentBreeds.SuspendLayout();
            tabPageRaising.SuspendLayout();
            tabPageTimer.SuspendLayout();
            tabPagePlayerTribes.SuspendLayout();
            tabPageNotes.SuspendLayout();
            TabPageOCR.SuspendLayout();
            tabPageExtractionTests.SuspendLayout();
            tabPageMultiplierTesting.SuspendLayout();
            statusStrip1.SuspendLayout();
            toolStrip2.SuspendLayout();
            panelToolBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbSpecies).BeginInit();
            contextMenuStripLibraryHeader.SuspendLayout();
            SuspendLayout();
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            aboutToolStripMenuItem.Text = "about…";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { newToolStripMenuItem, loadToolStripMenuItem, loadAndAddToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator2, recentlyUsedToolStripMenuItem, toolStripSeparator21, openFolderOfCurrentFileToolStripMenuItem, toolStripSeparator15, importingFromSavegameToolStripMenuItem, importExportedCreaturesToolStripMenuItem, importFromTabSeparatedFileToolStripMenuItem, toolStripSeparator19, copyLibrarydumpToClipboardToolStripMenuItem, toolStripSeparator10, quitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            newToolStripMenuItem.Text = "&New Library";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.ShortcutKeyDisplayString = "";
            loadToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
            loadToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            loadToolStripMenuItem.Text = "&Load...";
            loadToolStripMenuItem.Click += loadToolStripMenuItem_Click;
            // 
            // loadAndAddToolStripMenuItem
            // 
            loadAndAddToolStripMenuItem.Name = "loadAndAddToolStripMenuItem";
            loadAndAddToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            loadAndAddToolStripMenuItem.Text = "Load and A&dd...";
            loadAndAddToolStripMenuItem.ToolTipText = "Select a library-file and add all its creatures to the currently loaded library";
            loadAndAddToolStripMenuItem.Click += loadAndAddToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
            saveToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            saveToolStripMenuItem.Text = "&Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.S;
            saveAsToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            saveAsToolStripMenuItem.Text = "Save &as...";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(274, 6);
            // 
            // recentlyUsedToolStripMenuItem
            // 
            recentlyUsedToolStripMenuItem.Name = "recentlyUsedToolStripMenuItem";
            recentlyUsedToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            recentlyUsedToolStripMenuItem.Text = "Recently used";
            // 
            // toolStripSeparator21
            // 
            toolStripSeparator21.Name = "toolStripSeparator21";
            toolStripSeparator21.Size = new System.Drawing.Size(274, 6);
            // 
            // openFolderOfCurrentFileToolStripMenuItem
            // 
            openFolderOfCurrentFileToolStripMenuItem.Enabled = false;
            openFolderOfCurrentFileToolStripMenuItem.Name = "openFolderOfCurrentFileToolStripMenuItem";
            openFolderOfCurrentFileToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            openFolderOfCurrentFileToolStripMenuItem.Text = "Open folder of current file…";
            openFolderOfCurrentFileToolStripMenuItem.Click += openFolderOfCurrentFileToolStripMenuItem_Click;
            // 
            // toolStripSeparator15
            // 
            toolStripSeparator15.Name = "toolStripSeparator15";
            toolStripSeparator15.Size = new System.Drawing.Size(274, 6);
            // 
            // importingFromSavegameToolStripMenuItem
            // 
            importingFromSavegameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { selectSavegameFileToolStripMenuItem, configureSavegameImportToolStripMenuItem });
            importingFromSavegameToolStripMenuItem.Name = "importingFromSavegameToolStripMenuItem";
            importingFromSavegameToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            importingFromSavegameToolStripMenuItem.Text = "Importing from savegame (only ASE)";
            // 
            // selectSavegameFileToolStripMenuItem
            // 
            selectSavegameFileToolStripMenuItem.Name = "selectSavegameFileToolStripMenuItem";
            selectSavegameFileToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            selectSavegameFileToolStripMenuItem.Text = "Select savegame file…";
            selectSavegameFileToolStripMenuItem.Click += SavegameImportClick;
            // 
            // configureSavegameImportToolStripMenuItem
            // 
            configureSavegameImportToolStripMenuItem.Name = "configureSavegameImportToolStripMenuItem";
            configureSavegameImportToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            configureSavegameImportToolStripMenuItem.Text = "Configure savegame imports…";
            configureSavegameImportToolStripMenuItem.Click += importingFromSavegameEmptyToolStripMenuItem_Click;
            // 
            // importExportedCreaturesToolStripMenuItem
            // 
            importExportedCreaturesToolStripMenuItem.Name = "importExportedCreaturesToolStripMenuItem";
            importExportedCreaturesToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            importExportedCreaturesToolStripMenuItem.Text = "Import exported Creatures";
            // 
            // importFromTabSeparatedFileToolStripMenuItem
            // 
            importFromTabSeparatedFileToolStripMenuItem.Name = "importFromTabSeparatedFileToolStripMenuItem";
            importFromTabSeparatedFileToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            importFromTabSeparatedFileToolStripMenuItem.Text = "Import from tab separated values file…";
            importFromTabSeparatedFileToolStripMenuItem.Click += importFromTabSeparatedFileToolStripMenuItem_Click;
            // 
            // toolStripSeparator19
            // 
            toolStripSeparator19.Name = "toolStripSeparator19";
            toolStripSeparator19.Size = new System.Drawing.Size(274, 6);
            // 
            // copyLibrarydumpToClipboardToolStripMenuItem
            // 
            copyLibrarydumpToClipboardToolStripMenuItem.Name = "copyLibrarydumpToClipboardToolStripMenuItem";
            copyLibrarydumpToClipboardToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            copyLibrarydumpToClipboardToolStripMenuItem.Text = "Copy library-dump to clipboard";
            copyLibrarydumpToClipboardToolStripMenuItem.Click += copyLibrarydumpToClipboardToolStripMenuItem_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new System.Drawing.Size(274, 6);
            // 
            // quitToolStripMenuItem
            // 
            quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            quitToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            quitToolStripMenuItem.Text = "&Quit";
            quitToolStripMenuItem.Click += quitToolStripMenuItem_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lbImprintedCount);
            groupBox1.Controls.Add(BtSetImprinting100Tester);
            groupBox1.Controls.Add(labelImprintingTester);
            groupBox1.Controls.Add(numericUpDownImprintingBonusTester);
            groupBox1.Controls.Add(NumericUpDownTestingTE);
            groupBox1.Controls.Add(labelTesterTE);
            groupBox1.Location = new System.Drawing.Point(435, 7);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(306, 83);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Details";
            // 
            // lbImprintedCount
            // 
            lbImprintedCount.AutoSize = true;
            lbImprintedCount.Location = new System.Drawing.Point(214, 54);
            lbImprintedCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbImprintedCount.Name = "lbImprintedCount";
            lbImprintedCount.Size = new System.Drawing.Size(29, 15);
            lbImprintedCount.TabIndex = 5;
            lbImprintedCount.Text = "(0×)";
            lbImprintedCount.MouseClick += labelImprintedCount_MouseClick;
            // 
            // BtSetImprinting100Tester
            // 
            BtSetImprinting100Tester.Location = new System.Drawing.Point(259, 50);
            BtSetImprinting100Tester.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtSetImprinting100Tester.Name = "BtSetImprinting100Tester";
            BtSetImprinting100Tester.Size = new System.Drawing.Size(40, 23);
            BtSetImprinting100Tester.TabIndex = 15;
            BtSetImprinting100Tester.Text = "100";
            BtSetImprinting100Tester.UseVisualStyleBackColor = true;
            BtSetImprinting100Tester.Click += BtSetImprinting100Tester_Click;
            // 
            // labelImprintingTester
            // 
            labelImprintingTester.AutoSize = true;
            labelImprintingTester.Location = new System.Drawing.Point(94, 54);
            labelImprintingTester.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelImprintingTester.Name = "labelImprintingTester";
            labelImprintingTester.Size = new System.Drawing.Size(112, 15);
            labelImprintingTester.TabIndex = 5;
            labelImprintingTester.Text = "% Imprinting Bonus";
            // 
            // numericUpDownImprintingBonusTester
            // 
            numericUpDownImprintingBonusTester.DecimalPlaces = 5;
            numericUpDownImprintingBonusTester.ForeColor = System.Drawing.SystemColors.GrayText;
            numericUpDownImprintingBonusTester.Location = new System.Drawing.Point(7, 52);
            numericUpDownImprintingBonusTester.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            numericUpDownImprintingBonusTester.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDownImprintingBonusTester.Name = "numericUpDownImprintingBonusTester";
            numericUpDownImprintingBonusTester.Size = new System.Drawing.Size(80, 23);
            numericUpDownImprintingBonusTester.TabIndex = 4;
            numericUpDownImprintingBonusTester.ValueChanged += numericUpDownImprintingBonusTester_ValueChanged;
            // 
            // NumericUpDownTestingTE
            // 
            NumericUpDownTestingTE.DecimalPlaces = 2;
            NumericUpDownTestingTE.ForeColor = System.Drawing.SystemColors.WindowText;
            NumericUpDownTestingTE.Location = new System.Drawing.Point(7, 22);
            NumericUpDownTestingTE.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NumericUpDownTestingTE.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            NumericUpDownTestingTE.Name = "NumericUpDownTestingTE";
            NumericUpDownTestingTE.Size = new System.Drawing.Size(70, 23);
            NumericUpDownTestingTE.TabIndex = 0;
            NumericUpDownTestingTE.Value = new decimal(new int[] { 80, 0, 0, 0 });
            NumericUpDownTestingTE.ValueChanged += NumericUpDownTestingTE_ValueChanged;
            // 
            // labelTesterTE
            // 
            labelTesterTE.AutoSize = true;
            labelTesterTE.Location = new System.Drawing.Point(84, 24);
            labelTesterTE.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelTesterTE.Name = "labelTesterTE";
            labelTesterTE.Size = new System.Drawing.Size(131, 15);
            labelTesterTE.TabIndex = 1;
            labelTesterTE.Text = "% Taming Effectiveness";
            // 
            // groupBoxPossibilities
            // 
            groupBoxPossibilities.Controls.Add(listViewPossibilities);
            groupBoxPossibilities.Location = new System.Drawing.Point(749, 50);
            groupBoxPossibilities.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxPossibilities.Name = "groupBoxPossibilities";
            groupBoxPossibilities.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxPossibilities.Size = new System.Drawing.Size(234, 340);
            groupBoxPossibilities.TabIndex = 11;
            groupBoxPossibilities.TabStop = false;
            groupBoxPossibilities.Text = "Possible Levels";
            // 
            // listViewPossibilities
            // 
            listViewPossibilities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderWild, columnHeaderMutated, columnHeaderDom, columnHeaderTE, columnHeaderLW });
            listViewPossibilities.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewPossibilities.FullRowSelect = true;
            listViewPossibilities.Location = new System.Drawing.Point(4, 19);
            listViewPossibilities.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listViewPossibilities.MultiSelect = false;
            listViewPossibilities.Name = "listViewPossibilities";
            listViewPossibilities.ShowGroups = false;
            listViewPossibilities.Size = new System.Drawing.Size(226, 318);
            listViewPossibilities.TabIndex = 0;
            listViewPossibilities.UseCompatibleStateImageBehavior = false;
            listViewPossibilities.View = System.Windows.Forms.View.Details;
            listViewPossibilities.ColumnClick += listView_ColumnClick;
            listViewPossibilities.SelectedIndexChanged += listViewPossibilities_SelectedIndexChanged;
            // 
            // columnHeaderWild
            // 
            columnHeaderWild.Text = "Wild";
            columnHeaderWild.Width = 34;
            // 
            // columnHeaderMutated
            // 
            columnHeaderMutated.Text = "Mut";
            columnHeaderMutated.Width = 34;
            // 
            // columnHeaderDom
            // 
            columnHeaderDom.Text = "Dom";
            columnHeaderDom.Width = 34;
            // 
            // columnHeaderTE
            // 
            columnHeaderTE.Text = "TEff [%]";
            columnHeaderTE.Width = 49;
            // 
            // columnHeaderLW
            // 
            columnHeaderLW.Text = "WLvl";
            columnHeaderLW.Width = 37;
            // 
            // groupBoxDetailsExtractor
            // 
            groupBoxDetailsExtractor.Controls.Add(panelExtrImpr);
            groupBoxDetailsExtractor.Controls.Add(panelExtrTE);
            groupBoxDetailsExtractor.Location = new System.Drawing.Point(435, 7);
            groupBoxDetailsExtractor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxDetailsExtractor.Name = "groupBoxDetailsExtractor";
            groupBoxDetailsExtractor.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxDetailsExtractor.Size = new System.Drawing.Size(267, 87);
            groupBoxDetailsExtractor.TabIndex = 4;
            groupBoxDetailsExtractor.TabStop = false;
            groupBoxDetailsExtractor.Text = "Taming-Effectiveness";
            // 
            // panelExtrImpr
            // 
            panelExtrImpr.Controls.Add(BtSetImprinting0Extractor);
            panelExtrImpr.Controls.Add(BtSetImprinting100Extractor);
            panelExtrImpr.Controls.Add(cbExactlyImprinting);
            panelExtrImpr.Controls.Add(labelImprintingBonus);
            panelExtrImpr.Controls.Add(lbImprintingCuddleCountExtractor);
            panelExtrImpr.Controls.Add(numericUpDownImprintingBonusExtractor);
            panelExtrImpr.Location = new System.Drawing.Point(7, 18);
            panelExtrImpr.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelExtrImpr.Name = "panelExtrImpr";
            panelExtrImpr.Size = new System.Drawing.Size(257, 61);
            panelExtrImpr.TabIndex = 52;
            panelExtrImpr.Visible = false;
            // 
            // BtSetImprinting0Extractor
            // 
            BtSetImprinting0Extractor.Location = new System.Drawing.Point(4, 30);
            BtSetImprinting0Extractor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtSetImprinting0Extractor.Name = "BtSetImprinting0Extractor";
            BtSetImprinting0Extractor.Size = new System.Drawing.Size(41, 27);
            BtSetImprinting0Extractor.TabIndex = 1;
            BtSetImprinting0Extractor.Text = "0 %";
            BtSetImprinting0Extractor.UseVisualStyleBackColor = true;
            BtSetImprinting0Extractor.Click += BtSetImprinting0_Click;
            // 
            // BtSetImprinting100Extractor
            // 
            BtSetImprinting100Extractor.Location = new System.Drawing.Point(47, 30);
            BtSetImprinting100Extractor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtSetImprinting100Extractor.Name = "BtSetImprinting100Extractor";
            BtSetImprinting100Extractor.Size = new System.Drawing.Size(55, 27);
            BtSetImprinting100Extractor.TabIndex = 2;
            BtSetImprinting100Extractor.Text = "100 %";
            BtSetImprinting100Extractor.UseVisualStyleBackColor = true;
            BtSetImprinting100Extractor.Click += BtSetImprinting100_Click;
            // 
            // cbExactlyImprinting
            // 
            cbExactlyImprinting.AutoSize = true;
            cbExactlyImprinting.Location = new System.Drawing.Point(108, 33);
            cbExactlyImprinting.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbExactlyImprinting.Name = "cbExactlyImprinting";
            cbExactlyImprinting.Size = new System.Drawing.Size(131, 19);
            cbExactlyImprinting.TabIndex = 3;
            cbExactlyImprinting.Text = "Exactly, don't adjust";
            cbExactlyImprinting.UseVisualStyleBackColor = true;
            // 
            // labelImprintingBonus
            // 
            labelImprintingBonus.AutoSize = true;
            labelImprintingBonus.Location = new System.Drawing.Point(100, 6);
            labelImprintingBonus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelImprintingBonus.Name = "labelImprintingBonus";
            labelImprintingBonus.Size = new System.Drawing.Size(112, 15);
            labelImprintingBonus.TabIndex = 7;
            labelImprintingBonus.Text = "% Imprinting Bonus";
            // 
            // lbImprintingCuddleCountExtractor
            // 
            lbImprintingCuddleCountExtractor.AutoSize = true;
            lbImprintingCuddleCountExtractor.Location = new System.Drawing.Point(219, 6);
            lbImprintingCuddleCountExtractor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbImprintingCuddleCountExtractor.Name = "lbImprintingCuddleCountExtractor";
            lbImprintingCuddleCountExtractor.Size = new System.Drawing.Size(29, 15);
            lbImprintingCuddleCountExtractor.TabIndex = 50;
            lbImprintingCuddleCountExtractor.Text = "(0×)";
            // 
            // numericUpDownImprintingBonusExtractor
            // 
            numericUpDownImprintingBonusExtractor.DecimalPlaces = 5;
            numericUpDownImprintingBonusExtractor.ForeColor = System.Drawing.SystemColors.GrayText;
            numericUpDownImprintingBonusExtractor.Location = new System.Drawing.Point(4, 3);
            numericUpDownImprintingBonusExtractor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            numericUpDownImprintingBonusExtractor.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDownImprintingBonusExtractor.Name = "numericUpDownImprintingBonusExtractor";
            numericUpDownImprintingBonusExtractor.Size = new System.Drawing.Size(90, 23);
            numericUpDownImprintingBonusExtractor.TabIndex = 0;
            numericUpDownImprintingBonusExtractor.ValueChanged += numericUpDownImprintingBonusExtractor_ValueChanged;
            numericUpDownImprintingBonusExtractor.Enter += numericUpDown_Enter;
            // 
            // panelExtrTE
            // 
            panelExtrTE.Controls.Add(labelTE);
            panelExtrTE.Controls.Add(label2);
            panelExtrTE.Controls.Add(label1);
            panelExtrTE.Controls.Add(numericUpDownUpperTEffBound);
            panelExtrTE.Controls.Add(label3);
            panelExtrTE.Controls.Add(numericUpDownLowerTEffBound);
            panelExtrTE.Location = new System.Drawing.Point(7, 18);
            panelExtrTE.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelExtrTE.Name = "panelExtrTE";
            panelExtrTE.Size = new System.Drawing.Size(257, 61);
            panelExtrTE.TabIndex = 52;
            // 
            // labelTE
            // 
            labelTE.Location = new System.Drawing.Point(4, 36);
            labelTE.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelTE.Name = "labelTE";
            labelTE.Size = new System.Drawing.Size(245, 22);
            labelTE.TabIndex = 4;
            labelTE.Text = "TE differs in chosen possibilities";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(4, 6);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(76, 15);
            label2.TabIndex = 0;
            label2.Text = "Range to test";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(231, 7);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(17, 15);
            label1.TabIndex = 5;
            label1.Text = "%";
            // 
            // numericUpDownUpperTEffBound
            // 
            numericUpDownUpperTEffBound.ForeColor = System.Drawing.SystemColors.WindowText;
            numericUpDownUpperTEffBound.Location = new System.Drawing.Point(172, 3);
            numericUpDownUpperTEffBound.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            numericUpDownUpperTEffBound.Name = "numericUpDownUpperTEffBound";
            numericUpDownUpperTEffBound.Size = new System.Drawing.Size(52, 23);
            numericUpDownUpperTEffBound.TabIndex = 3;
            numericUpDownUpperTEffBound.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numericUpDownUpperTEffBound.Enter += numericUpDown_Enter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(153, 6);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(12, 15);
            label3.TabIndex = 2;
            label3.Text = "-";
            // 
            // numericUpDownLowerTEffBound
            // 
            numericUpDownLowerTEffBound.ForeColor = System.Drawing.SystemColors.WindowText;
            numericUpDownLowerTEffBound.Location = new System.Drawing.Point(93, 3);
            numericUpDownLowerTEffBound.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            numericUpDownLowerTEffBound.Name = "numericUpDownLowerTEffBound";
            numericUpDownLowerTEffBound.Size = new System.Drawing.Size(52, 23);
            numericUpDownLowerTEffBound.TabIndex = 1;
            numericUpDownLowerTEffBound.Value = new decimal(new int[] { 80, 0, 0, 0 });
            numericUpDownLowerTEffBound.Enter += numericUpDown_Enter;
            // 
            // lbLevel
            // 
            lbLevel.AutoSize = true;
            lbLevel.Location = new System.Drawing.Point(239, 13);
            lbLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbLevel.Name = "lbLevel";
            lbLevel.Size = new System.Drawing.Size(34, 15);
            lbLevel.TabIndex = 1;
            lbLevel.Text = "Level";
            // 
            // lbBreedingValueTester
            // 
            lbBreedingValueTester.AutoSize = true;
            lbBreedingValueTester.Location = new System.Drawing.Point(315, 0);
            lbBreedingValueTester.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbBreedingValueTester.Name = "lbBreedingValueTester";
            lbBreedingValueTester.Size = new System.Drawing.Size(85, 15);
            lbBreedingValueTester.TabIndex = 33;
            lbBreedingValueTester.Text = "Breeding Value";
            // 
            // lbTesterWildLevel
            // 
            lbTesterWildLevel.AutoSize = true;
            lbTesterWildLevel.Location = new System.Drawing.Point(9, 0);
            lbTesterWildLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbTesterWildLevel.Name = "lbTesterWildLevel";
            lbTesterWildLevel.Size = new System.Drawing.Size(50, 15);
            lbTesterWildLevel.TabIndex = 31;
            lbTesterWildLevel.Text = "Wild-Lvl";
            // 
            // lbTesterDomLevel
            // 
            lbTesterDomLevel.AutoSize = true;
            lbTesterDomLevel.Location = new System.Drawing.Point(131, 0);
            lbTesterDomLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbTesterDomLevel.Name = "lbTesterDomLevel";
            lbTesterDomLevel.Size = new System.Drawing.Size(52, 15);
            lbTesterDomLevel.TabIndex = 32;
            lbTesterDomLevel.Text = "Dom-Lvl";
            // 
            // lbInfoYellowStats
            // 
            lbInfoYellowStats.Location = new System.Drawing.Point(749, 393);
            lbInfoYellowStats.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbInfoYellowStats.Name = "lbInfoYellowStats";
            lbInfoYellowStats.Size = new System.Drawing.Size(298, 145);
            lbInfoYellowStats.TabIndex = 15;
            lbInfoYellowStats.Text = resources.GetString("lbInfoYellowStats.Text");
            // 
            // labelFootnote
            // 
            labelFootnote.Location = new System.Drawing.Point(4, 69);
            labelFootnote.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelFootnote.Name = "labelFootnote";
            labelFootnote.Size = new System.Drawing.Size(344, 18);
            labelFootnote.TabIndex = 18;
            labelFootnote.Text = "*Creature is not yet tamed and may get better values then.";
            // 
            // labelHBV
            // 
            labelHBV.AutoSize = true;
            labelHBV.Location = new System.Drawing.Point(315, 0);
            labelHBV.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelHBV.Name = "labelHBV";
            labelHBV.Size = new System.Drawing.Size(85, 15);
            labelHBV.TabIndex = 27;
            labelHBV.Text = "Breeding Value";
            // 
            // lbExtractorDomLevel
            // 
            lbExtractorDomLevel.AutoSize = true;
            lbExtractorDomLevel.Location = new System.Drawing.Point(261, 0);
            lbExtractorDomLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbExtractorDomLevel.Name = "lbExtractorDomLevel";
            lbExtractorDomLevel.Size = new System.Drawing.Size(52, 15);
            lbExtractorDomLevel.TabIndex = 26;
            lbExtractorDomLevel.Text = "Dom-Lvl";
            // 
            // lbExtractorWildLevel
            // 
            lbExtractorWildLevel.AutoSize = true;
            lbExtractorWildLevel.Location = new System.Drawing.Point(144, 0);
            lbExtractorWildLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbExtractorWildLevel.Name = "lbExtractorWildLevel";
            lbExtractorWildLevel.Size = new System.Drawing.Size(50, 15);
            lbExtractorWildLevel.TabIndex = 25;
            lbExtractorWildLevel.Text = "Wild-Lvl";
            // 
            // lbSum
            // 
            lbSum.AutoSize = true;
            lbSum.Location = new System.Drawing.Point(89, 2);
            lbSum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbSum.Name = "lbSum";
            lbSum.Size = new System.Drawing.Size(31, 15);
            lbSum.TabIndex = 29;
            lbSum.Text = "Sum";
            // 
            // lbSumDom
            // 
            lbSumDom.AutoSize = true;
            lbSumDom.Location = new System.Drawing.Point(281, 2);
            lbSumDom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbSumDom.Name = "lbSumDom";
            lbSumDom.Size = new System.Drawing.Size(25, 15);
            lbSumDom.TabIndex = 31;
            lbSumDom.Text = "100";
            lbSumDom.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbSumWild
            // 
            lbSumWild.AutoSize = true;
            lbSumWild.Location = new System.Drawing.Point(168, 2);
            lbSumWild.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbSumWild.Name = "lbSumWild";
            lbSumWild.Size = new System.Drawing.Size(25, 15);
            lbSumWild.TabIndex = 30;
            lbSumWild.Text = "100";
            lbSumWild.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, libraryFilterToolStripMenuItem, toolStripMenuItemMutationColumns, nameGeneratorToolStripMenuItem, settingsToolStripMenuItem, serverToolStripMenuItem, arkUtilsToolStripMenuItem, helpToolStripMenuItem, devToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(2191, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { exportValuesToClipboardToolStripMenuItem, toolStripSeparator13, setStatusToolStripMenuItem, multiSetterToolStripMenuItem, toolStripSeparator5, deleteSelectedToolStripMenuItem, findDuplicatesToolStripMenuItem, toolStripSeparator7, spawnWildToolStripMenuItem, exactSpawnCommandToolStripMenuItem, exactSpawnCommandDS2ToolStripMenuItem, commandMutationLevelsToolStripMenuItem, copyCoToolStripMenuItem, toolStripSeparator25, copyCreatureToolStripMenuItem, pasteCreatureToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // exportValuesToClipboardToolStripMenuItem
            // 
            exportValuesToClipboardToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { plainTextcurrentValuesToolStripMenuItem, plainTextbreedingValuesToolStripMenuItem, toolStripSeparator24, forSpreadsheetToolStripMenuItem, editSpreadsheetExportFieldsToolStripMenuItem });
            exportValuesToClipboardToolStripMenuItem.Name = "exportValuesToClipboardToolStripMenuItem";
            exportValuesToClipboardToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            exportValuesToClipboardToolStripMenuItem.Text = "Export to Clipboard";
            // 
            // plainTextcurrentValuesToolStripMenuItem
            // 
            plainTextcurrentValuesToolStripMenuItem.Name = "plainTextcurrentValuesToolStripMenuItem";
            plainTextcurrentValuesToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            plainTextcurrentValuesToolStripMenuItem.Text = "Plain Text (current values)";
            plainTextcurrentValuesToolStripMenuItem.Click += plainTextcurrentValuesToolStripMenuItem_Click;
            // 
            // plainTextbreedingValuesToolStripMenuItem
            // 
            plainTextbreedingValuesToolStripMenuItem.Name = "plainTextbreedingValuesToolStripMenuItem";
            plainTextbreedingValuesToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            plainTextbreedingValuesToolStripMenuItem.Text = "Plain Text (breeding values)";
            plainTextbreedingValuesToolStripMenuItem.Click += plainTextbreedingValuesToolStripMenuItem_Click;
            // 
            // toolStripSeparator24
            // 
            toolStripSeparator24.Name = "toolStripSeparator24";
            toolStripSeparator24.Size = new System.Drawing.Size(234, 6);
            // 
            // forSpreadsheetToolStripMenuItem
            // 
            forSpreadsheetToolStripMenuItem.Name = "forSpreadsheetToolStripMenuItem";
            forSpreadsheetToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            forSpreadsheetToolStripMenuItem.Text = "for Spreadsheet";
            forSpreadsheetToolStripMenuItem.Click += forSpreadsheetToolStripMenuItem_Click;
            // 
            // editSpreadsheetExportFieldsToolStripMenuItem
            // 
            editSpreadsheetExportFieldsToolStripMenuItem.Name = "editSpreadsheetExportFieldsToolStripMenuItem";
            editSpreadsheetExportFieldsToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            editSpreadsheetExportFieldsToolStripMenuItem.Text = "Edit Spreadsheet export fields…";
            editSpreadsheetExportFieldsToolStripMenuItem.Click += editSpreadsheetExportFieldsToolStripMenuItem_Click;
            // 
            // toolStripSeparator13
            // 
            toolStripSeparator13.Name = "toolStripSeparator13";
            toolStripSeparator13.Size = new System.Drawing.Size(233, 6);
            // 
            // setStatusToolStripMenuItem
            // 
            setStatusToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { aliveToolStripMenuItem, deadToolStripMenuItem, unavailableToolStripMenuItem, obeliskToolStripMenuItem1 });
            setStatusToolStripMenuItem.Name = "setStatusToolStripMenuItem";
            setStatusToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            setStatusToolStripMenuItem.Text = "Set Status";
            // 
            // aliveToolStripMenuItem
            // 
            aliveToolStripMenuItem.Name = "aliveToolStripMenuItem";
            aliveToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            aliveToolStripMenuItem.Text = "Available";
            aliveToolStripMenuItem.Click += aliveToolStripMenuItem_Click;
            // 
            // deadToolStripMenuItem
            // 
            deadToolStripMenuItem.Name = "deadToolStripMenuItem";
            deadToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            deadToolStripMenuItem.Text = "Dead";
            deadToolStripMenuItem.Click += deadToolStripMenuItem_Click;
            // 
            // unavailableToolStripMenuItem
            // 
            unavailableToolStripMenuItem.Name = "unavailableToolStripMenuItem";
            unavailableToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            unavailableToolStripMenuItem.Text = "Unavailable";
            unavailableToolStripMenuItem.Click += unavailableToolStripMenuItem_Click;
            // 
            // obeliskToolStripMenuItem1
            // 
            obeliskToolStripMenuItem1.Name = "obeliskToolStripMenuItem1";
            obeliskToolStripMenuItem1.Size = new System.Drawing.Size(135, 22);
            obeliskToolStripMenuItem1.Text = "Obelisk";
            obeliskToolStripMenuItem1.Click += obeliskToolStripMenuItem1_Click;
            // 
            // multiSetterToolStripMenuItem
            // 
            multiSetterToolStripMenuItem.Name = "multiSetterToolStripMenuItem";
            multiSetterToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            multiSetterToolStripMenuItem.Text = "MultiSetter…";
            multiSetterToolStripMenuItem.Click += multiSetterToolStripMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(233, 6);
            // 
            // deleteSelectedToolStripMenuItem
            // 
            deleteSelectedToolStripMenuItem.Name = "deleteSelectedToolStripMenuItem";
            deleteSelectedToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            deleteSelectedToolStripMenuItem.Text = "Remove…";
            deleteSelectedToolStripMenuItem.Click += deleteSelectedToolStripMenuItem_Click;
            // 
            // findDuplicatesToolStripMenuItem
            // 
            findDuplicatesToolStripMenuItem.Name = "findDuplicatesToolStripMenuItem";
            findDuplicatesToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            findDuplicatesToolStripMenuItem.Text = "Find Duplicates…";
            findDuplicatesToolStripMenuItem.Visible = false;
            findDuplicatesToolStripMenuItem.Click += findDuplicatesToolStripMenuItem_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new System.Drawing.Size(233, 6);
            // 
            // spawnWildToolStripMenuItem
            // 
            spawnWildToolStripMenuItem.Name = "spawnWildToolStripMenuItem";
            spawnWildToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            spawnWildToolStripMenuItem.Text = "Spawn wild console command";
            spawnWildToolStripMenuItem.Click += spawnWildToolStripMenuItem_Click;
            // 
            // exactSpawnCommandToolStripMenuItem
            // 
            exactSpawnCommandToolStripMenuItem.Name = "exactSpawnCommandToolStripMenuItem";
            exactSpawnCommandToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            exactSpawnCommandToolStripMenuItem.Text = "ExactSpawnCommand";
            exactSpawnCommandToolStripMenuItem.ToolTipText = "Creates a console command to spawn this creature in game. This command can crash your game";
            exactSpawnCommandToolStripMenuItem.Click += exactSpawnCommandToolStripMenuItem_Click;
            // 
            // exactSpawnCommandDS2ToolStripMenuItem
            // 
            exactSpawnCommandDS2ToolStripMenuItem.Name = "exactSpawnCommandDS2ToolStripMenuItem";
            exactSpawnCommandDS2ToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            exactSpawnCommandDS2ToolStripMenuItem.Text = "ExactSpawnCommandDS2";
            exactSpawnCommandDS2ToolStripMenuItem.ToolTipText = "Creates a console command to spawn this creature in game, used with the mod DinoStorageV2. This command is stable.";
            exactSpawnCommandDS2ToolStripMenuItem.Click += exactSpawnCommandDS2ToolStripMenuItem_Click;
            // 
            // commandMutationLevelsToolStripMenuItem
            // 
            commandMutationLevelsToolStripMenuItem.Name = "commandMutationLevelsToolStripMenuItem";
            commandMutationLevelsToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            commandMutationLevelsToolStripMenuItem.Text = "Mutation levels command";
            commandMutationLevelsToolStripMenuItem.ToolTipText = "Creates a console command to set the mutation levels of a creature";
            commandMutationLevelsToolStripMenuItem.Click += commandMutationLevelsToolStripMenuItem_Click;
            // 
            // copyCoToolStripMenuItem
            // 
            copyCoToolStripMenuItem.Name = "copyCoToolStripMenuItem";
            copyCoToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            copyCoToolStripMenuItem.Text = "Color console command";
            copyCoToolStripMenuItem.Click += copyConsoleColorToolStripMenuItem_Click;
            // 
            // toolStripSeparator25
            // 
            toolStripSeparator25.Name = "toolStripSeparator25";
            toolStripSeparator25.Size = new System.Drawing.Size(233, 6);
            // 
            // copyCreatureToolStripMenuItem
            // 
            copyCreatureToolStripMenuItem.Name = "copyCreatureToolStripMenuItem";
            copyCreatureToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            copyCreatureToolStripMenuItem.Text = "Copy Creature";
            copyCreatureToolStripMenuItem.Click += copyCreatureToolStripMenuItem_Click;
            // 
            // pasteCreatureToolStripMenuItem
            // 
            pasteCreatureToolStripMenuItem.Name = "pasteCreatureToolStripMenuItem";
            pasteCreatureToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            pasteCreatureToolStripMenuItem.Text = "Paste Creature";
            pasteCreatureToolStripMenuItem.Click += pasteCreatureToolStripMenuItem_Click;
            // 
            // libraryFilterToolStripMenuItem
            // 
            libraryFilterToolStripMenuItem.Name = "libraryFilterToolStripMenuItem";
            libraryFilterToolStripMenuItem.Size = new System.Drawing.Size(91, 20);
            libraryFilterToolStripMenuItem.Text = "Library filter…";
            libraryFilterToolStripMenuItem.Click += libraryFilterToolStripMenuItem_Click;
            // 
            // toolStripMenuItemMutationColumns
            // 
            toolStripMenuItemMutationColumns.CheckOnClick = true;
            toolStripMenuItemMutationColumns.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            toolStripMenuItemMutationColumns.Name = "toolStripMenuItemMutationColumns";
            toolStripMenuItemMutationColumns.Size = new System.Drawing.Size(119, 20);
            toolStripMenuItemMutationColumns.Text = "Mutation Columns";
            toolStripMenuItemMutationColumns.CheckedChanged += toolStripMenuItemMutationColumns_CheckedChanged;
            // 
            // nameGeneratorToolStripMenuItem
            // 
            nameGeneratorToolStripMenuItem.Name = "nameGeneratorToolStripMenuItem";
            nameGeneratorToolStripMenuItem.Size = new System.Drawing.Size(105, 20);
            nameGeneratorToolStripMenuItem.Text = "Name generator";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { openSettingsToolStripMenuItem, statsOptionsToolStripMenuItem, toolStripSeparator18, modValueManagerToolStripMenuItem, customStatOverridesToolStripMenuItem, toolStripSeparator1, speciesImagesToolStripMenuItem, extraDataToolStripMenuItem, toolStripSeparator23, openJsonDataFolderToolStripMenuItem, speciesSortingToolStripMenuItem, editVariantTagsToHideToolStripMenuItem, appSettingsToolStripMenuItem });
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            settingsToolStripMenuItem.Text = "Settings";
            // 
            // openSettingsToolStripMenuItem
            // 
            openSettingsToolStripMenuItem.Name = "openSettingsToolStripMenuItem";
            openSettingsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemcomma;
            openSettingsToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            openSettingsToolStripMenuItem.Text = "Settings…";
            openSettingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // statsOptionsToolStripMenuItem
            // 
            statsOptionsToolStripMenuItem.Name = "statsOptionsToolStripMenuItem";
            statsOptionsToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            statsOptionsToolStripMenuItem.Text = "Stats options";
            statsOptionsToolStripMenuItem.Click += statsOptionsToolStripMenuItem_Click;
            // 
            // toolStripSeparator18
            // 
            toolStripSeparator18.Name = "toolStripSeparator18";
            toolStripSeparator18.Size = new System.Drawing.Size(223, 6);
            // 
            // modValueManagerToolStripMenuItem
            // 
            modValueManagerToolStripMenuItem.Name = "modValueManagerToolStripMenuItem";
            modValueManagerToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            modValueManagerToolStripMenuItem.Text = "Mod value manager…";
            modValueManagerToolStripMenuItem.Click += loadAdditionalValuesToolStripMenuItem_Click;
            // 
            // customStatOverridesToolStripMenuItem
            // 
            customStatOverridesToolStripMenuItem.Name = "customStatOverridesToolStripMenuItem";
            customStatOverridesToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            customStatOverridesToolStripMenuItem.Text = "Custom stat overrides…";
            customStatOverridesToolStripMenuItem.Click += customStatOverridesToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(223, 6);
            // 
            // speciesImagesToolStripMenuItem
            // 
            speciesImagesToolStripMenuItem.Name = "speciesImagesToolStripMenuItem";
            speciesImagesToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            speciesImagesToolStripMenuItem.Text = "Species Images…";
            speciesImagesToolStripMenuItem.Click += speciesImagesToolStripMenuItem_Click;
            // 
            // extraDataToolStripMenuItem
            // 
            extraDataToolStripMenuItem.Name = "extraDataToolStripMenuItem";
            extraDataToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            extraDataToolStripMenuItem.Text = "Extra data…";
            extraDataToolStripMenuItem.Click += extraDataToolStripMenuItem_Click;
            // 
            // toolStripSeparator23
            // 
            toolStripSeparator23.Name = "toolStripSeparator23";
            toolStripSeparator23.Size = new System.Drawing.Size(223, 6);
            // 
            // openJsonDataFolderToolStripMenuItem
            // 
            openJsonDataFolderToolStripMenuItem.Name = "openJsonDataFolderToolStripMenuItem";
            openJsonDataFolderToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            openJsonDataFolderToolStripMenuItem.Text = "Open json data folder…";
            openJsonDataFolderToolStripMenuItem.Click += openJsonDataFolderToolStripMenuItem_Click;
            // 
            // speciesSortingToolStripMenuItem
            // 
            speciesSortingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { resetSortingToolStripMenuItem, resetSortingToolStripMenuItem1, toolStripSeparator26, editSortingToolStripMenuItem, applyChangedSortingToolStripMenuItem, helpAboutSpeciesSortingToolStripMenuItem });
            speciesSortingToolStripMenuItem.Name = "speciesSortingToolStripMenuItem";
            speciesSortingToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            speciesSortingToolStripMenuItem.Text = "Species sorting";
            // 
            // resetSortingToolStripMenuItem
            // 
            resetSortingToolStripMenuItem.Name = "resetSortingToolStripMenuItem";
            resetSortingToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            resetSortingToolStripMenuItem.Text = "Reset sorting to default";
            resetSortingToolStripMenuItem.Click += resetSortingToolStripMenuItem_Click;
            // 
            // resetSortingToolStripMenuItem1
            // 
            resetSortingToolStripMenuItem1.Name = "resetSortingToolStripMenuItem1";
            resetSortingToolStripMenuItem1.Size = new System.Drawing.Size(214, 22);
            resetSortingToolStripMenuItem1.Text = "Reset sorting";
            resetSortingToolStripMenuItem1.Click += resetSortingToolStripMenuItem1_Click;
            // 
            // toolStripSeparator26
            // 
            toolStripSeparator26.Name = "toolStripSeparator26";
            toolStripSeparator26.Size = new System.Drawing.Size(211, 6);
            // 
            // editSortingToolStripMenuItem
            // 
            editSortingToolStripMenuItem.Name = "editSortingToolStripMenuItem";
            editSortingToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            editSortingToolStripMenuItem.Text = "Edit sorting";
            editSortingToolStripMenuItem.Click += editSortingToolStripMenuItem_Click;
            // 
            // applyChangedSortingToolStripMenuItem
            // 
            applyChangedSortingToolStripMenuItem.Name = "applyChangedSortingToolStripMenuItem";
            applyChangedSortingToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            applyChangedSortingToolStripMenuItem.Text = "Apply changed sorting";
            applyChangedSortingToolStripMenuItem.Click += applyChangedSortingToolStripMenuItem_Click;
            // 
            // helpAboutSpeciesSortingToolStripMenuItem
            // 
            helpAboutSpeciesSortingToolStripMenuItem.Name = "helpAboutSpeciesSortingToolStripMenuItem";
            helpAboutSpeciesSortingToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            helpAboutSpeciesSortingToolStripMenuItem.Text = "Help about species sorting";
            helpAboutSpeciesSortingToolStripMenuItem.Click += helpAboutSpeciesSortingToolStripMenuItem_Click;
            // 
            // editVariantTagsToHideToolStripMenuItem
            // 
            editVariantTagsToHideToolStripMenuItem.Name = "editVariantTagsToHideToolStripMenuItem";
            editVariantTagsToHideToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            editVariantTagsToHideToolStripMenuItem.Text = "Edit variant tags to hide";
            editVariantTagsToHideToolStripMenuItem.ToolTipText = "Add all variant tags that should be hidden in species names in this file, one variant per line.";
            editVariantTagsToHideToolStripMenuItem.Click += editVariantTagsToHideToolStripMenuItem_Click;
            // 
            // appSettingsToolStripMenuItem
            // 
            appSettingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { showSettingsFileInExplorerToolStripMenuItem, loadAppSettingsFromFileToolStripMenuItem, saveAppSettingsTToolStripMenuItem, toolStripSeparator29, showStatsOptionsFileInExplorerToolStripMenuItem });
            appSettingsToolStripMenuItem.Name = "appSettingsToolStripMenuItem";
            appSettingsToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            appSettingsToolStripMenuItem.Text = "App settings";
            // 
            // showSettingsFileInExplorerToolStripMenuItem
            // 
            showSettingsFileInExplorerToolStripMenuItem.Name = "showSettingsFileInExplorerToolStripMenuItem";
            showSettingsFileInExplorerToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            showSettingsFileInExplorerToolStripMenuItem.Text = "Show settings file in explorer";
            showSettingsFileInExplorerToolStripMenuItem.Click += showSettingsFileInExplorerToolStripMenuItem_Click;
            // 
            // loadAppSettingsFromFileToolStripMenuItem
            // 
            loadAppSettingsFromFileToolStripMenuItem.Name = "loadAppSettingsFromFileToolStripMenuItem";
            loadAppSettingsFromFileToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            loadAppSettingsFromFileToolStripMenuItem.Text = "Import app settings from file";
            loadAppSettingsFromFileToolStripMenuItem.Click += loadAppSettingsFromFileToolStripMenuItem_Click;
            // 
            // saveAppSettingsTToolStripMenuItem
            // 
            saveAppSettingsTToolStripMenuItem.Name = "saveAppSettingsTToolStripMenuItem";
            saveAppSettingsTToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            saveAppSettingsTToolStripMenuItem.Text = "Export app settings to file";
            saveAppSettingsTToolStripMenuItem.Click += saveAppSettingsTToolStripMenuItem_Click;
            // 
            // toolStripSeparator29
            // 
            toolStripSeparator29.Name = "toolStripSeparator29";
            toolStripSeparator29.Size = new System.Drawing.Size(247, 6);
            // 
            // showStatsOptionsFileInExplorerToolStripMenuItem
            // 
            showStatsOptionsFileInExplorerToolStripMenuItem.Name = "showStatsOptionsFileInExplorerToolStripMenuItem";
            showStatsOptionsFileInExplorerToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            showStatsOptionsFileInExplorerToolStripMenuItem.Text = "Show StatsOptions file in explorer";
            showStatsOptionsFileInExplorerToolStripMenuItem.Click += showStatsOptionsFileInExplorerToolStripMenuItem_Click;
            // 
            // serverToolStripMenuItem
            // 
            serverToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { listenToolStripMenuItem, currentTokenToolStripMenuItem, listenWithNewTokenToolStripMenuItem, showTokenPopupOnListeningToolStripMenuItem, beginListeningToExportGunOnLaunchToolStripMenuItem, toolStripSeparator28, openModPageInBrowserToolStripMenuItem, sendExampleCreatureToolStripMenuItem, saveExportFileLocallyToolStripMenuItem, sendServerCreatureStatusNeuterToolStripMenuItem, sendServerCreatureStatusDeadToolStripMenuItem });
            serverToolStripMenuItem.Name = "serverToolStripMenuItem";
            serverToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            serverToolStripMenuItem.Text = "Export gun";
            // 
            // listenToolStripMenuItem
            // 
            listenToolStripMenuItem.CheckOnClick = true;
            listenToolStripMenuItem.Name = "listenToolStripMenuItem";
            listenToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            listenToolStripMenuItem.Text = "Listen";
            listenToolStripMenuItem.Click += listenToolStripMenuItem_Click;
            // 
            // currentTokenToolStripMenuItem
            // 
            currentTokenToolStripMenuItem.Name = "currentTokenToolStripMenuItem";
            currentTokenToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            currentTokenToolStripMenuItem.Text = "View current token";
            currentTokenToolStripMenuItem.Click += currentTokenToolStripMenuItem_Click;
            // 
            // listenWithNewTokenToolStripMenuItem
            // 
            listenWithNewTokenToolStripMenuItem.Name = "listenWithNewTokenToolStripMenuItem";
            listenWithNewTokenToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            listenWithNewTokenToolStripMenuItem.Text = "Listen with new token";
            listenWithNewTokenToolStripMenuItem.Click += listenWithNewTokenToolStripMenuItem_Click;
            // 
            // showTokenPopupOnListeningToolStripMenuItem
            // 
            showTokenPopupOnListeningToolStripMenuItem.CheckOnClick = true;
            showTokenPopupOnListeningToolStripMenuItem.Name = "showTokenPopupOnListeningToolStripMenuItem";
            showTokenPopupOnListeningToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            showTokenPopupOnListeningToolStripMenuItem.Text = "Show token popup on listening";
            showTokenPopupOnListeningToolStripMenuItem.Click += showTokenPopupOnListeningToolStripMenuItem_Click;
            // 
            // beginListeningToExportGunOnLaunchToolStripMenuItem
            // 
            beginListeningToExportGunOnLaunchToolStripMenuItem.CheckOnClick = true;
            beginListeningToExportGunOnLaunchToolStripMenuItem.Name = "beginListeningToExportGunOnLaunchToolStripMenuItem";
            beginListeningToExportGunOnLaunchToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            beginListeningToExportGunOnLaunchToolStripMenuItem.Text = "Start listening on app launch";
            beginListeningToExportGunOnLaunchToolStripMenuItem.Click += startListeningToExportGunOnLaunchToolStripMenuItem_Click;
            // 
            // toolStripSeparator28
            // 
            toolStripSeparator28.Name = "toolStripSeparator28";
            toolStripSeparator28.Size = new System.Drawing.Size(236, 6);
            // 
            // openModPageInBrowserToolStripMenuItem
            // 
            openModPageInBrowserToolStripMenuItem.Name = "openModPageInBrowserToolStripMenuItem";
            openModPageInBrowserToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            openModPageInBrowserToolStripMenuItem.Text = "Open mod page in browser";
            openModPageInBrowserToolStripMenuItem.Click += openModPageInBrowserToolStripMenuItem_Click;
            // 
            // sendExampleCreatureToolStripMenuItem
            // 
            sendExampleCreatureToolStripMenuItem.Name = "sendExampleCreatureToolStripMenuItem";
            sendExampleCreatureToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            sendExampleCreatureToolStripMenuItem.Text = "Send example creature";
            sendExampleCreatureToolStripMenuItem.Click += sendExampleCreatureToolStripMenuItem_Click;
            // 
            // saveExportFileLocallyToolStripMenuItem
            // 
            saveExportFileLocallyToolStripMenuItem.Name = "saveExportFileLocallyToolStripMenuItem";
            saveExportFileLocallyToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            saveExportFileLocallyToolStripMenuItem.Text = "Save export file locally…";
            saveExportFileLocallyToolStripMenuItem.Click += saveExportFileLocallyToolStripMenuItem_Click;
            // 
            // sendServerCreatureStatusNeuterToolStripMenuItem
            // 
            sendServerCreatureStatusNeuterToolStripMenuItem.Name = "sendServerCreatureStatusNeuterToolStripMenuItem";
            sendServerCreatureStatusNeuterToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            sendServerCreatureStatusNeuterToolStripMenuItem.Text = "Send neuter status";
            sendServerCreatureStatusNeuterToolStripMenuItem.Click += sendServerCreatureStatusNeuterToolStripMenuItem_Click;
            // 
            // sendServerCreatureStatusDeadToolStripMenuItem
            // 
            sendServerCreatureStatusDeadToolStripMenuItem.Name = "sendServerCreatureStatusDeadToolStripMenuItem";
            sendServerCreatureStatusDeadToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            sendServerCreatureStatusDeadToolStripMenuItem.Text = "Send dead status";
            sendServerCreatureStatusDeadToolStripMenuItem.Click += sendServerCreatureStatusDeadToolStripMenuItem_Click;
            // 
            // arkUtilsToolStripMenuItem
            // 
            arkUtilsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { howManyFemalesToolStripMenuItem, howGoodAreMyStatsToolStripMenuItem });
            arkUtilsToolStripMenuItem.Name = "arkUtilsToolStripMenuItem";
            arkUtilsToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            arkUtilsToolStripMenuItem.Text = "ArkUtils";
            // 
            // howManyFemalesToolStripMenuItem
            // 
            howManyFemalesToolStripMenuItem.Name = "howManyFemalesToolStripMenuItem";
            howManyFemalesToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            howManyFemalesToolStripMenuItem.Text = "How many females…";
            howManyFemalesToolStripMenuItem.Click += howManyFemalesToolStripMenuItem_Click;
            // 
            // howGoodAreMyStatsToolStripMenuItem
            // 
            howGoodAreMyStatsToolStripMenuItem.Name = "howGoodAreMyStatsToolStripMenuItem";
            howGoodAreMyStatsToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            howGoodAreMyStatsToolStripMenuItem.Text = "How good are my stats…";
            howGoodAreMyStatsToolStripMenuItem.Click += howGoodAreMyStatsToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { aboutToolStripMenuItem, toolStripSeparator11, discordServerToolStripMenuItem, onlinehelpToolStripMenuItem, BreedingPlanHelpToolStripMenuItem, extractionIssuesToolStripMenuItem, uIScalingIssueFixToolStripMenuItem, toolStripSeparator12, checkForUpdatedStatsToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(24, 20);
            helpToolStripMenuItem.Text = "?";
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new System.Drawing.Size(186, 6);
            // 
            // discordServerToolStripMenuItem
            // 
            discordServerToolStripMenuItem.Name = "discordServerToolStripMenuItem";
            discordServerToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            discordServerToolStripMenuItem.Text = "Go to Discord server…";
            discordServerToolStripMenuItem.Click += discordServerToolStripMenuItem_Click;
            // 
            // onlinehelpToolStripMenuItem
            // 
            onlinehelpToolStripMenuItem.Name = "onlinehelpToolStripMenuItem";
            onlinehelpToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            onlinehelpToolStripMenuItem.Text = "Online Manual…";
            onlinehelpToolStripMenuItem.Click += onlinehelpToolStripMenuItem_Click;
            // 
            // BreedingPlanHelpToolStripMenuItem
            // 
            BreedingPlanHelpToolStripMenuItem.Name = "BreedingPlanHelpToolStripMenuItem";
            BreedingPlanHelpToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            BreedingPlanHelpToolStripMenuItem.Text = "Breeding Plan…";
            BreedingPlanHelpToolStripMenuItem.Click += breedingPlanToolStripMenuItem_Click;
            // 
            // extractionIssuesToolStripMenuItem
            // 
            extractionIssuesToolStripMenuItem.Name = "extractionIssuesToolStripMenuItem";
            extractionIssuesToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            extractionIssuesToolStripMenuItem.Text = "Extraction Issues…";
            extractionIssuesToolStripMenuItem.Click += extractionIssuesToolStripMenuItem_Click;
            // 
            // uIScalingIssueFixToolStripMenuItem
            // 
            uIScalingIssueFixToolStripMenuItem.Name = "uIScalingIssueFixToolStripMenuItem";
            uIScalingIssueFixToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            uIScalingIssueFixToolStripMenuItem.Text = "UI scaling issue fix…";
            uIScalingIssueFixToolStripMenuItem.Click += uIScalingIssueFixToolStripMenuItem_Click;
            // 
            // toolStripSeparator12
            // 
            toolStripSeparator12.Name = "toolStripSeparator12";
            toolStripSeparator12.Size = new System.Drawing.Size(186, 6);
            // 
            // checkForUpdatedStatsToolStripMenuItem
            // 
            checkForUpdatedStatsToolStripMenuItem.Name = "checkForUpdatedStatsToolStripMenuItem";
            checkForUpdatedStatsToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            checkForUpdatedStatsToolStripMenuItem.Text = "Check for Updates";
            checkForUpdatedStatsToolStripMenuItem.Click += checkForUpdatedStatsToolStripMenuItem_Click;
            // 
            // devToolStripMenuItem
            // 
            devToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { addRandomCreaturesToolStripMenuItem, colorDefinitionsToClipboardToolStripMenuItem });
            devToolStripMenuItem.Name = "devToolStripMenuItem";
            devToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            devToolStripMenuItem.Text = "Dev";
            // 
            // addRandomCreaturesToolStripMenuItem
            // 
            addRandomCreaturesToolStripMenuItem.Name = "addRandomCreaturesToolStripMenuItem";
            addRandomCreaturesToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            addRandomCreaturesToolStripMenuItem.Text = "Add random creatures…";
            addRandomCreaturesToolStripMenuItem.Click += addRandomCreaturesToolStripMenuItem_Click;
            // 
            // colorDefinitionsToClipboardToolStripMenuItem
            // 
            colorDefinitionsToClipboardToolStripMenuItem.Name = "colorDefinitionsToClipboardToolStripMenuItem";
            colorDefinitionsToClipboardToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            colorDefinitionsToClipboardToolStripMenuItem.Text = "Color definitions to clipboard";
            colorDefinitionsToClipboardToolStripMenuItem.Click += colorDefinitionsToClipboardToolStripMenuItem_Click;
            // 
            // panelSums
            // 
            panelSums.Controls.Add(lbShouldBe);
            panelSums.Controls.Add(lbSumDomSB);
            panelSums.Controls.Add(lbSum);
            panelSums.Controls.Add(lbSumWild);
            panelSums.Controls.Add(lbSumDom);
            panelSums.Location = new System.Drawing.Point(4, 29);
            panelSums.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelSums.Name = "panelSums";
            panelSums.Size = new System.Drawing.Size(401, 37);
            panelSums.TabIndex = 8;
            // 
            // lbShouldBe
            // 
            lbShouldBe.AutoSize = true;
            lbShouldBe.Location = new System.Drawing.Point(57, 17);
            lbShouldBe.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbShouldBe.Name = "lbShouldBe";
            lbShouldBe.Size = new System.Drawing.Size(60, 15);
            lbShouldBe.TabIndex = 52;
            lbShouldBe.Text = "Should be";
            // 
            // lbSumDomSB
            // 
            lbSumDomSB.AutoSize = true;
            lbSumDomSB.Location = new System.Drawing.Point(281, 17);
            lbSumDomSB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbSumDomSB.Name = "lbSumDomSB";
            lbSumDomSB.Size = new System.Drawing.Size(25, 15);
            lbSumDomSB.TabIndex = 51;
            lbSumDomSB.Text = "100";
            lbSumDomSB.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panelWildTamedBred
            // 
            panelWildTamedBred.Controls.Add(rbBredExtractor);
            panelWildTamedBred.Controls.Add(rbTamedExtractor);
            panelWildTamedBred.Controls.Add(rbWildExtractor);
            panelWildTamedBred.Location = new System.Drawing.Point(9, 7);
            panelWildTamedBred.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelWildTamedBred.Name = "panelWildTamedBred";
            panelWildTamedBred.Size = new System.Drawing.Size(223, 29);
            panelWildTamedBred.TabIndex = 0;
            // 
            // rbBredExtractor
            // 
            rbBredExtractor.AutoSize = true;
            rbBredExtractor.Location = new System.Drawing.Point(139, 3);
            rbBredExtractor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rbBredExtractor.Name = "rbBredExtractor";
            rbBredExtractor.Size = new System.Drawing.Size(49, 19);
            rbBredExtractor.TabIndex = 3;
            rbBredExtractor.Text = "Bred";
            rbBredExtractor.UseVisualStyleBackColor = true;
            rbBredExtractor.CheckedChanged += radioButtonBred_CheckedChanged;
            // 
            // rbTamedExtractor
            // 
            rbTamedExtractor.AutoSize = true;
            rbTamedExtractor.Checked = true;
            rbTamedExtractor.Location = new System.Drawing.Point(64, 3);
            rbTamedExtractor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rbTamedExtractor.Name = "rbTamedExtractor";
            rbTamedExtractor.Size = new System.Drawing.Size(61, 19);
            rbTamedExtractor.TabIndex = 2;
            rbTamedExtractor.TabStop = true;
            rbTamedExtractor.Text = "Tamed";
            rbTamedExtractor.UseVisualStyleBackColor = true;
            rbTamedExtractor.CheckedChanged += radioButtonTamed_CheckedChanged;
            // 
            // rbWildExtractor
            // 
            rbWildExtractor.AutoSize = true;
            rbWildExtractor.Location = new System.Drawing.Point(4, 3);
            rbWildExtractor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rbWildExtractor.Name = "rbWildExtractor";
            rbWildExtractor.Size = new System.Drawing.Size(49, 19);
            rbWildExtractor.TabIndex = 1;
            rbWildExtractor.Text = "Wild";
            rbWildExtractor.UseVisualStyleBackColor = true;
            rbWildExtractor.CheckedChanged += radioButtonWild_CheckedChanged;
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabPageStatTesting);
            tabControlMain.Controls.Add(tabPageExtractor);
            tabControlMain.Controls.Add(tabPageLibrary);
            tabControlMain.Controls.Add(tabPageLibraryInfo);
            tabControlMain.Controls.Add(tabPagePedigree);
            tabControlMain.Controls.Add(tabPageTaming);
            tabControlMain.Controls.Add(tabPageBreedingPlan);
            tabControlMain.Controls.Add(tabPageCurrentBreeds);
            tabControlMain.Controls.Add(tabPageRaising);
            tabControlMain.Controls.Add(tabPageTimer);
            tabControlMain.Controls.Add(tabPagePlayerTribes);
            tabControlMain.Controls.Add(tabPageNotes);
            tabControlMain.Controls.Add(TabPageOCR);
            tabControlMain.Controls.Add(tabPageExtractionTests);
            tabControlMain.Controls.Add(tabPageMultiplierTesting);
            tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControlMain.Location = new System.Drawing.Point(0, 111);
            tabControlMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 1;
            tabControlMain.Size = new System.Drawing.Size(2191, 946);
            tabControlMain.TabIndex = 3;
            tabControlMain.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // tabPageStatTesting
            // 
            tabPageStatTesting.AutoScroll = true;
            tabPageStatTesting.Controls.Add(ColoredCreatureImageDisplayTester);
            tabPageStatTesting.Controls.Add(CbLinkWildMutatedLevelsTester);
            tabPageStatTesting.Controls.Add(statPotentials1);
            tabPageStatTesting.Controls.Add(gbStatChart);
            tabPageStatTesting.Controls.Add(panelWildTamedBredTester);
            tabPageStatTesting.Controls.Add(groupBox2);
            tabPageStatTesting.Controls.Add(gpPreviewEdit);
            tabPageStatTesting.Controls.Add(groupBox1);
            tabPageStatTesting.Controls.Add(creatureInfoInputTester);
            tabPageStatTesting.Location = new System.Drawing.Point(4, 24);
            tabPageStatTesting.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageStatTesting.Name = "tabPageStatTesting";
            tabPageStatTesting.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageStatTesting.Size = new System.Drawing.Size(2183, 918);
            tabPageStatTesting.TabIndex = 1;
            tabPageStatTesting.Text = "Stat Testing";
            // 
            // ColoredCreatureImageDisplayTester
            // 
            ColoredCreatureImageDisplayTester.Location = new System.Drawing.Point(748, 564);
            ColoredCreatureImageDisplayTester.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ColoredCreatureImageDisplayTester.Name = "ColoredCreatureImageDisplayTester";
            ColoredCreatureImageDisplayTester.Size = new System.Drawing.Size(299, 323);
            ColoredCreatureImageDisplayTester.TabIndex = 15;
            // 
            // CbLinkWildMutatedLevelsTester
            // 
            CbLinkWildMutatedLevelsTester.AutoSize = true;
            CbLinkWildMutatedLevelsTester.Location = new System.Drawing.Point(302, 12);
            CbLinkWildMutatedLevelsTester.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CbLinkWildMutatedLevelsTester.Name = "CbLinkWildMutatedLevelsTester";
            CbLinkWildMutatedLevelsTester.Size = new System.Drawing.Size(123, 19);
            CbLinkWildMutatedLevelsTester.TabIndex = 14;
            CbLinkWildMutatedLevelsTester.Text = "Link wild-mutated";
            CbLinkWildMutatedLevelsTester.UseVisualStyleBackColor = true;
            CbLinkWildMutatedLevelsTester.CheckedChanged += CbLinkWildMutatedLevelsTester_CheckedChanged;
            // 
            // statPotentials1
            // 
            statPotentials1.Location = new System.Drawing.Point(1003, 10);
            statPotentials1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            statPotentials1.Name = "statPotentials1";
            statPotentials1.Size = new System.Drawing.Size(342, 500);
            statPotentials1.TabIndex = 12;
            // 
            // gbStatChart
            // 
            gbStatChart.Controls.Add(radarChart1);
            gbStatChart.Location = new System.Drawing.Point(748, 10);
            gbStatChart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gbStatChart.Name = "gbStatChart";
            gbStatChart.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gbStatChart.Size = new System.Drawing.Size(248, 263);
            gbStatChart.TabIndex = 11;
            gbStatChart.TabStop = false;
            gbStatChart.Text = "Stat-Chart";
            // 
            // radarChart1
            // 
            radarChart1.Image = (System.Drawing.Image)resources.GetObject("radarChart1.Image");
            radarChart1.Location = new System.Drawing.Point(7, 22);
            radarChart1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            radarChart1.Name = "radarChart1";
            radarChart1.Size = new System.Drawing.Size(233, 231);
            radarChart1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            radarChart1.TabIndex = 10;
            radarChart1.TabStop = false;
            // 
            // panelWildTamedBredTester
            // 
            panelWildTamedBredTester.Controls.Add(rbBredTester);
            panelWildTamedBredTester.Controls.Add(rbTamedTester);
            panelWildTamedBredTester.Controls.Add(rbWildTester);
            panelWildTamedBredTester.Location = new System.Drawing.Point(9, 7);
            panelWildTamedBredTester.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelWildTamedBredTester.Name = "panelWildTamedBredTester";
            panelWildTamedBredTester.Size = new System.Drawing.Size(223, 29);
            panelWildTamedBredTester.TabIndex = 0;
            // 
            // rbBredTester
            // 
            rbBredTester.AutoSize = true;
            rbBredTester.Location = new System.Drawing.Point(139, 3);
            rbBredTester.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rbBredTester.Name = "rbBredTester";
            rbBredTester.Size = new System.Drawing.Size(49, 19);
            rbBredTester.TabIndex = 3;
            rbBredTester.Text = "Bred";
            rbBredTester.UseVisualStyleBackColor = true;
            rbBredTester.CheckedChanged += radioButtonTesterBred_CheckedChanged;
            // 
            // rbTamedTester
            // 
            rbTamedTester.AutoSize = true;
            rbTamedTester.Checked = true;
            rbTamedTester.Location = new System.Drawing.Point(64, 3);
            rbTamedTester.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rbTamedTester.Name = "rbTamedTester";
            rbTamedTester.Size = new System.Drawing.Size(61, 19);
            rbTamedTester.TabIndex = 2;
            rbTamedTester.TabStop = true;
            rbTamedTester.Text = "Tamed";
            rbTamedTester.UseVisualStyleBackColor = true;
            rbTamedTester.CheckedChanged += radioButtonTesterTamed_CheckedChanged;
            // 
            // rbWildTester
            // 
            rbWildTester.AutoSize = true;
            rbWildTester.Location = new System.Drawing.Point(4, 3);
            rbWildTester.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rbWildTester.Name = "rbWildTester";
            rbWildTester.Size = new System.Drawing.Size(49, 19);
            rbWildTester.TabIndex = 1;
            rbWildTester.Text = "Wild";
            rbWildTester.UseVisualStyleBackColor = true;
            rbWildTester.CheckedChanged += radioButtonTesterWild_CheckedChanged;
            // 
            // groupBox2
            // 
            groupBox2.AutoSize = true;
            groupBox2.Controls.Add(flowLayoutPanelStatIOsTester);
            groupBox2.Location = new System.Drawing.Point(9, 43);
            groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox2.Size = new System.Drawing.Size(419, 817);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Stats";
            // 
            // flowLayoutPanelStatIOsTester
            // 
            flowLayoutPanelStatIOsTester.AutoScroll = true;
            flowLayoutPanelStatIOsTester.Controls.Add(panel2);
            flowLayoutPanelStatIOsTester.Controls.Add(panelStatTesterFootnote);
            flowLayoutPanelStatIOsTester.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanelStatIOsTester.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            flowLayoutPanelStatIOsTester.Location = new System.Drawing.Point(4, 19);
            flowLayoutPanelStatIOsTester.Margin = new System.Windows.Forms.Padding(0);
            flowLayoutPanelStatIOsTester.Name = "flowLayoutPanelStatIOsTester";
            flowLayoutPanelStatIOsTester.Size = new System.Drawing.Size(411, 795);
            flowLayoutPanelStatIOsTester.TabIndex = 53;
            flowLayoutPanelStatIOsTester.WrapContents = false;
            // 
            // panel2
            // 
            panel2.Controls.Add(label4);
            panel2.Controls.Add(lbTesterWildLevel);
            panel2.Controls.Add(lbTesterDomLevel);
            panel2.Controls.Add(lbBreedingValueTester);
            panel2.Controls.Add(lbCurrentValue);
            panel2.Location = new System.Drawing.Point(0, 3);
            panel2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(408, 20);
            panel2.TabIndex = 54;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(70, 0);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(61, 15);
            label4.TabIndex = 37;
            label4.Text = "Mutations";
            // 
            // lbCurrentValue
            // 
            lbCurrentValue.AutoSize = true;
            lbCurrentValue.Location = new System.Drawing.Point(208, 0);
            lbCurrentValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbCurrentValue.Name = "lbCurrentValue";
            lbCurrentValue.Size = new System.Drawing.Size(78, 15);
            lbCurrentValue.TabIndex = 36;
            lbCurrentValue.Text = "Current Value";
            // 
            // panelStatTesterFootnote
            // 
            panelStatTesterFootnote.Controls.Add(LbWarningLevel255);
            panelStatTesterFootnote.Controls.Add(lbWildLevelTester);
            panelStatTesterFootnote.Controls.Add(labelDomLevelSum);
            panelStatTesterFootnote.Controls.Add(labelTesterTotalLevel);
            panelStatTesterFootnote.Controls.Add(lbNotYetTamed);
            panelStatTesterFootnote.Location = new System.Drawing.Point(4, 29);
            panelStatTesterFootnote.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelStatTesterFootnote.Name = "panelStatTesterFootnote";
            panelStatTesterFootnote.Size = new System.Drawing.Size(401, 148);
            panelStatTesterFootnote.TabIndex = 54;
            // 
            // LbWarningLevel255
            // 
            LbWarningLevel255.BackColor = System.Drawing.Color.Firebrick;
            LbWarningLevel255.ForeColor = System.Drawing.Color.White;
            LbWarningLevel255.Location = new System.Drawing.Point(9, 60);
            LbWarningLevel255.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbWarningLevel255.Name = "LbWarningLevel255";
            LbWarningLevel255.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LbWarningLevel255.Size = new System.Drawing.Size(388, 81);
            LbWarningLevel255.TabIndex = 50;
            // 
            // lbWildLevelTester
            // 
            lbWildLevelTester.AutoSize = true;
            lbWildLevelTester.Location = new System.Drawing.Point(9, 18);
            lbWildLevelTester.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbWildLevelTester.Name = "lbWildLevelTester";
            lbWildLevelTester.Size = new System.Drawing.Size(83, 15);
            lbWildLevelTester.TabIndex = 13;
            lbWildLevelTester.Text = "PreTame Level";
            // 
            // labelDomLevelSum
            // 
            labelDomLevelSum.AutoSize = true;
            labelDomLevelSum.Location = new System.Drawing.Point(9, 0);
            labelDomLevelSum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelDomLevelSum.Name = "labelDomLevelSum";
            labelDomLevelSum.Size = new System.Drawing.Size(68, 15);
            labelDomLevelSum.TabIndex = 46;
            labelDomLevelSum.Text = "Dom Levels";
            // 
            // labelTesterTotalLevel
            // 
            labelTesterTotalLevel.Location = new System.Drawing.Point(172, 0);
            labelTesterTotalLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelTesterTotalLevel.Name = "labelTesterTotalLevel";
            labelTesterTotalLevel.Size = new System.Drawing.Size(167, 15);
            labelTesterTotalLevel.TabIndex = 49;
            labelTesterTotalLevel.Text = "Total Level";
            labelTesterTotalLevel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbNotYetTamed
            // 
            lbNotYetTamed.Location = new System.Drawing.Point(7, 39);
            lbNotYetTamed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbNotYetTamed.Name = "lbNotYetTamed";
            lbNotYetTamed.Size = new System.Drawing.Size(332, 18);
            lbNotYetTamed.TabIndex = 41;
            lbNotYetTamed.Text = "*Creature is not yet tamed and may get better values then.";
            lbNotYetTamed.Visible = false;
            // 
            // gpPreviewEdit
            // 
            gpPreviewEdit.Controls.Add(lbCurrentCreature);
            gpPreviewEdit.Controls.Add(labelCurrentTesterCreature);
            gpPreviewEdit.Controls.Add(lbTestingInfo);
            gpPreviewEdit.Location = new System.Drawing.Point(435, 97);
            gpPreviewEdit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gpPreviewEdit.Name = "gpPreviewEdit";
            gpPreviewEdit.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gpPreviewEdit.Size = new System.Drawing.Size(306, 105);
            gpPreviewEdit.TabIndex = 3;
            gpPreviewEdit.TabStop = false;
            gpPreviewEdit.Text = "Preview / Edit";
            // 
            // lbCurrentCreature
            // 
            lbCurrentCreature.AutoSize = true;
            lbCurrentCreature.Location = new System.Drawing.Point(7, 40);
            lbCurrentCreature.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbCurrentCreature.Name = "lbCurrentCreature";
            lbCurrentCreature.Size = new System.Drawing.Size(95, 15);
            lbCurrentCreature.TabIndex = 39;
            lbCurrentCreature.Text = "Current Creature";
            // 
            // labelCurrentTesterCreature
            // 
            labelCurrentTesterCreature.AutoSize = true;
            labelCurrentTesterCreature.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            labelCurrentTesterCreature.Location = new System.Drawing.Point(7, 60);
            labelCurrentTesterCreature.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelCurrentTesterCreature.Name = "labelCurrentTesterCreature";
            labelCurrentTesterCreature.Size = new System.Drawing.Size(87, 13);
            labelCurrentTesterCreature.TabIndex = 38;
            labelCurrentTesterCreature.Text = "CreatureName";
            // 
            // lbTestingInfo
            // 
            lbTestingInfo.Location = new System.Drawing.Point(7, 18);
            lbTestingInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbTestingInfo.Name = "lbTestingInfo";
            lbTestingInfo.Size = new System.Drawing.Size(292, 29);
            lbTestingInfo.TabIndex = 37;
            lbTestingInfo.Text = "Preview or edit levels of a creature.";
            // 
            // creatureInfoInputTester
            // 
            creatureInfoInputTester.Location = new System.Drawing.Point(435, 212);
            creatureInfoInputTester.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            creatureInfoInputTester.Name = "creatureInfoInputTester";
            creatureInfoInputTester.Size = new System.Drawing.Size(306, 681);
            creatureInfoInputTester.TabIndex = 4;
            creatureInfoInputTester.Add2LibraryClicked += creatureInfoInputTester_Add2Library_Clicked;
            creatureInfoInputTester.Save2LibraryClicked += creatureInfoInputTester_Save2Library_Clicked;
            creatureInfoInputTester.ParentListRequested += CreatureInfoInput_ParentListRequested;
            // 
            // tabPageExtractor
            // 
            tabPageExtractor.AutoScroll = true;
            tabPageExtractor.Controls.Add(ColoredCreatureImageDisplayExtractor);
            tabPageExtractor.Controls.Add(pBondedTamingExtractor);
            tabPageExtractor.Controls.Add(LbAsa);
            tabPageExtractor.Controls.Add(LbBlueprintPath);
            tabPageExtractor.Controls.Add(BtCopyIssueDumpToClipboard);
            tabPageExtractor.Controls.Add(llOnlineHelpExtractionIssues);
            tabPageExtractor.Controls.Add(groupBoxRadarChartExtractor);
            tabPageExtractor.Controls.Add(lbImprintingFailInfo);
            tabPageExtractor.Controls.Add(groupBoxTamingInfo);
            tabPageExtractor.Controls.Add(button2TamingCalc);
            tabPageExtractor.Controls.Add(gbStatsExtractor);
            tabPageExtractor.Controls.Add(btExtractLevels);
            tabPageExtractor.Controls.Add(cbQuickWildCheck);
            tabPageExtractor.Controls.Add(panelWildTamedBred);
            tabPageExtractor.Controls.Add(lbInfoYellowStats);
            tabPageExtractor.Controls.Add(groupBoxDetailsExtractor);
            tabPageExtractor.Controls.Add(groupBoxPossibilities);
            tabPageExtractor.Controls.Add(lbLevel);
            tabPageExtractor.Controls.Add(labelErrorHelp);
            tabPageExtractor.Controls.Add(creatureAnalysis1);
            tabPageExtractor.Controls.Add(parentInheritanceExtractor);
            tabPageExtractor.Controls.Add(numericUpDownLevel);
            tabPageExtractor.Controls.Add(creatureInfoInputExtractor);
            tabPageExtractor.Location = new System.Drawing.Point(4, 24);
            tabPageExtractor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageExtractor.Name = "tabPageExtractor";
            tabPageExtractor.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageExtractor.Size = new System.Drawing.Size(192, 72);
            tabPageExtractor.TabIndex = 0;
            tabPageExtractor.Text = "Extractor";
            // 
            // ColoredCreatureImageDisplayExtractor
            // 
            ColoredCreatureImageDisplayExtractor.Location = new System.Drawing.Point(748, 564);
            ColoredCreatureImageDisplayExtractor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ColoredCreatureImageDisplayExtractor.Name = "ColoredCreatureImageDisplayExtractor";
            ColoredCreatureImageDisplayExtractor.Size = new System.Drawing.Size(299, 323);
            ColoredCreatureImageDisplayExtractor.TabIndex = 59;
            // 
            // pBondedTamingExtractor
            // 
            pBondedTamingExtractor.Controls.Add(RbBondedTaming3);
            pBondedTamingExtractor.Controls.Add(RbBondedTaming2);
            pBondedTamingExtractor.Controls.Add(RbBondedTaming1);
            pBondedTamingExtractor.Controls.Add(RbBondedTaming0);
            pBondedTamingExtractor.Controls.Add(LbBondedTaming);
            pBondedTamingExtractor.Location = new System.Drawing.Point(435, 93);
            pBondedTamingExtractor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pBondedTamingExtractor.Name = "pBondedTamingExtractor";
            pBondedTamingExtractor.Size = new System.Drawing.Size(267, 30);
            pBondedTamingExtractor.TabIndex = 58;
            // 
            // RbBondedTaming3
            // 
            RbBondedTaming3.Appearance = System.Windows.Forms.Appearance.Button;
            RbBondedTaming3.AutoSize = true;
            RbBondedTaming3.Location = new System.Drawing.Point(238, 2);
            RbBondedTaming3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RbBondedTaming3.Name = "RbBondedTaming3";
            RbBondedTaming3.Size = new System.Drawing.Size(23, 25);
            RbBondedTaming3.TabIndex = 61;
            RbBondedTaming3.TabStop = true;
            RbBondedTaming3.Text = "3";
            RbBondedTaming3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            RbBondedTaming3.UseVisualStyleBackColor = true;
            // 
            // RbBondedTaming2
            // 
            RbBondedTaming2.Appearance = System.Windows.Forms.Appearance.Button;
            RbBondedTaming2.AutoSize = true;
            RbBondedTaming2.Location = new System.Drawing.Point(211, 2);
            RbBondedTaming2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RbBondedTaming2.Name = "RbBondedTaming2";
            RbBondedTaming2.Size = new System.Drawing.Size(23, 25);
            RbBondedTaming2.TabIndex = 60;
            RbBondedTaming2.TabStop = true;
            RbBondedTaming2.Text = "2";
            RbBondedTaming2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            RbBondedTaming2.UseVisualStyleBackColor = true;
            // 
            // RbBondedTaming1
            // 
            RbBondedTaming1.Appearance = System.Windows.Forms.Appearance.Button;
            RbBondedTaming1.AutoSize = true;
            RbBondedTaming1.Location = new System.Drawing.Point(184, 2);
            RbBondedTaming1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RbBondedTaming1.Name = "RbBondedTaming1";
            RbBondedTaming1.Size = new System.Drawing.Size(23, 25);
            RbBondedTaming1.TabIndex = 59;
            RbBondedTaming1.TabStop = true;
            RbBondedTaming1.Text = "1";
            RbBondedTaming1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            RbBondedTaming1.UseVisualStyleBackColor = true;
            // 
            // RbBondedTaming0
            // 
            RbBondedTaming0.Appearance = System.Windows.Forms.Appearance.Button;
            RbBondedTaming0.AutoSize = true;
            RbBondedTaming0.Location = new System.Drawing.Point(158, 2);
            RbBondedTaming0.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RbBondedTaming0.Name = "RbBondedTaming0";
            RbBondedTaming0.Size = new System.Drawing.Size(23, 25);
            RbBondedTaming0.TabIndex = 58;
            RbBondedTaming0.TabStop = true;
            RbBondedTaming0.Text = "0";
            RbBondedTaming0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            RbBondedTaming0.UseVisualStyleBackColor = true;
            // 
            // LbBondedTaming
            // 
            LbBondedTaming.AutoSize = true;
            LbBondedTaming.Location = new System.Drawing.Point(2, 8);
            LbBondedTaming.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbBondedTaming.Name = "LbBondedTaming";
            LbBondedTaming.Size = new System.Drawing.Size(150, 15);
            LbBondedTaming.TabIndex = 57;
            LbBondedTaming.Text = "Bonded Taming talent rank";
            // 
            // LbAsa
            // 
            LbAsa.AutoSize = true;
            LbAsa.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            LbAsa.ForeColor = System.Drawing.SystemColors.GrayText;
            LbAsa.Location = new System.Drawing.Point(404, 76);
            LbAsa.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbAsa.Name = "LbAsa";
            LbAsa.Size = new System.Drawing.Size(20, 9);
            LbAsa.TabIndex = 56;
            LbAsa.Text = "ASA";
            // 
            // LbBlueprintPath
            // 
            LbBlueprintPath.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            LbBlueprintPath.ForeColor = System.Drawing.SystemColors.GrayText;
            LbBlueprintPath.Location = new System.Drawing.Point(9, 39);
            LbBlueprintPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbBlueprintPath.Name = "LbBlueprintPath";
            LbBlueprintPath.Size = new System.Drawing.Size(394, 47);
            LbBlueprintPath.TabIndex = 54;
            LbBlueprintPath.Text = "/Game/​PrimalEarth/​Dinos/​Direwolf/ ​Direwolf_Character_BP.Direwolf_Character_BP";
            LbBlueprintPath.Click += LbBlueprintPath_Click;
            // 
            // BtCopyIssueDumpToClipboard
            // 
            BtCopyIssueDumpToClipboard.Location = new System.Drawing.Point(749, 811);
            BtCopyIssueDumpToClipboard.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtCopyIssueDumpToClipboard.Name = "BtCopyIssueDumpToClipboard";
            BtCopyIssueDumpToClipboard.Size = new System.Drawing.Size(402, 27);
            BtCopyIssueDumpToClipboard.TabIndex = 53;
            BtCopyIssueDumpToClipboard.Text = "Copy library dump to clipboard for a report";
            BtCopyIssueDumpToClipboard.UseVisualStyleBackColor = true;
            BtCopyIssueDumpToClipboard.Click += BtCopyIssueDumpToClipboard_Click;
            // 
            // llOnlineHelpExtractionIssues
            // 
            llOnlineHelpExtractionIssues.AutoSize = true;
            llOnlineHelpExtractionIssues.Location = new System.Drawing.Point(752, 691);
            llOnlineHelpExtractionIssues.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            llOnlineHelpExtractionIssues.Name = "llOnlineHelpExtractionIssues";
            llOnlineHelpExtractionIssues.Size = new System.Drawing.Size(156, 15);
            llOnlineHelpExtractionIssues.TabIndex = 50;
            llOnlineHelpExtractionIssues.TabStop = true;
            llOnlineHelpExtractionIssues.Text = "Red Stat-boxes: Online-Help";
            llOnlineHelpExtractionIssues.LinkClicked += llOnlineHelpExtractionIssues_LinkClicked;
            // 
            // groupBoxRadarChartExtractor
            // 
            groupBoxRadarChartExtractor.Controls.Add(radarChartExtractor);
            groupBoxRadarChartExtractor.Location = new System.Drawing.Point(976, 7);
            groupBoxRadarChartExtractor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxRadarChartExtractor.Name = "groupBoxRadarChartExtractor";
            groupBoxRadarChartExtractor.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxRadarChartExtractor.Size = new System.Drawing.Size(175, 188);
            groupBoxRadarChartExtractor.TabIndex = 11;
            groupBoxRadarChartExtractor.TabStop = false;
            groupBoxRadarChartExtractor.Text = "Stat-Chart";
            // 
            // radarChartExtractor
            // 
            radarChartExtractor.Dock = System.Windows.Forms.DockStyle.Fill;
            radarChartExtractor.Image = (System.Drawing.Image)resources.GetObject("radarChartExtractor.Image");
            radarChartExtractor.Location = new System.Drawing.Point(4, 19);
            radarChartExtractor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            radarChartExtractor.Name = "radarChartExtractor";
            radarChartExtractor.Size = new System.Drawing.Size(167, 166);
            radarChartExtractor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            radarChartExtractor.TabIndex = 10;
            radarChartExtractor.TabStop = false;
            // 
            // lbImprintingFailInfo
            // 
            lbImprintingFailInfo.BackColor = System.Drawing.Color.MistyRose;
            lbImprintingFailInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbImprintingFailInfo.ForeColor = System.Drawing.Color.Maroon;
            lbImprintingFailInfo.Location = new System.Drawing.Point(748, 706);
            lbImprintingFailInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbImprintingFailInfo.Name = "lbImprintingFailInfo";
            lbImprintingFailInfo.Size = new System.Drawing.Size(516, 101);
            lbImprintingFailInfo.TabIndex = 49;
            lbImprintingFailInfo.Text = "If the creature is imprinted the extraction may fail because the game sometimes \"forgets\" to increase some stat-values during the imprinting-process. Usually it works after a server-restart.";
            lbImprintingFailInfo.Visible = false;
            // 
            // groupBoxTamingInfo
            // 
            groupBoxTamingInfo.Controls.Add(labelTamingInfo);
            groupBoxTamingInfo.Location = new System.Drawing.Point(749, 69);
            groupBoxTamingInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxTamingInfo.Name = "groupBoxTamingInfo";
            groupBoxTamingInfo.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxTamingInfo.Size = new System.Drawing.Size(203, 488);
            groupBoxTamingInfo.TabIndex = 48;
            groupBoxTamingInfo.TabStop = false;
            groupBoxTamingInfo.Text = "Taming Info";
            // 
            // labelTamingInfo
            // 
            labelTamingInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            labelTamingInfo.Location = new System.Drawing.Point(4, 19);
            labelTamingInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelTamingInfo.Name = "labelTamingInfo";
            labelTamingInfo.Size = new System.Drawing.Size(195, 466);
            labelTamingInfo.TabIndex = 0;
            // 
            // button2TamingCalc
            // 
            button2TamingCalc.Location = new System.Drawing.Point(749, 37);
            button2TamingCalc.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button2TamingCalc.Name = "button2TamingCalc";
            button2TamingCalc.Size = new System.Drawing.Size(206, 27);
            button2TamingCalc.TabIndex = 9;
            button2TamingCalc.Text = "Taming Calculator";
            button2TamingCalc.UseVisualStyleBackColor = true;
            button2TamingCalc.Visible = false;
            button2TamingCalc.Click += button2TamingCalc_Click;
            // 
            // gbStatsExtractor
            // 
            gbStatsExtractor.AutoSize = true;
            gbStatsExtractor.Controls.Add(flowLayoutPanelStatIOsExtractor);
            gbStatsExtractor.Location = new System.Drawing.Point(9, 88);
            gbStatsExtractor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gbStatsExtractor.Name = "gbStatsExtractor";
            gbStatsExtractor.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gbStatsExtractor.Size = new System.Drawing.Size(423, 756);
            gbStatsExtractor.TabIndex = 3;
            gbStatsExtractor.TabStop = false;
            gbStatsExtractor.Text = "Stats";
            // 
            // flowLayoutPanelStatIOsExtractor
            // 
            flowLayoutPanelStatIOsExtractor.AutoScroll = true;
            flowLayoutPanelStatIOsExtractor.Controls.Add(panel1);
            flowLayoutPanelStatIOsExtractor.Controls.Add(panelSums);
            flowLayoutPanelStatIOsExtractor.Controls.Add(labelFootnote);
            flowLayoutPanelStatIOsExtractor.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            flowLayoutPanelStatIOsExtractor.Location = new System.Drawing.Point(7, 22);
            flowLayoutPanelStatIOsExtractor.Margin = new System.Windows.Forms.Padding(0);
            flowLayoutPanelStatIOsExtractor.Name = "flowLayoutPanelStatIOsExtractor";
            flowLayoutPanelStatIOsExtractor.Size = new System.Drawing.Size(412, 712);
            flowLayoutPanelStatIOsExtractor.TabIndex = 52;
            flowLayoutPanelStatIOsExtractor.WrapContents = false;
            // 
            // panel1
            // 
            panel1.Controls.Add(label5);
            panel1.Controls.Add(lbCurrentStatEx);
            panel1.Controls.Add(lbExtractorWildLevel);
            panel1.Controls.Add(labelHBV);
            panel1.Controls.Add(lbExtractorDomLevel);
            panel1.Location = new System.Drawing.Point(4, 3);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(401, 20);
            panel1.TabIndex = 53;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(203, 0);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(61, 15);
            label5.TabIndex = 51;
            label5.Text = "Mutations";
            // 
            // lbCurrentStatEx
            // 
            lbCurrentStatEx.AutoSize = true;
            lbCurrentStatEx.Location = new System.Drawing.Point(4, 0);
            lbCurrentStatEx.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbCurrentStatEx.Name = "lbCurrentStatEx";
            lbCurrentStatEx.Size = new System.Drawing.Size(102, 15);
            lbCurrentStatEx.TabIndex = 50;
            lbCurrentStatEx.Text = "Current stat-value";
            // 
            // btExtractLevels
            // 
            btExtractLevels.Location = new System.Drawing.Point(435, 127);
            btExtractLevels.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btExtractLevels.Name = "btExtractLevels";
            btExtractLevels.Size = new System.Drawing.Size(267, 78);
            btExtractLevels.TabIndex = 6;
            btExtractLevels.Text = "Extract Level Distribution";
            btExtractLevels.UseVisualStyleBackColor = true;
            btExtractLevels.Click += buttonExtract_Click;
            // 
            // cbQuickWildCheck
            // 
            cbQuickWildCheck.AutoSize = true;
            cbQuickWildCheck.Location = new System.Drawing.Point(709, 9);
            cbQuickWildCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbQuickWildCheck.Name = "cbQuickWildCheck";
            cbQuickWildCheck.Size = new System.Drawing.Size(170, 19);
            cbQuickWildCheck.TabIndex = 8;
            cbQuickWildCheck.Text = "Quick Wild-Creature Check";
            cbQuickWildCheck.UseVisualStyleBackColor = true;
            cbQuickWildCheck.CheckedChanged += checkBoxQuickWildCheck_CheckedChanged;
            // 
            // labelErrorHelp
            // 
            labelErrorHelp.AutoEllipsis = true;
            labelErrorHelp.Location = new System.Drawing.Point(749, 50);
            labelErrorHelp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelErrorHelp.Name = "labelErrorHelp";
            labelErrorHelp.Size = new System.Drawing.Size(279, 657);
            labelErrorHelp.TabIndex = 40;
            labelErrorHelp.Text = resources.GetString("labelErrorHelp.Text");
            // 
            // creatureAnalysis1
            // 
            creatureAnalysis1.Location = new System.Drawing.Point(1054, 202);
            creatureAnalysis1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            creatureAnalysis1.Name = "creatureAnalysis1";
            creatureAnalysis1.Size = new System.Drawing.Size(393, 406);
            creatureAnalysis1.TabIndex = 55;
            // 
            // parentInheritanceExtractor
            // 
            parentInheritanceExtractor.Location = new System.Drawing.Point(1054, 615);
            parentInheritanceExtractor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            parentInheritanceExtractor.Name = "parentInheritanceExtractor";
            parentInheritanceExtractor.Size = new System.Drawing.Size(393, 245);
            parentInheritanceExtractor.TabIndex = 52;
            // 
            // numericUpDownLevel
            // 
            numericUpDownLevel.ForeColor = System.Drawing.SystemColors.WindowText;
            numericUpDownLevel.Location = new System.Drawing.Point(285, 10);
            numericUpDownLevel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            numericUpDownLevel.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numericUpDownLevel.Name = "numericUpDownLevel";
            numericUpDownLevel.Size = new System.Drawing.Size(65, 23);
            numericUpDownLevel.TabIndex = 2;
            numericUpDownLevel.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownLevel.ValueChanged += numericUpDownLevel_ValueChanged;
            numericUpDownLevel.Enter += numericUpDown_Enter;
            // 
            // creatureInfoInputExtractor
            // 
            creatureInfoInputExtractor.Location = new System.Drawing.Point(435, 212);
            creatureInfoInputExtractor.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            creatureInfoInputExtractor.Name = "creatureInfoInputExtractor";
            creatureInfoInputExtractor.Size = new System.Drawing.Size(306, 681);
            creatureInfoInputExtractor.TabIndex = 7;
            creatureInfoInputExtractor.Add2LibraryClicked += creatureInfoInputExtractor_Add2Library_Clicked;
            creatureInfoInputExtractor.ParentListRequested += CreatureInfoInput_ParentListRequested;
            // 
            // tabPageLibrary
            // 
            tabPageLibrary.Controls.Add(tableLayoutPanelLibrary);
            tabPageLibrary.Location = new System.Drawing.Point(4, 24);
            tabPageLibrary.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageLibrary.Name = "tabPageLibrary";
            tabPageLibrary.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageLibrary.Size = new System.Drawing.Size(192, 72);
            tabPageLibrary.TabIndex = 2;
            tabPageLibrary.Text = "Library";
            // 
            // tableLayoutPanelLibrary
            // 
            tableLayoutPanelLibrary.ColumnCount = 2;
            tableLayoutPanelLibrary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanelLibrary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanelLibrary.Controls.Add(listViewLibrary, 1, 0);
            tableLayoutPanelLibrary.Controls.Add(tableLayoutPanel1, 0, 0);
            tableLayoutPanelLibrary.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanelLibrary.Location = new System.Drawing.Point(4, 3);
            tableLayoutPanelLibrary.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanelLibrary.Name = "tableLayoutPanelLibrary";
            tableLayoutPanelLibrary.RowCount = 1;
            tableLayoutPanelLibrary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanelLibrary.Size = new System.Drawing.Size(184, 66);
            tableLayoutPanelLibrary.TabIndex = 4;
            // 
            // listViewLibrary
            // 
            listViewLibrary.AllowColumnReorder = true;
            listViewLibrary.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderName, columnHeaderOwner, columnHeaderNote, columnHeaderServer, columnHeaderSex, columnHeaderDomesticated, columnHeaderTopness, columnHeaderTopStatsNr, columnHeaderGen, columnHeaderFound, columnHeaderMutations, columnHeaderCooldown, columnHeaderHP, columnHeaderSt, columnHeaderTo, columnHeaderOx, columnHeaderFo, columnHeaderWa, columnHeaderTm, columnHeaderWe, columnHeaderDm, columnHeaderSp, columnHeaderFr, columnHeaderCr, columnHeaderHPM, columnHeaderStM, columnHeaderToM, columnHeaderOxM, columnHeaderFoM, columnHeaderWaM, columnHeaderTmM, columnHeaderWeM, columnHeaderDmM, columnHeaderSpM, columnHeaderFrM, columnHeaderCrM, columnHeaderColor0, columnHeaderColor1, columnHeaderColor2, columnHeaderColor3, columnHeaderColor4, columnHeaderColor5, columnHeaderSpecies, columnHeaderStatus, columnHeaderTribe, columnHeaderStatusIcon, columnHeaderMutagen, columnHeaderCurrentLevel, columnHeaderMaxPossibleLevel, columnHeaderTraits });
            listViewLibrary.ContextMenuStrip = contextMenuStripLibrary;
            listViewLibrary.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewLibrary.FullRowSelect = true;
            listViewLibrary.Location = new System.Drawing.Point(239, 3);
            listViewLibrary.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listViewLibrary.Name = "listViewLibrary";
            listViewLibrary.ShowItemToolTips = true;
            listViewLibrary.Size = new System.Drawing.Size(1, 60);
            listViewLibrary.TabIndex = 2;
            listViewLibrary.UseCompatibleStateImageBehavior = false;
            listViewLibrary.View = System.Windows.Forms.View.Details;
            listViewLibrary.ColumnClick += libraryListView_ColumnClick;
            listViewLibrary.SelectedIndexChanged += listViewLibrary_SelectedIndexChanged;
            listViewLibrary.KeyDown += listViewLibrary_KeyDown;
            listViewLibrary.KeyUp += listViewLibrary_KeyUp;
            // 
            // columnHeaderName
            // 
            columnHeaderName.DisplayIndex = 1;
            columnHeaderName.Text = "Name";
            columnHeaderName.Width = 97;
            // 
            // columnHeaderOwner
            // 
            columnHeaderOwner.DisplayIndex = 2;
            columnHeaderOwner.Text = "Owner";
            columnHeaderOwner.Width = 48;
            // 
            // columnHeaderNote
            // 
            columnHeaderNote.DisplayIndex = 3;
            columnHeaderNote.Text = "Notes";
            columnHeaderNote.Width = 48;
            // 
            // columnHeaderServer
            // 
            columnHeaderServer.DisplayIndex = 4;
            columnHeaderServer.Text = "Server";
            // 
            // columnHeaderSex
            // 
            columnHeaderSex.DisplayIndex = 5;
            columnHeaderSex.Text = "S";
            columnHeaderSex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            columnHeaderSex.Width = 22;
            // 
            // columnHeaderDomesticated
            // 
            columnHeaderDomesticated.DisplayIndex = 36;
            columnHeaderDomesticated.Text = "Domesticated";
            // 
            // columnHeaderTopness
            // 
            columnHeaderTopness.DisplayIndex = 30;
            columnHeaderTopness.Text = "Tp%";
            columnHeaderTopness.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderTopness.Width = 33;
            // 
            // columnHeaderTopStatsNr
            // 
            columnHeaderTopStatsNr.DisplayIndex = 31;
            columnHeaderTopStatsNr.Text = "Top";
            columnHeaderTopStatsNr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderTopStatsNr.Width = 31;
            // 
            // columnHeaderGen
            // 
            columnHeaderGen.DisplayIndex = 32;
            columnHeaderGen.Text = "Gen";
            columnHeaderGen.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            columnHeaderGen.Width = 34;
            // 
            // columnHeaderFound
            // 
            columnHeaderFound.DisplayIndex = 33;
            columnHeaderFound.Text = "LW";
            columnHeaderFound.Width = 30;
            // 
            // columnHeaderMutations
            // 
            columnHeaderMutations.DisplayIndex = 34;
            columnHeaderMutations.Text = "Mu";
            columnHeaderMutations.Width = 30;
            // 
            // columnHeaderCooldown
            // 
            columnHeaderCooldown.DisplayIndex = 37;
            columnHeaderCooldown.Text = "Cooldown/Growing";
            // 
            // columnHeaderHP
            // 
            columnHeaderHP.DisplayIndex = 6;
            columnHeaderHP.Text = "HP";
            columnHeaderHP.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderHP.Width = 30;
            // 
            // columnHeaderSt
            // 
            columnHeaderSt.DisplayIndex = 8;
            columnHeaderSt.Text = "St";
            columnHeaderSt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderSt.Width = 30;
            // 
            // columnHeaderTo
            // 
            columnHeaderTo.DisplayIndex = 28;
            columnHeaderTo.Text = "To";
            columnHeaderTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderTo.Width = 30;
            // 
            // columnHeaderOx
            // 
            columnHeaderOx.DisplayIndex = 10;
            columnHeaderOx.Text = "Ox";
            columnHeaderOx.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderOx.Width = 30;
            // 
            // columnHeaderFo
            // 
            columnHeaderFo.DisplayIndex = 12;
            columnHeaderFo.Text = "Fo";
            columnHeaderFo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderFo.Width = 30;
            // 
            // columnHeaderWa
            // 
            columnHeaderWa.DisplayIndex = 14;
            columnHeaderWa.Text = "Wa";
            columnHeaderWa.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderWa.Width = 30;
            // 
            // columnHeaderTm
            // 
            columnHeaderTm.DisplayIndex = 16;
            columnHeaderTm.Text = "Te";
            columnHeaderTm.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderTm.Width = 30;
            // 
            // columnHeaderWe
            // 
            columnHeaderWe.DisplayIndex = 18;
            columnHeaderWe.Text = "We";
            columnHeaderWe.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderWe.Width = 30;
            // 
            // columnHeaderDm
            // 
            columnHeaderDm.Text = "Dm";
            columnHeaderDm.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderDm.Width = 30;
            // 
            // columnHeaderSp
            // 
            columnHeaderSp.DisplayIndex = 22;
            columnHeaderSp.Text = "Sp";
            columnHeaderSp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderSp.Width = 30;
            // 
            // columnHeaderFr
            // 
            columnHeaderFr.DisplayIndex = 24;
            columnHeaderFr.Text = "Fr";
            columnHeaderFr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderFr.Width = 30;
            // 
            // columnHeaderCr
            // 
            columnHeaderCr.DisplayIndex = 26;
            columnHeaderCr.Text = "Cr";
            columnHeaderCr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderCr.Width = 30;
            // 
            // columnHeaderHPM
            // 
            columnHeaderHPM.DisplayIndex = 7;
            columnHeaderHPM.Text = "HPM";
            columnHeaderHPM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderHPM.Width = 35;
            // 
            // columnHeaderStM
            // 
            columnHeaderStM.DisplayIndex = 9;
            columnHeaderStM.Text = "StM";
            columnHeaderStM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderStM.Width = 35;
            // 
            // columnHeaderToM
            // 
            columnHeaderToM.DisplayIndex = 29;
            columnHeaderToM.Text = "ToM";
            columnHeaderToM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderToM.Width = 35;
            // 
            // columnHeaderOxM
            // 
            columnHeaderOxM.DisplayIndex = 11;
            columnHeaderOxM.Text = "OxM";
            columnHeaderOxM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderOxM.Width = 35;
            // 
            // columnHeaderFoM
            // 
            columnHeaderFoM.DisplayIndex = 13;
            columnHeaderFoM.Text = "FoM";
            columnHeaderFoM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderFoM.Width = 35;
            // 
            // columnHeaderWaM
            // 
            columnHeaderWaM.DisplayIndex = 15;
            columnHeaderWaM.Text = "WaM";
            columnHeaderWaM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderWaM.Width = 0;
            // 
            // columnHeaderTmM
            // 
            columnHeaderTmM.DisplayIndex = 17;
            columnHeaderTmM.Text = "TeM";
            columnHeaderTmM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderTmM.Width = 0;
            // 
            // columnHeaderWeM
            // 
            columnHeaderWeM.DisplayIndex = 19;
            columnHeaderWeM.Text = "WeM";
            columnHeaderWeM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderWeM.Width = 35;
            // 
            // columnHeaderDmM
            // 
            columnHeaderDmM.DisplayIndex = 21;
            columnHeaderDmM.Text = "DmM";
            columnHeaderDmM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderDmM.Width = 35;
            // 
            // columnHeaderSpM
            // 
            columnHeaderSpM.DisplayIndex = 23;
            columnHeaderSpM.Text = "SpM";
            columnHeaderSpM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderSpM.Width = 35;
            // 
            // columnHeaderFrM
            // 
            columnHeaderFrM.DisplayIndex = 25;
            columnHeaderFrM.Text = "FrM";
            columnHeaderFrM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderFrM.Width = 0;
            // 
            // columnHeaderCrM
            // 
            columnHeaderCrM.DisplayIndex = 27;
            columnHeaderCrM.Text = "CrM";
            columnHeaderCrM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderCrM.Width = 0;
            // 
            // columnHeaderColor0
            // 
            columnHeaderColor0.DisplayIndex = 38;
            columnHeaderColor0.Text = "C0";
            columnHeaderColor0.Width = 25;
            // 
            // columnHeaderColor1
            // 
            columnHeaderColor1.DisplayIndex = 39;
            columnHeaderColor1.Text = "C1";
            columnHeaderColor1.Width = 25;
            // 
            // columnHeaderColor2
            // 
            columnHeaderColor2.DisplayIndex = 40;
            columnHeaderColor2.Text = "C2";
            columnHeaderColor2.Width = 25;
            // 
            // columnHeaderColor3
            // 
            columnHeaderColor3.DisplayIndex = 41;
            columnHeaderColor3.Text = "C3";
            columnHeaderColor3.Width = 25;
            // 
            // columnHeaderColor4
            // 
            columnHeaderColor4.DisplayIndex = 42;
            columnHeaderColor4.Text = "C4";
            columnHeaderColor4.Width = 25;
            // 
            // columnHeaderColor5
            // 
            columnHeaderColor5.DisplayIndex = 43;
            columnHeaderColor5.Text = "C5";
            columnHeaderColor5.Width = 25;
            // 
            // columnHeaderSpecies
            // 
            columnHeaderSpecies.DisplayIndex = 44;
            columnHeaderSpecies.Text = "Species";
            // 
            // columnHeaderStatus
            // 
            columnHeaderStatus.DisplayIndex = 0;
            columnHeaderStatus.Text = "Status";
            // 
            // columnHeaderTribe
            // 
            columnHeaderTribe.DisplayIndex = 45;
            columnHeaderTribe.Text = "Tribe";
            // 
            // columnHeaderStatusIcon
            // 
            columnHeaderStatusIcon.DisplayIndex = 46;
            columnHeaderStatusIcon.Text = "Status";
            columnHeaderStatusIcon.Width = 35;
            // 
            // columnHeaderMutagen
            // 
            columnHeaderMutagen.DisplayIndex = 35;
            columnHeaderMutagen.Text = "Mutagen";
            columnHeaderMutagen.Width = 30;
            // 
            // columnHeaderCurrentLevel
            // 
            columnHeaderCurrentLevel.Text = "Lvl";
            columnHeaderCurrentLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderCurrentLevel.Width = 40;
            // 
            // columnHeaderMaxPossibleLevel
            // 
            columnHeaderMaxPossibleLevel.Text = "max Lvl";
            columnHeaderMaxPossibleLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            columnHeaderMaxPossibleLevel.Width = 40;
            // 
            // columnHeaderTraits
            // 
            columnHeaderTraits.Text = "Traits";
            // 
            // contextMenuStripLibrary
            // 
            contextMenuStripLibrary.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItemEdit, editAllSelectedToolStripMenuItem, toolStripSeparator17, toolStripMenuItemGenerateCreatureName, toolStripMenuItemCopyGeneratedCreatureName, toolStripMenuItemCopyCreatureName, toolStripSeparator9, copyValuesToExtractorToolStripMenuItem, exportToClipboardToolStripMenuItem1, copyInfographicToClipboardToolStripMenuItem, saveInfographicsToFolderToolStripMenuItem, viewColorsInLibraryInfoToolStripMenuItem, toolStripSeparator22, SetMaturityCooldownToolStripMenuItem, bestBreedingPartnersToolStripMenuItem, breedingPlanForSelectedCreaturesToolStripMenuItem, toolStripMenuItemStatus, applyMutagenToolStripMenuItem, editTraitsToolStripMenuItem, toolStripSeparator16, adminCommandToSetColorsToolStripMenuItem, adminCommandToSpawnExactDinoToolStripMenuItem, adminCommandToSpawnExactDinoDS2ToolStripMenuItem, adminCommandSetMutationLevelsToolStripMenuItem, fixColorsToolStripMenuItem, toolStripSeparator6, toolStripMenuItemOpenWiki, toolStripSeparator14, toolStripMenuItemRemove });
            contextMenuStripLibrary.Name = "contextMenuStripLibrary";
            contextMenuStripLibrary.Size = new System.Drawing.Size(302, 546);
            contextMenuStripLibrary.Opening += contextMenuStripLibrary_Opening;
            // 
            // toolStripMenuItemEdit
            // 
            toolStripMenuItemEdit.Name = "toolStripMenuItemEdit";
            toolStripMenuItemEdit.ShortcutKeys = System.Windows.Forms.Keys.F2;
            toolStripMenuItemEdit.Size = new System.Drawing.Size(301, 22);
            toolStripMenuItemEdit.Text = "Edit";
            toolStripMenuItemEdit.Click += toolStripMenuItemEdit_Click;
            // 
            // editAllSelectedToolStripMenuItem
            // 
            editAllSelectedToolStripMenuItem.Name = "editAllSelectedToolStripMenuItem";
            editAllSelectedToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            editAllSelectedToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            editAllSelectedToolStripMenuItem.Text = "Edit all Selected...";
            editAllSelectedToolStripMenuItem.Click += editAllSelectedToolStripMenuItem_Click;
            // 
            // toolStripSeparator17
            // 
            toolStripSeparator17.Name = "toolStripSeparator17";
            toolStripSeparator17.Size = new System.Drawing.Size(298, 6);
            // 
            // toolStripMenuItemGenerateCreatureName
            // 
            toolStripMenuItemGenerateCreatureName.Name = "toolStripMenuItemGenerateCreatureName";
            toolStripMenuItemGenerateCreatureName.Size = new System.Drawing.Size(301, 22);
            toolStripMenuItemGenerateCreatureName.Text = "Apply Name Pattern";
            toolStripMenuItemGenerateCreatureName.ToolTipText = "Applies the naming pattern on the selected creatures";
            // 
            // toolStripMenuItemCopyGeneratedCreatureName
            // 
            toolStripMenuItemCopyGeneratedCreatureName.Name = "toolStripMenuItemCopyGeneratedCreatureName";
            toolStripMenuItemCopyGeneratedCreatureName.Size = new System.Drawing.Size(301, 22);
            toolStripMenuItemCopyGeneratedCreatureName.Text = "Copy generated name to clipboard";
            toolStripMenuItemCopyGeneratedCreatureName.ToolTipText = "Generates a name and copies it to the clipboard";
            // 
            // toolStripMenuItemCopyCreatureName
            // 
            toolStripMenuItemCopyCreatureName.Name = "toolStripMenuItemCopyCreatureName";
            toolStripMenuItemCopyCreatureName.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B;
            toolStripMenuItemCopyCreatureName.Size = new System.Drawing.Size(301, 22);
            toolStripMenuItemCopyCreatureName.Text = "Copy creature Name";
            toolStripMenuItemCopyCreatureName.Click += toolStripMenuItemCopyCreatureName_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new System.Drawing.Size(298, 6);
            // 
            // copyValuesToExtractorToolStripMenuItem
            // 
            copyValuesToExtractorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { currentValuesToolStripMenuItem, wildValuesToolStripMenuItem });
            copyValuesToExtractorToolStripMenuItem.Name = "copyValuesToExtractorToolStripMenuItem";
            copyValuesToExtractorToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            copyValuesToExtractorToolStripMenuItem.Text = "Copy Values to Extractor";
            // 
            // currentValuesToolStripMenuItem
            // 
            currentValuesToolStripMenuItem.Name = "currentValuesToolStripMenuItem";
            currentValuesToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            currentValuesToolStripMenuItem.Text = "Current Values";
            currentValuesToolStripMenuItem.Click += currentValuesToolStripMenuItem_Click;
            // 
            // wildValuesToolStripMenuItem
            // 
            wildValuesToolStripMenuItem.Name = "wildValuesToolStripMenuItem";
            wildValuesToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            wildValuesToolStripMenuItem.Text = "Only Wild Values";
            wildValuesToolStripMenuItem.Click += wildValuesToolStripMenuItem_Click;
            // 
            // exportToClipboardToolStripMenuItem1
            // 
            exportToClipboardToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { plainTextcurrentValuesToolStripMenuItem1, plainTextbreedingValuesToolStripMenuItem1, forSpreadsheetToolStripMenuItem1 });
            exportToClipboardToolStripMenuItem1.Name = "exportToClipboardToolStripMenuItem1";
            exportToClipboardToolStripMenuItem1.Size = new System.Drawing.Size(301, 22);
            exportToClipboardToolStripMenuItem1.Text = "Export to Clipboard";
            // 
            // plainTextcurrentValuesToolStripMenuItem1
            // 
            plainTextcurrentValuesToolStripMenuItem1.Name = "plainTextcurrentValuesToolStripMenuItem1";
            plainTextcurrentValuesToolStripMenuItem1.Size = new System.Drawing.Size(218, 22);
            plainTextcurrentValuesToolStripMenuItem1.Text = "Plain Text (current values)";
            plainTextcurrentValuesToolStripMenuItem1.Click += plainTextcurrentValuesToolStripMenuItem1_Click;
            // 
            // plainTextbreedingValuesToolStripMenuItem1
            // 
            plainTextbreedingValuesToolStripMenuItem1.Name = "plainTextbreedingValuesToolStripMenuItem1";
            plainTextbreedingValuesToolStripMenuItem1.Size = new System.Drawing.Size(218, 22);
            plainTextbreedingValuesToolStripMenuItem1.Text = "Plain Text (breeding values)";
            plainTextbreedingValuesToolStripMenuItem1.Click += plainTextbreedingValuesToolStripMenuItem1_Click;
            // 
            // forSpreadsheetToolStripMenuItem1
            // 
            forSpreadsheetToolStripMenuItem1.Name = "forSpreadsheetToolStripMenuItem1";
            forSpreadsheetToolStripMenuItem1.Size = new System.Drawing.Size(218, 22);
            forSpreadsheetToolStripMenuItem1.Text = "for Spreadsheet";
            forSpreadsheetToolStripMenuItem1.Click += forSpreadsheetToolStripMenuItem_Click;
            // 
            // copyInfographicToClipboardToolStripMenuItem
            // 
            copyInfographicToClipboardToolStripMenuItem.Name = "copyInfographicToClipboardToolStripMenuItem";
            copyInfographicToClipboardToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            copyInfographicToClipboardToolStripMenuItem.Text = "Copy Infographic to Clipboard";
            copyInfographicToClipboardToolStripMenuItem.Click += copyInfographicToClipboardToolStripMenuItem_Click;
            // 
            // saveInfographicsToFolderToolStripMenuItem
            // 
            saveInfographicsToFolderToolStripMenuItem.Name = "saveInfographicsToFolderToolStripMenuItem";
            saveInfographicsToFolderToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            saveInfographicsToFolderToolStripMenuItem.Text = "Save Infographics to folder…";
            saveInfographicsToFolderToolStripMenuItem.Click += saveInfographicsToFolderToolStripMenuItem_Click;
            // 
            // viewColorsInLibraryInfoToolStripMenuItem
            // 
            viewColorsInLibraryInfoToolStripMenuItem.Name = "viewColorsInLibraryInfoToolStripMenuItem";
            viewColorsInLibraryInfoToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            viewColorsInLibraryInfoToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            viewColorsInLibraryInfoToolStripMenuItem.Text = "View colors in Library Info";
            viewColorsInLibraryInfoToolStripMenuItem.Click += viewColorsInLibraryInfoToolStripMenuItem_Click;
            // 
            // toolStripSeparator22
            // 
            toolStripSeparator22.Name = "toolStripSeparator22";
            toolStripSeparator22.Size = new System.Drawing.Size(298, 6);
            // 
            // SetMaturityCooldownToolStripMenuItem
            // 
            SetMaturityCooldownToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { clearMatingCooldownToolStripMenuItem, justMatedToolStripMenuItem, maturationSeparator });
            SetMaturityCooldownToolStripMenuItem.Name = "SetMaturityCooldownToolStripMenuItem";
            SetMaturityCooldownToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            SetMaturityCooldownToolStripMenuItem.Text = "Set cooldown / maturity";
            // 
            // clearMatingCooldownToolStripMenuItem
            // 
            clearMatingCooldownToolStripMenuItem.Name = "clearMatingCooldownToolStripMenuItem";
            clearMatingCooldownToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            clearMatingCooldownToolStripMenuItem.Text = "Clear mating cooldown";
            clearMatingCooldownToolStripMenuItem.Click += clearMatingCooldownToolStripMenuItem_Click;
            // 
            // justMatedToolStripMenuItem
            // 
            justMatedToolStripMenuItem.Name = "justMatedToolStripMenuItem";
            justMatedToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            justMatedToolStripMenuItem.Text = "Just mated";
            justMatedToolStripMenuItem.Click += justMatedToolStripMenuItem_Click;
            // 
            // maturationSeparator
            // 
            maturationSeparator.Name = "maturationSeparator";
            maturationSeparator.Size = new System.Drawing.Size(195, 6);
            // 
            // bestBreedingPartnersToolStripMenuItem
            // 
            bestBreedingPartnersToolStripMenuItem.Name = "bestBreedingPartnersToolStripMenuItem";
            bestBreedingPartnersToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            bestBreedingPartnersToolStripMenuItem.Text = "Best Breeding Partners...";
            bestBreedingPartnersToolStripMenuItem.Click += bestBreedingPartnersToolStripMenuItem_Click;
            // 
            // breedingPlanForSelectedCreaturesToolStripMenuItem
            // 
            breedingPlanForSelectedCreaturesToolStripMenuItem.Name = "breedingPlanForSelectedCreaturesToolStripMenuItem";
            breedingPlanForSelectedCreaturesToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            breedingPlanForSelectedCreaturesToolStripMenuItem.Text = "Breeding Plan for selected Creatures…";
            breedingPlanForSelectedCreaturesToolStripMenuItem.Click += breedingPlanForSelectedCreaturesToolStripMenuItem_Click;
            // 
            // toolStripMenuItemStatus
            // 
            toolStripMenuItemStatus.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItem2, toolStripMenuItem3, toolStripMenuItem4, obeliskToolStripMenuItem, cryopodToolStripMenuItem });
            toolStripMenuItemStatus.Name = "toolStripMenuItemStatus";
            toolStripMenuItemStatus.Size = new System.Drawing.Size(301, 22);
            toolStripMenuItemStatus.Text = "Set Status";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R;
            toolStripMenuItem2.Size = new System.Drawing.Size(177, 22);
            toolStripMenuItem2.Text = "Available";
            toolStripMenuItem2.Click += toolStripMenuItem2_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U;
            toolStripMenuItem3.Size = new System.Drawing.Size(177, 22);
            toolStripMenuItem3.Text = "Unavailable";
            toolStripMenuItem3.Click += toolStripMenuItem3_Click;
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D;
            toolStripMenuItem4.Size = new System.Drawing.Size(177, 22);
            toolStripMenuItem4.Text = "Dead";
            toolStripMenuItem4.Click += toolStripMenuItem4_Click;
            // 
            // obeliskToolStripMenuItem
            // 
            obeliskToolStripMenuItem.Name = "obeliskToolStripMenuItem";
            obeliskToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            obeliskToolStripMenuItem.Text = "Obelisk";
            obeliskToolStripMenuItem.Click += obeliskToolStripMenuItem_Click;
            // 
            // cryopodToolStripMenuItem
            // 
            cryopodToolStripMenuItem.Name = "cryopodToolStripMenuItem";
            cryopodToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y;
            cryopodToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            cryopodToolStripMenuItem.Text = "Cryopod";
            cryopodToolStripMenuItem.Click += cryopodToolStripMenuItem_Click;
            // 
            // applyMutagenToolStripMenuItem
            // 
            applyMutagenToolStripMenuItem.Name = "applyMutagenToolStripMenuItem";
            applyMutagenToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            applyMutagenToolStripMenuItem.Text = "Apply Mutagen";
            applyMutagenToolStripMenuItem.Click += applyMutagenToolStripMenuItem_Click;
            // 
            // editTraitsToolStripMenuItem
            // 
            editTraitsToolStripMenuItem.Name = "editTraitsToolStripMenuItem";
            editTraitsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            editTraitsToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            editTraitsToolStripMenuItem.Text = "Edit Traits…";
            editTraitsToolStripMenuItem.Click += editTraitsToolStripMenuItem_Click;
            // 
            // toolStripSeparator16
            // 
            toolStripSeparator16.Name = "toolStripSeparator16";
            toolStripSeparator16.Size = new System.Drawing.Size(298, 6);
            // 
            // adminCommandToSetColorsToolStripMenuItem
            // 
            adminCommandToSetColorsToolStripMenuItem.Name = "adminCommandToSetColorsToolStripMenuItem";
            adminCommandToSetColorsToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            adminCommandToSetColorsToolStripMenuItem.Text = "Admin Command to set Colors";
            adminCommandToSetColorsToolStripMenuItem.Click += adminCommandToSetColorsToolStripMenuItem_Click;
            // 
            // adminCommandToSpawnExactDinoToolStripMenuItem
            // 
            adminCommandToSpawnExactDinoToolStripMenuItem.Name = "adminCommandToSpawnExactDinoToolStripMenuItem";
            adminCommandToSpawnExactDinoToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            adminCommandToSpawnExactDinoToolStripMenuItem.Text = "Admin Command to spawn exact dino";
            adminCommandToSpawnExactDinoToolStripMenuItem.Click += adminCommandToSpawnExactDinoToolStripMenuItem_Click;
            // 
            // adminCommandToSpawnExactDinoDS2ToolStripMenuItem
            // 
            adminCommandToSpawnExactDinoDS2ToolStripMenuItem.Name = "adminCommandToSpawnExactDinoDS2ToolStripMenuItem";
            adminCommandToSpawnExactDinoDS2ToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            adminCommandToSpawnExactDinoDS2ToolStripMenuItem.Text = "Admin Command to spawn exact dino DS2";
            adminCommandToSpawnExactDinoDS2ToolStripMenuItem.Click += adminCommandToSpawnExactDinoDS2ToolStripMenuItem_Click;
            // 
            // adminCommandSetMutationLevelsToolStripMenuItem
            // 
            adminCommandSetMutationLevelsToolStripMenuItem.Name = "adminCommandSetMutationLevelsToolStripMenuItem";
            adminCommandSetMutationLevelsToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            adminCommandSetMutationLevelsToolStripMenuItem.Text = "Admin Command set mutation levels";
            adminCommandSetMutationLevelsToolStripMenuItem.Click += adminCommandSetMutationLevelsToolStripMenuItem_Click;
            // 
            // fixColorsToolStripMenuItem
            // 
            fixColorsToolStripMenuItem.Name = "fixColorsToolStripMenuItem";
            fixColorsToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            fixColorsToolStripMenuItem.Text = "Fix colors";
            fixColorsToolStripMenuItem.ToolTipText = resources.GetString("fixColorsToolStripMenuItem.ToolTipText");
            fixColorsToolStripMenuItem.Click += fixColorsToolStripMenuItem_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(298, 6);
            // 
            // toolStripMenuItemOpenWiki
            // 
            toolStripMenuItemOpenWiki.Name = "toolStripMenuItemOpenWiki";
            toolStripMenuItemOpenWiki.Size = new System.Drawing.Size(301, 22);
            toolStripMenuItemOpenWiki.Text = "Open Wiki-page in Browser";
            toolStripMenuItemOpenWiki.Click += ToolStripMenuItemOpenWiki_Click;
            // 
            // toolStripSeparator14
            // 
            toolStripSeparator14.Name = "toolStripSeparator14";
            toolStripSeparator14.Size = new System.Drawing.Size(298, 6);
            // 
            // toolStripMenuItemRemove
            // 
            toolStripMenuItemRemove.Name = "toolStripMenuItemRemove";
            toolStripMenuItemRemove.Size = new System.Drawing.Size(301, 22);
            toolStripMenuItemRemove.Text = "Delete creature...";
            toolStripMenuItemRemove.Click += toolStripMenuItemRemove_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.AutoScrollMinSize = new System.Drawing.Size(0, 620);
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tabControlLibFilter, 0, 1);
            tableLayoutPanel1.Controls.Add(creatureBoxListView, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(4, 3);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(227, 60);
            tableLayoutPanel1.TabIndex = 6;
            // 
            // tabControlLibFilter
            // 
            tabControlLibFilter.Controls.Add(tabPage1);
            tabControlLibFilter.Controls.Add(tabPage3);
            tabControlLibFilter.Controls.Add(tabPageLibRadarChart);
            tabControlLibFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControlLibFilter.Location = new System.Drawing.Point(4, 477);
            tabControlLibFilter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControlLibFilter.Name = "tabControlLibFilter";
            tabControlLibFilter.SelectedIndex = 0;
            tabControlLibFilter.Size = new System.Drawing.Size(219, 140);
            tabControlLibFilter.TabIndex = 5;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(listBoxSpeciesLib);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage1.Size = new System.Drawing.Size(211, 112);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Species";
            // 
            // listBoxSpeciesLib
            // 
            listBoxSpeciesLib.Dock = System.Windows.Forms.DockStyle.Fill;
            listBoxSpeciesLib.FormattingEnabled = true;
            listBoxSpeciesLib.Location = new System.Drawing.Point(4, 3);
            listBoxSpeciesLib.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listBoxSpeciesLib.Name = "listBoxSpeciesLib";
            listBoxSpeciesLib.Size = new System.Drawing.Size(203, 106);
            listBoxSpeciesLib.TabIndex = 0;
            listBoxSpeciesLib.Click += listBoxSpeciesLib_Click;
            listBoxSpeciesLib.SelectedIndexChanged += listBoxSpeciesLib_SelectedIndexChanged;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(tableLayoutPanel2);
            tabPage3.Location = new System.Drawing.Point(4, 24);
            tabPage3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage3.Size = new System.Drawing.Size(211, 112);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Stats";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(BtRecalculateTopStatsAfterChange, 0, 2);
            tableLayoutPanel2.Controls.Add(label17, 0, 0);
            tableLayoutPanel2.Controls.Add(buttonRecalculateTops, 0, 1);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(4, 3);
            tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.Size = new System.Drawing.Size(203, 106);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // BtRecalculateTopStatsAfterChange
            // 
            BtRecalculateTopStatsAfterChange.Dock = System.Windows.Forms.DockStyle.Top;
            BtRecalculateTopStatsAfterChange.Location = new System.Drawing.Point(4, 66);
            BtRecalculateTopStatsAfterChange.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtRecalculateTopStatsAfterChange.Name = "BtRecalculateTopStatsAfterChange";
            BtRecalculateTopStatsAfterChange.Size = new System.Drawing.Size(195, 60);
            BtRecalculateTopStatsAfterChange.TabIndex = 5;
            BtRecalculateTopStatsAfterChange.Text = "Recalculate top stats after change";
            BtRecalculateTopStatsAfterChange.UseVisualStyleBackColor = true;
            BtRecalculateTopStatsAfterChange.Click += BtRecalculateTopStatsAfterChange_Click;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new System.Drawing.Point(4, 0);
            label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(187, 30);
            label17.TabIndex = 4;
            label17.Text = "Select the stats considered for the TopStat-Calculation and Coloring";
            // 
            // buttonRecalculateTops
            // 
            buttonRecalculateTops.Location = new System.Drawing.Point(4, 33);
            buttonRecalculateTops.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonRecalculateTops.Name = "buttonRecalculateTops";
            buttonRecalculateTops.Size = new System.Drawing.Size(195, 27);
            buttonRecalculateTops.TabIndex = 2;
            buttonRecalculateTops.Text = "Open stats settings";
            buttonRecalculateTops.UseVisualStyleBackColor = true;
            buttonRecalculateTops.Click += ButtonOpenTopStatsSettingsClick;
            // 
            // tabPageLibRadarChart
            // 
            tabPageLibRadarChart.Controls.Add(radarChartLibrary);
            tabPageLibRadarChart.Location = new System.Drawing.Point(4, 24);
            tabPageLibRadarChart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageLibRadarChart.Name = "tabPageLibRadarChart";
            tabPageLibRadarChart.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageLibRadarChart.Size = new System.Drawing.Size(211, 112);
            tabPageLibRadarChart.TabIndex = 4;
            tabPageLibRadarChart.Text = "Chart";
            // 
            // radarChartLibrary
            // 
            radarChartLibrary.Dock = System.Windows.Forms.DockStyle.Top;
            radarChartLibrary.Image = (System.Drawing.Image)resources.GetObject("radarChartLibrary.Image");
            radarChartLibrary.Location = new System.Drawing.Point(4, 3);
            radarChartLibrary.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            radarChartLibrary.Name = "radarChartLibrary";
            radarChartLibrary.Size = new System.Drawing.Size(203, 331);
            radarChartLibrary.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            radarChartLibrary.TabIndex = 0;
            radarChartLibrary.TabStop = false;
            // 
            // creatureBoxListView
            // 
            creatureBoxListView.Location = new System.Drawing.Point(5, 3);
            creatureBoxListView.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            creatureBoxListView.Name = "creatureBoxListView";
            creatureBoxListView.Size = new System.Drawing.Size(217, 468);
            creatureBoxListView.TabIndex = 0;
            creatureBoxListView.Changed += UpdateDisplayedCreatureValues;
            creatureBoxListView.GiveParents += CreatureBoxListView_FindParents;
            creatureBoxListView.SelectCreature += SelectCreatureInLibrary;
            // 
            // tabPageLibraryInfo
            // 
            tabPageLibraryInfo.Controls.Add(tlpLibraryInfo);
            tabPageLibraryInfo.Location = new System.Drawing.Point(4, 24);
            tabPageLibraryInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageLibraryInfo.Name = "tabPageLibraryInfo";
            tabPageLibraryInfo.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageLibraryInfo.Size = new System.Drawing.Size(192, 72);
            tabPageLibraryInfo.TabIndex = 14;
            tabPageLibraryInfo.Text = "Library Info";
            // 
            // tlpLibraryInfo
            // 
            tlpLibraryInfo.AutoScroll = true;
            tlpLibraryInfo.ColumnCount = 1;
            tlpLibraryInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tlpLibraryInfo.Controls.Add(tableLayoutPanel3, 0, 0);
            tlpLibraryInfo.Controls.Add(libraryInfoControl1, 0, 1);
            tlpLibraryInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            tlpLibraryInfo.Location = new System.Drawing.Point(4, 3);
            tlpLibraryInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tlpLibraryInfo.Name = "tlpLibraryInfo";
            tlpLibraryInfo.RowCount = 2;
            tlpLibraryInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tlpLibraryInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tlpLibraryInfo.Size = new System.Drawing.Size(184, 66);
            tlpLibraryInfo.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel3.Controls.Add(CbLibraryInfoUseFilter, 1, 0);
            tableLayoutPanel3.Controls.Add(BtCopyLibraryColorToClipboard, 0, 0);
            tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel3.Location = new System.Drawing.Point(4, 3);
            tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.Size = new System.Drawing.Size(176, 35);
            tableLayoutPanel3.TabIndex = 2;
            // 
            // CbLibraryInfoUseFilter
            // 
            CbLibraryInfoUseFilter.AutoSize = true;
            CbLibraryInfoUseFilter.Location = new System.Drawing.Point(262, 3);
            CbLibraryInfoUseFilter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CbLibraryInfoUseFilter.Name = "CbLibraryInfoUseFilter";
            CbLibraryInfoUseFilter.Size = new System.Drawing.Size(108, 19);
            CbLibraryInfoUseFilter.TabIndex = 1;
            CbLibraryInfoUseFilter.Text = "Use library filter";
            CbLibraryInfoUseFilter.UseVisualStyleBackColor = true;
            CbLibraryInfoUseFilter.CheckedChanged += CbLibraryInfoUseFilter_CheckedChanged;
            // 
            // BtCopyLibraryColorToClipboard
            // 
            BtCopyLibraryColorToClipboard.Location = new System.Drawing.Point(4, 3);
            BtCopyLibraryColorToClipboard.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtCopyLibraryColorToClipboard.Name = "BtCopyLibraryColorToClipboard";
            BtCopyLibraryColorToClipboard.Size = new System.Drawing.Size(250, 27);
            BtCopyLibraryColorToClipboard.TabIndex = 0;
            BtCopyLibraryColorToClipboard.Text = "Copy this text to the clipboard";
            BtCopyLibraryColorToClipboard.UseVisualStyleBackColor = true;
            BtCopyLibraryColorToClipboard.Click += BtCopyLibraryColorToClipboard_Click;
            // 
            // libraryInfoControl1
            // 
            libraryInfoControl1.AutoScroll = true;
            libraryInfoControl1.ColumnCount = 4;
            libraryInfoControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            libraryInfoControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            libraryInfoControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            libraryInfoControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            libraryInfoControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            libraryInfoControl1.Location = new System.Drawing.Point(4, 44);
            libraryInfoControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            libraryInfoControl1.Name = "libraryInfoControl1";
            libraryInfoControl1.RowCount = 2;
            libraryInfoControl1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            libraryInfoControl1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            libraryInfoControl1.Size = new System.Drawing.Size(176, 19);
            libraryInfoControl1.TabIndex = 3;
            // 
            // tabPagePedigree
            // 
            tabPagePedigree.Controls.Add(pedigree1);
            tabPagePedigree.Location = new System.Drawing.Point(4, 24);
            tabPagePedigree.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPagePedigree.Name = "tabPagePedigree";
            tabPagePedigree.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPagePedigree.Size = new System.Drawing.Size(192, 72);
            tabPagePedigree.TabIndex = 3;
            tabPagePedigree.Text = "Pedigree";
            // 
            // pedigree1
            // 
            pedigree1.AutoScroll = true;
            pedigree1.Dock = System.Windows.Forms.DockStyle.Fill;
            pedigree1.Location = new System.Drawing.Point(4, 3);
            pedigree1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            pedigree1.Name = "pedigree1";
            pedigree1.Size = new System.Drawing.Size(184, 66);
            pedigree1.TabIndex = 0;
            // 
            // tabPageTaming
            // 
            tabPageTaming.Controls.Add(tamingControl1);
            tabPageTaming.Location = new System.Drawing.Point(4, 24);
            tabPageTaming.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageTaming.Name = "tabPageTaming";
            tabPageTaming.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageTaming.Size = new System.Drawing.Size(192, 72);
            tabPageTaming.TabIndex = 8;
            tabPageTaming.Text = "Taming";
            // 
            // tamingControl1
            // 
            tamingControl1.AutoScroll = true;
            tamingControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tamingControl1.Location = new System.Drawing.Point(4, 3);
            tamingControl1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            tamingControl1.Name = "tamingControl1";
            tamingControl1.Size = new System.Drawing.Size(184, 66);
            tamingControl1.TabIndex = 0;
            // 
            // tabPageBreedingPlan
            // 
            tabPageBreedingPlan.Controls.Add(breedingPlan1);
            tabPageBreedingPlan.Location = new System.Drawing.Point(4, 24);
            tabPageBreedingPlan.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageBreedingPlan.Name = "tabPageBreedingPlan";
            tabPageBreedingPlan.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageBreedingPlan.Size = new System.Drawing.Size(192, 72);
            tabPageBreedingPlan.TabIndex = 4;
            tabPageBreedingPlan.Text = "Breeding Plan";
            // 
            // breedingPlan1
            // 
            breedingPlan1.AutoScroll = true;
            breedingPlan1.Dock = System.Windows.Forms.DockStyle.Fill;
            breedingPlan1.Location = new System.Drawing.Point(4, 3);
            breedingPlan1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            breedingPlan1.Name = "breedingPlan1";
            breedingPlan1.Size = new System.Drawing.Size(184, 66);
            breedingPlan1.TabIndex = 0;
            // 
            // tabPageCurrentBreeds
            // 
            tabPageCurrentBreeds.Controls.Add(currentBreeds1);
            tabPageCurrentBreeds.Controls.Add(hatching1);
            tabPageCurrentBreeds.Location = new System.Drawing.Point(4, 24);
            tabPageCurrentBreeds.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageCurrentBreeds.Name = "tabPageCurrentBreeds";
            tabPageCurrentBreeds.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageCurrentBreeds.Size = new System.Drawing.Size(2183, 918);
            tabPageCurrentBreeds.TabIndex = 13;
            tabPageCurrentBreeds.Text = "Current Breeds";
            // 
            // currentBreeds1
            // 
            currentBreeds1.Dock = System.Windows.Forms.DockStyle.Left;
            currentBreeds1.Location = new System.Drawing.Point(4, 3);
            currentBreeds1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            currentBreeds1.Name = "currentBreeds1";
            currentBreeds1.Size = new System.Drawing.Size(929, 912);
            currentBreeds1.TabIndex = 1;
            // 
            // hatching1
            // 
            hatching1.Location = new System.Drawing.Point(926, 7);
            hatching1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            hatching1.Name = "hatching1";
            hatching1.Size = new System.Drawing.Size(502, 445);
            hatching1.TabIndex = 0;
            // 
            // tabPageRaising
            // 
            tabPageRaising.Controls.Add(raisingControl1);
            tabPageRaising.Location = new System.Drawing.Point(4, 24);
            tabPageRaising.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageRaising.Name = "tabPageRaising";
            tabPageRaising.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageRaising.Size = new System.Drawing.Size(192, 72);
            tabPageRaising.TabIndex = 9;
            tabPageRaising.Text = "Raising";
            // 
            // raisingControl1
            // 
            raisingControl1.AutoScroll = true;
            raisingControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            raisingControl1.Location = new System.Drawing.Point(4, 3);
            raisingControl1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            raisingControl1.Name = "raisingControl1";
            raisingControl1.Size = new System.Drawing.Size(184, 66);
            raisingControl1.TabIndex = 0;
            // 
            // tabPageTimer
            // 
            tabPageTimer.Controls.Add(timerList1);
            tabPageTimer.Location = new System.Drawing.Point(4, 24);
            tabPageTimer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageTimer.Name = "tabPageTimer";
            tabPageTimer.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageTimer.Size = new System.Drawing.Size(192, 72);
            tabPageTimer.TabIndex = 6;
            tabPageTimer.Text = "Timer";
            // 
            // timerList1
            // 
            timerList1.Dock = System.Windows.Forms.DockStyle.Fill;
            timerList1.Location = new System.Drawing.Point(4, 3);
            timerList1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            timerList1.Name = "timerList1";
            timerList1.Size = new System.Drawing.Size(184, 66);
            timerList1.TabIndex = 0;
            // 
            // tabPagePlayerTribes
            // 
            tabPagePlayerTribes.Controls.Add(tribesControl1);
            tabPagePlayerTribes.Location = new System.Drawing.Point(4, 24);
            tabPagePlayerTribes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPagePlayerTribes.Name = "tabPagePlayerTribes";
            tabPagePlayerTribes.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPagePlayerTribes.Size = new System.Drawing.Size(192, 72);
            tabPagePlayerTribes.TabIndex = 7;
            tabPagePlayerTribes.Text = "Player";
            // 
            // tribesControl1
            // 
            tribesControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tribesControl1.Location = new System.Drawing.Point(4, 3);
            tribesControl1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            tribesControl1.Name = "tribesControl1";
            tribesControl1.Size = new System.Drawing.Size(184, 66);
            tribesControl1.TabIndex = 0;
            // 
            // tabPageNotes
            // 
            tabPageNotes.Controls.Add(notesControl1);
            tabPageNotes.Location = new System.Drawing.Point(4, 24);
            tabPageNotes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageNotes.Name = "tabPageNotes";
            tabPageNotes.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageNotes.Size = new System.Drawing.Size(192, 72);
            tabPageNotes.TabIndex = 10;
            tabPageNotes.Text = "Notes";
            // 
            // notesControl1
            // 
            notesControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            notesControl1.Location = new System.Drawing.Point(4, 3);
            notesControl1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            notesControl1.Name = "notesControl1";
            notesControl1.Size = new System.Drawing.Size(184, 66);
            notesControl1.TabIndex = 0;
            // 
            // TabPageOCR
            // 
            TabPageOCR.Controls.Add(ocrControl1);
            TabPageOCR.Location = new System.Drawing.Point(4, 24);
            TabPageOCR.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TabPageOCR.Name = "TabPageOCR";
            TabPageOCR.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TabPageOCR.Size = new System.Drawing.Size(192, 72);
            TabPageOCR.TabIndex = 5;
            TabPageOCR.Text = "Experimental OCR";
            // 
            // ocrControl1
            // 
            ocrControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            ocrControl1.Location = new System.Drawing.Point(4, 3);
            ocrControl1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            ocrControl1.Name = "ocrControl1";
            ocrControl1.Size = new System.Drawing.Size(184, 66);
            ocrControl1.TabIndex = 2;
            // 
            // tabPageExtractionTests
            // 
            tabPageExtractionTests.Controls.Add(extractionTestControl1);
            tabPageExtractionTests.Location = new System.Drawing.Point(4, 24);
            tabPageExtractionTests.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageExtractionTests.Name = "tabPageExtractionTests";
            tabPageExtractionTests.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageExtractionTests.Size = new System.Drawing.Size(192, 72);
            tabPageExtractionTests.TabIndex = 11;
            tabPageExtractionTests.Text = "Extraction Tests";
            // 
            // extractionTestControl1
            // 
            extractionTestControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            extractionTestControl1.Location = new System.Drawing.Point(4, 3);
            extractionTestControl1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            extractionTestControl1.Name = "extractionTestControl1";
            extractionTestControl1.Size = new System.Drawing.Size(184, 66);
            extractionTestControl1.TabIndex = 0;
            // 
            // tabPageMultiplierTesting
            // 
            tabPageMultiplierTesting.Controls.Add(statsMultiplierTesting1);
            tabPageMultiplierTesting.Location = new System.Drawing.Point(4, 24);
            tabPageMultiplierTesting.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageMultiplierTesting.Name = "tabPageMultiplierTesting";
            tabPageMultiplierTesting.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageMultiplierTesting.Size = new System.Drawing.Size(192, 72);
            tabPageMultiplierTesting.TabIndex = 12;
            tabPageMultiplierTesting.Text = "Multiplier Testing";
            // 
            // statsMultiplierTesting1
            // 
            statsMultiplierTesting1.AllowDrop = true;
            statsMultiplierTesting1.Dock = System.Windows.Forms.DockStyle.Fill;
            statsMultiplierTesting1.Location = new System.Drawing.Point(4, 3);
            statsMultiplierTesting1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            statsMultiplierTesting1.Name = "statsMultiplierTesting1";
            statsMultiplierTesting1.Size = new System.Drawing.Size(184, 66);
            statsMultiplierTesting1.TabIndex = 0;
            // 
            // btReadValuesFromArk
            // 
            btReadValuesFromArk.Location = new System.Drawing.Point(306, 3);
            btReadValuesFromArk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btReadValuesFromArk.Name = "btReadValuesFromArk";
            btReadValuesFromArk.Size = new System.Drawing.Size(130, 52);
            btReadValuesFromArk.TabIndex = 3;
            btReadValuesFromArk.Text = "Read Values From ARK Window";
            btReadValuesFromArk.UseVisualStyleBackColor = true;
            btReadValuesFromArk.Click += btnReadValuesFromArk_Click;
            // 
            // cbEventMultipliers
            // 
            cbEventMultipliers.AutoSize = true;
            cbEventMultipliers.Location = new System.Drawing.Point(62, 33);
            cbEventMultipliers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbEventMultipliers.Name = "cbEventMultipliers";
            cbEventMultipliers.Size = new System.Drawing.Size(55, 19);
            cbEventMultipliers.TabIndex = 1;
            cbEventMultipliers.Text = "Event";
            cbEventMultipliers.UseVisualStyleBackColor = true;
            cbEventMultipliers.CheckedChanged += cbEvolutionEvent_CheckedChanged;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripProgressBar1, toolStripStatusLabel, ToolStripStatusLabelImport });
            statusStrip1.Location = new System.Drawing.Point(0, 1057);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip1.Size = new System.Drawing.Size(2191, 22);
            statusStrip1.TabIndex = 44;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new System.Drawing.Size(117, 18);
            toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new System.Drawing.Size(120, 17);
            toolStripStatusLabel.Text = "ToolStripStatusLabel1";
            // 
            // ToolStripStatusLabelImport
            // 
            ToolStripStatusLabelImport.BackColor = System.Drawing.Color.Yellow;
            ToolStripStatusLabelImport.ForeColor = System.Drawing.Color.Black;
            ToolStripStatusLabelImport.Name = "ToolStripStatusLabelImport";
            ToolStripStatusLabelImport.Size = new System.Drawing.Size(125, 17);
            ToolStripStatusLabelImport.Text = "Importing savegame…";
            ToolStripStatusLabelImport.Visible = false;
            // 
            // toolStrip2
            // 
            toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { newToolStripButton1, openToolStripButton1, saveToolStripButton1, toolStripSeparator20, TsbQuickSaveGameImport, toolStripSeparator3, toolStripButtonSettings, toolStripSeparator4, toolStripButtonCopy2Tester, toolStripButtonCopy2Extractor, toolStripButtonClear, toolStripButtonAddPlayer, toolStripButtonAddTribe, toolStripButtonDeleteExpiredIncubationTimers, toolStripSeparator8, toolStripButtonSaveCreatureValuesTemp, toolStripCBTempCreatures, toolStripButtonDeleteTempCreature, tsBtAddAsExtractionTest, copyToMultiplierTesterToolStripButton, ToolStripLabelFilter, ToolStripTextBoxLibraryFilter, ToolStripButtonLibraryFilterClear, ToolStripButtonSaveFilterPreset, TsSpOcrLabel, TsLbLabelSet, TsCbbLabelSets });
            toolStrip2.Location = new System.Drawing.Point(0, 24);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.Size = new System.Drawing.Size(2191, 25);
            toolStrip2.TabIndex = 1;
            toolStrip2.Text = "toolStrip2";
            // 
            // newToolStripButton1
            // 
            newToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            newToolStripButton1.Image = (System.Drawing.Image)resources.GetObject("newToolStripButton1.Image");
            newToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            newToolStripButton1.Name = "newToolStripButton1";
            newToolStripButton1.Size = new System.Drawing.Size(23, 22);
            newToolStripButton1.Text = "&New";
            newToolStripButton1.Click += newToolStripButton1_Click;
            // 
            // openToolStripButton1
            // 
            openToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            openToolStripButton1.Image = (System.Drawing.Image)resources.GetObject("openToolStripButton1.Image");
            openToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            openToolStripButton1.Name = "openToolStripButton1";
            openToolStripButton1.Size = new System.Drawing.Size(23, 22);
            openToolStripButton1.Text = "&Open";
            openToolStripButton1.Click += openToolStripButton1_Click;
            // 
            // saveToolStripButton1
            // 
            saveToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            saveToolStripButton1.Image = (System.Drawing.Image)resources.GetObject("saveToolStripButton1.Image");
            saveToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            saveToolStripButton1.Name = "saveToolStripButton1";
            saveToolStripButton1.Size = new System.Drawing.Size(23, 22);
            saveToolStripButton1.Text = "&Save";
            saveToolStripButton1.Click += saveToolStripButton1_Click;
            // 
            // toolStripSeparator20
            // 
            toolStripSeparator20.Name = "toolStripSeparator20";
            toolStripSeparator20.Size = new System.Drawing.Size(6, 25);
            // 
            // TsbQuickSaveGameImport
            // 
            TsbQuickSaveGameImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            TsbQuickSaveGameImport.Image = (System.Drawing.Image)resources.GetObject("TsbQuickSaveGameImport.Image");
            TsbQuickSaveGameImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            TsbQuickSaveGameImport.Name = "TsbQuickSaveGameImport";
            TsbQuickSaveGameImport.Size = new System.Drawing.Size(45, 22);
            TsbQuickSaveGameImport.Text = "ImpSg";
            TsbQuickSaveGameImport.ToolTipText = "Import Savegame";
            TsbQuickSaveGameImport.Click += TsbQuickSaveGameImport_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSettings
            // 
            toolStripButtonSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            toolStripButtonSettings.Image = Properties.Resources.settings;
            toolStripButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonSettings.Name = "toolStripButtonSettings";
            toolStripButtonSettings.Size = new System.Drawing.Size(23, 22);
            toolStripButtonSettings.Text = "Settings";
            toolStripButtonSettings.Click += toolStripButtonSettings_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonCopy2Tester
            // 
            toolStripButtonCopy2Tester.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButtonCopy2Tester.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonCopy2Tester.Image");
            toolStripButtonCopy2Tester.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonCopy2Tester.Name = "toolStripButtonCopy2Tester";
            toolStripButtonCopy2Tester.Size = new System.Drawing.Size(87, 22);
            toolStripButtonCopy2Tester.Text = "Copy to Tester";
            toolStripButtonCopy2Tester.Click += toolStripButtonCopy2Tester_Click;
            // 
            // toolStripButtonCopy2Extractor
            // 
            toolStripButtonCopy2Extractor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButtonCopy2Extractor.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonCopy2Extractor.Image");
            toolStripButtonCopy2Extractor.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonCopy2Extractor.Name = "toolStripButtonCopy2Extractor";
            toolStripButtonCopy2Extractor.Size = new System.Drawing.Size(102, 22);
            toolStripButtonCopy2Extractor.Text = "Copy to Extractor";
            toolStripButtonCopy2Extractor.Visible = false;
            toolStripButtonCopy2Extractor.Click += toolStripButtonCopy2Extractor_Click;
            // 
            // toolStripButtonClear
            // 
            toolStripButtonClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButtonClear.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonClear.Image");
            toolStripButtonClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonClear.Name = "toolStripButtonClear";
            toolStripButtonClear.Size = new System.Drawing.Size(38, 22);
            toolStripButtonClear.Text = "Clear";
            toolStripButtonClear.Click += toolStripButtonClear_Click;
            // 
            // toolStripButtonAddPlayer
            // 
            toolStripButtonAddPlayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            toolStripButtonAddPlayer.Image = Properties.Resources.newPlayer;
            toolStripButtonAddPlayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonAddPlayer.Name = "toolStripButtonAddPlayer";
            toolStripButtonAddPlayer.Size = new System.Drawing.Size(23, 22);
            toolStripButtonAddPlayer.Text = "Add Player";
            toolStripButtonAddPlayer.Visible = false;
            toolStripButtonAddPlayer.Click += toolStripButtonAddPlayer_Click;
            // 
            // toolStripButtonAddTribe
            // 
            toolStripButtonAddTribe.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            toolStripButtonAddTribe.Image = Properties.Resources.newTribe;
            toolStripButtonAddTribe.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonAddTribe.Name = "toolStripButtonAddTribe";
            toolStripButtonAddTribe.Size = new System.Drawing.Size(23, 22);
            toolStripButtonAddTribe.Text = "Add Tribe";
            toolStripButtonAddTribe.Visible = false;
            toolStripButtonAddTribe.Click += toolStripButtonAddTribe_Click;
            // 
            // toolStripButtonDeleteExpiredIncubationTimers
            // 
            toolStripButtonDeleteExpiredIncubationTimers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButtonDeleteExpiredIncubationTimers.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonDeleteExpiredIncubationTimers.Image");
            toolStripButtonDeleteExpiredIncubationTimers.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonDeleteExpiredIncubationTimers.Name = "toolStripButtonDeleteExpiredIncubationTimers";
            toolStripButtonDeleteExpiredIncubationTimers.Size = new System.Drawing.Size(102, 22);
            toolStripButtonDeleteExpiredIncubationTimers.Text = "Delete All Expired";
            toolStripButtonDeleteExpiredIncubationTimers.Visible = false;
            toolStripButtonDeleteExpiredIncubationTimers.Click += toolStripButtonDeleteExpiredIncubationTimers_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSaveCreatureValuesTemp
            // 
            toolStripButtonSaveCreatureValuesTemp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButtonSaveCreatureValuesTemp.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonSaveCreatureValuesTemp.Image");
            toolStripButtonSaveCreatureValuesTemp.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonSaveCreatureValuesTemp.Name = "toolStripButtonSaveCreatureValuesTemp";
            toolStripButtonSaveCreatureValuesTemp.Size = new System.Drawing.Size(71, 22);
            toolStripButtonSaveCreatureValuesTemp.Text = "Save values";
            toolStripButtonSaveCreatureValuesTemp.ToolTipText = "Save entered values until extraction-issue is resolved. This creature cannot be used in other parts of this application until it is properly extracted.";
            toolStripButtonSaveCreatureValuesTemp.Click += toolStripButtonSaveCreatureValuesTemp_Click;
            // 
            // toolStripCBTempCreatures
            // 
            toolStripCBTempCreatures.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            toolStripCBTempCreatures.Name = "toolStripCBTempCreatures";
            toolStripCBTempCreatures.Size = new System.Drawing.Size(209, 25);
            toolStripCBTempCreatures.SelectedIndexChanged += toolStripCBTempCreatures_SelectedIndexChanged;
            // 
            // toolStripButtonDeleteTempCreature
            // 
            toolStripButtonDeleteTempCreature.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButtonDeleteTempCreature.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonDeleteTempCreature.Image");
            toolStripButtonDeleteTempCreature.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonDeleteTempCreature.Name = "toolStripButtonDeleteTempCreature";
            toolStripButtonDeleteTempCreature.Size = new System.Drawing.Size(90, 22);
            toolStripButtonDeleteTempCreature.Text = "Delete temp Cr";
            toolStripButtonDeleteTempCreature.ToolTipText = "Delete currently selected data of the temporary creature";
            toolStripButtonDeleteTempCreature.Visible = false;
            toolStripButtonDeleteTempCreature.Click += toolStripButtonDeleteTempCreature_Click;
            // 
            // tsBtAddAsExtractionTest
            // 
            tsBtAddAsExtractionTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            tsBtAddAsExtractionTest.Image = (System.Drawing.Image)resources.GetObject("tsBtAddAsExtractionTest.Image");
            tsBtAddAsExtractionTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsBtAddAsExtractionTest.Name = "tsBtAddAsExtractionTest";
            tsBtAddAsExtractionTest.Size = new System.Drawing.Size(71, 22);
            tsBtAddAsExtractionTest.Text = "Add as Test";
            tsBtAddAsExtractionTest.Click += tsBtAddAsExtractionTest_Click;
            // 
            // copyToMultiplierTesterToolStripButton
            // 
            copyToMultiplierTesterToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            copyToMultiplierTesterToolStripButton.Image = (System.Drawing.Image)resources.GetObject("copyToMultiplierTesterToolStripButton.Image");
            copyToMultiplierTesterToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            copyToMultiplierTesterToolStripButton.Name = "copyToMultiplierTesterToolStripButton";
            copyToMultiplierTesterToolStripButton.Size = new System.Drawing.Size(138, 22);
            copyToMultiplierTesterToolStripButton.Text = "Copy to MultiplierTester";
            copyToMultiplierTesterToolStripButton.Click += copyToMultiplierTesterToolStripButton_Click;
            // 
            // ToolStripLabelFilter
            // 
            ToolStripLabelFilter.Name = "ToolStripLabelFilter";
            ToolStripLabelFilter.Size = new System.Drawing.Size(33, 22);
            ToolStripLabelFilter.Text = "Filter";
            ToolStripLabelFilter.Visible = false;
            // 
            // ToolStripTextBoxLibraryFilter
            // 
            ToolStripTextBoxLibraryFilter.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            ToolStripTextBoxLibraryFilter.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            ToolStripTextBoxLibraryFilter.Name = "ToolStripTextBoxLibraryFilter";
            ToolStripTextBoxLibraryFilter.Size = new System.Drawing.Size(233, 25);
            ToolStripTextBoxLibraryFilter.Visible = false;
            ToolStripTextBoxLibraryFilter.Click += ToolStripTextBoxLibraryFilter_Click;
            ToolStripTextBoxLibraryFilter.TextChanged += ToolStripTextBoxLibraryFilter_TextChanged;
            // 
            // ToolStripButtonLibraryFilterClear
            // 
            ToolStripButtonLibraryFilterClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            ToolStripButtonLibraryFilterClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            ToolStripButtonLibraryFilterClear.Name = "ToolStripButtonLibraryFilterClear";
            ToolStripButtonLibraryFilterClear.Size = new System.Drawing.Size(23, 22);
            ToolStripButtonLibraryFilterClear.Text = "×";
            ToolStripButtonLibraryFilterClear.Visible = false;
            ToolStripButtonLibraryFilterClear.Click += ToolStripButtonLibraryFilterClear_Click;
            // 
            // ToolStripButtonSaveFilterPreset
            // 
            ToolStripButtonSaveFilterPreset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            ToolStripButtonSaveFilterPreset.Image = (System.Drawing.Image)resources.GetObject("ToolStripButtonSaveFilterPreset.Image");
            ToolStripButtonSaveFilterPreset.ImageTransparentColor = System.Drawing.Color.Magenta;
            ToolStripButtonSaveFilterPreset.Name = "ToolStripButtonSaveFilterPreset";
            ToolStripButtonSaveFilterPreset.Size = new System.Drawing.Size(23, 22);
            ToolStripButtonSaveFilterPreset.Text = "▼";
            ToolStripButtonSaveFilterPreset.ToolTipText = "Save filter as preset";
            ToolStripButtonSaveFilterPreset.Visible = false;
            ToolStripButtonSaveFilterPreset.Click += ToolStripButtonSaveFilterPresetClick;
            // 
            // TsSpOcrLabel
            // 
            TsSpOcrLabel.Name = "TsSpOcrLabel";
            TsSpOcrLabel.Size = new System.Drawing.Size(6, 25);
            // 
            // TsLbLabelSet
            // 
            TsLbLabelSet.Name = "TsLbLabelSet";
            TsLbLabelSet.Size = new System.Drawing.Size(77, 22);
            TsLbLabelSet.Text = "OCR label set";
            // 
            // TsCbbLabelSets
            // 
            TsCbbLabelSets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            TsCbbLabelSets.Name = "TsCbbLabelSets";
            TsCbbLabelSets.Size = new System.Drawing.Size(140, 25);
            TsCbbLabelSets.SelectedIndexChanged += TsCbbLabelSets_SelectedIndexChanged;
            // 
            // panelToolBar
            // 
            panelToolBar.Controls.Add(btReadValuesFromArk);
            panelToolBar.Controls.Add(btImportLastExported);
            panelToolBar.Controls.Add(pbSpecies);
            panelToolBar.Controls.Add(tbSpeciesGlobal);
            panelToolBar.Controls.Add(cbGuessSpecies);
            panelToolBar.Controls.Add(cbToggleOverlay);
            panelToolBar.Controls.Add(lbListening);
            panelToolBar.Controls.Add(cbEventMultipliers);
            panelToolBar.Controls.Add(lbSpecies);
            panelToolBar.Controls.Add(TbMessageLabel);
            panelToolBar.Dock = System.Windows.Forms.DockStyle.Top;
            panelToolBar.Location = new System.Drawing.Point(0, 49);
            panelToolBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelToolBar.Name = "panelToolBar";
            panelToolBar.Size = new System.Drawing.Size(2191, 62);
            panelToolBar.TabIndex = 2;
            // 
            // btImportLastExported
            // 
            btImportLastExported.Location = new System.Drawing.Point(442, 3);
            btImportLastExported.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btImportLastExported.Name = "btImportLastExported";
            btImportLastExported.Size = new System.Drawing.Size(99, 51);
            btImportLastExported.TabIndex = 4;
            btImportLastExported.Text = "Last Export";
            btImportLastExported.UseVisualStyleBackColor = true;
            btImportLastExported.Click += btImportLastExported_Click;
            // 
            // pbSpecies
            // 
            pbSpecies.Location = new System.Drawing.Point(4, 3);
            pbSpecies.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbSpecies.Name = "pbSpecies";
            pbSpecies.Size = new System.Drawing.Size(51, 51);
            pbSpecies.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pbSpecies.TabIndex = 13;
            pbSpecies.TabStop = false;
            pbSpecies.Click += pbSpecies_Click;
            // 
            // tbSpeciesGlobal
            // 
            tbSpeciesGlobal.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            tbSpeciesGlobal.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            tbSpeciesGlobal.Location = new System.Drawing.Point(121, 3);
            tbSpeciesGlobal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tbSpeciesGlobal.Name = "tbSpeciesGlobal";
            tbSpeciesGlobal.Size = new System.Drawing.Size(177, 23);
            tbSpeciesGlobal.TabIndex = 8;
            tbSpeciesGlobal.Click += tbSpeciesGlobal_Click;
            tbSpeciesGlobal.Enter += tbSpeciesGlobal_Enter;
            tbSpeciesGlobal.KeyUp += TbSpeciesGlobal_KeyUp;
            // 
            // cbGuessSpecies
            // 
            cbGuessSpecies.AutoSize = true;
            cbGuessSpecies.Location = new System.Drawing.Point(148, 33);
            cbGuessSpecies.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbGuessSpecies.Name = "cbGuessSpecies";
            cbGuessSpecies.Size = new System.Drawing.Size(99, 19);
            cbGuessSpecies.TabIndex = 2;
            cbGuessSpecies.Text = "Guess Species";
            cbGuessSpecies.UseVisualStyleBackColor = true;
            // 
            // cbToggleOverlay
            // 
            cbToggleOverlay.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            cbToggleOverlay.Appearance = System.Windows.Forms.Appearance.Button;
            cbToggleOverlay.AutoSize = true;
            cbToggleOverlay.Location = new System.Drawing.Point(2119, 32);
            cbToggleOverlay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbToggleOverlay.Name = "cbToggleOverlay";
            cbToggleOverlay.Size = new System.Drawing.Size(57, 25);
            cbToggleOverlay.TabIndex = 7;
            cbToggleOverlay.Text = "Overlay";
            cbToggleOverlay.UseVisualStyleBackColor = true;
            cbToggleOverlay.CheckedChanged += chkbToggleOverlay_CheckedChanged;
            // 
            // lbListening
            // 
            lbListening.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lbListening.AutoSize = true;
            lbListening.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lbListening.ForeColor = System.Drawing.SystemColors.GrayText;
            lbListening.Location = new System.Drawing.Point(2152, 3);
            lbListening.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbListening.Name = "lbListening";
            lbListening.Size = new System.Drawing.Size(25, 20);
            lbListening.TabIndex = 6;
            lbListening.Text = "🎤";
            lbListening.TextAlign = System.Drawing.ContentAlignment.TopRight;
            lbListening.Click += labelListening_Click;
            // 
            // lbSpecies
            // 
            lbSpecies.AutoSize = true;
            lbSpecies.Location = new System.Drawing.Point(62, 7);
            lbSpecies.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbSpecies.Name = "lbSpecies";
            lbSpecies.Size = new System.Drawing.Size(46, 15);
            lbSpecies.TabIndex = 0;
            lbSpecies.Text = "Species";
            // 
            // TbMessageLabel
            // 
            TbMessageLabel.AcceptsReturn = true;
            TbMessageLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            TbMessageLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            TbMessageLabel.Location = new System.Drawing.Point(548, 3);
            TbMessageLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TbMessageLabel.Multiline = true;
            TbMessageLabel.Name = "TbMessageLabel";
            TbMessageLabel.ReadOnly = true;
            TbMessageLabel.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            TbMessageLabel.Size = new System.Drawing.Size(1037, 55);
            TbMessageLabel.TabIndex = 14;
            TbMessageLabel.Click += TbMessageLabel_Click;
            // 
            // contextMenuStripLibraryHeader
            // 
            contextMenuStripLibraryHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItemResetLibraryColumnWidths, resetColumnWidthNoMutationLevelColumnsToolStripMenuItem, restoreMutationLevelsASAToolStripMenuItem, collapseMutationsLevelsASEToolStripMenuItem, toolStripSeparator27, resetColumnOrderToolStripMenuItem });
            contextMenuStripLibraryHeader.Name = "contextMenuStrip1";
            contextMenuStripLibraryHeader.Size = new System.Drawing.Size(333, 120);
            // 
            // toolStripMenuItemResetLibraryColumnWidths
            // 
            toolStripMenuItemResetLibraryColumnWidths.Name = "toolStripMenuItemResetLibraryColumnWidths";
            toolStripMenuItemResetLibraryColumnWidths.Size = new System.Drawing.Size(332, 22);
            toolStripMenuItemResetLibraryColumnWidths.Text = "Reset all column widths";
            toolStripMenuItemResetLibraryColumnWidths.Click += toolStripMenuItemResetLibraryColumnWidths_Click;
            // 
            // resetColumnWidthNoMutationLevelColumnsToolStripMenuItem
            // 
            resetColumnWidthNoMutationLevelColumnsToolStripMenuItem.Name = "resetColumnWidthNoMutationLevelColumnsToolStripMenuItem";
            resetColumnWidthNoMutationLevelColumnsToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            resetColumnWidthNoMutationLevelColumnsToolStripMenuItem.Text = "Reset all column widths, collapse mutation levels";
            resetColumnWidthNoMutationLevelColumnsToolStripMenuItem.Click += resetColumnWidthNoMutationLevelColumnsToolStripMenuItem_Click;
            // 
            // restoreMutationLevelsASAToolStripMenuItem
            // 
            restoreMutationLevelsASAToolStripMenuItem.Name = "restoreMutationLevelsASAToolStripMenuItem";
            restoreMutationLevelsASAToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            restoreMutationLevelsASAToolStripMenuItem.Text = "Restore mutation levels (ASA)";
            restoreMutationLevelsASAToolStripMenuItem.Click += restoreMutationLevelsASAToolStripMenuItem_Click;
            // 
            // collapseMutationsLevelsASEToolStripMenuItem
            // 
            collapseMutationsLevelsASEToolStripMenuItem.Name = "collapseMutationsLevelsASEToolStripMenuItem";
            collapseMutationsLevelsASEToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            collapseMutationsLevelsASEToolStripMenuItem.Text = "Collapse mutations levels (ASE)";
            collapseMutationsLevelsASEToolStripMenuItem.Click += collapseMutationsLevelsASEToolStripMenuItem_Click;
            // 
            // toolStripSeparator27
            // 
            toolStripSeparator27.Name = "toolStripSeparator27";
            toolStripSeparator27.Size = new System.Drawing.Size(329, 6);
            // 
            // resetColumnOrderToolStripMenuItem
            // 
            resetColumnOrderToolStripMenuItem.Name = "resetColumnOrderToolStripMenuItem";
            resetColumnOrderToolStripMenuItem.Size = new System.Drawing.Size(332, 22);
            resetColumnOrderToolStripMenuItem.Text = "Reset column order";
            resetColumnOrderToolStripMenuItem.Click += resetColumnOrderToolStripMenuItem_Click;
            // 
            // speciesSelector1
            // 
            speciesSelector1.Dock = System.Windows.Forms.DockStyle.Fill;
            speciesSelector1.Location = new System.Drawing.Point(0, 111);
            speciesSelector1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            speciesSelector1.Name = "speciesSelector1";
            speciesSelector1.Size = new System.Drawing.Size(2191, 946);
            speciesSelector1.TabIndex = 0;
            // 
            // Form1
            // 
            AcceptButton = btExtractLevels;
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(2191, 1079);
            Controls.Add(tabControlMain);
            Controls.Add(speciesSelector1);
            Controls.Add(panelToolBar);
            Controls.Add(toolStrip2);
            Controls.Add(menuStrip1);
            Controls.Add(statusStrip1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "Form1";
            Text = "ARK Smart Breeding";
            FormClosing += Form1_FormClosing;
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            DragDrop += Form1_DragDrop;
            DragEnter += Form1_DragEnter;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownImprintingBonusTester).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericUpDownTestingTE).EndInit();
            groupBoxPossibilities.ResumeLayout(false);
            groupBoxDetailsExtractor.ResumeLayout(false);
            panelExtrImpr.ResumeLayout(false);
            panelExtrImpr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownImprintingBonusExtractor).EndInit();
            panelExtrTE.ResumeLayout(false);
            panelExtrTE.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownUpperTEffBound).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownLowerTEffBound).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            panelSums.ResumeLayout(false);
            panelSums.PerformLayout();
            panelWildTamedBred.ResumeLayout(false);
            panelWildTamedBred.PerformLayout();
            tabControlMain.ResumeLayout(false);
            tabPageStatTesting.ResumeLayout(false);
            tabPageStatTesting.PerformLayout();
            gbStatChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)radarChart1).EndInit();
            panelWildTamedBredTester.ResumeLayout(false);
            panelWildTamedBredTester.PerformLayout();
            groupBox2.ResumeLayout(false);
            flowLayoutPanelStatIOsTester.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panelStatTesterFootnote.ResumeLayout(false);
            panelStatTesterFootnote.PerformLayout();
            gpPreviewEdit.ResumeLayout(false);
            gpPreviewEdit.PerformLayout();
            tabPageExtractor.ResumeLayout(false);
            tabPageExtractor.PerformLayout();
            pBondedTamingExtractor.ResumeLayout(false);
            pBondedTamingExtractor.PerformLayout();
            groupBoxRadarChartExtractor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)radarChartExtractor).EndInit();
            groupBoxTamingInfo.ResumeLayout(false);
            gbStatsExtractor.ResumeLayout(false);
            flowLayoutPanelStatIOsExtractor.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownLevel).EndInit();
            tabPageLibrary.ResumeLayout(false);
            tableLayoutPanelLibrary.ResumeLayout(false);
            contextMenuStripLibrary.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tabControlLibFilter.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tabPageLibRadarChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)radarChartLibrary).EndInit();
            tabPageLibraryInfo.ResumeLayout(false);
            tlpLibraryInfo.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tabPagePedigree.ResumeLayout(false);
            tabPageTaming.ResumeLayout(false);
            tabPageBreedingPlan.ResumeLayout(false);
            tabPageCurrentBreeds.ResumeLayout(false);
            tabPageRaising.ResumeLayout(false);
            tabPageTimer.ResumeLayout(false);
            tabPagePlayerTribes.ResumeLayout(false);
            tabPageNotes.ResumeLayout(false);
            TabPageOCR.ResumeLayout(false);
            tabPageExtractionTests.ResumeLayout(false);
            tabPageMultiplierTesting.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            panelToolBar.ResumeLayout(false);
            panelToolBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbSpecies).EndInit();
            contextMenuStripLibraryHeader.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

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
        private System.Windows.Forms.ToolStripMenuItem SetMaturityCooldownToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem configureSavegameImportToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopyGeneratedCreatureName;
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
        private System.Windows.Forms.ToolStripSeparator maturationSeparator;
        private System.Windows.Forms.ToolStripMenuItem clearMatingCooldownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem justMatedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem libraryFilterToolStripMenuItem;
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
        private System.Windows.Forms.TabPage tabPageCurrentBreeds;
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
        private System.Windows.Forms.ToolStripMenuItem sendServerCreatureStatusNeuterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendServerCreatureStatusDeadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listenWithNewTokenToolStripMenuItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox CbLinkWildMutatedLevelsTester;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMutationColumns;
        private System.Windows.Forms.ToolStripMenuItem currentTokenToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private uiControls.LibraryInfoControl libraryInfoControl1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator28;
        private System.Windows.Forms.ToolStripMenuItem openModPageInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nameGeneratorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectSavegameFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTokenPopupOnListeningToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem beginListeningToExportGunOnLaunchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem adminCommandSetMutationLevelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commandMutationLevelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statsOptionsToolStripMenuItem;
        private System.Windows.Forms.Button BtSetImprinting100Extractor;
        private System.Windows.Forms.Button BtSetImprinting0Extractor;
        private System.Windows.Forms.Button BtSetImprinting100Tester;
        private System.Windows.Forms.ToolStripMenuItem appSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSettingsFileInExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadAppSettingsFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAppSettingsTToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator29;
        private System.Windows.Forms.ToolStripMenuItem showStatsOptionsFileInExplorerToolStripMenuItem;
        private System.Windows.Forms.Button BtRecalculateTopStatsAfterChange;
        private System.Windows.Forms.ColumnHeader columnHeaderCurrentLevel;
        private System.Windows.Forms.ColumnHeader columnHeaderMaxPossibleLevel;
        private System.Windows.Forms.ColumnHeader columnHeaderTraits;
        private System.Windows.Forms.ToolStripMenuItem editVariantTagsToHideToolStripMenuItem;
        private System.Windows.Forms.Panel pBondedTamingExtractor;
        private System.Windows.Forms.RadioButton RbBondedTaming3;
        private System.Windows.Forms.RadioButton RbBondedTaming2;
        private System.Windows.Forms.RadioButton RbBondedTaming1;
        private System.Windows.Forms.RadioButton RbBondedTaming0;
        private System.Windows.Forms.Label LbBondedTaming;
        private System.Windows.Forms.ToolStripMenuItem saveExportFileLocallyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editTraitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arkUtilsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem howManyFemalesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem howGoodAreMyStatsToolStripMenuItem;
        private uiControls.CurrentBreeds currentBreeds1;
        private System.Windows.Forms.ColumnHeader columnHeaderMutated;
        private System.Windows.Forms.ToolStripMenuItem speciesImagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewColorsInLibraryInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyCoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spawnWildToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uIScalingIssueFixToolStripMenuItem;
        private uiControls.ColoredCreatureImageWithPose ColoredCreatureImageDisplayTester;
        private uiControls.ColoredCreatureImageWithPose ColoredCreatureImageDisplayExtractor;
    }
}
