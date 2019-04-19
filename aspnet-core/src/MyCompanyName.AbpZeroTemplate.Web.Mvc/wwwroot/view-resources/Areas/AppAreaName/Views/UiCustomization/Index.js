(function ($) {
    $(function () {
        var _uiCustomizationSettingsService = abp.services.app.uiCustomizationSettings;

        $("input[name='AllowAsideHiding'], input[name='AllowAsideMinimizing']").change(function () {
            toggleLeftMenuHideMode();
        });

        function toggleLeftMenuHideMode() {
            var allowAsideMinimizing = $("input[name='AllowAsideMinimizing']").is(':checked');

            if (allowAsideMinimizing) {
                $("input[name='DefaultMinimizedAside']").removeAttr('disabled');

                $("input[name='AllowAsideHiding']").prop("checked", false);
                $("input[name='AllowAsideHiding']").attr('disabled', 'disabled');

                $("input[name='DefaultHiddenAside']").prop("checked", false);
                $("input[name='DefaultHiddenAside']").attr('disabled', 'disabled');
            } else {
                $("input[name='AllowAsideHiding']").removeAttr('disabled');
                $("input[name='DefaultHiddenAside']").removeAttr('disabled');

                $("input[name='DefaultMinimizedAside']").prop("checked", false);
                $("input[name='DefaultMinimizedAside']").attr('disabled', 'disabled');
            }

            var allowAsideHiding = $("input[name='AllowAsideHiding']").is(':checked');

            if (allowAsideHiding) {
                $("input[name='DefaultHiddenAside']").removeAttr('disabled');
            } else {
                $("input[name='DefaultHiddenAside']").prop("checked", false);
                $("input[name='DefaultHiddenAside']").attr('disabled', 'disabled');
            }
        }

        toggleLeftMenuHideMode();
        
        $('#SaveSettingsButton').click(function () {
            var activeThemeTab = $('#metronicThemes').find('.tab-pane.theme-selection.active.show')[0];
            _uiCustomizationSettingsService.updateUiManagementSettings({
                theme: $(activeThemeTab).find('input[name="Theme"]').val(),
                layout: $(activeThemeTab).find('.LayoutSettingsForm').serializeFormToObject(),
                header: $(activeThemeTab).find('.HeaderSettingsForm').serializeFormToObject(),
                menu: $(activeThemeTab).find('.MenuSettingsForm').serializeFormToObject(),
                footer: $(activeThemeTab).find('.FooterSettingsForm').serializeFormToObject()
            }).done(function () {
                window.location.reload();
            });
        });

        $('#SaveDefaultSettingsButton').click(function () {
            var activeThemeTab = $('#metronicThemes').find('.tab-pane.theme-selection.active.show')[0];
            _uiCustomizationSettingsService.updateDefaultUiManagementSettings({
                theme: $(activeThemeTab).find('input[name="Theme"]').val(),
                layout: $(activeThemeTab).find('.LayoutSettingsForm').serializeFormToObject(),
                header: $(activeThemeTab).find('.HeaderSettingsForm').serializeFormToObject(),
                menu: $(activeThemeTab).find('.MenuSettingsForm').serializeFormToObject(),
                footer: $(activeThemeTab).find('.FooterSettingsForm').serializeFormToObject()
            }).done(function () {
                window.location.reload();
            });
        });

        $('#UseSystemDefaultSettings').click(function () {
            _uiCustomizationSettingsService.useSystemDefaultSettings().done(function () {
                window.location.reload();
            });
        });

    });
})(jQuery);