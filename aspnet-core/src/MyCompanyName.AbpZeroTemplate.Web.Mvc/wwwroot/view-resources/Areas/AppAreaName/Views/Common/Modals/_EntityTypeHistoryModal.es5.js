﻿'use strict';

(function () {
    app.modals.EntityTypeHistoryModal = function () {

        var _modalManager;
        var _dataTable;
        var _$table;
        var _args;

        var _auditLogService = abp.services.app.auditLog;

        var _options = {
            serviceMethod: null, //Required
            title: app.localize('SelectAnItem'),
            loadOnStartup: true,
            showFilter: true,
            filterText: '',
            pageSize: app.consts.grid.defaultPageSize,
            canSelect: function canSelect(item) {
                /* This method can return boolean or a promise which returns boolean.
                 * A false value is used to prevent selection.
                 */
                return true;
            }
        };

        function refreshTable() {
            _dataTable.ajax.reload();
        };

        function showEntityChangeDetails(entityChange) {
            var entityChangeDetailModal = new app.ModalManager({
                viewUrl: abp.appPath + 'AppAreaName/AuditLogs/EntityChangeDetailModal',
                modalClass: 'EntityChangeDetailModal'
            });

            entityChangeDetailModal.open({ entityChangeListDto: entityChange });
        }

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _options = $.extend(_options, _modalManager.getOptions().lookupOptions);
            _$table = _modalManager.getModal().find('.entity-type-history-table');
            _args = _modalManager.getArgs();

            _dataTable = _$table.DataTable({
                paging: true,
                serverSide: true,
                processing: true,
                lengthChange: false,
                pageLength: _options.pageSize,
                deferLoading: _options.loadOnStartup ? null : 0,
                listAction: {
                    ajaxFunction: _auditLogService.getEntityTypeChanges,
                    inputFilter: function inputFilter() {
                        return {
                            entityTypeFullName: _args.entityTypeFullName,
                            entityId: _args.entityId
                        };
                    }
                },
                columnDefs: [{
                    targets: 0,
                    data: null,
                    orderable: false,
                    defaultContent: '',
                    rowAction: {
                        element: $("<div/>").addClass("text-center").append($("<button/>").addClass("btn m-btn m-btn--hover-accent m-btn--icon m-btn--icon-only m-btn--pill").attr("title", app.localize("EntityChangeDetail")).append($("<i/>").addClass("la la-search"))).click(function () {
                            showEntityChangeDetails($(this).data());
                        })
                    }
                }, {
                    targets: 1,
                    data: "changeTypeName",
                    orderable: false,
                    render: function render(changeTypeName) {
                        return app.localize(changeTypeName);
                    }
                }, {
                    targets: 2,
                    data: "userName"
                }, {
                    targets: 3,
                    data: "changeTime",
                    render: function render(changeTime) {
                        return moment(changeTime).format('L LT');
                    }
                }]
            });

            refreshTable();
        };
    };

    app.modals.EntityTypeHistoryModal.create = function () {
        return new app.ModalManager({
            viewUrl: abp.appPath + 'AppAreaName/Common/EntityTypeHistoryModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/AppAreaName/Views/Common/Modals/_EntityTypeHistoryModal.js',
            modalClass: 'EntityTypeHistoryModal'
        });
    };
})();

