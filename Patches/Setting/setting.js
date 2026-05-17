if (window.XSOverlayTWEAK_SETTING) return 'XSOverlayTWEAK_SETTING already injected';
window.XSOverlayTWEAK_SETTING = true;

function InjectKBOSCTab() {
    var scr = document.createElement('script');
    scr.type = 'module';
    scr.textContent = "import * as Ui from './_Shared/js/uiComponents.js'; (" + function (Ui) {
        // --- Configuration ---
        const CONFIG = {
            pageId: 'Page_XSOverlayTWEAK',
            pageName: 'XSOverlay TWEAK',
            pageIcon: 'bi-tools',
            targetIndex: 0 // 0 for top, 1 for after General, etc.
        };

        const SECTIONS = [
            {
                name: 'Refresh Rate', priority: 1, settings: [
                    { type: Ui.ComponentType.Toggle, id: 'EnableRefreshRate', name: 'Enable', description: 'Enable overriding the XSOverlay render refresh rate.', default: false },
                    { type: Ui.ComponentType.Slider, id: 'RefreshRate', name: 'Refresh Rate', description: 'The target frame rate for XSOverlay rendering.<br>Higher values improve responsiveness but increase CPU usage.', default: 90, options: [90, 500, 10], unit: 'FPS' }
                ]
            },
            {
                name: 'Cursor', priority: 2, settings: [
                    { type: Ui.ComponentType.Toggle, id: 'AlwayUpdateCursor', name: 'Always Update Cursor', description: 'Reduces perceived cursor latency by ensuring the system cursor is updated immediately before the desktop frame is captured.<br>Without this, the cursor often appears to lag one frame behind the actual pointer position.', default: true },
                    { type: Ui.ComponentType.Toggle, id: 'AlwaysHideCursor', name: 'Always Hide Cursor', description: 'Forcefully hide the system cursor in Window Capture overlays.', default: false },
                    { type: Ui.ComponentType.Toggle, id: 'PhysicalMouseDetector', name: 'Physical Mouse Detector', description: 'Automatically release VR pointer control when physical mouse movement is detected. Click to regain control.', default: true }
                ]
            },
            {
                name: 'Pointer', priority: 3, settings: [
                    { type: Ui.ComponentType.Toggle, id: 'ActivesPointerColor', name: 'Active Pointer Highlight', description: 'Highlight the non-active hand\'s pointer in red for easier identification.', default: true },
                    { type: Ui.ComponentType.Slider, id: 'ActivePointerOpacity', name: 'Inactive Opacity', description: 'Set the opacity of the non-active hand\'s pointer.', default: 0.5, options: [0.0, 1.0, 0.1], unit: 'Unit' },
                    { type: Ui.ComponentType.Slider, id: 'PointerScaleMultiply', name: 'Scale Multiplier', description: 'Multiplier for the pointer scale relative to the global XSOverlay settings.', default: 1.0, options: [1.0, 10.0, 0.1], unit: 'Unit' },
                    { type: Ui.ComponentType.Toggle, id: 'PointerDoubleClickDelay', name: 'Double Click Delay', description: 'Apply a Double Click Delay setting to the Pointer itself, not just the cursor.', default: true }
                ]
            },
            {
                name: 'Mouse Navigation', priority: 4, settings: [
                    { type: Ui.ComponentType.Toggle, id: 'MouseNavigation', name: 'Enable', description: 'Enable custom keybindings for Mouse Forward/Back navigation. Configure these in the SteamVR \'Binding\' tab.', default: true },
                    { type: Ui.ComponentType.Toggle, id: 'MouseNavigationUseModifiedKey', name: 'Use Alt+Left/Right', description: 'Use Alt+Left/Right keyboard shortcuts for navigation instead of mouse clicks. Targets the focused window instead of the hovered window.', default: false }
                ]
            }

            // addSetting(Cursor, Ui.ComponentType.Button, 'Button', 'Button', 'Button', null);
            // addSetting(Cursor, Ui.ComponentType.Text, 'Text', 'Text', '');
            // addSetting(Cursor, Ui.ComponentType.Dropdown, 'KBDropdownExample', 'Dropdown Example', 'This is an example dropdown.', 'Option 1', ['Option 1', 'Option 2', 'Option 3']);
        ];

        const sidebar = document.querySelector('.side-bar-button-container');
        const wrapper = document.querySelector('.page-wrapper');
        if (!sidebar || !wrapper || document.getElementById(CONFIG.pageId)) return;

        // --- Sidebar Navigation Button ---
        const existingBtns = Array.from(sidebar.querySelectorAll('.side-bar-button'));

        const navBtn = Ui.CreateElement(sidebar, 'button', ['side-bar-button']);
        Ui.CreateElement(navBtn, 'i', ['side-bar-button-icon', 'theme-font-contrast', 'bi', CONFIG.pageIcon]);
        const navLabel = Ui.CreateElement(navBtn, 'div', ['side-bar-button-text']);
        navLabel.innerHTML = CONFIG.pageName;

        // Determine insertion point for the button
        let referenceNodeForButton = null;
        if (CONFIG.targetIndex !== null && CONFIG.targetIndex < existingBtns.length) {
            referenceNodeForButton = existingBtns[CONFIG.targetIndex];
            sidebar.insertBefore(navBtn, referenceNodeForButton);
        }

        // Conditionally add a divider after the new button, mimicking existing sidebar behavior.
        // A divider is added after a button if there are other buttons following it.
        if (CONFIG.targetIndex !== null && CONFIG.targetIndex < existingBtns.length) {
            const newDivider = Ui.CreateElement(sidebar, 'div', ['sidebar-divider']);
            sidebar.insertBefore(newDivider, navBtn.nextSibling);
        }

        // --- Settings Page Layout ---
        const pageRoot = Ui.CreateElement(wrapper, 'div', ['page-container', 'theme-dark']);
        pageRoot.id = CONFIG.pageId;
        pageRoot.style.cssText = 'position:absolute; opacity:0; pointer-events:none;';

        const header = Ui.CreateElement(pageRoot, 'div', ['page-header']);
        const headerText = Ui.CreateElement(header, 'div', ['page-header-text']);
        headerText.innerHTML = CONFIG.pageName;


        // --- Setting Builder Helper ---
        const addSetting = (sectionObj, type, id, name, desc, defaultValue, opts, opts1) => {
            const setting = new Ui.Setting(type, name, desc, defaultValue, opts, opts1);
            setting.internalName = id;
            setting.sectionID = sectionObj.Name;

            const componentCreators = {
                [Ui.ComponentType.Toggle]: () => Ui.Toggle(setting, name, defaultValue, null, sectionObj.Background),
                [Ui.ComponentType.Button]: () => Ui.Button(setting, sectionObj.Background),
                [Ui.ComponentType.Slider]: () => Ui.Slider(setting, name, defaultValue, opts, opts1, sectionObj.Background, 300),
                [Ui.ComponentType.Dropdown]: () => Ui.Dropdown(setting, name, defaultValue, opts, sectionObj.Background, 300)
            };

            if (componentCreators[type]) componentCreators[type]();
            if (desc || type === Ui.ComponentType.Text) Ui.Description(sectionObj.Background, desc || '', id + '_Desc');

            Ui.Divider(sectionObj.Background, 'divider');
        };

        // --- Build Sections ---
        SECTIONS.forEach(s => {
            const section = new Ui.Section(s.name, s.priority, pageRoot);
            s.settings.forEach(set => {
                addSetting(section, set.type, set.id, set.name, set.description, set.default, set.options, set.unit);
            });
        });

        // --- Navigation Logic ---
        const switchPage = () => {
            wrapper.querySelectorAll('.page-container, .page-container-no-overflow').forEach(p => {
                if (p !== pageRoot) {
                    p.style.animation = '0.3s ease fade-out forwards';
                    p.style.pointerEvents = 'none';
                }
            });

            pageRoot.style.animation = '0.3s ease fade-in forwards';
            pageRoot.style.pointerEvents = 'auto';
        };

        sidebar.addEventListener('click', (e) => {
            const btn = e.target.closest('.side-bar-button');
            if (!btn) return;

            if (btn === navBtn) {
                switchPage();
            } else {
                pageRoot.style.animation = '0.3s ease fade-out forwards';
                pageRoot.style.pointerEvents = 'none';

                wrapper.querySelectorAll('.page-container, .page-container-no-overflow').forEach(p => {
                    if (p !== pageRoot) {
                        p.style.pointerEvents = 'auto';
                    }
                });
            }

            // Update visual selection for all buttons in sidebar
            sidebar.querySelectorAll('.side-bar-button').forEach(b => {
                const isTarget = b === btn;
                b.classList.toggle('side-bar-button-selected', isTarget);
                if (b.firstElementChild) b.firstElementChild.classList.toggle('selected-icon', isTarget);
            });
        });
    }.toString() + ") (Ui);";
    document.body.appendChild(scr);
}

InjectKBOSCTab()

return 'XSOverlayTWEAK_SETTING injected';