if (window.__XSO_TWEAK_INIT__) return;
window.__XSO_TWEAK_INIT__ = true;

var _init;
Object.defineProperty(window, 'Initialize', {
    get: function () { return _init; },
    set: function (orig) {
        _init = function (name) {
            orig(name);
            var scr = document.createElement('script');
            scr.type = 'module';
            scr.textContent = "import * as Ui from './_Shared/js/uiComponents.js'; (" + function (Ui) {
                // --- Configuration ---
                const CONFIG = {
                    pageId: 'Page_Tweak',
                    pageName: 'XSOverlay Tweak',
                    pageIcon: 'bi-tools',
                    targetIndex: 0 // 0 for top, 1 for after General, etc.
                };

                const sidebar = document.querySelector('.side-bar-button-container');
                const wrapper = document.querySelector('.page-wrapper');
                if (!sidebar || !wrapper || document.getElementById(CONFIG.pageId)) return;

                // --- Sidebar Navigation Button ---
                const existingBtns = Array.from(sidebar.querySelectorAll('.side-bar-button'));

                Ui.Divider(sidebar, 'sidebar-divider');
                const navBtn = Ui.CreateElement(sidebar, 'button', ['side-bar-button']);
                const navDivider = navBtn.previousElementSibling;

                // Reorder the button if a valid target index is provided
                if (CONFIG.targetIndex !== null && CONFIG.targetIndex < existingBtns.length) {
                    const referenceBtn = existingBtns[CONFIG.targetIndex];
                    sidebar.insertBefore(navBtn, referenceBtn);
                    sidebar.insertBefore(navDivider, navBtn);
                }

                Ui.CreateElement(navBtn, 'i', ['side-bar-button-icon', 'theme-font-contrast', 'bi', CONFIG.pageIcon]);
                const navLabel = Ui.CreateElement(navBtn, 'div', ['side-bar-button-text']);
                navLabel.innerHTML = CONFIG.pageName;

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

                // Cursor Section
                const Cursor = new Ui.Section('Cursor', 3, pageRoot);
                addSetting(Cursor, Ui.ComponentType.Toggle, 'AlwayUpdateCursor', 'AlwayUpdateCursor', 'By default, XSOverlay displays the captured Desktop before sending new cursor position data to the actual cursor', true);
                addSetting(Cursor, Ui.ComponentType.Toggle, 'AlwayHideCursor', 'AlwayHideCursor', 'Always hide Window Capture cursor.', false);
                addSetting(Cursor, Ui.ComponentType.Toggle, 'PhysicalMouseDetector', 'PhysicalMouseDetector', 'When a physical mouse is moving the desktop cursor, Pointer will no longer control the cursor until clicking.', true);

                // General Section
                const General = new Ui.Section('General', 1, pageRoot);
                addSetting(General, Ui.ComponentType.Slider, 'RefreshRate', 'RefreshRate', 'Change the XSOverlay render frame rate.', 0, [-1, 1000, 10], 'FPS');
                
                // Mouse Navigation Section
                const MouseNavigation = new Ui.Section('Mouse Navigation', 2, pageRoot);
                addSetting(MouseNavigation, Ui.ComponentType.Toggle, 'MouseNavigation', 'MouseNavigation', 'Enable custom keybinding to simulate the side mouse Forward/Back button.', true);
                addSetting(MouseNavigation, Ui.ComponentType.Toggle, 'MouseNavigationUseModifiedKey', 'MouseNavigationUseModifiedKey', 'Enable custom keybinding to simulate the side mouse Forward/Back button.', false);
                
                // Pointer Section
                const Pointer = new Ui.Section('Pointer', 4, pageRoot);
                addSetting(Pointer, Ui.ComponentType.Toggle, 'ActivePointerColor', 'ActivePointerColor', 'Determine the activated hand Pointer by red color.', true);
                addSetting(Pointer, Ui.ComponentType.Slider, 'ActivePointerOpacity', 'ActivePointerOpacity', 'Determine the deactivated hand Pointer by opacity.', 0.5, [0.0, 1.0, 0.1], '');
                addSetting(Pointer, Ui.ComponentType.Slider, 'PointerScaleMultiply', 'PointerScaleMultiply', 'Multiply the Pointer scale from the common setting.', 1.0, [1.0, 10.0, 0.1], '');
                addSetting(Pointer, Ui.ComponentType.Toggle, 'PointerDoubleClickDelay', 'PointerDoubleClickDelay', 'Apply a Double Click Delay setting to the Pointer itself, not just the cursor.', true);

                // addSetting(Cursor, Ui.ComponentType.Button, 'Button', 'Button', 'Button', null);
                // addSetting(Cursor, Ui.ComponentType.Text, 'Text', 'Text', '');
                // addSetting(Cursor, Ui.ComponentType.Dropdown, 'KBDropdownExample', 'Dropdown Example', 'This is an example dropdown.', 'Option 1', ['Option 1', 'Option 2', 'Option 3']);

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
        };
    },
    configurable: true
});