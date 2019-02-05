/*
 * This combined file was created by the DataTables downloader builder:
 *   https://datatables.net/download
 *
 * To rebuild or modify this file with the latest versions of the included
 * software please visit:
 *   https://datatables.net/download/#dt/dt-1.10.18/e-1.8.1/af-2.3.2/fc-3.2.5/fh-3.1.4/sl-1.2.6
 *
 * Included libraries:
 *   DataTables 1.10.18, Editor 1.8.1, AutoFill 2.3.2, FixedColumns 3.2.5, FixedHeader 3.1.4, Select 1.2.6
 */

/*! DataTables 1.10.18
 * ©2008-2018 SpryMedia Ltd - datatables.net/license
 */

/**
 * @summary     DataTables
 * @description Paginate, search and order HTML tables
 * @version     1.10.18
 * @file        jquery.dataTables.js
 * @author      SpryMedia Ltd
 * @contact     www.datatables.net
 * @copyright   Copyright 2008-2018 SpryMedia Ltd.
 *
 * This source file is free software, available under the following license:
 *   MIT license - http://datatables.net/license
 *
 * This source file is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the license files for details.
 *
 * For details please refer to: http://www.datatables.net
 */

/*jslint evil: true, undef: true, browser: true */
/*globals $,require,jQuery,define,_selector_run,_selector_opts,_selector_first,_selector_row_indexes,_ext,_Api,_api_register,_api_registerPlural,_re_new_lines,_re_html,_re_formatted_numeric,_re_escape_regex,_empty,_intVal,_numToDecimal,_isNumber,_isHtml,_htmlNumeric,_pluck,_pluck_order,_range,_stripHtml,_unique,_fnBuildAjax,_fnAjaxUpdate,_fnAjaxParameters,_fnAjaxUpdateDraw,_fnAjaxDataSrc,_fnAddColumn,_fnColumnOptions,_fnAdjustColumnSizing,_fnVisibleToColumnIndex,_fnColumnIndexToVisible,_fnVisbleColumns,_fnGetColumns,_fnColumnTypes,_fnApplyColumnDefs,_fnHungarianMap,_fnCamelToHungarian,_fnLanguageCompat,_fnBrowserDetect,_fnAddData,_fnAddTr,_fnNodeToDataIndex,_fnNodeToColumnIndex,_fnGetCellData,_fnSetCellData,_fnSplitObjNotation,_fnGetObjectDataFn,_fnSetObjectDataFn,_fnGetDataMaster,_fnClearTable,_fnDeleteIndex,_fnInvalidate,_fnGetRowElements,_fnCreateTr,_fnBuildHead,_fnDrawHead,_fnDraw,_fnReDraw,_fnAddOptionsHtml,_fnDetectHeader,_fnGetUniqueThs,_fnFeatureHtmlFilter,_fnFilterComplete,_fnFilterCustom,_fnFilterColumn,_fnFilter,_fnFilterCreateSearch,_fnEscapeRegex,_fnFilterData,_fnFeatureHtmlInfo,_fnUpdateInfo,_fnInfoMacros,_fnInitialise,_fnInitComplete,_fnLengthChange,_fnFeatureHtmlLength,_fnFeatureHtmlPaginate,_fnPageChange,_fnFeatureHtmlProcessing,_fnProcessingDisplay,_fnFeatureHtmlTable,_fnScrollDraw,_fnApplyToChildren,_fnCalculateColumnWidths,_fnThrottle,_fnConvertToWidth,_fnGetWidestNode,_fnGetMaxLenString,_fnStringToCss,_fnSortFlatten,_fnSort,_fnSortAria,_fnSortListener,_fnSortAttachListener,_fnSortingClasses,_fnSortData,_fnSaveState,_fnLoadState,_fnSettingsFromNode,_fnLog,_fnMap,_fnBindAction,_fnCallbackReg,_fnCallbackFire,_fnLengthOverflow,_fnRenderer,_fnDataSource,_fnRowAttributes*/

(function( factory ) {
	"use strict";

	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery'], function ( $ ) {
			return factory( $, window, document );
		} );
	}
	else if ( typeof exports === 'object' ) {
		// CommonJS
		module.exports = function (root, $) {
			if ( ! root ) {
				// CommonJS environments without a window global must pass a
				// root. This will give an error otherwise
				root = window;
			}

			if ( ! $ ) {
				$ = typeof window !== 'undefined' ? // jQuery's factory checks for a global window
					require('jquery') :
					require('jquery')( root );
			}

			return factory( $, root, root.document );
		};
	}
	else {
		// Browser
		factory( jQuery, window, document );
	}
}
(function( $, window, document, undefined ) {
	"use strict";

	/**
	 * DataTables is a plug-in for the jQuery Javascript library. It is a highly
	 * flexible tool, based upon the foundations of progressive enhancement,
	 * which will add advanced interaction controls to any HTML table. For a
	 * full list of features please refer to
	 * [DataTables.net](href="http://datatables.net).
	 *
	 * Note that the `DataTable` object is not a global variable but is aliased
	 * to `jQuery.fn.DataTable` and `jQuery.fn.dataTable` through which it may
	 * be  accessed.
	 *
	 *  @class
	 *  @param {object} [init={}] Configuration object for DataTables. Options
	 *    are defined by {@link DataTable.defaults}
	 *  @requires jQuery 1.7+
	 *
	 *  @example
	 *    // Basic initialisation
	 *    $(document).ready( function {
	 *      $('#example').dataTable();
	 *    } );
	 *
	 *  @example
	 *    // Initialisation with configuration options - in this case, disable
	 *    // pagination and sorting.
	 *    $(document).ready( function {
	 *      $('#example').dataTable( {
	 *        "paginate": false,
	 *        "sort": false
	 *      } );
	 *    } );
	 */
	var DataTable = function ( options )
	{
		/**
		 * Perform a jQuery selector action on the table's TR elements (from the tbody) and
		 * return the resulting jQuery object.
		 *  @param {string|node|jQuery} sSelector jQuery selector or node collection to act on
		 *  @param {object} [oOpts] Optional parameters for modifying the rows to be included
		 *  @param {string} [oOpts.filter=none] Select TR elements that meet the current filter
		 *    criterion ("applied") or all TR elements (i.e. no filter).
		 *  @param {string} [oOpts.order=current] Order of the TR elements in the processed array.
		 *    Can be either 'current', whereby the current sorting of the table is used, or
		 *    'original' whereby the original order the data was read into the table is used.
		 *  @param {string} [oOpts.page=all] Limit the selection to the currently displayed page
		 *    ("current") or not ("all"). If 'current' is given, then order is assumed to be
		 *    'current' and filter is 'applied', regardless of what they might be given as.
		 *  @returns {object} jQuery object, filtered by the given selector.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Highlight every second row
		 *      oTable.$('tr:odd').css('backgroundColor', 'blue');
		 *    } );
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Filter to rows with 'Webkit' in them, add a background colour and then
		 *      // remove the filter, thus highlighting the 'Webkit' rows only.
		 *      oTable.fnFilter('Webkit');
		 *      oTable.$('tr', {"search": "applied"}).css('backgroundColor', 'blue');
		 *      oTable.fnFilter('');
		 *    } );
		 */
		this.$ = function ( sSelector, oOpts )
		{
			return this.api(true).$( sSelector, oOpts );
		};
		
		
		/**
		 * Almost identical to $ in operation, but in this case returns the data for the matched
		 * rows - as such, the jQuery selector used should match TR row nodes or TD/TH cell nodes
		 * rather than any descendants, so the data can be obtained for the row/cell. If matching
		 * rows are found, the data returned is the original data array/object that was used to
		 * create the row (or a generated array if from a DOM source).
		 *
		 * This method is often useful in-combination with $ where both functions are given the
		 * same parameters and the array indexes will match identically.
		 *  @param {string|node|jQuery} sSelector jQuery selector or node collection to act on
		 *  @param {object} [oOpts] Optional parameters for modifying the rows to be included
		 *  @param {string} [oOpts.filter=none] Select elements that meet the current filter
		 *    criterion ("applied") or all elements (i.e. no filter).
		 *  @param {string} [oOpts.order=current] Order of the data in the processed array.
		 *    Can be either 'current', whereby the current sorting of the table is used, or
		 *    'original' whereby the original order the data was read into the table is used.
		 *  @param {string} [oOpts.page=all] Limit the selection to the currently displayed page
		 *    ("current") or not ("all"). If 'current' is given, then order is assumed to be
		 *    'current' and filter is 'applied', regardless of what they might be given as.
		 *  @returns {array} Data for the matched elements. If any elements, as a result of the
		 *    selector, were not TR, TD or TH elements in the DataTable, they will have a null
		 *    entry in the array.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Get the data from the first row in the table
		 *      var data = oTable._('tr:first');
		 *
		 *      // Do something useful with the data
		 *      alert( "First cell is: "+data[0] );
		 *    } );
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Filter to 'Webkit' and get all data for
		 *      oTable.fnFilter('Webkit');
		 *      var data = oTable._('tr', {"search": "applied"});
		 *
		 *      // Do something with the data
		 *      alert( data.length+" rows matched the search" );
		 *    } );
		 */
		this._ = function ( sSelector, oOpts )
		{
			return this.api(true).rows( sSelector, oOpts ).data();
		};
		
		
		/**
		 * Create a DataTables Api instance, with the currently selected tables for
		 * the Api's context.
		 * @param {boolean} [traditional=false] Set the API instance's context to be
		 *   only the table referred to by the `DataTable.ext.iApiIndex` option, as was
		 *   used in the API presented by DataTables 1.9- (i.e. the traditional mode),
		 *   or if all tables captured in the jQuery object should be used.
		 * @return {DataTables.Api}
		 */
		this.api = function ( traditional )
		{
			return traditional ?
				new _Api(
					_fnSettingsFromNode( this[ _ext.iApiIndex ] )
				) :
				new _Api( this );
		};
		
		
		/**
		 * Add a single new row or multiple rows of data to the table. Please note
		 * that this is suitable for client-side processing only - if you are using
		 * server-side processing (i.e. "bServerSide": true), then to add data, you
		 * must add it to the data source, i.e. the server-side, through an Ajax call.
		 *  @param {array|object} data The data to be added to the table. This can be:
		 *    <ul>
		 *      <li>1D array of data - add a single row with the data provided</li>
		 *      <li>2D array of arrays - add multiple rows in a single call</li>
		 *      <li>object - data object when using <i>mData</i></li>
		 *      <li>array of objects - multiple data objects when using <i>mData</i></li>
		 *    </ul>
		 *  @param {bool} [redraw=true] redraw the table or not
		 *  @returns {array} An array of integers, representing the list of indexes in
		 *    <i>aoData</i> ({@link DataTable.models.oSettings}) that have been added to
		 *    the table.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    // Global var for counter
		 *    var giCount = 2;
		 *
		 *    $(document).ready(function() {
		 *      $('#example').dataTable();
		 *    } );
		 *
		 *    function fnClickAddRow() {
		 *      $('#example').dataTable().fnAddData( [
		 *        giCount+".1",
		 *        giCount+".2",
		 *        giCount+".3",
		 *        giCount+".4" ]
		 *      );
		 *
		 *      giCount++;
		 *    }
		 */
		this.fnAddData = function( data, redraw )
		{
			var api = this.api( true );
		
			/* Check if we want to add multiple rows or not */
			var rows = $.isArray(data) && ( $.isArray(data[0]) || $.isPlainObject(data[0]) ) ?
				api.rows.add( data ) :
				api.row.add( data );
		
			if ( redraw === undefined || redraw ) {
				api.draw();
			}
		
			return rows.flatten().toArray();
		};
		
		
		/**
		 * This function will make DataTables recalculate the column sizes, based on the data
		 * contained in the table and the sizes applied to the columns (in the DOM, CSS or
		 * through the sWidth parameter). This can be useful when the width of the table's
		 * parent element changes (for example a window resize).
		 *  @param {boolean} [bRedraw=true] Redraw the table or not, you will typically want to
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable( {
		 *        "sScrollY": "200px",
		 *        "bPaginate": false
		 *      } );
		 *
		 *      $(window).on('resize', function () {
		 *        oTable.fnAdjustColumnSizing();
		 *      } );
		 *    } );
		 */
		this.fnAdjustColumnSizing = function ( bRedraw )
		{
			var api = this.api( true ).columns.adjust();
			var settings = api.settings()[0];
			var scroll = settings.oScroll;
		
			if ( bRedraw === undefined || bRedraw ) {
				api.draw( false );
			}
			else if ( scroll.sX !== "" || scroll.sY !== "" ) {
				/* If not redrawing, but scrolling, we want to apply the new column sizes anyway */
				_fnScrollDraw( settings );
			}
		};
		
		
		/**
		 * Quickly and simply clear a table
		 *  @param {bool} [bRedraw=true] redraw the table or not
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Immediately 'nuke' the current rows (perhaps waiting for an Ajax callback...)
		 *      oTable.fnClearTable();
		 *    } );
		 */
		this.fnClearTable = function( bRedraw )
		{
			var api = this.api( true ).clear();
		
			if ( bRedraw === undefined || bRedraw ) {
				api.draw();
			}
		};
		
		
		/**
		 * The exact opposite of 'opening' a row, this function will close any rows which
		 * are currently 'open'.
		 *  @param {node} nTr the table row to 'close'
		 *  @returns {int} 0 on success, or 1 if failed (can't find the row)
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable;
		 *
		 *      // 'open' an information row when a row is clicked on
		 *      $('#example tbody tr').click( function () {
		 *        if ( oTable.fnIsOpen(this) ) {
		 *          oTable.fnClose( this );
		 *        } else {
		 *          oTable.fnOpen( this, "Temporary row opened", "info_row" );
		 *        }
		 *      } );
		 *
		 *      oTable = $('#example').dataTable();
		 *    } );
		 */
		this.fnClose = function( nTr )
		{
			this.api( true ).row( nTr ).child.hide();
		};
		
		
		/**
		 * Remove a row for the table
		 *  @param {mixed} target The index of the row from aoData to be deleted, or
		 *    the TR element you want to delete
		 *  @param {function|null} [callBack] Callback function
		 *  @param {bool} [redraw=true] Redraw the table or not
		 *  @returns {array} The row that was deleted
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Immediately remove the first row
		 *      oTable.fnDeleteRow( 0 );
		 *    } );
		 */
		this.fnDeleteRow = function( target, callback, redraw )
		{
			var api = this.api( true );
			var rows = api.rows( target );
			var settings = rows.settings()[0];
			var data = settings.aoData[ rows[0][0] ];
		
			rows.remove();
		
			if ( callback ) {
				callback.call( this, settings, data );
			}
		
			if ( redraw === undefined || redraw ) {
				api.draw();
			}
		
			return data;
		};
		
		
		/**
		 * Restore the table to it's original state in the DOM by removing all of DataTables
		 * enhancements, alterations to the DOM structure of the table and event listeners.
		 *  @param {boolean} [remove=false] Completely remove the table from the DOM
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      // This example is fairly pointless in reality, but shows how fnDestroy can be used
		 *      var oTable = $('#example').dataTable();
		 *      oTable.fnDestroy();
		 *    } );
		 */
		this.fnDestroy = function ( remove )
		{
			this.api( true ).destroy( remove );
		};
		
		
		/**
		 * Redraw the table
		 *  @param {bool} [complete=true] Re-filter and resort (if enabled) the table before the draw.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Re-draw the table - you wouldn't want to do it here, but it's an example :-)
		 *      oTable.fnDraw();
		 *    } );
		 */
		this.fnDraw = function( complete )
		{
			// Note that this isn't an exact match to the old call to _fnDraw - it takes
			// into account the new data, but can hold position.
			this.api( true ).draw( complete );
		};
		
		
		/**
		 * Filter the input based on data
		 *  @param {string} sInput String to filter the table on
		 *  @param {int|null} [iColumn] Column to limit filtering to
		 *  @param {bool} [bRegex=false] Treat as regular expression or not
		 *  @param {bool} [bSmart=true] Perform smart filtering or not
		 *  @param {bool} [bShowGlobal=true] Show the input global filter in it's input box(es)
		 *  @param {bool} [bCaseInsensitive=true] Do case-insensitive matching (true) or not (false)
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Sometime later - filter...
		 *      oTable.fnFilter( 'test string' );
		 *    } );
		 */
		this.fnFilter = function( sInput, iColumn, bRegex, bSmart, bShowGlobal, bCaseInsensitive )
		{
			var api = this.api( true );
		
			if ( iColumn === null || iColumn === undefined ) {
				api.search( sInput, bRegex, bSmart, bCaseInsensitive );
			}
			else {
				api.column( iColumn ).search( sInput, bRegex, bSmart, bCaseInsensitive );
			}
		
			api.draw();
		};
		
		
		/**
		 * Get the data for the whole table, an individual row or an individual cell based on the
		 * provided parameters.
		 *  @param {int|node} [src] A TR row node, TD/TH cell node or an integer. If given as
		 *    a TR node then the data source for the whole row will be returned. If given as a
		 *    TD/TH cell node then iCol will be automatically calculated and the data for the
		 *    cell returned. If given as an integer, then this is treated as the aoData internal
		 *    data index for the row (see fnGetPosition) and the data for that row used.
		 *  @param {int} [col] Optional column index that you want the data of.
		 *  @returns {array|object|string} If mRow is undefined, then the data for all rows is
		 *    returned. If mRow is defined, just data for that row, and is iCol is
		 *    defined, only data for the designated cell is returned.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    // Row data
		 *    $(document).ready(function() {
		 *      oTable = $('#example').dataTable();
		 *
		 *      oTable.$('tr').click( function () {
		 *        var data = oTable.fnGetData( this );
		 *        // ... do something with the array / object of data for the row
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Individual cell data
		 *    $(document).ready(function() {
		 *      oTable = $('#example').dataTable();
		 *
		 *      oTable.$('td').click( function () {
		 *        var sData = oTable.fnGetData( this );
		 *        alert( 'The cell clicked on had the value of '+sData );
		 *      } );
		 *    } );
		 */
		this.fnGetData = function( src, col )
		{
			var api = this.api( true );
		
			if ( src !== undefined ) {
				var type = src.nodeName ? src.nodeName.toLowerCase() : '';
		
				return col !== undefined || type == 'td' || type == 'th' ?
					api.cell( src, col ).data() :
					api.row( src ).data() || null;
			}
		
			return api.data().toArray();
		};
		
		
		/**
		 * Get an array of the TR nodes that are used in the table's body. Note that you will
		 * typically want to use the '$' API method in preference to this as it is more
		 * flexible.
		 *  @param {int} [iRow] Optional row index for the TR element you want
		 *  @returns {array|node} If iRow is undefined, returns an array of all TR elements
		 *    in the table's body, or iRow is defined, just the TR element requested.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Get the nodes from the table
		 *      var nNodes = oTable.fnGetNodes( );
		 *    } );
		 */
		this.fnGetNodes = function( iRow )
		{
			var api = this.api( true );
		
			return iRow !== undefined ?
				api.row( iRow ).node() :
				api.rows().nodes().flatten().toArray();
		};
		
		
		/**
		 * Get the array indexes of a particular cell from it's DOM element
		 * and column index including hidden columns
		 *  @param {node} node this can either be a TR, TD or TH in the table's body
		 *  @returns {int} If nNode is given as a TR, then a single index is returned, or
		 *    if given as a cell, an array of [row index, column index (visible),
		 *    column index (all)] is given.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      $('#example tbody td').click( function () {
		 *        // Get the position of the current data from the node
		 *        var aPos = oTable.fnGetPosition( this );
		 *
		 *        // Get the data array for this row
		 *        var aData = oTable.fnGetData( aPos[0] );
		 *
		 *        // Update the data array and return the value
		 *        aData[ aPos[1] ] = 'clicked';
		 *        this.innerHTML = 'clicked';
		 *      } );
		 *
		 *      // Init DataTables
		 *      oTable = $('#example').dataTable();
		 *    } );
		 */
		this.fnGetPosition = function( node )
		{
			var api = this.api( true );
			var nodeName = node.nodeName.toUpperCase();
		
			if ( nodeName == 'TR' ) {
				return api.row( node ).index();
			}
			else if ( nodeName == 'TD' || nodeName == 'TH' ) {
				var cell = api.cell( node ).index();
		
				return [
					cell.row,
					cell.columnVisible,
					cell.column
				];
			}
			return null;
		};
		
		
		/**
		 * Check to see if a row is 'open' or not.
		 *  @param {node} nTr the table row to check
		 *  @returns {boolean} true if the row is currently open, false otherwise
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable;
		 *
		 *      // 'open' an information row when a row is clicked on
		 *      $('#example tbody tr').click( function () {
		 *        if ( oTable.fnIsOpen(this) ) {
		 *          oTable.fnClose( this );
		 *        } else {
		 *          oTable.fnOpen( this, "Temporary row opened", "info_row" );
		 *        }
		 *      } );
		 *
		 *      oTable = $('#example').dataTable();
		 *    } );
		 */
		this.fnIsOpen = function( nTr )
		{
			return this.api( true ).row( nTr ).child.isShown();
		};
		
		
		/**
		 * This function will place a new row directly after a row which is currently
		 * on display on the page, with the HTML contents that is passed into the
		 * function. This can be used, for example, to ask for confirmation that a
		 * particular record should be deleted.
		 *  @param {node} nTr The table row to 'open'
		 *  @param {string|node|jQuery} mHtml The HTML to put into the row
		 *  @param {string} sClass Class to give the new TD cell
		 *  @returns {node} The row opened. Note that if the table row passed in as the
		 *    first parameter, is not found in the table, this method will silently
		 *    return.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable;
		 *
		 *      // 'open' an information row when a row is clicked on
		 *      $('#example tbody tr').click( function () {
		 *        if ( oTable.fnIsOpen(this) ) {
		 *          oTable.fnClose( this );
		 *        } else {
		 *          oTable.fnOpen( this, "Temporary row opened", "info_row" );
		 *        }
		 *      } );
		 *
		 *      oTable = $('#example').dataTable();
		 *    } );
		 */
		this.fnOpen = function( nTr, mHtml, sClass )
		{
			return this.api( true )
				.row( nTr )
				.child( mHtml, sClass )
				.show()
				.child()[0];
		};
		
		
		/**
		 * Change the pagination - provides the internal logic for pagination in a simple API
		 * function. With this function you can have a DataTables table go to the next,
		 * previous, first or last pages.
		 *  @param {string|int} mAction Paging action to take: "first", "previous", "next" or "last"
		 *    or page number to jump to (integer), note that page 0 is the first page.
		 *  @param {bool} [bRedraw=true] Redraw the table or not
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *      oTable.fnPageChange( 'next' );
		 *    } );
		 */
		this.fnPageChange = function ( mAction, bRedraw )
		{
			var api = this.api( true ).page( mAction );
		
			if ( bRedraw === undefined || bRedraw ) {
				api.draw(false);
			}
		};
		
		
		/**
		 * Show a particular column
		 *  @param {int} iCol The column whose display should be changed
		 *  @param {bool} bShow Show (true) or hide (false) the column
		 *  @param {bool} [bRedraw=true] Redraw the table or not
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Hide the second column after initialisation
		 *      oTable.fnSetColumnVis( 1, false );
		 *    } );
		 */
		this.fnSetColumnVis = function ( iCol, bShow, bRedraw )
		{
			var api = this.api( true ).column( iCol ).visible( bShow );
		
			if ( bRedraw === undefined || bRedraw ) {
				api.columns.adjust().draw();
			}
		};
		
		
		/**
		 * Get the settings for a particular table for external manipulation
		 *  @returns {object} DataTables settings object. See
		 *    {@link DataTable.models.oSettings}
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *      var oSettings = oTable.fnSettings();
		 *
		 *      // Show an example parameter from the settings
		 *      alert( oSettings._iDisplayStart );
		 *    } );
		 */
		this.fnSettings = function()
		{
			return _fnSettingsFromNode( this[_ext.iApiIndex] );
		};
		
		
		/**
		 * Sort the table by a particular column
		 *  @param {int} iCol the data index to sort on. Note that this will not match the
		 *    'display index' if you have hidden data entries
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Sort immediately with columns 0 and 1
		 *      oTable.fnSort( [ [0,'asc'], [1,'asc'] ] );
		 *    } );
		 */
		this.fnSort = function( aaSort )
		{
			this.api( true ).order( aaSort ).draw();
		};
		
		
		/**
		 * Attach a sort listener to an element for a given column
		 *  @param {node} nNode the element to attach the sort listener to
		 *  @param {int} iColumn the column that a click on this node will sort on
		 *  @param {function} [fnCallback] callback function when sort is run
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Sort on column 1, when 'sorter' is clicked on
		 *      oTable.fnSortListener( document.getElementById('sorter'), 1 );
		 *    } );
		 */
		this.fnSortListener = function( nNode, iColumn, fnCallback )
		{
			this.api( true ).order.listener( nNode, iColumn, fnCallback );
		};
		
		
		/**
		 * Update a table cell or row - this method will accept either a single value to
		 * update the cell with, an array of values with one element for each column or
		 * an object in the same format as the original data source. The function is
		 * self-referencing in order to make the multi column updates easier.
		 *  @param {object|array|string} mData Data to update the cell/row with
		 *  @param {node|int} mRow TR element you want to update or the aoData index
		 *  @param {int} [iColumn] The column to update, give as null or undefined to
		 *    update a whole row.
		 *  @param {bool} [bRedraw=true] Redraw the table or not
		 *  @param {bool} [bAction=true] Perform pre-draw actions or not
		 *  @returns {int} 0 on success, 1 on error
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *      oTable.fnUpdate( 'Example update', 0, 0 ); // Single cell
		 *      oTable.fnUpdate( ['a', 'b', 'c', 'd', 'e'], $('tbody tr')[0] ); // Row
		 *    } );
		 */
		this.fnUpdate = function( mData, mRow, iColumn, bRedraw, bAction )
		{
			var api = this.api( true );
		
			if ( iColumn === undefined || iColumn === null ) {
				api.row( mRow ).data( mData );
			}
			else {
				api.cell( mRow, iColumn ).data( mData );
			}
		
			if ( bAction === undefined || bAction ) {
				api.columns.adjust();
			}
		
			if ( bRedraw === undefined || bRedraw ) {
				api.draw();
			}
			return 0;
		};
		
		
		/**
		 * Provide a common method for plug-ins to check the version of DataTables being used, in order
		 * to ensure compatibility.
		 *  @param {string} sVersion Version string to check for, in the format "X.Y.Z". Note that the
		 *    formats "X" and "X.Y" are also acceptable.
		 *  @returns {boolean} true if this version of DataTables is greater or equal to the required
		 *    version, or false if this version of DataTales is not suitable
		 *  @method
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *      alert( oTable.fnVersionCheck( '1.9.0' ) );
		 *    } );
		 */
		this.fnVersionCheck = _ext.fnVersionCheck;
		

		var _that = this;
		var emptyInit = options === undefined;
		var len = this.length;

		if ( emptyInit ) {
			options = {};
		}

		this.oApi = this.internal = _ext.internal;

		// Extend with old style plug-in API methods
		for ( var fn in DataTable.ext.internal ) {
			if ( fn ) {
				this[fn] = _fnExternApiFunc(fn);
			}
		}

		this.each(function() {
			// For each initialisation we want to give it a clean initialisation
			// object that can be bashed around
			var o = {};
			var oInit = len > 1 ? // optimisation for single table case
				_fnExtend( o, options, true ) :
				options;

			/*global oInit,_that,emptyInit*/
			var i=0, iLen, j, jLen, k, kLen;
			var sId = this.getAttribute( 'id' );
			var bInitHandedOff = false;
			var defaults = DataTable.defaults;
			var $this = $(this);
			
			
			/* Sanity check */
			if ( this.nodeName.toLowerCase() != 'table' )
			{
				_fnLog( null, 0, 'Non-table node initialisation ('+this.nodeName+')', 2 );
				return;
			}
			
			/* Backwards compatibility for the defaults */
			_fnCompatOpts( defaults );
			_fnCompatCols( defaults.column );
			
			/* Convert the camel-case defaults to Hungarian */
			_fnCamelToHungarian( defaults, defaults, true );
			_fnCamelToHungarian( defaults.column, defaults.column, true );
			
			/* Setting up the initialisation object */
			_fnCamelToHungarian( defaults, $.extend( oInit, $this.data() ) );
			
			
			
			/* Check to see if we are re-initialising a table */
			var allSettings = DataTable.settings;
			for ( i=0, iLen=allSettings.length ; i<iLen ; i++ )
			{
				var s = allSettings[i];
			
				/* Base check on table node */
				if (
					s.nTable == this ||
					(s.nTHead && s.nTHead.parentNode == this) ||
					(s.nTFoot && s.nTFoot.parentNode == this)
				) {
					var bRetrieve = oInit.bRetrieve !== undefined ? oInit.bRetrieve : defaults.bRetrieve;
					var bDestroy = oInit.bDestroy !== undefined ? oInit.bDestroy : defaults.bDestroy;
			
					if ( emptyInit || bRetrieve )
					{
						return s.oInstance;
					}
					else if ( bDestroy )
					{
						s.oInstance.fnDestroy();
						break;
					}
					else
					{
						_fnLog( s, 0, 'Cannot reinitialise DataTable', 3 );
						return;
					}
				}
			
				/* If the element we are initialising has the same ID as a table which was previously
				 * initialised, but the table nodes don't match (from before) then we destroy the old
				 * instance by simply deleting it. This is under the assumption that the table has been
				 * destroyed by other methods. Anyone using non-id selectors will need to do this manually
				 */
				if ( s.sTableId == this.id )
				{
					allSettings.splice( i, 1 );
					break;
				}
			}
			
			/* Ensure the table has an ID - required for accessibility */
			if ( sId === null || sId === "" )
			{
				sId = "DataTables_Table_"+(DataTable.ext._unique++);
				this.id = sId;
			}
			
			/* Create the settings object for this table and set some of the default parameters */
			var oSettings = $.extend( true, {}, DataTable.models.oSettings, {
				"sDestroyWidth": $this[0].style.width,
				"sInstance":     sId,
				"sTableId":      sId
			} );
			oSettings.nTable = this;
			oSettings.oApi   = _that.internal;
			oSettings.oInit  = oInit;
			
			allSettings.push( oSettings );
			
			// Need to add the instance after the instance after the settings object has been added
			// to the settings array, so we can self reference the table instance if more than one
			oSettings.oInstance = (_that.length===1) ? _that : $this.dataTable();
			
			// Backwards compatibility, before we apply all the defaults
			_fnCompatOpts( oInit );
			_fnLanguageCompat( oInit.oLanguage );
			
			// If the length menu is given, but the init display length is not, use the length menu
			if ( oInit.aLengthMenu && ! oInit.iDisplayLength )
			{
				oInit.iDisplayLength = $.isArray( oInit.aLengthMenu[0] ) ?
					oInit.aLengthMenu[0][0] : oInit.aLengthMenu[0];
			}
			
			// Apply the defaults and init options to make a single init object will all
			// options defined from defaults and instance options.
			oInit = _fnExtend( $.extend( true, {}, defaults ), oInit );
			
			
			// Map the initialisation options onto the settings object
			_fnMap( oSettings.oFeatures, oInit, [
				"bPaginate",
				"bLengthChange",
				"bFilter",
				"bSort",
				"bSortMulti",
				"bInfo",
				"bProcessing",
				"bAutoWidth",
				"bSortClasses",
				"bServerSide",
				"bDeferRender"
			] );
			_fnMap( oSettings, oInit, [
				"asStripeClasses",
				"ajax",
				"fnServerData",
				"fnFormatNumber",
				"sServerMethod",
				"aaSorting",
				"aaSortingFixed",
				"aLengthMenu",
				"sPaginationType",
				"sAjaxSource",
				"sAjaxDataProp",
				"iStateDuration",
				"sDom",
				"bSortCellsTop",
				"iTabIndex",
				"fnStateLoadCallback",
				"fnStateSaveCallback",
				"renderer",
				"searchDelay",
				"rowId",
				[ "iCookieDuration", "iStateDuration" ], // backwards compat
				[ "oSearch", "oPreviousSearch" ],
				[ "aoSearchCols", "aoPreSearchCols" ],
				[ "iDisplayLength", "_iDisplayLength" ]
			] );
			_fnMap( oSettings.oScroll, oInit, [
				[ "sScrollX", "sX" ],
				[ "sScrollXInner", "sXInner" ],
				[ "sScrollY", "sY" ],
				[ "bScrollCollapse", "bCollapse" ]
			] );
			_fnMap( oSettings.oLanguage, oInit, "fnInfoCallback" );
			
			/* Callback functions which are array driven */
			_fnCallbackReg( oSettings, 'aoDrawCallback',       oInit.fnDrawCallback,      'user' );
			_fnCallbackReg( oSettings, 'aoServerParams',       oInit.fnServerParams,      'user' );
			_fnCallbackReg( oSettings, 'aoStateSaveParams',    oInit.fnStateSaveParams,   'user' );
			_fnCallbackReg( oSettings, 'aoStateLoadParams',    oInit.fnStateLoadParams,   'user' );
			_fnCallbackReg( oSettings, 'aoStateLoaded',        oInit.fnStateLoaded,       'user' );
			_fnCallbackReg( oSettings, 'aoRowCallback',        oInit.fnRowCallback,       'user' );
			_fnCallbackReg( oSettings, 'aoRowCreatedCallback', oInit.fnCreatedRow,        'user' );
			_fnCallbackReg( oSettings, 'aoHeaderCallback',     oInit.fnHeaderCallback,    'user' );
			_fnCallbackReg( oSettings, 'aoFooterCallback',     oInit.fnFooterCallback,    'user' );
			_fnCallbackReg( oSettings, 'aoInitComplete',       oInit.fnInitComplete,      'user' );
			_fnCallbackReg( oSettings, 'aoPreDrawCallback',    oInit.fnPreDrawCallback,   'user' );
			
			oSettings.rowIdFn = _fnGetObjectDataFn( oInit.rowId );
			
			/* Browser support detection */
			_fnBrowserDetect( oSettings );
			
			var oClasses = oSettings.oClasses;
			
			$.extend( oClasses, DataTable.ext.classes, oInit.oClasses );
			$this.addClass( oClasses.sTable );
			
			
			if ( oSettings.iInitDisplayStart === undefined )
			{
				/* Display start point, taking into account the save saving */
				oSettings.iInitDisplayStart = oInit.iDisplayStart;
				oSettings._iDisplayStart = oInit.iDisplayStart;
			}
			
			if ( oInit.iDeferLoading !== null )
			{
				oSettings.bDeferLoading = true;
				var tmp = $.isArray( oInit.iDeferLoading );
				oSettings._iRecordsDisplay = tmp ? oInit.iDeferLoading[0] : oInit.iDeferLoading;
				oSettings._iRecordsTotal = tmp ? oInit.iDeferLoading[1] : oInit.iDeferLoading;
			}
			
			/* Language definitions */
			var oLanguage = oSettings.oLanguage;
			$.extend( true, oLanguage, oInit.oLanguage );
			
			if ( oLanguage.sUrl )
			{
				/* Get the language definitions from a file - because this Ajax call makes the language
				 * get async to the remainder of this function we use bInitHandedOff to indicate that
				 * _fnInitialise will be fired by the returned Ajax handler, rather than the constructor
				 */
				$.ajax( {
					dataType: 'json',
					url: oLanguage.sUrl,
					success: function ( json ) {
						_fnLanguageCompat( json );
						_fnCamelToHungarian( defaults.oLanguage, json );
						$.extend( true, oLanguage, json );
						_fnInitialise( oSettings );
					},
					error: function () {
						// Error occurred loading language file, continue on as best we can
						_fnInitialise( oSettings );
					}
				} );
				bInitHandedOff = true;
			}
			
			/*
			 * Stripes
			 */
			if ( oInit.asStripeClasses === null )
			{
				oSettings.asStripeClasses =[
					oClasses.sStripeOdd,
					oClasses.sStripeEven
				];
			}
			
			/* Remove row stripe classes if they are already on the table row */
			var stripeClasses = oSettings.asStripeClasses;
			var rowOne = $this.children('tbody').find('tr').eq(0);
			if ( $.inArray( true, $.map( stripeClasses, function(el, i) {
				return rowOne.hasClass(el);
			} ) ) !== -1 ) {
				$('tbody tr', this).removeClass( stripeClasses.join(' ') );
				oSettings.asDestroyStripes = stripeClasses.slice();
			}
			
			/*
			 * Columns
			 * See if we should load columns automatically or use defined ones
			 */
			var anThs = [];
			var aoColumnsInit;
			var nThead = this.getElementsByTagName('thead');
			if ( nThead.length !== 0 )
			{
				_fnDetectHeader( oSettings.aoHeader, nThead[0] );
				anThs = _fnGetUniqueThs( oSettings );
			}
			
			/* If not given a column array, generate one with nulls */
			if ( oInit.aoColumns === null )
			{
				aoColumnsInit = [];
				for ( i=0, iLen=anThs.length ; i<iLen ; i++ )
				{
					aoColumnsInit.push( null );
				}
			}
			else
			{
				aoColumnsInit = oInit.aoColumns;
			}
			
			/* Add the columns */
			for ( i=0, iLen=aoColumnsInit.length ; i<iLen ; i++ )
			{
				_fnAddColumn( oSettings, anThs ? anThs[i] : null );
			}
			
			/* Apply the column definitions */
			_fnApplyColumnDefs( oSettings, oInit.aoColumnDefs, aoColumnsInit, function (iCol, oDef) {
				_fnColumnOptions( oSettings, iCol, oDef );
			} );
			
			/* HTML5 attribute detection - build an mData object automatically if the
			 * attributes are found
			 */
			if ( rowOne.length ) {
				var a = function ( cell, name ) {
					return cell.getAttribute( 'data-'+name ) !== null ? name : null;
				};
			
				$( rowOne[0] ).children('th, td').each( function (i, cell) {
					var col = oSettings.aoColumns[i];
			
					if ( col.mData === i ) {
						var sort = a( cell, 'sort' ) || a( cell, 'order' );
						var filter = a( cell, 'filter' ) || a( cell, 'search' );
			
						if ( sort !== null || filter !== null ) {
							col.mData = {
								_:      i+'.display',
								sort:   sort !== null   ? i+'.@data-'+sort   : undefined,
								type:   sort !== null   ? i+'.@data-'+sort   : undefined,
								filter: filter !== null ? i+'.@data-'+filter : undefined
							};
			
							_fnColumnOptions( oSettings, i );
						}
					}
				} );
			}
			
			var features = oSettings.oFeatures;
			var loadedInit = function () {
				/*
				 * Sorting
				 * @todo For modularisation (1.11) this needs to do into a sort start up handler
				 */
			
				// If aaSorting is not defined, then we use the first indicator in asSorting
				// in case that has been altered, so the default sort reflects that option
				if ( oInit.aaSorting === undefined ) {
					var sorting = oSettings.aaSorting;
					for ( i=0, iLen=sorting.length ; i<iLen ; i++ ) {
						sorting[i][1] = oSettings.aoColumns[ i ].asSorting[0];
					}
				}
			
				/* Do a first pass on the sorting classes (allows any size changes to be taken into
				 * account, and also will apply sorting disabled classes if disabled
				 */
				_fnSortingClasses( oSettings );
			
				if ( features.bSort ) {
					_fnCallbackReg( oSettings, 'aoDrawCallback', function () {
						if ( oSettings.bSorted ) {
							var aSort = _fnSortFlatten( oSettings );
							var sortedColumns = {};
			
							$.each( aSort, function (i, val) {
								sortedColumns[ val.src ] = val.dir;
							} );
			
							_fnCallbackFire( oSettings, null, 'order', [oSettings, aSort, sortedColumns] );
							_fnSortAria( oSettings );
						}
					} );
				}
			
				_fnCallbackReg( oSettings, 'aoDrawCallback', function () {
					if ( oSettings.bSorted || _fnDataSource( oSettings ) === 'ssp' || features.bDeferRender ) {
						_fnSortingClasses( oSettings );
					}
				}, 'sc' );
			
			
				/*
				 * Final init
				 * Cache the header, body and footer as required, creating them if needed
				 */
			
				// Work around for Webkit bug 83867 - store the caption-side before removing from doc
				var captions = $this.children('caption').each( function () {
					this._captionSide = $(this).css('caption-side');
				} );
			
				var thead = $this.children('thead');
				if ( thead.length === 0 ) {
					thead = $('<thead/>').appendTo($this);
				}
				oSettings.nTHead = thead[0];
			
				var tbody = $this.children('tbody');
				if ( tbody.length === 0 ) {
					tbody = $('<tbody/>').appendTo($this);
				}
				oSettings.nTBody = tbody[0];
			
				var tfoot = $this.children('tfoot');
				if ( tfoot.length === 0 && captions.length > 0 && (oSettings.oScroll.sX !== "" || oSettings.oScroll.sY !== "") ) {
					// If we are a scrolling table, and no footer has been given, then we need to create
					// a tfoot element for the caption element to be appended to
					tfoot = $('<tfoot/>').appendTo($this);
				}
			
				if ( tfoot.length === 0 || tfoot.children().length === 0 ) {
					$this.addClass( oClasses.sNoFooter );
				}
				else if ( tfoot.length > 0 ) {
					oSettings.nTFoot = tfoot[0];
					_fnDetectHeader( oSettings.aoFooter, oSettings.nTFoot );
				}
			
				/* Check if there is data passing into the constructor */
				if ( oInit.aaData ) {
					for ( i=0 ; i<oInit.aaData.length ; i++ ) {
						_fnAddData( oSettings, oInit.aaData[ i ] );
					}
				}
				else if ( oSettings.bDeferLoading || _fnDataSource( oSettings ) == 'dom' ) {
					/* Grab the data from the page - only do this when deferred loading or no Ajax
					 * source since there is no point in reading the DOM data if we are then going
					 * to replace it with Ajax data
					 */
					_fnAddTr( oSettings, $(oSettings.nTBody).children('tr') );
				}
			
				/* Copy the data index array */
				oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
			
				/* Initialisation complete - table can be drawn */
				oSettings.bInitialised = true;
			
				/* Check if we need to initialise the table (it might not have been handed off to the
				 * language processor)
				 */
				if ( bInitHandedOff === false ) {
					_fnInitialise( oSettings );
				}
			};
			
			/* Must be done after everything which can be overridden by the state saving! */
			if ( oInit.bStateSave )
			{
				features.bStateSave = true;
				_fnCallbackReg( oSettings, 'aoDrawCallback', _fnSaveState, 'state_save' );
				_fnLoadState( oSettings, oInit, loadedInit );
			}
			else {
				loadedInit();
			}
			
		} );
		_that = null;
		return this;
	};

	
	/*
	 * It is useful to have variables which are scoped locally so only the
	 * DataTables functions can access them and they don't leak into global space.
	 * At the same time these functions are often useful over multiple files in the
	 * core and API, so we list, or at least document, all variables which are used
	 * by DataTables as private variables here. This also ensures that there is no
	 * clashing of variable names and that they can easily referenced for reuse.
	 */
	
	
	// Defined else where
	//  _selector_run
	//  _selector_opts
	//  _selector_first
	//  _selector_row_indexes
	
	var _ext; // DataTable.ext
	var _Api; // DataTable.Api
	var _api_register; // DataTable.Api.register
	var _api_registerPlural; // DataTable.Api.registerPlural
	
	var _re_dic = {};
	var _re_new_lines = /[\r\n]/g;
	var _re_html = /<.*?>/g;
	
	// This is not strict ISO8601 - Date.parse() is quite lax, although
	// implementations differ between browsers.
	var _re_date = /^\d{2,4}[\.\/\-]\d{1,2}[\.\/\-]\d{1,2}([T ]{1}\d{1,2}[:\.]\d{2}([\.:]\d{2})?)?$/;
	
	// Escape regular expression special characters
	var _re_escape_regex = new RegExp( '(\\' + [ '/', '.', '*', '+', '?', '|', '(', ')', '[', ']', '{', '}', '\\', '$', '^', '-' ].join('|\\') + ')', 'g' );
	
	// http://en.wikipedia.org/wiki/Foreign_exchange_market
	// - \u20BD - Russian ruble.
	// - \u20a9 - South Korean Won
	// - \u20BA - Turkish Lira
	// - \u20B9 - Indian Rupee
	// - R - Brazil (R$) and South Africa
	// - fr - Swiss Franc
	// - kr - Swedish krona, Norwegian krone and Danish krone
	// - \u2009 is thin space and \u202F is narrow no-break space, both used in many
	// - Ƀ - Bitcoin
	// - Ξ - Ethereum
	//   standards as thousands separators.
	var _re_formatted_numeric = /[',$£€¥%\u2009\u202F\u20BD\u20a9\u20BArfkɃΞ]/gi;
	
	
	var _empty = function ( d ) {
		return !d || d === true || d === '-' ? true : false;
	};
	
	
	var _intVal = function ( s ) {
		var integer = parseInt( s, 10 );
		return !isNaN(integer) && isFinite(s) ? integer : null;
	};
	
	// Convert from a formatted number with characters other than `.` as the
	// decimal place, to a Javascript number
	var _numToDecimal = function ( num, decimalPoint ) {
		// Cache created regular expressions for speed as this function is called often
		if ( ! _re_dic[ decimalPoint ] ) {
			_re_dic[ decimalPoint ] = new RegExp( _fnEscapeRegex( decimalPoint ), 'g' );
		}
		return typeof num === 'string' && decimalPoint !== '.' ?
			num.replace( /\./g, '' ).replace( _re_dic[ decimalPoint ], '.' ) :
			num;
	};
	
	
	var _isNumber = function ( d, decimalPoint, formatted ) {
		var strType = typeof d === 'string';
	
		// If empty return immediately so there must be a number if it is a
		// formatted string (this stops the string "k", or "kr", etc being detected
		// as a formatted number for currency
		if ( _empty( d ) ) {
			return true;
		}
	
		if ( decimalPoint && strType ) {
			d = _numToDecimal( d, decimalPoint );
		}
	
		if ( formatted && strType ) {
			d = d.replace( _re_formatted_numeric, '' );
		}
	
		return !isNaN( parseFloat(d) ) && isFinite( d );
	};
	
	
	// A string without HTML in it can be considered to be HTML still
	var _isHtml = function ( d ) {
		return _empty( d ) || typeof d === 'string';
	};
	
	
	var _htmlNumeric = function ( d, decimalPoint, formatted ) {
		if ( _empty( d ) ) {
			return true;
		}
	
		var html = _isHtml( d );
		return ! html ?
			null :
			_isNumber( _stripHtml( d ), decimalPoint, formatted ) ?
				true :
				null;
	};
	
	
	var _pluck = function ( a, prop, prop2 ) {
		var out = [];
		var i=0, ien=a.length;
	
		// Could have the test in the loop for slightly smaller code, but speed
		// is essential here
		if ( prop2 !== undefined ) {
			for ( ; i<ien ; i++ ) {
				if ( a[i] && a[i][ prop ] ) {
					out.push( a[i][ prop ][ prop2 ] );
				}
			}
		}
		else {
			for ( ; i<ien ; i++ ) {
				if ( a[i] ) {
					out.push( a[i][ prop ] );
				}
			}
		}
	
		return out;
	};
	
	
	// Basically the same as _pluck, but rather than looping over `a` we use `order`
	// as the indexes to pick from `a`
	var _pluck_order = function ( a, order, prop, prop2 )
	{
		var out = [];
		var i=0, ien=order.length;
	
		// Could have the test in the loop for slightly smaller code, but speed
		// is essential here
		if ( prop2 !== undefined ) {
			for ( ; i<ien ; i++ ) {
				if ( a[ order[i] ][ prop ] ) {
					out.push( a[ order[i] ][ prop ][ prop2 ] );
				}
			}
		}
		else {
			for ( ; i<ien ; i++ ) {
				out.push( a[ order[i] ][ prop ] );
			}
		}
	
		return out;
	};
	
	
	var _range = function ( len, start )
	{
		var out = [];
		var end;
	
		if ( start === undefined ) {
			start = 0;
			end = len;
		}
		else {
			end = start;
			start = len;
		}
	
		for ( var i=start ; i<end ; i++ ) {
			out.push( i );
		}
	
		return out;
	};
	
	
	var _removeEmpty = function ( a )
	{
		var out = [];
	
		for ( var i=0, ien=a.length ; i<ien ; i++ ) {
			if ( a[i] ) { // careful - will remove all falsy values!
				out.push( a[i] );
			}
		}
	
		return out;
	};
	
	
	var _stripHtml = function ( d ) {
		return d.replace( _re_html, '' );
	};
	
	
	/**
	 * Determine if all values in the array are unique. This means we can short
	 * cut the _unique method at the cost of a single loop. A sorted array is used
	 * to easily check the values.
	 *
	 * @param  {array} src Source array
	 * @return {boolean} true if all unique, false otherwise
	 * @ignore
	 */
	var _areAllUnique = function ( src ) {
		if ( src.length < 2 ) {
			return true;
		}
	
		var sorted = src.slice().sort();
		var last = sorted[0];
	
		for ( var i=1, ien=sorted.length ; i<ien ; i++ ) {
			if ( sorted[i] === last ) {
				return false;
			}
	
			last = sorted[i];
		}
	
		return true;
	};
	
	
	/**
	 * Find the unique elements in a source array.
	 *
	 * @param  {array} src Source array
	 * @return {array} Array of unique items
	 * @ignore
	 */
	var _unique = function ( src )
	{
		if ( _areAllUnique( src ) ) {
			return src.slice();
		}
	
		// A faster unique method is to use object keys to identify used values,
		// but this doesn't work with arrays or objects, which we must also
		// consider. See jsperf.com/compare-array-unique-versions/4 for more
		// information.
		var
			out = [],
			val,
			i, ien=src.length,
			j, k=0;
	
		again: for ( i=0 ; i<ien ; i++ ) {
			val = src[i];
	
			for ( j=0 ; j<k ; j++ ) {
				if ( out[j] === val ) {
					continue again;
				}
			}
	
			out.push( val );
			k++;
		}
	
		return out;
	};
	
	
	/**
	 * DataTables utility methods
	 * 
	 * This namespace provides helper methods that DataTables uses internally to
	 * create a DataTable, but which are not exclusively used only for DataTables.
	 * These methods can be used by extension authors to save the duplication of
	 * code.
	 *
	 *  @namespace
	 */
	DataTable.util = {
		/**
		 * Throttle the calls to a function. Arguments and context are maintained
		 * for the throttled function.
		 *
		 * @param {function} fn Function to be called
		 * @param {integer} freq Call frequency in mS
		 * @return {function} Wrapped function
		 */
		throttle: function ( fn, freq ) {
			var
				frequency = freq !== undefined ? freq : 200,
				last,
				timer;
	
			return function () {
				var
					that = this,
					now  = +new Date(),
					args = arguments;
	
				if ( last && now < last + frequency ) {
					clearTimeout( timer );
	
					timer = setTimeout( function () {
						last = undefined;
						fn.apply( that, args );
					}, frequency );
				}
				else {
					last = now;
					fn.apply( that, args );
				}
			};
		},
	
	
		/**
		 * Escape a string such that it can be used in a regular expression
		 *
		 *  @param {string} val string to escape
		 *  @returns {string} escaped string
		 */
		escapeRegex: function ( val ) {
			return val.replace( _re_escape_regex, '\\$1' );
		}
	};
	
	
	
	/**
	 * Create a mapping object that allows camel case parameters to be looked up
	 * for their Hungarian counterparts. The mapping is stored in a private
	 * parameter called `_hungarianMap` which can be accessed on the source object.
	 *  @param {object} o
	 *  @memberof DataTable#oApi
	 */
	function _fnHungarianMap ( o )
	{
		var
			hungarian = 'a aa ai ao as b fn i m o s ',
			match,
			newKey,
			map = {};
	
		$.each( o, function (key, val) {
			match = key.match(/^([^A-Z]+?)([A-Z])/);
	
			if ( match && hungarian.indexOf(match[1]+' ') !== -1 )
			{
				newKey = key.replace( match[0], match[2].toLowerCase() );
				map[ newKey ] = key;
	
				if ( match[1] === 'o' )
				{
					_fnHungarianMap( o[key] );
				}
			}
		} );
	
		o._hungarianMap = map;
	}
	
	
	/**
	 * Convert from camel case parameters to Hungarian, based on a Hungarian map
	 * created by _fnHungarianMap.
	 *  @param {object} src The model object which holds all parameters that can be
	 *    mapped.
	 *  @param {object} user The object to convert from camel case to Hungarian.
	 *  @param {boolean} force When set to `true`, properties which already have a
	 *    Hungarian value in the `user` object will be overwritten. Otherwise they
	 *    won't be.
	 *  @memberof DataTable#oApi
	 */
	function _fnCamelToHungarian ( src, user, force )
	{
		if ( ! src._hungarianMap ) {
			_fnHungarianMap( src );
		}
	
		var hungarianKey;
	
		$.each( user, function (key, val) {
			hungarianKey = src._hungarianMap[ key ];
	
			if ( hungarianKey !== undefined && (force || user[hungarianKey] === undefined) )
			{
				// For objects, we need to buzz down into the object to copy parameters
				if ( hungarianKey.charAt(0) === 'o' )
				{
					// Copy the camelCase options over to the hungarian
					if ( ! user[ hungarianKey ] ) {
						user[ hungarianKey ] = {};
					}
					$.extend( true, user[hungarianKey], user[key] );
	
					_fnCamelToHungarian( src[hungarianKey], user[hungarianKey], force );
				}
				else {
					user[hungarianKey] = user[ key ];
				}
			}
		} );
	}
	
	
	/**
	 * Language compatibility - when certain options are given, and others aren't, we
	 * need to duplicate the values over, in order to provide backwards compatibility
	 * with older language files.
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnLanguageCompat( lang )
	{
		// Note the use of the Hungarian notation for the parameters in this method as
		// this is called after the mapping of camelCase to Hungarian
		var defaults = DataTable.defaults.oLanguage;
	
		// Default mapping
		var defaultDecimal = defaults.sDecimal;
		if ( defaultDecimal ) {
			_addNumericSort( defaultDecimal );
		}
	
		if ( lang ) {
			var zeroRecords = lang.sZeroRecords;
	
			// Backwards compatibility - if there is no sEmptyTable given, then use the same as
			// sZeroRecords - assuming that is given.
			if ( ! lang.sEmptyTable && zeroRecords &&
				defaults.sEmptyTable === "No data available in table" )
			{
				_fnMap( lang, lang, 'sZeroRecords', 'sEmptyTable' );
			}
	
			// Likewise with loading records
			if ( ! lang.sLoadingRecords && zeroRecords &&
				defaults.sLoadingRecords === "Loading..." )
			{
				_fnMap( lang, lang, 'sZeroRecords', 'sLoadingRecords' );
			}
	
			// Old parameter name of the thousands separator mapped onto the new
			if ( lang.sInfoThousands ) {
				lang.sThousands = lang.sInfoThousands;
			}
	
			var decimal = lang.sDecimal;
			if ( decimal && defaultDecimal !== decimal ) {
				_addNumericSort( decimal );
			}
		}
	}
	
	
	/**
	 * Map one parameter onto another
	 *  @param {object} o Object to map
	 *  @param {*} knew The new parameter name
	 *  @param {*} old The old parameter name
	 */
	var _fnCompatMap = function ( o, knew, old ) {
		if ( o[ knew ] !== undefined ) {
			o[ old ] = o[ knew ];
		}
	};
	
	
	/**
	 * Provide backwards compatibility for the main DT options. Note that the new
	 * options are mapped onto the old parameters, so this is an external interface
	 * change only.
	 *  @param {object} init Object to map
	 */
	function _fnCompatOpts ( init )
	{
		_fnCompatMap( init, 'ordering',      'bSort' );
		_fnCompatMap( init, 'orderMulti',    'bSortMulti' );
		_fnCompatMap( init, 'orderClasses',  'bSortClasses' );
		_fnCompatMap( init, 'orderCellsTop', 'bSortCellsTop' );
		_fnCompatMap( init, 'order',         'aaSorting' );
		_fnCompatMap( init, 'orderFixed',    'aaSortingFixed' );
		_fnCompatMap( init, 'paging',        'bPaginate' );
		_fnCompatMap( init, 'pagingType',    'sPaginationType' );
		_fnCompatMap( init, 'pageLength',    'iDisplayLength' );
		_fnCompatMap( init, 'searching',     'bFilter' );
	
		// Boolean initialisation of x-scrolling
		if ( typeof init.sScrollX === 'boolean' ) {
			init.sScrollX = init.sScrollX ? '100%' : '';
		}
		if ( typeof init.scrollX === 'boolean' ) {
			init.scrollX = init.scrollX ? '100%' : '';
		}
	
		// Column search objects are in an array, so it needs to be converted
		// element by element
		var searchCols = init.aoSearchCols;
	
		if ( searchCols ) {
			for ( var i=0, ien=searchCols.length ; i<ien ; i++ ) {
				if ( searchCols[i] ) {
					_fnCamelToHungarian( DataTable.models.oSearch, searchCols[i] );
				}
			}
		}
	}
	
	
	/**
	 * Provide backwards compatibility for column options. Note that the new options
	 * are mapped onto the old parameters, so this is an external interface change
	 * only.
	 *  @param {object} init Object to map
	 */
	function _fnCompatCols ( init )
	{
		_fnCompatMap( init, 'orderable',     'bSortable' );
		_fnCompatMap( init, 'orderData',     'aDataSort' );
		_fnCompatMap( init, 'orderSequence', 'asSorting' );
		_fnCompatMap( init, 'orderDataType', 'sortDataType' );
	
		// orderData can be given as an integer
		var dataSort = init.aDataSort;
		if ( typeof dataSort === 'number' && ! $.isArray( dataSort ) ) {
			init.aDataSort = [ dataSort ];
		}
	}
	
	
	/**
	 * Browser feature detection for capabilities, quirks
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnBrowserDetect( settings )
	{
		// We don't need to do this every time DataTables is constructed, the values
		// calculated are specific to the browser and OS configuration which we
		// don't expect to change between initialisations
		if ( ! DataTable.__browser ) {
			var browser = {};
			DataTable.__browser = browser;
	
			// Scrolling feature / quirks detection
			var n = $('<div/>')
				.css( {
					position: 'fixed',
					top: 0,
					left: $(window).scrollLeft()*-1, // allow for scrolling
					height: 1,
					width: 1,
					overflow: 'hidden'
				} )
				.append(
					$('<div/>')
						.css( {
							position: 'absolute',
							top: 1,
							left: 1,
							width: 100,
							overflow: 'scroll'
						} )
						.append(
							$('<div/>')
								.css( {
									width: '100%',
									height: 10
								} )
						)
				)
				.appendTo( 'body' );
	
			var outer = n.children();
			var inner = outer.children();
	
			// Numbers below, in order, are:
			// inner.offsetWidth, inner.clientWidth, outer.offsetWidth, outer.clientWidth
			//
			// IE6 XP:                           100 100 100  83
			// IE7 Vista:                        100 100 100  83
			// IE 8+ Windows:                     83  83 100  83
			// Evergreen Windows:                 83  83 100  83
			// Evergreen Mac with scrollbars:     85  85 100  85
			// Evergreen Mac without scrollbars: 100 100 100 100
	
			// Get scrollbar width
			browser.barWidth = outer[0].offsetWidth - outer[0].clientWidth;
	
			// IE6/7 will oversize a width 100% element inside a scrolling element, to
			// include the width of the scrollbar, while other browsers ensure the inner
			// element is contained without forcing scrolling
			browser.bScrollOversize = inner[0].offsetWidth === 100 && outer[0].clientWidth !== 100;
	
			// In rtl text layout, some browsers (most, but not all) will place the
			// scrollbar on the left, rather than the right.
			browser.bScrollbarLeft = Math.round( inner.offset().left ) !== 1;
	
			// IE8- don't provide height and width for getBoundingClientRect
			browser.bBounding = n[0].getBoundingClientRect().width ? true : false;
	
			n.remove();
		}
	
		$.extend( settings.oBrowser, DataTable.__browser );
		settings.oScroll.iBarWidth = DataTable.__browser.barWidth;
	}
	
	
	/**
	 * Array.prototype reduce[Right] method, used for browsers which don't support
	 * JS 1.6. Done this way to reduce code size, since we iterate either way
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnReduce ( that, fn, init, start, end, inc )
	{
		var
			i = start,
			value,
			isSet = false;
	
		if ( init !== undefined ) {
			value = init;
			isSet = true;
		}
	
		while ( i !== end ) {
			if ( ! that.hasOwnProperty(i) ) {
				continue;
			}
	
			value = isSet ?
				fn( value, that[i], i, that ) :
				that[i];
	
			isSet = true;
			i += inc;
		}
	
		return value;
	}
	
	/**
	 * Add a column to the list used for the table with default values
	 *  @param {object} oSettings dataTables settings object
	 *  @param {node} nTh The th element for this column
	 *  @memberof DataTable#oApi
	 */
	function _fnAddColumn( oSettings, nTh )
	{
		// Add column to aoColumns array
		var oDefaults = DataTable.defaults.column;
		var iCol = oSettings.aoColumns.length;
		var oCol = $.extend( {}, DataTable.models.oColumn, oDefaults, {
			"nTh": nTh ? nTh : document.createElement('th'),
			"sTitle":    oDefaults.sTitle    ? oDefaults.sTitle    : nTh ? nTh.innerHTML : '',
			"aDataSort": oDefaults.aDataSort ? oDefaults.aDataSort : [iCol],
			"mData": oDefaults.mData ? oDefaults.mData : iCol,
			idx: iCol
		} );
		oSettings.aoColumns.push( oCol );
	
		// Add search object for column specific search. Note that the `searchCols[ iCol ]`
		// passed into extend can be undefined. This allows the user to give a default
		// with only some of the parameters defined, and also not give a default
		var searchCols = oSettings.aoPreSearchCols;
		searchCols[ iCol ] = $.extend( {}, DataTable.models.oSearch, searchCols[ iCol ] );
	
		// Use the default column options function to initialise classes etc
		_fnColumnOptions( oSettings, iCol, $(nTh).data() );
	}
	
	
	/**
	 * Apply options for a column
	 *  @param {object} oSettings dataTables settings object
	 *  @param {int} iCol column index to consider
	 *  @param {object} oOptions object with sType, bVisible and bSearchable etc
	 *  @memberof DataTable#oApi
	 */
	function _fnColumnOptions( oSettings, iCol, oOptions )
	{
		var oCol = oSettings.aoColumns[ iCol ];
		var oClasses = oSettings.oClasses;
		var th = $(oCol.nTh);
	
		// Try to get width information from the DOM. We can't get it from CSS
		// as we'd need to parse the CSS stylesheet. `width` option can override
		if ( ! oCol.sWidthOrig ) {
			// Width attribute
			oCol.sWidthOrig = th.attr('width') || null;
	
			// Style attribute
			var t = (th.attr('style') || '').match(/width:\s*(\d+[pxem%]+)/);
			if ( t ) {
				oCol.sWidthOrig = t[1];
			}
		}
	
		/* User specified column options */
		if ( oOptions !== undefined && oOptions !== null )
		{
			// Backwards compatibility
			_fnCompatCols( oOptions );
	
			// Map camel case parameters to their Hungarian counterparts
			_fnCamelToHungarian( DataTable.defaults.column, oOptions );
	
			/* Backwards compatibility for mDataProp */
			if ( oOptions.mDataProp !== undefined && !oOptions.mData )
			{
				oOptions.mData = oOptions.mDataProp;
			}
	
			if ( oOptions.sType )
			{
				oCol._sManualType = oOptions.sType;
			}
	
			// `class` is a reserved word in Javascript, so we need to provide
			// the ability to use a valid name for the camel case input
			if ( oOptions.className && ! oOptions.sClass )
			{
				oOptions.sClass = oOptions.className;
			}
			if ( oOptions.sClass ) {
				th.addClass( oOptions.sClass );
			}
	
			$.extend( oCol, oOptions );
			_fnMap( oCol, oOptions, "sWidth", "sWidthOrig" );
	
			/* iDataSort to be applied (backwards compatibility), but aDataSort will take
			 * priority if defined
			 */
			if ( oOptions.iDataSort !== undefined )
			{
				oCol.aDataSort = [ oOptions.iDataSort ];
			}
			_fnMap( oCol, oOptions, "aDataSort" );
		}
	
		/* Cache the data get and set functions for speed */
		var mDataSrc = oCol.mData;
		var mData = _fnGetObjectDataFn( mDataSrc );
		var mRender = oCol.mRender ? _fnGetObjectDataFn( oCol.mRender ) : null;
	
		var attrTest = function( src ) {
			return typeof src === 'string' && src.indexOf('@') !== -1;
		};
		oCol._bAttrSrc = $.isPlainObject( mDataSrc ) && (
			attrTest(mDataSrc.sort) || attrTest(mDataSrc.type) || attrTest(mDataSrc.filter)
		);
		oCol._setter = null;
	
		oCol.fnGetData = function (rowData, type, meta) {
			var innerData = mData( rowData, type, undefined, meta );
	
			return mRender && type ?
				mRender( innerData, type, rowData, meta ) :
				innerData;
		};
		oCol.fnSetData = function ( rowData, val, meta ) {
			return _fnSetObjectDataFn( mDataSrc )( rowData, val, meta );
		};
	
		// Indicate if DataTables should read DOM data as an object or array
		// Used in _fnGetRowElements
		if ( typeof mDataSrc !== 'number' ) {
			oSettings._rowReadObject = true;
		}
	
		/* Feature sorting overrides column specific when off */
		if ( !oSettings.oFeatures.bSort )
		{
			oCol.bSortable = false;
			th.addClass( oClasses.sSortableNone ); // Have to add class here as order event isn't called
		}
	
		/* Check that the class assignment is correct for sorting */
		var bAsc = $.inArray('asc', oCol.asSorting) !== -1;
		var bDesc = $.inArray('desc', oCol.asSorting) !== -1;
		if ( !oCol.bSortable || (!bAsc && !bDesc) )
		{
			oCol.sSortingClass = oClasses.sSortableNone;
			oCol.sSortingClassJUI = "";
		}
		else if ( bAsc && !bDesc )
		{
			oCol.sSortingClass = oClasses.sSortableAsc;
			oCol.sSortingClassJUI = oClasses.sSortJUIAscAllowed;
		}
		else if ( !bAsc && bDesc )
		{
			oCol.sSortingClass = oClasses.sSortableDesc;
			oCol.sSortingClassJUI = oClasses.sSortJUIDescAllowed;
		}
		else
		{
			oCol.sSortingClass = oClasses.sSortable;
			oCol.sSortingClassJUI = oClasses.sSortJUI;
		}
	}
	
	
	/**
	 * Adjust the table column widths for new data. Note: you would probably want to
	 * do a redraw after calling this function!
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnAdjustColumnSizing ( settings )
	{
		/* Not interested in doing column width calculation if auto-width is disabled */
		if ( settings.oFeatures.bAutoWidth !== false )
		{
			var columns = settings.aoColumns;
	
			_fnCalculateColumnWidths( settings );
			for ( var i=0 , iLen=columns.length ; i<iLen ; i++ )
			{
				columns[i].nTh.style.width = columns[i].sWidth;
			}
		}
	
		var scroll = settings.oScroll;
		if ( scroll.sY !== '' || scroll.sX !== '')
		{
			_fnScrollDraw( settings );
		}
	
		_fnCallbackFire( settings, null, 'column-sizing', [settings] );
	}
	
	
	/**
	 * Covert the index of a visible column to the index in the data array (take account
	 * of hidden columns)
	 *  @param {object} oSettings dataTables settings object
	 *  @param {int} iMatch Visible column index to lookup
	 *  @returns {int} i the data index
	 *  @memberof DataTable#oApi
	 */
	function _fnVisibleToColumnIndex( oSettings, iMatch )
	{
		var aiVis = _fnGetColumns( oSettings, 'bVisible' );
	
		return typeof aiVis[iMatch] === 'number' ?
			aiVis[iMatch] :
			null;
	}
	
	
	/**
	 * Covert the index of an index in the data array and convert it to the visible
	 *   column index (take account of hidden columns)
	 *  @param {int} iMatch Column index to lookup
	 *  @param {object} oSettings dataTables settings object
	 *  @returns {int} i the data index
	 *  @memberof DataTable#oApi
	 */
	function _fnColumnIndexToVisible( oSettings, iMatch )
	{
		var aiVis = _fnGetColumns( oSettings, 'bVisible' );
		var iPos = $.inArray( iMatch, aiVis );
	
		return iPos !== -1 ? iPos : null;
	}
	
	
	/**
	 * Get the number of visible columns
	 *  @param {object} oSettings dataTables settings object
	 *  @returns {int} i the number of visible columns
	 *  @memberof DataTable#oApi
	 */
	function _fnVisbleColumns( oSettings )
	{
		var vis = 0;
	
		// No reduce in IE8, use a loop for now
		$.each( oSettings.aoColumns, function ( i, col ) {
			if ( col.bVisible && $(col.nTh).css('display') !== 'none' ) {
				vis++;
			}
		} );
	
		return vis;
	}
	
	
	/**
	 * Get an array of column indexes that match a given property
	 *  @param {object} oSettings dataTables settings object
	 *  @param {string} sParam Parameter in aoColumns to look for - typically
	 *    bVisible or bSearchable
	 *  @returns {array} Array of indexes with matched properties
	 *  @memberof DataTable#oApi
	 */
	function _fnGetColumns( oSettings, sParam )
	{
		var a = [];
	
		$.map( oSettings.aoColumns, function(val, i) {
			if ( val[sParam] ) {
				a.push( i );
			}
		} );
	
		return a;
	}
	
	
	/**
	 * Calculate the 'type' of a column
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnColumnTypes ( settings )
	{
		var columns = settings.aoColumns;
		var data = settings.aoData;
		var types = DataTable.ext.type.detect;
		var i, ien, j, jen, k, ken;
		var col, cell, detectedType, cache;
	
		// For each column, spin over the 
		for ( i=0, ien=columns.length ; i<ien ; i++ ) {
			col = columns[i];
			cache = [];
	
			if ( ! col.sType && col._sManualType ) {
				col.sType = col._sManualType;
			}
			else if ( ! col.sType ) {
				for ( j=0, jen=types.length ; j<jen ; j++ ) {
					for ( k=0, ken=data.length ; k<ken ; k++ ) {
						// Use a cache array so we only need to get the type data
						// from the formatter once (when using multiple detectors)
						if ( cache[k] === undefined ) {
							cache[k] = _fnGetCellData( settings, k, i, 'type' );
						}
	
						detectedType = types[j]( cache[k], settings );
	
						// If null, then this type can't apply to this column, so
						// rather than testing all cells, break out. There is an
						// exception for the last type which is `html`. We need to
						// scan all rows since it is possible to mix string and HTML
						// types
						if ( ! detectedType && j !== types.length-1 ) {
							break;
						}
	
						// Only a single match is needed for html type since it is
						// bottom of the pile and very similar to string
						if ( detectedType === 'html' ) {
							break;
						}
					}
	
					// Type is valid for all data points in the column - use this
					// type
					if ( detectedType ) {
						col.sType = detectedType;
						break;
					}
				}
	
				// Fall back - if no type was detected, always use string
				if ( ! col.sType ) {
					col.sType = 'string';
				}
			}
		}
	}
	
	
	/**
	 * Take the column definitions and static columns arrays and calculate how
	 * they relate to column indexes. The callback function will then apply the
	 * definition found for a column to a suitable configuration object.
	 *  @param {object} oSettings dataTables settings object
	 *  @param {array} aoColDefs The aoColumnDefs array that is to be applied
	 *  @param {array} aoCols The aoColumns array that defines columns individually
	 *  @param {function} fn Callback function - takes two parameters, the calculated
	 *    column index and the definition for that column.
	 *  @memberof DataTable#oApi
	 */
	function _fnApplyColumnDefs( oSettings, aoColDefs, aoCols, fn )
	{
		var i, iLen, j, jLen, k, kLen, def;
		var columns = oSettings.aoColumns;
	
		// Column definitions with aTargets
		if ( aoColDefs )
		{
			/* Loop over the definitions array - loop in reverse so first instance has priority */
			for ( i=aoColDefs.length-1 ; i>=0 ; i-- )
			{
				def = aoColDefs[i];
	
				/* Each definition can target multiple columns, as it is an array */
				var aTargets = def.targets !== undefined ?
					def.targets :
					def.aTargets;
	
				if ( ! $.isArray( aTargets ) )
				{
					aTargets = [ aTargets ];
				}
	
				for ( j=0, jLen=aTargets.length ; j<jLen ; j++ )
				{
					if ( typeof aTargets[j] === 'number' && aTargets[j] >= 0 )
					{
						/* Add columns that we don't yet know about */
						while( columns.length <= aTargets[j] )
						{
							_fnAddColumn( oSettings );
						}
	
						/* Integer, basic index */
						fn( aTargets[j], def );
					}
					else if ( typeof aTargets[j] === 'number' && aTargets[j] < 0 )
					{
						/* Negative integer, right to left column counting */
						fn( columns.length+aTargets[j], def );
					}
					else if ( typeof aTargets[j] === 'string' )
					{
						/* Class name matching on TH element */
						for ( k=0, kLen=columns.length ; k<kLen ; k++ )
						{
							if ( aTargets[j] == "_all" ||
							     $(columns[k].nTh).hasClass( aTargets[j] ) )
							{
								fn( k, def );
							}
						}
					}
				}
			}
		}
	
		// Statically defined columns array
		if ( aoCols )
		{
			for ( i=0, iLen=aoCols.length ; i<iLen ; i++ )
			{
				fn( i, aoCols[i] );
			}
		}
	}
	
	/**
	 * Add a data array to the table, creating DOM node etc. This is the parallel to
	 * _fnGatherData, but for adding rows from a Javascript source, rather than a
	 * DOM source.
	 *  @param {object} oSettings dataTables settings object
	 *  @param {array} aData data array to be added
	 *  @param {node} [nTr] TR element to add to the table - optional. If not given,
	 *    DataTables will create a row automatically
	 *  @param {array} [anTds] Array of TD|TH elements for the row - must be given
	 *    if nTr is.
	 *  @returns {int} >=0 if successful (index of new aoData entry), -1 if failed
	 *  @memberof DataTable#oApi
	 */
	function _fnAddData ( oSettings, aDataIn, nTr, anTds )
	{
		/* Create the object for storing information about this new row */
		var iRow = oSettings.aoData.length;
		var oData = $.extend( true, {}, DataTable.models.oRow, {
			src: nTr ? 'dom' : 'data',
			idx: iRow
		} );
	
		oData._aData = aDataIn;
		oSettings.aoData.push( oData );
	
		/* Create the cells */
		var nTd, sThisType;
		var columns = oSettings.aoColumns;
	
		// Invalidate the column types as the new data needs to be revalidated
		for ( var i=0, iLen=columns.length ; i<iLen ; i++ )
		{
			columns[i].sType = null;
		}
	
		/* Add to the display array */
		oSettings.aiDisplayMaster.push( iRow );
	
		var id = oSettings.rowIdFn( aDataIn );
		if ( id !== undefined ) {
			oSettings.aIds[ id ] = oData;
		}
	
		/* Create the DOM information, or register it if already present */
		if ( nTr || ! oSettings.oFeatures.bDeferRender )
		{
			_fnCreateTr( oSettings, iRow, nTr, anTds );
		}
	
		return iRow;
	}
	
	
	/**
	 * Add one or more TR elements to the table. Generally we'd expect to
	 * use this for reading data from a DOM sourced table, but it could be
	 * used for an TR element. Note that if a TR is given, it is used (i.e.
	 * it is not cloned).
	 *  @param {object} settings dataTables settings object
	 *  @param {array|node|jQuery} trs The TR element(s) to add to the table
	 *  @returns {array} Array of indexes for the added rows
	 *  @memberof DataTable#oApi
	 */
	function _fnAddTr( settings, trs )
	{
		var row;
	
		// Allow an individual node to be passed in
		if ( ! (trs instanceof $) ) {
			trs = $(trs);
		}
	
		return trs.map( function (i, el) {
			row = _fnGetRowElements( settings, el );
			return _fnAddData( settings, row.data, el, row.cells );
		} );
	}
	
	
	/**
	 * Take a TR element and convert it to an index in aoData
	 *  @param {object} oSettings dataTables settings object
	 *  @param {node} n the TR element to find
	 *  @returns {int} index if the node is found, null if not
	 *  @memberof DataTable#oApi
	 */
	function _fnNodeToDataIndex( oSettings, n )
	{
		return (n._DT_RowIndex!==undefined) ? n._DT_RowIndex : null;
	}
	
	
	/**
	 * Take a TD element and convert it into a column data index (not the visible index)
	 *  @param {object} oSettings dataTables settings object
	 *  @param {int} iRow The row number the TD/TH can be found in
	 *  @param {node} n The TD/TH element to find
	 *  @returns {int} index if the node is found, -1 if not
	 *  @memberof DataTable#oApi
	 */
	function _fnNodeToColumnIndex( oSettings, iRow, n )
	{
		return $.inArray( n, oSettings.aoData[ iRow ].anCells );
	}
	
	
	/**
	 * Get the data for a given cell from the internal cache, taking into account data mapping
	 *  @param {object} settings dataTables settings object
	 *  @param {int} rowIdx aoData row id
	 *  @param {int} colIdx Column index
	 *  @param {string} type data get type ('display', 'type' 'filter' 'sort')
	 *  @returns {*} Cell data
	 *  @memberof DataTable#oApi
	 */
	function _fnGetCellData( settings, rowIdx, colIdx, type )
	{
		var draw           = settings.iDraw;
		var col            = settings.aoColumns[colIdx];
		var rowData        = settings.aoData[rowIdx]._aData;
		var defaultContent = col.sDefaultContent;
		var cellData       = col.fnGetData( rowData, type, {
			settings: settings,
			row:      rowIdx,
			col:      colIdx
		} );
	
		if ( cellData === undefined ) {
			if ( settings.iDrawError != draw && defaultContent === null ) {
				_fnLog( settings, 0, "Requested unknown parameter "+
					(typeof col.mData=='function' ? '{function}' : "'"+col.mData+"'")+
					" for row "+rowIdx+", column "+colIdx, 4 );
				settings.iDrawError = draw;
			}
			return defaultContent;
		}
	
		// When the data source is null and a specific data type is requested (i.e.
		// not the original data), we can use default column data
		if ( (cellData === rowData || cellData === null) && defaultContent !== null && type !== undefined ) {
			cellData = defaultContent;
		}
		else if ( typeof cellData === 'function' ) {
			// If the data source is a function, then we run it and use the return,
			// executing in the scope of the data object (for instances)
			return cellData.call( rowData );
		}
	
		if ( cellData === null && type == 'display' ) {
			return '';
		}
		return cellData;
	}
	
	
	/**
	 * Set the value for a specific cell, into the internal data cache
	 *  @param {object} settings dataTables settings object
	 *  @param {int} rowIdx aoData row id
	 *  @param {int} colIdx Column index
	 *  @param {*} val Value to set
	 *  @memberof DataTable#oApi
	 */
	function _fnSetCellData( settings, rowIdx, colIdx, val )
	{
		var col     = settings.aoColumns[colIdx];
		var rowData = settings.aoData[rowIdx]._aData;
	
		col.fnSetData( rowData, val, {
			settings: settings,
			row:      rowIdx,
			col:      colIdx
		}  );
	}
	
	
	// Private variable that is used to match action syntax in the data property object
	var __reArray = /\[.*?\]$/;
	var __reFn = /\(\)$/;
	
	/**
	 * Split string on periods, taking into account escaped periods
	 * @param  {string} str String to split
	 * @return {array} Split string
	 */
	function _fnSplitObjNotation( str )
	{
		return $.map( str.match(/(\\.|[^\.])+/g) || [''], function ( s ) {
			return s.replace(/\\\./g, '.');
		} );
	}
	
	
	/**
	 * Return a function that can be used to get data from a source object, taking
	 * into account the ability to use nested objects as a source
	 *  @param {string|int|function} mSource The data source for the object
	 *  @returns {function} Data get function
	 *  @memberof DataTable#oApi
	 */
	function _fnGetObjectDataFn( mSource )
	{
		if ( $.isPlainObject( mSource ) )
		{
			/* Build an object of get functions, and wrap them in a single call */
			var o = {};
			$.each( mSource, function (key, val) {
				if ( val ) {
					o[key] = _fnGetObjectDataFn( val );
				}
			} );
	
			return function (data, type, row, meta) {
				var t = o[type] || o._;
				return t !== undefined ?
					t(data, type, row, meta) :
					data;
			};
		}
		else if ( mSource === null )
		{
			/* Give an empty string for rendering / sorting etc */
			return function (data) { // type, row and meta also passed, but not used
				return data;
			};
		}
		else if ( typeof mSource === 'function' )
		{
			return function (data, type, row, meta) {
				return mSource( data, type, row, meta );
			};
		}
		else if ( typeof mSource === 'string' && (mSource.indexOf('.') !== -1 ||
			      mSource.indexOf('[') !== -1 || mSource.indexOf('(') !== -1) )
		{
			/* If there is a . in the source string then the data source is in a
			 * nested object so we loop over the data for each level to get the next
			 * level down. On each loop we test for undefined, and if found immediately
			 * return. This allows entire objects to be missing and sDefaultContent to
			 * be used if defined, rather than throwing an error
			 */
			var fetchData = function (data, type, src) {
				var arrayNotation, funcNotation, out, innerSrc;
	
				if ( src !== "" )
				{
					var a = _fnSplitObjNotation( src );
	
					for ( var i=0, iLen=a.length ; i<iLen ; i++ )
					{
						// Check if we are dealing with special notation
						arrayNotation = a[i].match(__reArray);
						funcNotation = a[i].match(__reFn);
	
						if ( arrayNotation )
						{
							// Array notation
							a[i] = a[i].replace(__reArray, '');
	
							// Condition allows simply [] to be passed in
							if ( a[i] !== "" ) {
								data = data[ a[i] ];
							}
							out = [];
	
							// Get the remainder of the nested object to get
							a.splice( 0, i+1 );
							innerSrc = a.join('.');
	
							// Traverse each entry in the array getting the properties requested
							if ( $.isArray( data ) ) {
								for ( var j=0, jLen=data.length ; j<jLen ; j++ ) {
									out.push( fetchData( data[j], type, innerSrc ) );
								}
							}
	
							// If a string is given in between the array notation indicators, that
							// is used to join the strings together, otherwise an array is returned
							var join = arrayNotation[0].substring(1, arrayNotation[0].length-1);
							data = (join==="") ? out : out.join(join);
	
							// The inner call to fetchData has already traversed through the remainder
							// of the source requested, so we exit from the loop
							break;
						}
						else if ( funcNotation )
						{
							// Function call
							a[i] = a[i].replace(__reFn, '');
							data = data[ a[i] ]();
							continue;
						}
	
						if ( data === null || data[ a[i] ] === undefined )
						{
							return undefined;
						}
						data = data[ a[i] ];
					}
				}
	
				return data;
			};
	
			return function (data, type) { // row and meta also passed, but not used
				return fetchData( data, type, mSource );
			};
		}
		else
		{
			/* Array or flat object mapping */
			return function (data, type) { // row and meta also passed, but not used
				return data[mSource];
			};
		}
	}
	
	
	/**
	 * Return a function that can be used to set data from a source object, taking
	 * into account the ability to use nested objects as a source
	 *  @param {string|int|function} mSource The data source for the object
	 *  @returns {function} Data set function
	 *  @memberof DataTable#oApi
	 */
	function _fnSetObjectDataFn( mSource )
	{
		if ( $.isPlainObject( mSource ) )
		{
			/* Unlike get, only the underscore (global) option is used for for
			 * setting data since we don't know the type here. This is why an object
			 * option is not documented for `mData` (which is read/write), but it is
			 * for `mRender` which is read only.
			 */
			return _fnSetObjectDataFn( mSource._ );
		}
		else if ( mSource === null )
		{
			/* Nothing to do when the data source is null */
			return function () {};
		}
		else if ( typeof mSource === 'function' )
		{
			return function (data, val, meta) {
				mSource( data, 'set', val, meta );
			};
		}
		else if ( typeof mSource === 'string' && (mSource.indexOf('.') !== -1 ||
			      mSource.indexOf('[') !== -1 || mSource.indexOf('(') !== -1) )
		{
			/* Like the get, we need to get data from a nested object */
			var setData = function (data, val, src) {
				var a = _fnSplitObjNotation( src ), b;
				var aLast = a[a.length-1];
				var arrayNotation, funcNotation, o, innerSrc;
	
				for ( var i=0, iLen=a.length-1 ; i<iLen ; i++ )
				{
					// Check if we are dealing with an array notation request
					arrayNotation = a[i].match(__reArray);
					funcNotation = a[i].match(__reFn);
	
					if ( arrayNotation )
					{
						a[i] = a[i].replace(__reArray, '');
						data[ a[i] ] = [];
	
						// Get the remainder of the nested object to set so we can recurse
						b = a.slice();
						b.splice( 0, i+1 );
						innerSrc = b.join('.');
	
						// Traverse each entry in the array setting the properties requested
						if ( $.isArray( val ) )
						{
							for ( var j=0, jLen=val.length ; j<jLen ; j++ )
							{
								o = {};
								setData( o, val[j], innerSrc );
								data[ a[i] ].push( o );
							}
						}
						else
						{
							// We've been asked to save data to an array, but it
							// isn't array data to be saved. Best that can be done
							// is to just save the value.
							data[ a[i] ] = val;
						}
	
						// The inner call to setData has already traversed through the remainder
						// of the source and has set the data, thus we can exit here
						return;
					}
					else if ( funcNotation )
					{
						// Function call
						a[i] = a[i].replace(__reFn, '');
						data = data[ a[i] ]( val );
					}
	
					// If the nested object doesn't currently exist - since we are
					// trying to set the value - create it
					if ( data[ a[i] ] === null || data[ a[i] ] === undefined )
					{
						data[ a[i] ] = {};
					}
					data = data[ a[i] ];
				}
	
				// Last item in the input - i.e, the actual set
				if ( aLast.match(__reFn ) )
				{
					// Function call
					data = data[ aLast.replace(__reFn, '') ]( val );
				}
				else
				{
					// If array notation is used, we just want to strip it and use the property name
					// and assign the value. If it isn't used, then we get the result we want anyway
					data[ aLast.replace(__reArray, '') ] = val;
				}
			};
	
			return function (data, val) { // meta is also passed in, but not used
				return setData( data, val, mSource );
			};
		}
		else
		{
			/* Array or flat object mapping */
			return function (data, val) { // meta is also passed in, but not used
				data[mSource] = val;
			};
		}
	}
	
	
	/**
	 * Return an array with the full table data
	 *  @param {object} oSettings dataTables settings object
	 *  @returns array {array} aData Master data array
	 *  @memberof DataTable#oApi
	 */
	function _fnGetDataMaster ( settings )
	{
		return _pluck( settings.aoData, '_aData' );
	}
	
	
	/**
	 * Nuke the table
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnClearTable( settings )
	{
		settings.aoData.length = 0;
		settings.aiDisplayMaster.length = 0;
		settings.aiDisplay.length = 0;
		settings.aIds = {};
	}
	
	
	 /**
	 * Take an array of integers (index array) and remove a target integer (value - not
	 * the key!)
	 *  @param {array} a Index array to target
	 *  @param {int} iTarget value to find
	 *  @memberof DataTable#oApi
	 */
	function _fnDeleteIndex( a, iTarget, splice )
	{
		var iTargetIndex = -1;
	
		for ( var i=0, iLen=a.length ; i<iLen ; i++ )
		{
			if ( a[i] == iTarget )
			{
				iTargetIndex = i;
			}
			else if ( a[i] > iTarget )
			{
				a[i]--;
			}
		}
	
		if ( iTargetIndex != -1 && splice === undefined )
		{
			a.splice( iTargetIndex, 1 );
		}
	}
	
	
	/**
	 * Mark cached data as invalid such that a re-read of the data will occur when
	 * the cached data is next requested. Also update from the data source object.
	 *
	 * @param {object} settings DataTables settings object
	 * @param {int}    rowIdx   Row index to invalidate
	 * @param {string} [src]    Source to invalidate from: undefined, 'auto', 'dom'
	 *     or 'data'
	 * @param {int}    [colIdx] Column index to invalidate. If undefined the whole
	 *     row will be invalidated
	 * @memberof DataTable#oApi
	 *
	 * @todo For the modularisation of v1.11 this will need to become a callback, so
	 *   the sort and filter methods can subscribe to it. That will required
	 *   initialisation options for sorting, which is why it is not already baked in
	 */
	function _fnInvalidate( settings, rowIdx, src, colIdx )
	{
		var row = settings.aoData[ rowIdx ];
		var i, ien;
		var cellWrite = function ( cell, col ) {
			// This is very frustrating, but in IE if you just write directly
			// to innerHTML, and elements that are overwritten are GC'ed,
			// even if there is a reference to them elsewhere
			while ( cell.childNodes.length ) {
				cell.removeChild( cell.firstChild );
			}
	
			cell.innerHTML = _fnGetCellData( settings, rowIdx, col, 'display' );
		};
	
		// Are we reading last data from DOM or the data object?
		if ( src === 'dom' || ((! src || src === 'auto') && row.src === 'dom') ) {
			// Read the data from the DOM
			row._aData = _fnGetRowElements(
					settings, row, colIdx, colIdx === undefined ? undefined : row._aData
				)
				.data;
		}
		else {
			// Reading from data object, update the DOM
			var cells = row.anCells;
	
			if ( cells ) {
				if ( colIdx !== undefined ) {
					cellWrite( cells[colIdx], colIdx );
				}
				else {
					for ( i=0, ien=cells.length ; i<ien ; i++ ) {
						cellWrite( cells[i], i );
					}
				}
			}
		}
	
		// For both row and cell invalidation, the cached data for sorting and
		// filtering is nulled out
		row._aSortData = null;
		row._aFilterData = null;
	
		// Invalidate the type for a specific column (if given) or all columns since
		// the data might have changed
		var cols = settings.aoColumns;
		if ( colIdx !== undefined ) {
			cols[ colIdx ].sType = null;
		}
		else {
			for ( i=0, ien=cols.length ; i<ien ; i++ ) {
				cols[i].sType = null;
			}
	
			// Update DataTables special `DT_*` attributes for the row
			_fnRowAttributes( settings, row );
		}
	}
	
	
	/**
	 * Build a data source object from an HTML row, reading the contents of the
	 * cells that are in the row.
	 *
	 * @param {object} settings DataTables settings object
	 * @param {node|object} TR element from which to read data or existing row
	 *   object from which to re-read the data from the cells
	 * @param {int} [colIdx] Optional column index
	 * @param {array|object} [d] Data source object. If `colIdx` is given then this
	 *   parameter should also be given and will be used to write the data into.
	 *   Only the column in question will be written
	 * @returns {object} Object with two parameters: `data` the data read, in
	 *   document order, and `cells` and array of nodes (they can be useful to the
	 *   caller, so rather than needing a second traversal to get them, just return
	 *   them from here).
	 * @memberof DataTable#oApi
	 */
	function _fnGetRowElements( settings, row, colIdx, d )
	{
		var
			tds = [],
			td = row.firstChild,
			name, col, o, i=0, contents,
			columns = settings.aoColumns,
			objectRead = settings._rowReadObject;
	
		// Allow the data object to be passed in, or construct
		d = d !== undefined ?
			d :
			objectRead ?
				{} :
				[];
	
		var attr = function ( str, td  ) {
			if ( typeof str === 'string' ) {
				var idx = str.indexOf('@');
	
				if ( idx !== -1 ) {
					var attr = str.substring( idx+1 );
					var setter = _fnSetObjectDataFn( str );
					setter( d, td.getAttribute( attr ) );
				}
			}
		};
	
		// Read data from a cell and store into the data object
		var cellProcess = function ( cell ) {
			if ( colIdx === undefined || colIdx === i ) {
				col = columns[i];
				contents = $.trim(cell.innerHTML);
	
				if ( col && col._bAttrSrc ) {
					var setter = _fnSetObjectDataFn( col.mData._ );
					setter( d, contents );
	
					attr( col.mData.sort, cell );
					attr( col.mData.type, cell );
					attr( col.mData.filter, cell );
				}
				else {
					// Depending on the `data` option for the columns the data can
					// be read to either an object or an array.
					if ( objectRead ) {
						if ( ! col._setter ) {
							// Cache the setter function
							col._setter = _fnSetObjectDataFn( col.mData );
						}
						col._setter( d, contents );
					}
					else {
						d[i] = contents;
					}
				}
			}
	
			i++;
		};
	
		if ( td ) {
			// `tr` element was passed in
			while ( td ) {
				name = td.nodeName.toUpperCase();
	
				if ( name == "TD" || name == "TH" ) {
					cellProcess( td );
					tds.push( td );
				}
	
				td = td.nextSibling;
			}
		}
		else {
			// Existing row object passed in
			tds = row.anCells;
	
			for ( var j=0, jen=tds.length ; j<jen ; j++ ) {
				cellProcess( tds[j] );
			}
		}
	
		// Read the ID from the DOM if present
		var rowNode = row.firstChild ? row : row.nTr;
	
		if ( rowNode ) {
			var id = rowNode.getAttribute( 'id' );
	
			if ( id ) {
				_fnSetObjectDataFn( settings.rowId )( d, id );
			}
		}
	
		return {
			data: d,
			cells: tds
		};
	}
	/**
	 * Create a new TR element (and it's TD children) for a row
	 *  @param {object} oSettings dataTables settings object
	 *  @param {int} iRow Row to consider
	 *  @param {node} [nTrIn] TR element to add to the table - optional. If not given,
	 *    DataTables will create a row automatically
	 *  @param {array} [anTds] Array of TD|TH elements for the row - must be given
	 *    if nTr is.
	 *  @memberof DataTable#oApi
	 */
	function _fnCreateTr ( oSettings, iRow, nTrIn, anTds )
	{
		var
			row = oSettings.aoData[iRow],
			rowData = row._aData,
			cells = [],
			nTr, nTd, oCol,
			i, iLen;
	
		if ( row.nTr === null )
		{
			nTr = nTrIn || document.createElement('tr');
	
			row.nTr = nTr;
			row.anCells = cells;
	
			/* Use a private property on the node to allow reserve mapping from the node
			 * to the aoData array for fast look up
			 */
			nTr._DT_RowIndex = iRow;
	
			/* Special parameters can be given by the data source to be used on the row */
			_fnRowAttributes( oSettings, row );
	
			/* Process each column */
			for ( i=0, iLen=oSettings.aoColumns.length ; i<iLen ; i++ )
			{
				oCol = oSettings.aoColumns[i];
	
				nTd = nTrIn ? anTds[i] : document.createElement( oCol.sCellType );
				nTd._DT_CellIndex = {
					row: iRow,
					column: i
				};
				
				cells.push( nTd );
	
				// Need to create the HTML if new, or if a rendering function is defined
				if ( (!nTrIn || oCol.mRender || oCol.mData !== i) &&
					 (!$.isPlainObject(oCol.mData) || oCol.mData._ !== i+'.display')
				) {
					nTd.innerHTML = _fnGetCellData( oSettings, iRow, i, 'display' );
				}
	
				/* Add user defined class */
				if ( oCol.sClass )
				{
					nTd.className += ' '+oCol.sClass;
				}
	
				// Visibility - add or remove as required
				if ( oCol.bVisible && ! nTrIn )
				{
					nTr.appendChild( nTd );
				}
				else if ( ! oCol.bVisible && nTrIn )
				{
					nTd.parentNode.removeChild( nTd );
				}
	
				if ( oCol.fnCreatedCell )
				{
					oCol.fnCreatedCell.call( oSettings.oInstance,
						nTd, _fnGetCellData( oSettings, iRow, i ), rowData, iRow, i
					);
				}
			}
	
			_fnCallbackFire( oSettings, 'aoRowCreatedCallback', null, [nTr, rowData, iRow, cells] );
		}
	
		// Remove once webkit bug 131819 and Chromium bug 365619 have been resolved
		// and deployed
		row.nTr.setAttribute( 'role', 'row' );
	}
	
	
	/**
	 * Add attributes to a row based on the special `DT_*` parameters in a data
	 * source object.
	 *  @param {object} settings DataTables settings object
	 *  @param {object} DataTables row object for the row to be modified
	 *  @memberof DataTable#oApi
	 */
	function _fnRowAttributes( settings, row )
	{
		var tr = row.nTr;
		var data = row._aData;
	
		if ( tr ) {
			var id = settings.rowIdFn( data );
	
			if ( id ) {
				tr.id = id;
			}
	
			if ( data.DT_RowClass ) {
				// Remove any classes added by DT_RowClass before
				var a = data.DT_RowClass.split(' ');
				row.__rowc = row.__rowc ?
					_unique( row.__rowc.concat( a ) ) :
					a;
	
				$(tr)
					.removeClass( row.__rowc.join(' ') )
					.addClass( data.DT_RowClass );
			}
	
			if ( data.DT_RowAttr ) {
				$(tr).attr( data.DT_RowAttr );
			}
	
			if ( data.DT_RowData ) {
				$(tr).data( data.DT_RowData );
			}
		}
	}
	
	
	/**
	 * Create the HTML header for the table
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnBuildHead( oSettings )
	{
		var i, ien, cell, row, column;
		var thead = oSettings.nTHead;
		var tfoot = oSettings.nTFoot;
		var createHeader = $('th, td', thead).length === 0;
		var classes = oSettings.oClasses;
		var columns = oSettings.aoColumns;
	
		if ( createHeader ) {
			row = $('<tr/>').appendTo( thead );
		}
	
		for ( i=0, ien=columns.length ; i<ien ; i++ ) {
			column = columns[i];
			cell = $( column.nTh ).addClass( column.sClass );
	
			if ( createHeader ) {
				cell.appendTo( row );
			}
	
			// 1.11 move into sorting
			if ( oSettings.oFeatures.bSort ) {
				cell.addClass( column.sSortingClass );
	
				if ( column.bSortable !== false ) {
					cell
						.attr( 'tabindex', oSettings.iTabIndex )
						.attr( 'aria-controls', oSettings.sTableId );
	
					_fnSortAttachListener( oSettings, column.nTh, i );
				}
			}
	
			if ( column.sTitle != cell[0].innerHTML ) {
				cell.html( column.sTitle );
			}
	
			_fnRenderer( oSettings, 'header' )(
				oSettings, cell, column, classes
			);
		}
	
		if ( createHeader ) {
			_fnDetectHeader( oSettings.aoHeader, thead );
		}
		
		/* ARIA role for the rows */
	 	$(thead).find('>tr').attr('role', 'row');
	
		/* Deal with the footer - add classes if required */
		$(thead).find('>tr>th, >tr>td').addClass( classes.sHeaderTH );
		$(tfoot).find('>tr>th, >tr>td').addClass( classes.sFooterTH );
	
		// Cache the footer cells. Note that we only take the cells from the first
		// row in the footer. If there is more than one row the user wants to
		// interact with, they need to use the table().foot() method. Note also this
		// allows cells to be used for multiple columns using colspan
		if ( tfoot !== null ) {
			var cells = oSettings.aoFooter[0];
	
			for ( i=0, ien=cells.length ; i<ien ; i++ ) {
				column = columns[i];
				column.nTf = cells[i].cell;
	
				if ( column.sClass ) {
					$(column.nTf).addClass( column.sClass );
				}
			}
		}
	}
	
	
	/**
	 * Draw the header (or footer) element based on the column visibility states. The
	 * methodology here is to use the layout array from _fnDetectHeader, modified for
	 * the instantaneous column visibility, to construct the new layout. The grid is
	 * traversed over cell at a time in a rows x columns grid fashion, although each
	 * cell insert can cover multiple elements in the grid - which is tracks using the
	 * aApplied array. Cell inserts in the grid will only occur where there isn't
	 * already a cell in that position.
	 *  @param {object} oSettings dataTables settings object
	 *  @param array {objects} aoSource Layout array from _fnDetectHeader
	 *  @param {boolean} [bIncludeHidden=false] If true then include the hidden columns in the calc,
	 *  @memberof DataTable#oApi
	 */
	function _fnDrawHead( oSettings, aoSource, bIncludeHidden )
	{
		var i, iLen, j, jLen, k, kLen, n, nLocalTr;
		var aoLocal = [];
		var aApplied = [];
		var iColumns = oSettings.aoColumns.length;
		var iRowspan, iColspan;
	
		if ( ! aoSource )
		{
			return;
		}
	
		if (  bIncludeHidden === undefined )
		{
			bIncludeHidden = false;
		}
	
		/* Make a copy of the master layout array, but without the visible columns in it */
		for ( i=0, iLen=aoSource.length ; i<iLen ; i++ )
		{
			aoLocal[i] = aoSource[i].slice();
			aoLocal[i].nTr = aoSource[i].nTr;
	
			/* Remove any columns which are currently hidden */
			for ( j=iColumns-1 ; j>=0 ; j-- )
			{
				if ( !oSettings.aoColumns[j].bVisible && !bIncludeHidden )
				{
					aoLocal[i].splice( j, 1 );
				}
			}
	
			/* Prep the applied array - it needs an element for each row */
			aApplied.push( [] );
		}
	
		for ( i=0, iLen=aoLocal.length ; i<iLen ; i++ )
		{
			nLocalTr = aoLocal[i].nTr;
	
			/* All cells are going to be replaced, so empty out the row */
			if ( nLocalTr )
			{
				while( (n = nLocalTr.firstChild) )
				{
					nLocalTr.removeChild( n );
				}
			}
	
			for ( j=0, jLen=aoLocal[i].length ; j<jLen ; j++ )
			{
				iRowspan = 1;
				iColspan = 1;
	
				/* Check to see if there is already a cell (row/colspan) covering our target
				 * insert point. If there is, then there is nothing to do.
				 */
				if ( aApplied[i][j] === undefined )
				{
					nLocalTr.appendChild( aoLocal[i][j].cell );
					aApplied[i][j] = 1;
	
					/* Expand the cell to cover as many rows as needed */
					while ( aoLocal[i+iRowspan] !== undefined &&
					        aoLocal[i][j].cell == aoLocal[i+iRowspan][j].cell )
					{
						aApplied[i+iRowspan][j] = 1;
						iRowspan++;
					}
	
					/* Expand the cell to cover as many columns as needed */
					while ( aoLocal[i][j+iColspan] !== undefined &&
					        aoLocal[i][j].cell == aoLocal[i][j+iColspan].cell )
					{
						/* Must update the applied array over the rows for the columns */
						for ( k=0 ; k<iRowspan ; k++ )
						{
							aApplied[i+k][j+iColspan] = 1;
						}
						iColspan++;
					}
	
					/* Do the actual expansion in the DOM */
					$(aoLocal[i][j].cell)
						.attr('rowspan', iRowspan)
						.attr('colspan', iColspan);
				}
			}
		}
	}
	
	
	/**
	 * Insert the required TR nodes into the table for display
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnDraw( oSettings )
	{
		/* Provide a pre-callback function which can be used to cancel the draw is false is returned */
		var aPreDraw = _fnCallbackFire( oSettings, 'aoPreDrawCallback', 'preDraw', [oSettings] );
		if ( $.inArray( false, aPreDraw ) !== -1 )
		{
			_fnProcessingDisplay( oSettings, false );
			return;
		}
	
		var i, iLen, n;
		var anRows = [];
		var iRowCount = 0;
		var asStripeClasses = oSettings.asStripeClasses;
		var iStripes = asStripeClasses.length;
		var iOpenRows = oSettings.aoOpenRows.length;
		var oLang = oSettings.oLanguage;
		var iInitDisplayStart = oSettings.iInitDisplayStart;
		var bServerSide = _fnDataSource( oSettings ) == 'ssp';
		var aiDisplay = oSettings.aiDisplay;
	
		oSettings.bDrawing = true;
	
		/* Check and see if we have an initial draw position from state saving */
		if ( iInitDisplayStart !== undefined && iInitDisplayStart !== -1 )
		{
			oSettings._iDisplayStart = bServerSide ?
				iInitDisplayStart :
				iInitDisplayStart >= oSettings.fnRecordsDisplay() ?
					0 :
					iInitDisplayStart;
	
			oSettings.iInitDisplayStart = -1;
		}
	
		var iDisplayStart = oSettings._iDisplayStart;
		var iDisplayEnd = oSettings.fnDisplayEnd();
	
		/* Server-side processing draw intercept */
		if ( oSettings.bDeferLoading )
		{
			oSettings.bDeferLoading = false;
			oSettings.iDraw++;
			_fnProcessingDisplay( oSettings, false );
		}
		else if ( !bServerSide )
		{
			oSettings.iDraw++;
		}
		else if ( !oSettings.bDestroying && !_fnAjaxUpdate( oSettings ) )
		{
			return;
		}
	
		if ( aiDisplay.length !== 0 )
		{
			var iStart = bServerSide ? 0 : iDisplayStart;
			var iEnd = bServerSide ? oSettings.aoData.length : iDisplayEnd;
	
			for ( var j=iStart ; j<iEnd ; j++ )
			{
				var iDataIndex = aiDisplay[j];
				var aoData = oSettings.aoData[ iDataIndex ];
				if ( aoData.nTr === null )
				{
					_fnCreateTr( oSettings, iDataIndex );
				}
	
				var nRow = aoData.nTr;
	
				/* Remove the old striping classes and then add the new one */
				if ( iStripes !== 0 )
				{
					var sStripe = asStripeClasses[ iRowCount % iStripes ];
					if ( aoData._sRowStripe != sStripe )
					{
						$(nRow).removeClass( aoData._sRowStripe ).addClass( sStripe );
						aoData._sRowStripe = sStripe;
					}
				}
	
				// Row callback functions - might want to manipulate the row
				// iRowCount and j are not currently documented. Are they at all
				// useful?
				_fnCallbackFire( oSettings, 'aoRowCallback', null,
					[nRow, aoData._aData, iRowCount, j, iDataIndex] );
	
				anRows.push( nRow );
				iRowCount++;
			}
		}
		else
		{
			/* Table is empty - create a row with an empty message in it */
			var sZero = oLang.sZeroRecords;
			if ( oSettings.iDraw == 1 &&  _fnDataSource( oSettings ) == 'ajax' )
			{
				sZero = oLang.sLoadingRecords;
			}
			else if ( oLang.sEmptyTable && oSettings.fnRecordsTotal() === 0 )
			{
				sZero = oLang.sEmptyTable;
			}
	
			anRows[ 0 ] = $( '<tr/>', { 'class': iStripes ? asStripeClasses[0] : '' } )
				.append( $('<td />', {
					'valign':  'top',
					'colSpan': _fnVisbleColumns( oSettings ),
					'class':   oSettings.oClasses.sRowEmpty
				} ).html( sZero ) )[0];
		}
	
		/* Header and footer callbacks */
		_fnCallbackFire( oSettings, 'aoHeaderCallback', 'header', [ $(oSettings.nTHead).children('tr')[0],
			_fnGetDataMaster( oSettings ), iDisplayStart, iDisplayEnd, aiDisplay ] );
	
		_fnCallbackFire( oSettings, 'aoFooterCallback', 'footer', [ $(oSettings.nTFoot).children('tr')[0],
			_fnGetDataMaster( oSettings ), iDisplayStart, iDisplayEnd, aiDisplay ] );
	
		var body = $(oSettings.nTBody);
	
		body.children().detach();
		body.append( $(anRows) );
	
		/* Call all required callback functions for the end of a draw */
		_fnCallbackFire( oSettings, 'aoDrawCallback', 'draw', [oSettings] );
	
		/* Draw is complete, sorting and filtering must be as well */
		oSettings.bSorted = false;
		oSettings.bFiltered = false;
		oSettings.bDrawing = false;
	}
	
	
	/**
	 * Redraw the table - taking account of the various features which are enabled
	 *  @param {object} oSettings dataTables settings object
	 *  @param {boolean} [holdPosition] Keep the current paging position. By default
	 *    the paging is reset to the first page
	 *  @memberof DataTable#oApi
	 */
	function _fnReDraw( settings, holdPosition )
	{
		var
			features = settings.oFeatures,
			sort     = features.bSort,
			filter   = features.bFilter;
	
		if ( sort ) {
			_fnSort( settings );
		}
	
		if ( filter ) {
			_fnFilterComplete( settings, settings.oPreviousSearch );
		}
		else {
			// No filtering, so we want to just use the display master
			settings.aiDisplay = settings.aiDisplayMaster.slice();
		}
	
		if ( holdPosition !== true ) {
			settings._iDisplayStart = 0;
		}
	
		// Let any modules know about the draw hold position state (used by
		// scrolling internally)
		settings._drawHold = holdPosition;
	
		_fnDraw( settings );
	
		settings._drawHold = false;
	}
	
	
	/**
	 * Add the options to the page HTML for the table
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnAddOptionsHtml ( oSettings )
	{
		var classes = oSettings.oClasses;
		var table = $(oSettings.nTable);
		var holding = $('<div/>').insertBefore( table ); // Holding element for speed
		var features = oSettings.oFeatures;
	
		// All DataTables are wrapped in a div
		var insert = $('<div/>', {
			id:      oSettings.sTableId+'_wrapper',
			'class': classes.sWrapper + (oSettings.nTFoot ? '' : ' '+classes.sNoFooter)
		} );
	
		oSettings.nHolding = holding[0];
		oSettings.nTableWrapper = insert[0];
		oSettings.nTableReinsertBefore = oSettings.nTable.nextSibling;
	
		/* Loop over the user set positioning and place the elements as needed */
		var aDom = oSettings.sDom.split('');
		var featureNode, cOption, nNewNode, cNext, sAttr, j;
		for ( var i=0 ; i<aDom.length ; i++ )
		{
			featureNode = null;
			cOption = aDom[i];
	
			if ( cOption == '<' )
			{
				/* New container div */
				nNewNode = $('<div/>')[0];
	
				/* Check to see if we should append an id and/or a class name to the container */
				cNext = aDom[i+1];
				if ( cNext == "'" || cNext == '"' )
				{
					sAttr = "";
					j = 2;
					while ( aDom[i+j] != cNext )
					{
						sAttr += aDom[i+j];
						j++;
					}
	
					/* Replace jQuery UI constants @todo depreciated */
					if ( sAttr == "H" )
					{
						sAttr = classes.sJUIHeader;
					}
					else if ( sAttr == "F" )
					{
						sAttr = classes.sJUIFooter;
					}
	
					/* The attribute can be in the format of "#id.class", "#id" or "class" This logic
					 * breaks the string into parts and applies them as needed
					 */
					if ( sAttr.indexOf('.') != -1 )
					{
						var aSplit = sAttr.split('.');
						nNewNode.id = aSplit[0].substr(1, aSplit[0].length-1);
						nNewNode.className = aSplit[1];
					}
					else if ( sAttr.charAt(0) == "#" )
					{
						nNewNode.id = sAttr.substr(1, sAttr.length-1);
					}
					else
					{
						nNewNode.className = sAttr;
					}
	
					i += j; /* Move along the position array */
				}
	
				insert.append( nNewNode );
				insert = $(nNewNode);
			}
			else if ( cOption == '>' )
			{
				/* End container div */
				insert = insert.parent();
			}
			// @todo Move options into their own plugins?
			else if ( cOption == 'l' && features.bPaginate && features.bLengthChange )
			{
				/* Length */
				featureNode = _fnFeatureHtmlLength( oSettings );
			}
			else if ( cOption == 'f' && features.bFilter )
			{
				/* Filter */
				featureNode = _fnFeatureHtmlFilter( oSettings );
			}
			else if ( cOption == 'r' && features.bProcessing )
			{
				/* pRocessing */
				featureNode = _fnFeatureHtmlProcessing( oSettings );
			}
			else if ( cOption == 't' )
			{
				/* Table */
				featureNode = _fnFeatureHtmlTable( oSettings );
			}
			else if ( cOption ==  'i' && features.bInfo )
			{
				/* Info */
				featureNode = _fnFeatureHtmlInfo( oSettings );
			}
			else if ( cOption == 'p' && features.bPaginate )
			{
				/* Pagination */
				featureNode = _fnFeatureHtmlPaginate( oSettings );
			}
			else if ( DataTable.ext.feature.length !== 0 )
			{
				/* Plug-in features */
				var aoFeatures = DataTable.ext.feature;
				for ( var k=0, kLen=aoFeatures.length ; k<kLen ; k++ )
				{
					if ( cOption == aoFeatures[k].cFeature )
					{
						featureNode = aoFeatures[k].fnInit( oSettings );
						break;
					}
				}
			}
	
			/* Add to the 2D features array */
			if ( featureNode )
			{
				var aanFeatures = oSettings.aanFeatures;
	
				if ( ! aanFeatures[cOption] )
				{
					aanFeatures[cOption] = [];
				}
	
				aanFeatures[cOption].push( featureNode );
				insert.append( featureNode );
			}
		}
	
		/* Built our DOM structure - replace the holding div with what we want */
		holding.replaceWith( insert );
		oSettings.nHolding = null;
	}
	
	
	/**
	 * Use the DOM source to create up an array of header cells. The idea here is to
	 * create a layout grid (array) of rows x columns, which contains a reference
	 * to the cell that that point in the grid (regardless of col/rowspan), such that
	 * any column / row could be removed and the new grid constructed
	 *  @param array {object} aLayout Array to store the calculated layout in
	 *  @param {node} nThead The header/footer element for the table
	 *  @memberof DataTable#oApi
	 */
	function _fnDetectHeader ( aLayout, nThead )
	{
		var nTrs = $(nThead).children('tr');
		var nTr, nCell;
		var i, k, l, iLen, jLen, iColShifted, iColumn, iColspan, iRowspan;
		var bUnique;
		var fnShiftCol = function ( a, i, j ) {
			var k = a[i];
	                while ( k[j] ) {
				j++;
			}
			return j;
		};
	
		aLayout.splice( 0, aLayout.length );
	
		/* We know how many rows there are in the layout - so prep it */
		for ( i=0, iLen=nTrs.length ; i<iLen ; i++ )
		{
			aLayout.push( [] );
		}
	
		/* Calculate a layout array */
		for ( i=0, iLen=nTrs.length ; i<iLen ; i++ )
		{
			nTr = nTrs[i];
			iColumn = 0;
	
			/* For every cell in the row... */
			nCell = nTr.firstChild;
			while ( nCell ) {
				if ( nCell.nodeName.toUpperCase() == "TD" ||
				     nCell.nodeName.toUpperCase() == "TH" )
				{
					/* Get the col and rowspan attributes from the DOM and sanitise them */
					iColspan = nCell.getAttribute('colspan') * 1;
					iRowspan = nCell.getAttribute('rowspan') * 1;
					iColspan = (!iColspan || iColspan===0 || iColspan===1) ? 1 : iColspan;
					iRowspan = (!iRowspan || iRowspan===0 || iRowspan===1) ? 1 : iRowspan;
	
					/* There might be colspan cells already in this row, so shift our target
					 * accordingly
					 */
					iColShifted = fnShiftCol( aLayout, i, iColumn );
	
					/* Cache calculation for unique columns */
					bUnique = iColspan === 1 ? true : false;
	
					/* If there is col / rowspan, copy the information into the layout grid */
					for ( l=0 ; l<iColspan ; l++ )
					{
						for ( k=0 ; k<iRowspan ; k++ )
						{
							aLayout[i+k][iColShifted+l] = {
								"cell": nCell,
								"unique": bUnique
							};
							aLayout[i+k].nTr = nTr;
						}
					}
				}
				nCell = nCell.nextSibling;
			}
		}
	}
	
	
	/**
	 * Get an array of unique th elements, one for each column
	 *  @param {object} oSettings dataTables settings object
	 *  @param {node} nHeader automatically detect the layout from this node - optional
	 *  @param {array} aLayout thead/tfoot layout from _fnDetectHeader - optional
	 *  @returns array {node} aReturn list of unique th's
	 *  @memberof DataTable#oApi
	 */
	function _fnGetUniqueThs ( oSettings, nHeader, aLayout )
	{
		var aReturn = [];
		if ( !aLayout )
		{
			aLayout = oSettings.aoHeader;
			if ( nHeader )
			{
				aLayout = [];
				_fnDetectHeader( aLayout, nHeader );
			}
		}
	
		for ( var i=0, iLen=aLayout.length ; i<iLen ; i++ )
		{
			for ( var j=0, jLen=aLayout[i].length ; j<jLen ; j++ )
			{
				if ( aLayout[i][j].unique &&
					 (!aReturn[j] || !oSettings.bSortCellsTop) )
				{
					aReturn[j] = aLayout[i][j].cell;
				}
			}
		}
	
		return aReturn;
	}
	
	/**
	 * Create an Ajax call based on the table's settings, taking into account that
	 * parameters can have multiple forms, and backwards compatibility.
	 *
	 * @param {object} oSettings dataTables settings object
	 * @param {array} data Data to send to the server, required by
	 *     DataTables - may be augmented by developer callbacks
	 * @param {function} fn Callback function to run when data is obtained
	 */
	function _fnBuildAjax( oSettings, data, fn )
	{
		// Compatibility with 1.9-, allow fnServerData and event to manipulate
		_fnCallbackFire( oSettings, 'aoServerParams', 'serverParams', [data] );
	
		// Convert to object based for 1.10+ if using the old array scheme which can
		// come from server-side processing or serverParams
		if ( data && $.isArray(data) ) {
			var tmp = {};
			var rbracket = /(.*?)\[\]$/;
	
			$.each( data, function (key, val) {
				var match = val.name.match(rbracket);
	
				if ( match ) {
					// Support for arrays
					var name = match[0];
	
					if ( ! tmp[ name ] ) {
						tmp[ name ] = [];
					}
					tmp[ name ].push( val.value );
				}
				else {
					tmp[val.name] = val.value;
				}
			} );
			data = tmp;
		}
	
		var ajaxData;
		var ajax = oSettings.ajax;
		var instance = oSettings.oInstance;
		var callback = function ( json ) {
			_fnCallbackFire( oSettings, null, 'xhr', [oSettings, json, oSettings.jqXHR] );
			fn( json );
		};
	
		if ( $.isPlainObject( ajax ) && ajax.data )
		{
			ajaxData = ajax.data;
	
			var newData = typeof ajaxData === 'function' ?
				ajaxData( data, oSettings ) :  // fn can manipulate data or return
				ajaxData;                      // an object object or array to merge
	
			// If the function returned something, use that alone
			data = typeof ajaxData === 'function' && newData ?
				newData :
				$.extend( true, data, newData );
	
			// Remove the data property as we've resolved it already and don't want
			// jQuery to do it again (it is restored at the end of the function)
			delete ajax.data;
		}
	
		var baseAjax = {
			"data": data,
			"success": function (json) {
				var error = json.error || json.sError;
				if ( error ) {
					_fnLog( oSettings, 0, error );
				}
	
				oSettings.json = json;
				callback( json );
			},
			"dataType": "json",
			"cache": false,
			"type": oSettings.sServerMethod,
			"error": function (xhr, error, thrown) {
				var ret = _fnCallbackFire( oSettings, null, 'xhr', [oSettings, null, oSettings.jqXHR] );
	
				if ( $.inArray( true, ret ) === -1 ) {
					if ( error == "parsererror" ) {
						_fnLog( oSettings, 0, 'Invalid JSON response', 1 );
					}
					else if ( xhr.readyState === 4 ) {
						_fnLog( oSettings, 0, 'Ajax error', 7 );
					}
				}
	
				_fnProcessingDisplay( oSettings, false );
			}
		};
	
		// Store the data submitted for the API
		oSettings.oAjaxData = data;
	
		// Allow plug-ins and external processes to modify the data
		_fnCallbackFire( oSettings, null, 'preXhr', [oSettings, data] );
	
		if ( oSettings.fnServerData )
		{
			// DataTables 1.9- compatibility
			oSettings.fnServerData.call( instance,
				oSettings.sAjaxSource,
				$.map( data, function (val, key) { // Need to convert back to 1.9 trad format
					return { name: key, value: val };
				} ),
				callback,
				oSettings
			);
		}
		else if ( oSettings.sAjaxSource || typeof ajax === 'string' )
		{
			// DataTables 1.9- compatibility
			oSettings.jqXHR = $.ajax( $.extend( baseAjax, {
				url: ajax || oSettings.sAjaxSource
			} ) );
		}
		else if ( typeof ajax === 'function' )
		{
			// Is a function - let the caller define what needs to be done
			oSettings.jqXHR = ajax.call( instance, data, callback, oSettings );
		}
		else
		{
			// Object to extend the base settings
			oSettings.jqXHR = $.ajax( $.extend( baseAjax, ajax ) );
	
			// Restore for next time around
			ajax.data = ajaxData;
		}
	}
	
	
	/**
	 * Update the table using an Ajax call
	 *  @param {object} settings dataTables settings object
	 *  @returns {boolean} Block the table drawing or not
	 *  @memberof DataTable#oApi
	 */
	function _fnAjaxUpdate( settings )
	{
		if ( settings.bAjaxDataGet ) {
			settings.iDraw++;
			_fnProcessingDisplay( settings, true );
	
			_fnBuildAjax(
				settings,
				_fnAjaxParameters( settings ),
				function(json) {
					_fnAjaxUpdateDraw( settings, json );
				}
			);
	
			return false;
		}
		return true;
	}
	
	
	/**
	 * Build up the parameters in an object needed for a server-side processing
	 * request. Note that this is basically done twice, is different ways - a modern
	 * method which is used by default in DataTables 1.10 which uses objects and
	 * arrays, or the 1.9- method with is name / value pairs. 1.9 method is used if
	 * the sAjaxSource option is used in the initialisation, or the legacyAjax
	 * option is set.
	 *  @param {object} oSettings dataTables settings object
	 *  @returns {bool} block the table drawing or not
	 *  @memberof DataTable#oApi
	 */
	function _fnAjaxParameters( settings )
	{
		var
			columns = settings.aoColumns,
			columnCount = columns.length,
			features = settings.oFeatures,
			preSearch = settings.oPreviousSearch,
			preColSearch = settings.aoPreSearchCols,
			i, data = [], dataProp, column, columnSearch,
			sort = _fnSortFlatten( settings ),
			displayStart = settings._iDisplayStart,
			displayLength = features.bPaginate !== false ?
				settings._iDisplayLength :
				-1;
	
		var param = function ( name, value ) {
			data.push( { 'name': name, 'value': value } );
		};
	
		// DataTables 1.9- compatible method
		param( 'sEcho',          settings.iDraw );
		param( 'iColumns',       columnCount );
		param( 'sColumns',       _pluck( columns, 'sName' ).join(',') );
		param( 'iDisplayStart',  displayStart );
		param( 'iDisplayLength', displayLength );
	
		// DataTables 1.10+ method
		var d = {
			draw:    settings.iDraw,
			columns: [],
			order:   [],
			start:   displayStart,
			length:  displayLength,
			search:  {
				value: preSearch.sSearch,
				regex: preSearch.bRegex
			}
		};
	
		for ( i=0 ; i<columnCount ; i++ ) {
			column = columns[i];
			columnSearch = preColSearch[i];
			dataProp = typeof column.mData=="function" ? 'function' : column.mData ;
	
			d.columns.push( {
				data:       dataProp,
				name:       column.sName,
				searchable: column.bSearchable,
				orderable:  column.bSortable,
				search:     {
					value: columnSearch.sSearch,
					regex: columnSearch.bRegex
				}
			} );
	
			param( "mDataProp_"+i, dataProp );
	
			if ( features.bFilter ) {
				param( 'sSearch_'+i,     columnSearch.sSearch );
				param( 'bRegex_'+i,      columnSearch.bRegex );
				param( 'bSearchable_'+i, column.bSearchable );
			}
	
			if ( features.bSort ) {
				param( 'bSortable_'+i, column.bSortable );
			}
		}
	
		if ( features.bFilter ) {
			param( 'sSearch', preSearch.sSearch );
			param( 'bRegex', preSearch.bRegex );
		}
	
		if ( features.bSort ) {
			$.each( sort, function ( i, val ) {
				d.order.push( { column: val.col, dir: val.dir } );
	
				param( 'iSortCol_'+i, val.col );
				param( 'sSortDir_'+i, val.dir );
			} );
	
			param( 'iSortingCols', sort.length );
		}
	
		// If the legacy.ajax parameter is null, then we automatically decide which
		// form to use, based on sAjaxSource
		var legacy = DataTable.ext.legacy.ajax;
		if ( legacy === null ) {
			return settings.sAjaxSource ? data : d;
		}
	
		// Otherwise, if legacy has been specified then we use that to decide on the
		// form
		return legacy ? data : d;
	}
	
	
	/**
	 * Data the data from the server (nuking the old) and redraw the table
	 *  @param {object} oSettings dataTables settings object
	 *  @param {object} json json data return from the server.
	 *  @param {string} json.sEcho Tracking flag for DataTables to match requests
	 *  @param {int} json.iTotalRecords Number of records in the data set, not accounting for filtering
	 *  @param {int} json.iTotalDisplayRecords Number of records in the data set, accounting for filtering
	 *  @param {array} json.aaData The data to display on this page
	 *  @param {string} [json.sColumns] Column ordering (sName, comma separated)
	 *  @memberof DataTable#oApi
	 */
	function _fnAjaxUpdateDraw ( settings, json )
	{
		// v1.10 uses camelCase variables, while 1.9 uses Hungarian notation.
		// Support both
		var compat = function ( old, modern ) {
			return json[old] !== undefined ? json[old] : json[modern];
		};
	
		var data = _fnAjaxDataSrc( settings, json );
		var draw            = compat( 'sEcho',                'draw' );
		var recordsTotal    = compat( 'iTotalRecords',        'recordsTotal' );
		var recordsFiltered = compat( 'iTotalDisplayRecords', 'recordsFiltered' );
	
		if ( draw ) {
			// Protect against out of sequence returns
			if ( draw*1 < settings.iDraw ) {
				return;
			}
			settings.iDraw = draw * 1;
		}
	
		_fnClearTable( settings );
		settings._iRecordsTotal   = parseInt(recordsTotal, 10);
		settings._iRecordsDisplay = parseInt(recordsFiltered, 10);
	
		for ( var i=0, ien=data.length ; i<ien ; i++ ) {
			_fnAddData( settings, data[i] );
		}
		settings.aiDisplay = settings.aiDisplayMaster.slice();
	
		settings.bAjaxDataGet = false;
		_fnDraw( settings );
	
		if ( ! settings._bInitComplete ) {
			_fnInitComplete( settings, json );
		}
	
		settings.bAjaxDataGet = true;
		_fnProcessingDisplay( settings, false );
	}
	
	
	/**
	 * Get the data from the JSON data source to use for drawing a table. Using
	 * `_fnGetObjectDataFn` allows the data to be sourced from a property of the
	 * source object, or from a processing function.
	 *  @param {object} oSettings dataTables settings object
	 *  @param  {object} json Data source object / array from the server
	 *  @return {array} Array of data to use
	 */
	function _fnAjaxDataSrc ( oSettings, json )
	{
		var dataSrc = $.isPlainObject( oSettings.ajax ) && oSettings.ajax.dataSrc !== undefined ?
			oSettings.ajax.dataSrc :
			oSettings.sAjaxDataProp; // Compatibility with 1.9-.
	
		// Compatibility with 1.9-. In order to read from aaData, check if the
		// default has been changed, if not, check for aaData
		if ( dataSrc === 'data' ) {
			return json.aaData || json[dataSrc];
		}
	
		return dataSrc !== "" ?
			_fnGetObjectDataFn( dataSrc )( json ) :
			json;
	}
	
	/**
	 * Generate the node required for filtering text
	 *  @returns {node} Filter control element
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnFeatureHtmlFilter ( settings )
	{
		var classes = settings.oClasses;
		var tableId = settings.sTableId;
		var language = settings.oLanguage;
		var previousSearch = settings.oPreviousSearch;
		var features = settings.aanFeatures;
		var input = '<input type="search" class="'+classes.sFilterInput+'"/>';
	
		var str = language.sSearch;
		str = str.match(/_INPUT_/) ?
			str.replace('_INPUT_', input) :
			str+input;
	
		var filter = $('<div/>', {
				'id': ! features.f ? tableId+'_filter' : null,
				'class': classes.sFilter
			} )
			.append( $('<label/>' ).append( str ) );
	
		var searchFn = function() {
			/* Update all other filter input elements for the new display */
			var n = features.f;
			var val = !this.value ? "" : this.value; // mental IE8 fix :-(
	
			/* Now do the filter */
			if ( val != previousSearch.sSearch ) {
				_fnFilterComplete( settings, {
					"sSearch": val,
					"bRegex": previousSearch.bRegex,
					"bSmart": previousSearch.bSmart ,
					"bCaseInsensitive": previousSearch.bCaseInsensitive
				} );
	
				// Need to redraw, without resorting
				settings._iDisplayStart = 0;
				_fnDraw( settings );
			}
		};
	
		var searchDelay = settings.searchDelay !== null ?
			settings.searchDelay :
			_fnDataSource( settings ) === 'ssp' ?
				400 :
				0;
	
		var jqFilter = $('input', filter)
			.val( previousSearch.sSearch )
			.attr( 'placeholder', language.sSearchPlaceholder )
			.on(
				'keyup.DT search.DT input.DT paste.DT cut.DT',
				searchDelay ?
					_fnThrottle( searchFn, searchDelay ) :
					searchFn
			)
			.on( 'keypress.DT', function(e) {
				/* Prevent form submission */
				if ( e.keyCode == 13 ) {
					return false;
				}
			} )
			.attr('aria-controls', tableId);
	
		// Update the input elements whenever the table is filtered
		$(settings.nTable).on( 'search.dt.DT', function ( ev, s ) {
			if ( settings === s ) {
				// IE9 throws an 'unknown error' if document.activeElement is used
				// inside an iframe or frame...
				try {
					if ( jqFilter[0] !== document.activeElement ) {
						jqFilter.val( previousSearch.sSearch );
					}
				}
				catch ( e ) {}
			}
		} );
	
		return filter[0];
	}
	
	
	/**
	 * Filter the table using both the global filter and column based filtering
	 *  @param {object} oSettings dataTables settings object
	 *  @param {object} oSearch search information
	 *  @param {int} [iForce] force a research of the master array (1) or not (undefined or 0)
	 *  @memberof DataTable#oApi
	 */
	function _fnFilterComplete ( oSettings, oInput, iForce )
	{
		var oPrevSearch = oSettings.oPreviousSearch;
		var aoPrevSearch = oSettings.aoPreSearchCols;
		var fnSaveFilter = function ( oFilter ) {
			/* Save the filtering values */
			oPrevSearch.sSearch = oFilter.sSearch;
			oPrevSearch.bRegex = oFilter.bRegex;
			oPrevSearch.bSmart = oFilter.bSmart;
			oPrevSearch.bCaseInsensitive = oFilter.bCaseInsensitive;
		};
		var fnRegex = function ( o ) {
			// Backwards compatibility with the bEscapeRegex option
			return o.bEscapeRegex !== undefined ? !o.bEscapeRegex : o.bRegex;
		};
	
		// Resolve any column types that are unknown due to addition or invalidation
		// @todo As per sort - can this be moved into an event handler?
		_fnColumnTypes( oSettings );
	
		/* In server-side processing all filtering is done by the server, so no point hanging around here */
		if ( _fnDataSource( oSettings ) != 'ssp' )
		{
			/* Global filter */
			_fnFilter( oSettings, oInput.sSearch, iForce, fnRegex(oInput), oInput.bSmart, oInput.bCaseInsensitive );
			fnSaveFilter( oInput );
	
			/* Now do the individual column filter */
			for ( var i=0 ; i<aoPrevSearch.length ; i++ )
			{
				_fnFilterColumn( oSettings, aoPrevSearch[i].sSearch, i, fnRegex(aoPrevSearch[i]),
					aoPrevSearch[i].bSmart, aoPrevSearch[i].bCaseInsensitive );
			}
	
			/* Custom filtering */
			_fnFilterCustom( oSettings );
		}
		else
		{
			fnSaveFilter( oInput );
		}
	
		/* Tell the draw function we have been filtering */
		oSettings.bFiltered = true;
		_fnCallbackFire( oSettings, null, 'search', [oSettings] );
	}
	
	
	/**
	 * Apply custom filtering functions
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnFilterCustom( settings )
	{
		var filters = DataTable.ext.search;
		var displayRows = settings.aiDisplay;
		var row, rowIdx;
	
		for ( var i=0, ien=filters.length ; i<ien ; i++ ) {
			var rows = [];
	
			// Loop over each row and see if it should be included
			for ( var j=0, jen=displayRows.length ; j<jen ; j++ ) {
				rowIdx = displayRows[ j ];
				row = settings.aoData[ rowIdx ];
	
				if ( filters[i]( settings, row._aFilterData, rowIdx, row._aData, j ) ) {
					rows.push( rowIdx );
				}
			}
	
			// So the array reference doesn't break set the results into the
			// existing array
			displayRows.length = 0;
			$.merge( displayRows, rows );
		}
	}
	
	
	/**
	 * Filter the table on a per-column basis
	 *  @param {object} oSettings dataTables settings object
	 *  @param {string} sInput string to filter on
	 *  @param {int} iColumn column to filter
	 *  @param {bool} bRegex treat search string as a regular expression or not
	 *  @param {bool} bSmart use smart filtering or not
	 *  @param {bool} bCaseInsensitive Do case insenstive matching or not
	 *  @memberof DataTable#oApi
	 */
	function _fnFilterColumn ( settings, searchStr, colIdx, regex, smart, caseInsensitive )
	{
		if ( searchStr === '' ) {
			return;
		}
	
		var data;
		var out = [];
		var display = settings.aiDisplay;
		var rpSearch = _fnFilterCreateSearch( searchStr, regex, smart, caseInsensitive );
	
		for ( var i=0 ; i<display.length ; i++ ) {
			data = settings.aoData[ display[i] ]._aFilterData[ colIdx ];
	
			if ( rpSearch.test( data ) ) {
				out.push( display[i] );
			}
		}
	
		settings.aiDisplay = out;
	}
	
	
	/**
	 * Filter the data table based on user input and draw the table
	 *  @param {object} settings dataTables settings object
	 *  @param {string} input string to filter on
	 *  @param {int} force optional - force a research of the master array (1) or not (undefined or 0)
	 *  @param {bool} regex treat as a regular expression or not
	 *  @param {bool} smart perform smart filtering or not
	 *  @param {bool} caseInsensitive Do case insenstive matching or not
	 *  @memberof DataTable#oApi
	 */
	function _fnFilter( settings, input, force, regex, smart, caseInsensitive )
	{
		var rpSearch = _fnFilterCreateSearch( input, regex, smart, caseInsensitive );
		var prevSearch = settings.oPreviousSearch.sSearch;
		var displayMaster = settings.aiDisplayMaster;
		var display, invalidated, i;
		var filtered = [];
	
		// Need to take account of custom filtering functions - always filter
		if ( DataTable.ext.search.length !== 0 ) {
			force = true;
		}
	
		// Check if any of the rows were invalidated
		invalidated = _fnFilterData( settings );
	
		// If the input is blank - we just want the full data set
		if ( input.length <= 0 ) {
			settings.aiDisplay = displayMaster.slice();
		}
		else {
			// New search - start from the master array
			if ( invalidated ||
				 force ||
				 prevSearch.length > input.length ||
				 input.indexOf(prevSearch) !== 0 ||
				 settings.bSorted // On resort, the display master needs to be
				                  // re-filtered since indexes will have changed
			) {
				settings.aiDisplay = displayMaster.slice();
			}
	
			// Search the display array
			display = settings.aiDisplay;
	
			for ( i=0 ; i<display.length ; i++ ) {
				if ( rpSearch.test( settings.aoData[ display[i] ]._sFilterRow ) ) {
					filtered.push( display[i] );
				}
			}
	
			settings.aiDisplay = filtered;
		}
	}
	
	
	/**
	 * Build a regular expression object suitable for searching a table
	 *  @param {string} sSearch string to search for
	 *  @param {bool} bRegex treat as a regular expression or not
	 *  @param {bool} bSmart perform smart filtering or not
	 *  @param {bool} bCaseInsensitive Do case insensitive matching or not
	 *  @returns {RegExp} constructed object
	 *  @memberof DataTable#oApi
	 */
	function _fnFilterCreateSearch( search, regex, smart, caseInsensitive )
	{
		search = regex ?
			search :
			_fnEscapeRegex( search );
		
		if ( smart ) {
			/* For smart filtering we want to allow the search to work regardless of
			 * word order. We also want double quoted text to be preserved, so word
			 * order is important - a la google. So this is what we want to
			 * generate:
			 * 
			 * ^(?=.*?\bone\b)(?=.*?\btwo three\b)(?=.*?\bfour\b).*$
			 */
			var a = $.map( search.match( /"[^"]+"|[^ ]+/g ) || [''], function ( word ) {
				if ( word.charAt(0) === '"' ) {
					var m = word.match( /^"(.*)"$/ );
					word = m ? m[1] : word;
				}
	
				return word.replace('"', '');
			} );
	
			search = '^(?=.*?'+a.join( ')(?=.*?' )+').*$';
		}
	
		return new RegExp( search, caseInsensitive ? 'i' : '' );
	}
	
	
	/**
	 * Escape a string such that it can be used in a regular expression
	 *  @param {string} sVal string to escape
	 *  @returns {string} escaped string
	 *  @memberof DataTable#oApi
	 */
	var _fnEscapeRegex = DataTable.util.escapeRegex;
	
	var __filter_div = $('<div>')[0];
	var __filter_div_textContent = __filter_div.textContent !== undefined;
	
	// Update the filtering data for each row if needed (by invalidation or first run)
	function _fnFilterData ( settings )
	{
		var columns = settings.aoColumns;
		var column;
		var i, j, ien, jen, filterData, cellData, row;
		var fomatters = DataTable.ext.type.search;
		var wasInvalidated = false;
	
		for ( i=0, ien=settings.aoData.length ; i<ien ; i++ ) {
			row = settings.aoData[i];
	
			if ( ! row._aFilterData ) {
				filterData = [];
	
				for ( j=0, jen=columns.length ; j<jen ; j++ ) {
					column = columns[j];
	
					if ( column.bSearchable ) {
						cellData = _fnGetCellData( settings, i, j, 'filter' );
	
						if ( fomatters[ column.sType ] ) {
							cellData = fomatters[ column.sType ]( cellData );
						}
	
						// Search in DataTables 1.10 is string based. In 1.11 this
						// should be altered to also allow strict type checking.
						if ( cellData === null ) {
							cellData = '';
						}
	
						if ( typeof cellData !== 'string' && cellData.toString ) {
							cellData = cellData.toString();
						}
					}
					else {
						cellData = '';
					}
	
					// If it looks like there is an HTML entity in the string,
					// attempt to decode it so sorting works as expected. Note that
					// we could use a single line of jQuery to do this, but the DOM
					// method used here is much faster http://jsperf.com/html-decode
					if ( cellData.indexOf && cellData.indexOf('&') !== -1 ) {
						__filter_div.innerHTML = cellData;
						cellData = __filter_div_textContent ?
							__filter_div.textContent :
							__filter_div.innerText;
					}
	
					if ( cellData.replace ) {
						cellData = cellData.replace(/[\r\n]/g, '');
					}
	
					filterData.push( cellData );
				}
	
				row._aFilterData = filterData;
				row._sFilterRow = filterData.join('  ');
				wasInvalidated = true;
			}
		}
	
		return wasInvalidated;
	}
	
	
	/**
	 * Convert from the internal Hungarian notation to camelCase for external
	 * interaction
	 *  @param {object} obj Object to convert
	 *  @returns {object} Inverted object
	 *  @memberof DataTable#oApi
	 */
	function _fnSearchToCamel ( obj )
	{
		return {
			search:          obj.sSearch,
			smart:           obj.bSmart,
			regex:           obj.bRegex,
			caseInsensitive: obj.bCaseInsensitive
		};
	}
	
	
	
	/**
	 * Convert from camelCase notation to the internal Hungarian. We could use the
	 * Hungarian convert function here, but this is cleaner
	 *  @param {object} obj Object to convert
	 *  @returns {object} Inverted object
	 *  @memberof DataTable#oApi
	 */
	function _fnSearchToHung ( obj )
	{
		return {
			sSearch:          obj.search,
			bSmart:           obj.smart,
			bRegex:           obj.regex,
			bCaseInsensitive: obj.caseInsensitive
		};
	}
	
	/**
	 * Generate the node required for the info display
	 *  @param {object} oSettings dataTables settings object
	 *  @returns {node} Information element
	 *  @memberof DataTable#oApi
	 */
	function _fnFeatureHtmlInfo ( settings )
	{
		var
			tid = settings.sTableId,
			nodes = settings.aanFeatures.i,
			n = $('<div/>', {
				'class': settings.oClasses.sInfo,
				'id': ! nodes ? tid+'_info' : null
			} );
	
		if ( ! nodes ) {
			// Update display on each draw
			settings.aoDrawCallback.push( {
				"fn": _fnUpdateInfo,
				"sName": "information"
			} );
	
			n
				.attr( 'role', 'status' )
				.attr( 'aria-live', 'polite' );
	
			// Table is described by our info div
			$(settings.nTable).attr( 'aria-describedby', tid+'_info' );
		}
	
		return n[0];
	}
	
	
	/**
	 * Update the information elements in the display
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnUpdateInfo ( settings )
	{
		/* Show information about the table */
		var nodes = settings.aanFeatures.i;
		if ( nodes.length === 0 ) {
			return;
		}
	
		var
			lang  = settings.oLanguage,
			start = settings._iDisplayStart+1,
			end   = settings.fnDisplayEnd(),
			max   = settings.fnRecordsTotal(),
			total = settings.fnRecordsDisplay(),
			out   = total ?
				lang.sInfo :
				lang.sInfoEmpty;
	
		if ( total !== max ) {
			/* Record set after filtering */
			out += ' ' + lang.sInfoFiltered;
		}
	
		// Convert the macros
		out += lang.sInfoPostFix;
		out = _fnInfoMacros( settings, out );
	
		var callback = lang.fnInfoCallback;
		if ( callback !== null ) {
			out = callback.call( settings.oInstance,
				settings, start, end, max, total, out
			);
		}
	
		$(nodes).html( out );
	}
	
	
	function _fnInfoMacros ( settings, str )
	{
		// When infinite scrolling, we are always starting at 1. _iDisplayStart is used only
		// internally
		var
			formatter  = settings.fnFormatNumber,
			start      = settings._iDisplayStart+1,
			len        = settings._iDisplayLength,
			vis        = settings.fnRecordsDisplay(),
			all        = len === -1;
	
		return str.
			replace(/_START_/g, formatter.call( settings, start ) ).
			replace(/_END_/g,   formatter.call( settings, settings.fnDisplayEnd() ) ).
			replace(/_MAX_/g,   formatter.call( settings, settings.fnRecordsTotal() ) ).
			replace(/_TOTAL_/g, formatter.call( settings, vis ) ).
			replace(/_PAGE_/g,  formatter.call( settings, all ? 1 : Math.ceil( start / len ) ) ).
			replace(/_PAGES_/g, formatter.call( settings, all ? 1 : Math.ceil( vis / len ) ) );
	}
	
	
	
	/**
	 * Draw the table for the first time, adding all required features
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnInitialise ( settings )
	{
		var i, iLen, iAjaxStart=settings.iInitDisplayStart;
		var columns = settings.aoColumns, column;
		var features = settings.oFeatures;
		var deferLoading = settings.bDeferLoading; // value modified by the draw
	
		/* Ensure that the table data is fully initialised */
		if ( ! settings.bInitialised ) {
			setTimeout( function(){ _fnInitialise( settings ); }, 200 );
			return;
		}
	
		/* Show the display HTML options */
		_fnAddOptionsHtml( settings );
	
		/* Build and draw the header / footer for the table */
		_fnBuildHead( settings );
		_fnDrawHead( settings, settings.aoHeader );
		_fnDrawHead( settings, settings.aoFooter );
	
		/* Okay to show that something is going on now */
		_fnProcessingDisplay( settings, true );
	
		/* Calculate sizes for columns */
		if ( features.bAutoWidth ) {
			_fnCalculateColumnWidths( settings );
		}
	
		for ( i=0, iLen=columns.length ; i<iLen ; i++ ) {
			column = columns[i];
	
			if ( column.sWidth ) {
				column.nTh.style.width = _fnStringToCss( column.sWidth );
			}
		}
	
		_fnCallbackFire( settings, null, 'preInit', [settings] );
	
		// If there is default sorting required - let's do it. The sort function
		// will do the drawing for us. Otherwise we draw the table regardless of the
		// Ajax source - this allows the table to look initialised for Ajax sourcing
		// data (show 'loading' message possibly)
		_fnReDraw( settings );
	
		// Server-side processing init complete is done by _fnAjaxUpdateDraw
		var dataSrc = _fnDataSource( settings );
		if ( dataSrc != 'ssp' || deferLoading ) {
			// if there is an ajax source load the data
			if ( dataSrc == 'ajax' ) {
				_fnBuildAjax( settings, [], function(json) {
					var aData = _fnAjaxDataSrc( settings, json );
	
					// Got the data - add it to the table
					for ( i=0 ; i<aData.length ; i++ ) {
						_fnAddData( settings, aData[i] );
					}
	
					// Reset the init display for cookie saving. We've already done
					// a filter, and therefore cleared it before. So we need to make
					// it appear 'fresh'
					settings.iInitDisplayStart = iAjaxStart;
	
					_fnReDraw( settings );
	
					_fnProcessingDisplay( settings, false );
					_fnInitComplete( settings, json );
				}, settings );
			}
			else {
				_fnProcessingDisplay( settings, false );
				_fnInitComplete( settings );
			}
		}
	}
	
	
	/**
	 * Draw the table for the first time, adding all required features
	 *  @param {object} oSettings dataTables settings object
	 *  @param {object} [json] JSON from the server that completed the table, if using Ajax source
	 *    with client-side processing (optional)
	 *  @memberof DataTable#oApi
	 */
	function _fnInitComplete ( settings, json )
	{
		settings._bInitComplete = true;
	
		// When data was added after the initialisation (data or Ajax) we need to
		// calculate the column sizing
		if ( json || settings.oInit.aaData ) {
			_fnAdjustColumnSizing( settings );
		}
	
		_fnCallbackFire( settings, null, 'plugin-init', [settings, json] );
		_fnCallbackFire( settings, 'aoInitComplete', 'init', [settings, json] );
	}
	
	
	function _fnLengthChange ( settings, val )
	{
		var len = parseInt( val, 10 );
		settings._iDisplayLength = len;
	
		_fnLengthOverflow( settings );
	
		// Fire length change event
		_fnCallbackFire( settings, null, 'length', [settings, len] );
	}
	
	
	/**
	 * Generate the node required for user display length changing
	 *  @param {object} settings dataTables settings object
	 *  @returns {node} Display length feature node
	 *  @memberof DataTable#oApi
	 */
	function _fnFeatureHtmlLength ( settings )
	{
		var
			classes  = settings.oClasses,
			tableId  = settings.sTableId,
			menu     = settings.aLengthMenu,
			d2       = $.isArray( menu[0] ),
			lengths  = d2 ? menu[0] : menu,
			language = d2 ? menu[1] : menu;
	
		var select = $('<select/>', {
			'name':          tableId+'_length',
			'aria-controls': tableId,
			'class':         classes.sLengthSelect
		} );
	
		for ( var i=0, ien=lengths.length ; i<ien ; i++ ) {
			select[0][ i ] = new Option(
				typeof language[i] === 'number' ?
					settings.fnFormatNumber( language[i] ) :
					language[i],
				lengths[i]
			);
		}
	
		var div = $('<div><label/></div>').addClass( classes.sLength );
		if ( ! settings.aanFeatures.l ) {
			div[0].id = tableId+'_length';
		}
	
		div.children().append(
			settings.oLanguage.sLengthMenu.replace( '_MENU_', select[0].outerHTML )
		);
	
		// Can't use `select` variable as user might provide their own and the
		// reference is broken by the use of outerHTML
		$('select', div)
			.val( settings._iDisplayLength )
			.on( 'change.DT', function(e) {
				_fnLengthChange( settings, $(this).val() );
				_fnDraw( settings );
			} );
	
		// Update node value whenever anything changes the table's length
		$(settings.nTable).on( 'length.dt.DT', function (e, s, len) {
			if ( settings === s ) {
				$('select', div).val( len );
			}
		} );
	
		return div[0];
	}
	
	
	
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Note that most of the paging logic is done in
	 * DataTable.ext.pager
	 */
	
	/**
	 * Generate the node required for default pagination
	 *  @param {object} oSettings dataTables settings object
	 *  @returns {node} Pagination feature node
	 *  @memberof DataTable#oApi
	 */
	function _fnFeatureHtmlPaginate ( settings )
	{
		var
			type   = settings.sPaginationType,
			plugin = DataTable.ext.pager[ type ],
			modern = typeof plugin === 'function',
			redraw = function( settings ) {
				_fnDraw( settings );
			},
			node = $('<div/>').addClass( settings.oClasses.sPaging + type )[0],
			features = settings.aanFeatures;
	
		if ( ! modern ) {
			plugin.fnInit( settings, node, redraw );
		}
	
		/* Add a draw callback for the pagination on first instance, to update the paging display */
		if ( ! features.p )
		{
			node.id = settings.sTableId+'_paginate';
	
			settings.aoDrawCallback.push( {
				"fn": function( settings ) {
					if ( modern ) {
						var
							start      = settings._iDisplayStart,
							len        = settings._iDisplayLength,
							visRecords = settings.fnRecordsDisplay(),
							all        = len === -1,
							page = all ? 0 : Math.ceil( start / len ),
							pages = all ? 1 : Math.ceil( visRecords / len ),
							buttons = plugin(page, pages),
							i, ien;
	
						for ( i=0, ien=features.p.length ; i<ien ; i++ ) {
							_fnRenderer( settings, 'pageButton' )(
								settings, features.p[i], i, buttons, page, pages
							);
						}
					}
					else {
						plugin.fnUpdate( settings, redraw );
					}
				},
				"sName": "pagination"
			} );
		}
	
		return node;
	}
	
	
	/**
	 * Alter the display settings to change the page
	 *  @param {object} settings DataTables settings object
	 *  @param {string|int} action Paging action to take: "first", "previous",
	 *    "next" or "last" or page number to jump to (integer)
	 *  @param [bool] redraw Automatically draw the update or not
	 *  @returns {bool} true page has changed, false - no change
	 *  @memberof DataTable#oApi
	 */
	function _fnPageChange ( settings, action, redraw )
	{
		var
			start     = settings._iDisplayStart,
			len       = settings._iDisplayLength,
			records   = settings.fnRecordsDisplay();
	
		if ( records === 0 || len === -1 )
		{
			start = 0;
		}
		else if ( typeof action === "number" )
		{
			start = action * len;
	
			if ( start > records )
			{
				start = 0;
			}
		}
		else if ( action == "first" )
		{
			start = 0;
		}
		else if ( action == "previous" )
		{
			start = len >= 0 ?
				start - len :
				0;
	
			if ( start < 0 )
			{
			  start = 0;
			}
		}
		else if ( action == "next" )
		{
			if ( start + len < records )
			{
				start += len;
			}
		}
		else if ( action == "last" )
		{
			start = Math.floor( (records-1) / len) * len;
		}
		else
		{
			_fnLog( settings, 0, "Unknown paging action: "+action, 5 );
		}
	
		var changed = settings._iDisplayStart !== start;
		settings._iDisplayStart = start;
	
		if ( changed ) {
			_fnCallbackFire( settings, null, 'page', [settings] );
	
			if ( redraw ) {
				_fnDraw( settings );
			}
		}
	
		return changed;
	}
	
	
	
	/**
	 * Generate the node required for the processing node
	 *  @param {object} settings dataTables settings object
	 *  @returns {node} Processing element
	 *  @memberof DataTable#oApi
	 */
	function _fnFeatureHtmlProcessing ( settings )
	{
		return $('<div/>', {
				'id': ! settings.aanFeatures.r ? settings.sTableId+'_processing' : null,
				'class': settings.oClasses.sProcessing
			} )
			.html( settings.oLanguage.sProcessing )
			.insertBefore( settings.nTable )[0];
	}
	
	
	/**
	 * Display or hide the processing indicator
	 *  @param {object} settings dataTables settings object
	 *  @param {bool} show Show the processing indicator (true) or not (false)
	 *  @memberof DataTable#oApi
	 */
	function _fnProcessingDisplay ( settings, show )
	{
		if ( settings.oFeatures.bProcessing ) {
			$(settings.aanFeatures.r).css( 'display', show ? 'block' : 'none' );
		}
	
		_fnCallbackFire( settings, null, 'processing', [settings, show] );
	}
	
	/**
	 * Add any control elements for the table - specifically scrolling
	 *  @param {object} settings dataTables settings object
	 *  @returns {node} Node to add to the DOM
	 *  @memberof DataTable#oApi
	 */
	function _fnFeatureHtmlTable ( settings )
	{
		var table = $(settings.nTable);
	
		// Add the ARIA grid role to the table
		table.attr( 'role', 'grid' );
	
		// Scrolling from here on in
		var scroll = settings.oScroll;
	
		if ( scroll.sX === '' && scroll.sY === '' ) {
			return settings.nTable;
		}
	
		var scrollX = scroll.sX;
		var scrollY = scroll.sY;
		var classes = settings.oClasses;
		var caption = table.children('caption');
		var captionSide = caption.length ? caption[0]._captionSide : null;
		var headerClone = $( table[0].cloneNode(false) );
		var footerClone = $( table[0].cloneNode(false) );
		var footer = table.children('tfoot');
		var _div = '<div/>';
		var size = function ( s ) {
			return !s ? null : _fnStringToCss( s );
		};
	
		if ( ! footer.length ) {
			footer = null;
		}
	
		/*
		 * The HTML structure that we want to generate in this function is:
		 *  div - scroller
		 *    div - scroll head
		 *      div - scroll head inner
		 *        table - scroll head table
		 *          thead - thead
		 *    div - scroll body
		 *      table - table (master table)
		 *        thead - thead clone for sizing
		 *        tbody - tbody
		 *    div - scroll foot
		 *      div - scroll foot inner
		 *        table - scroll foot table
		 *          tfoot - tfoot
		 */
		var scroller = $( _div, { 'class': classes.sScrollWrapper } )
			.append(
				$(_div, { 'class': classes.sScrollHead } )
					.css( {
						overflow: 'hidden',
						position: 'relative',
						border: 0,
						width: scrollX ? size(scrollX) : '100%'
					} )
					.append(
						$(_div, { 'class': classes.sScrollHeadInner } )
							.css( {
								'box-sizing': 'content-box',
								width: scroll.sXInner || '100%'
							} )
							.append(
								headerClone
									.removeAttr('id')
									.css( 'margin-left', 0 )
									.append( captionSide === 'top' ? caption : null )
									.append(
										table.children('thead')
									)
							)
					)
			)
			.append(
				$(_div, { 'class': classes.sScrollBody } )
					.css( {
						position: 'relative',
						overflow: 'auto',
						width: size( scrollX )
					} )
					.append( table )
			);
	
		if ( footer ) {
			scroller.append(
				$(_div, { 'class': classes.sScrollFoot } )
					.css( {
						overflow: 'hidden',
						border: 0,
						width: scrollX ? size(scrollX) : '100%'
					} )
					.append(
						$(_div, { 'class': classes.sScrollFootInner } )
							.append(
								footerClone
									.removeAttr('id')
									.css( 'margin-left', 0 )
									.append( captionSide === 'bottom' ? caption : null )
									.append(
										table.children('tfoot')
									)
							)
					)
			);
		}
	
		var children = scroller.children();
		var scrollHead = children[0];
		var scrollBody = children[1];
		var scrollFoot = footer ? children[2] : null;
	
		// When the body is scrolled, then we also want to scroll the headers
		if ( scrollX ) {
			$(scrollBody).on( 'scroll.DT', function (e) {
				var scrollLeft = this.scrollLeft;
	
				scrollHead.scrollLeft = scrollLeft;
	
				if ( footer ) {
					scrollFoot.scrollLeft = scrollLeft;
				}
			} );
		}
	
		$(scrollBody).css(
			scrollY && scroll.bCollapse ? 'max-height' : 'height', 
			scrollY
		);
	
		settings.nScrollHead = scrollHead;
		settings.nScrollBody = scrollBody;
		settings.nScrollFoot = scrollFoot;
	
		// On redraw - align columns
		settings.aoDrawCallback.push( {
			"fn": _fnScrollDraw,
			"sName": "scrolling"
		} );
	
		return scroller[0];
	}
	
	
	
	/**
	 * Update the header, footer and body tables for resizing - i.e. column
	 * alignment.
	 *
	 * Welcome to the most horrible function DataTables. The process that this
	 * function follows is basically:
	 *   1. Re-create the table inside the scrolling div
	 *   2. Take live measurements from the DOM
	 *   3. Apply the measurements to align the columns
	 *   4. Clean up
	 *
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnScrollDraw ( settings )
	{
		// Given that this is such a monster function, a lot of variables are use
		// to try and keep the minimised size as small as possible
		var
			scroll         = settings.oScroll,
			scrollX        = scroll.sX,
			scrollXInner   = scroll.sXInner,
			scrollY        = scroll.sY,
			barWidth       = scroll.iBarWidth,
			divHeader      = $(settings.nScrollHead),
			divHeaderStyle = divHeader[0].style,
			divHeaderInner = divHeader.children('div'),
			divHeaderInnerStyle = divHeaderInner[0].style,
			divHeaderTable = divHeaderInner.children('table'),
			divBodyEl      = settings.nScrollBody,
			divBody        = $(divBodyEl),
			divBodyStyle   = divBodyEl.style,
			divFooter      = $(settings.nScrollFoot),
			divFooterInner = divFooter.children('div'),
			divFooterTable = divFooterInner.children('table'),
			header         = $(settings.nTHead),
			table          = $(settings.nTable),
			tableEl        = table[0],
			tableStyle     = tableEl.style,
			footer         = settings.nTFoot ? $(settings.nTFoot) : null,
			browser        = settings.oBrowser,
			ie67           = browser.bScrollOversize,
			dtHeaderCells  = _pluck( settings.aoColumns, 'nTh' ),
			headerTrgEls, footerTrgEls,
			headerSrcEls, footerSrcEls,
			headerCopy, footerCopy,
			headerWidths=[], footerWidths=[],
			headerContent=[], footerContent=[],
			idx, correction, sanityWidth,
			zeroOut = function(nSizer) {
				var style = nSizer.style;
				style.paddingTop = "0";
				style.paddingBottom = "0";
				style.borderTopWidth = "0";
				style.borderBottomWidth = "0";
				style.height = 0;
			};
	
		// If the scrollbar visibility has changed from the last draw, we need to
		// adjust the column sizes as the table width will have changed to account
		// for the scrollbar
		var scrollBarVis = divBodyEl.scrollHeight > divBodyEl.clientHeight;
		
		if ( settings.scrollBarVis !== scrollBarVis && settings.scrollBarVis !== undefined ) {
			settings.scrollBarVis = scrollBarVis;
			_fnAdjustColumnSizing( settings );
			return; // adjust column sizing will call this function again
		}
		else {
			settings.scrollBarVis = scrollBarVis;
		}
	
		/*
		 * 1. Re-create the table inside the scrolling div
		 */
	
		// Remove the old minimised thead and tfoot elements in the inner table
		table.children('thead, tfoot').remove();
	
		if ( footer ) {
			footerCopy = footer.clone().prependTo( table );
			footerTrgEls = footer.find('tr'); // the original tfoot is in its own table and must be sized
			footerSrcEls = footerCopy.find('tr');
		}
	
		// Clone the current header and footer elements and then place it into the inner table
		headerCopy = header.clone().prependTo( table );
		headerTrgEls = header.find('tr'); // original header is in its own table
		headerSrcEls = headerCopy.find('tr');
		headerCopy.find('th, td').removeAttr('tabindex');
	
	
		/*
		 * 2. Take live measurements from the DOM - do not alter the DOM itself!
		 */
	
		// Remove old sizing and apply the calculated column widths
		// Get the unique column headers in the newly created (cloned) header. We want to apply the
		// calculated sizes to this header
		if ( ! scrollX )
		{
			divBodyStyle.width = '100%';
			divHeader[0].style.width = '100%';
		}
	
		$.each( _fnGetUniqueThs( settings, headerCopy ), function ( i, el ) {
			idx = _fnVisibleToColumnIndex( settings, i );
			el.style.width = settings.aoColumns[idx].sWidth;
		} );
	
		if ( footer ) {
			_fnApplyToChildren( function(n) {
				n.style.width = "";
			}, footerSrcEls );
		}
	
		// Size the table as a whole
		sanityWidth = table.outerWidth();
		if ( scrollX === "" ) {
			// No x scrolling
			tableStyle.width = "100%";
	
			// IE7 will make the width of the table when 100% include the scrollbar
			// - which is shouldn't. When there is a scrollbar we need to take this
			// into account.
			if ( ie67 && (table.find('tbody').height() > divBodyEl.offsetHeight ||
				divBody.css('overflow-y') == "scroll")
			) {
				tableStyle.width = _fnStringToCss( table.outerWidth() - barWidth);
			}
	
			// Recalculate the sanity width
			sanityWidth = table.outerWidth();
		}
		else if ( scrollXInner !== "" ) {
			// legacy x scroll inner has been given - use it
			tableStyle.width = _fnStringToCss(scrollXInner);
	
			// Recalculate the sanity width
			sanityWidth = table.outerWidth();
		}
	
		// Hidden header should have zero height, so remove padding and borders. Then
		// set the width based on the real headers
	
		// Apply all styles in one pass
		_fnApplyToChildren( zeroOut, headerSrcEls );
	
		// Read all widths in next pass
		_fnApplyToChildren( function(nSizer) {
			headerContent.push( nSizer.innerHTML );
			headerWidths.push( _fnStringToCss( $(nSizer).css('width') ) );
		}, headerSrcEls );
	
		// Apply all widths in final pass
		_fnApplyToChildren( function(nToSize, i) {
			// Only apply widths to the DataTables detected header cells - this
			// prevents complex headers from having contradictory sizes applied
			if ( $.inArray( nToSize, dtHeaderCells ) !== -1 ) {
				nToSize.style.width = headerWidths[i];
			}
		}, headerTrgEls );
	
		$(headerSrcEls).height(0);
	
		/* Same again with the footer if we have one */
		if ( footer )
		{
			_fnApplyToChildren( zeroOut, footerSrcEls );
	
			_fnApplyToChildren( function(nSizer) {
				footerContent.push( nSizer.innerHTML );
				footerWidths.push( _fnStringToCss( $(nSizer).css('width') ) );
			}, footerSrcEls );
	
			_fnApplyToChildren( function(nToSize, i) {
				nToSize.style.width = footerWidths[i];
			}, footerTrgEls );
	
			$(footerSrcEls).height(0);
		}
	
	
		/*
		 * 3. Apply the measurements
		 */
	
		// "Hide" the header and footer that we used for the sizing. We need to keep
		// the content of the cell so that the width applied to the header and body
		// both match, but we want to hide it completely. We want to also fix their
		// width to what they currently are
		_fnApplyToChildren( function(nSizer, i) {
			nSizer.innerHTML = '<div class="dataTables_sizing">'+headerContent[i]+'</div>';
			nSizer.childNodes[0].style.height = "0";
			nSizer.childNodes[0].style.overflow = "hidden";
			nSizer.style.width = headerWidths[i];
		}, headerSrcEls );
	
		if ( footer )
		{
			_fnApplyToChildren( function(nSizer, i) {
				nSizer.innerHTML = '<div class="dataTables_sizing">'+footerContent[i]+'</div>';
				nSizer.childNodes[0].style.height = "0";
				nSizer.childNodes[0].style.overflow = "hidden";
				nSizer.style.width = footerWidths[i];
			}, footerSrcEls );
		}
	
		// Sanity check that the table is of a sensible width. If not then we are going to get
		// misalignment - try to prevent this by not allowing the table to shrink below its min width
		if ( table.outerWidth() < sanityWidth )
		{
			// The min width depends upon if we have a vertical scrollbar visible or not */
			correction = ((divBodyEl.scrollHeight > divBodyEl.offsetHeight ||
				divBody.css('overflow-y') == "scroll")) ?
					sanityWidth+barWidth :
					sanityWidth;
	
			// IE6/7 are a law unto themselves...
			if ( ie67 && (divBodyEl.scrollHeight >
				divBodyEl.offsetHeight || divBody.css('overflow-y') == "scroll")
			) {
				tableStyle.width = _fnStringToCss( correction-barWidth );
			}
	
			// And give the user a warning that we've stopped the table getting too small
			if ( scrollX === "" || scrollXInner !== "" ) {
				_fnLog( settings, 1, 'Possible column misalignment', 6 );
			}
		}
		else
		{
			correction = '100%';
		}
	
		// Apply to the container elements
		divBodyStyle.width = _fnStringToCss( correction );
		divHeaderStyle.width = _fnStringToCss( correction );
	
		if ( footer ) {
			settings.nScrollFoot.style.width = _fnStringToCss( correction );
		}
	
	
		/*
		 * 4. Clean up
		 */
		if ( ! scrollY ) {
			/* IE7< puts a vertical scrollbar in place (when it shouldn't be) due to subtracting
			 * the scrollbar height from the visible display, rather than adding it on. We need to
			 * set the height in order to sort this. Don't want to do it in any other browsers.
			 */
			if ( ie67 ) {
				divBodyStyle.height = _fnStringToCss( tableEl.offsetHeight+barWidth );
			}
		}
	
		/* Finally set the width's of the header and footer tables */
		var iOuterWidth = table.outerWidth();
		divHeaderTable[0].style.width = _fnStringToCss( iOuterWidth );
		divHeaderInnerStyle.width = _fnStringToCss( iOuterWidth );
	
		// Figure out if there are scrollbar present - if so then we need a the header and footer to
		// provide a bit more space to allow "overflow" scrolling (i.e. past the scrollbar)
		var bScrolling = table.height() > divBodyEl.clientHeight || divBody.css('overflow-y') == "scroll";
		var padding = 'padding' + (browser.bScrollbarLeft ? 'Left' : 'Right' );
		divHeaderInnerStyle[ padding ] = bScrolling ? barWidth+"px" : "0px";
	
		if ( footer ) {
			divFooterTable[0].style.width = _fnStringToCss( iOuterWidth );
			divFooterInner[0].style.width = _fnStringToCss( iOuterWidth );
			divFooterInner[0].style[padding] = bScrolling ? barWidth+"px" : "0px";
		}
	
		// Correct DOM ordering for colgroup - comes before the thead
		table.children('colgroup').insertBefore( table.children('thead') );
	
		/* Adjust the position of the header in case we loose the y-scrollbar */
		divBody.scroll();
	
		// If sorting or filtering has occurred, jump the scrolling back to the top
		// only if we aren't holding the position
		if ( (settings.bSorted || settings.bFiltered) && ! settings._drawHold ) {
			divBodyEl.scrollTop = 0;
		}
	}
	
	
	
	/**
	 * Apply a given function to the display child nodes of an element array (typically
	 * TD children of TR rows
	 *  @param {function} fn Method to apply to the objects
	 *  @param array {nodes} an1 List of elements to look through for display children
	 *  @param array {nodes} an2 Another list (identical structure to the first) - optional
	 *  @memberof DataTable#oApi
	 */
	function _fnApplyToChildren( fn, an1, an2 )
	{
		var index=0, i=0, iLen=an1.length;
		var nNode1, nNode2;
	
		while ( i < iLen ) {
			nNode1 = an1[i].firstChild;
			nNode2 = an2 ? an2[i].firstChild : null;
	
			while ( nNode1 ) {
				if ( nNode1.nodeType === 1 ) {
					if ( an2 ) {
						fn( nNode1, nNode2, index );
					}
					else {
						fn( nNode1, index );
					}
	
					index++;
				}
	
				nNode1 = nNode1.nextSibling;
				nNode2 = an2 ? nNode2.nextSibling : null;
			}
	
			i++;
		}
	}
	
	
	
	var __re_html_remove = /<.*?>/g;
	
	
	/**
	 * Calculate the width of columns for the table
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnCalculateColumnWidths ( oSettings )
	{
		var
			table = oSettings.nTable,
			columns = oSettings.aoColumns,
			scroll = oSettings.oScroll,
			scrollY = scroll.sY,
			scrollX = scroll.sX,
			scrollXInner = scroll.sXInner,
			columnCount = columns.length,
			visibleColumns = _fnGetColumns( oSettings, 'bVisible' ),
			headerCells = $('th', oSettings.nTHead),
			tableWidthAttr = table.getAttribute('width'), // from DOM element
			tableContainer = table.parentNode,
			userInputs = false,
			i, column, columnIdx, width, outerWidth,
			browser = oSettings.oBrowser,
			ie67 = browser.bScrollOversize;
	
		var styleWidth = table.style.width;
		if ( styleWidth && styleWidth.indexOf('%') !== -1 ) {
			tableWidthAttr = styleWidth;
		}
	
		/* Convert any user input sizes into pixel sizes */
		for ( i=0 ; i<visibleColumns.length ; i++ ) {
			column = columns[ visibleColumns[i] ];
	
			if ( column.sWidth !== null ) {
				column.sWidth = _fnConvertToWidth( column.sWidthOrig, tableContainer );
	
				userInputs = true;
			}
		}
	
		/* If the number of columns in the DOM equals the number that we have to
		 * process in DataTables, then we can use the offsets that are created by
		 * the web- browser. No custom sizes can be set in order for this to happen,
		 * nor scrolling used
		 */
		if ( ie67 || ! userInputs && ! scrollX && ! scrollY &&
		     columnCount == _fnVisbleColumns( oSettings ) &&
		     columnCount == headerCells.length
		) {
			for ( i=0 ; i<columnCount ; i++ ) {
				var colIdx = _fnVisibleToColumnIndex( oSettings, i );
	
				if ( colIdx !== null ) {
					columns[ colIdx ].sWidth = _fnStringToCss( headerCells.eq(i).width() );
				}
			}
		}
		else
		{
			// Otherwise construct a single row, worst case, table with the widest
			// node in the data, assign any user defined widths, then insert it into
			// the DOM and allow the browser to do all the hard work of calculating
			// table widths
			var tmpTable = $(table).clone() // don't use cloneNode - IE8 will remove events on the main table
				.css( 'visibility', 'hidden' )
				.removeAttr( 'id' );
	
			// Clean up the table body
			tmpTable.find('tbody tr').remove();
			var tr = $('<tr/>').appendTo( tmpTable.find('tbody') );
	
			// Clone the table header and footer - we can't use the header / footer
			// from the cloned table, since if scrolling is active, the table's
			// real header and footer are contained in different table tags
			tmpTable.find('thead, tfoot').remove();
			tmpTable
				.append( $(oSettings.nTHead).clone() )
				.append( $(oSettings.nTFoot).clone() );
	
			// Remove any assigned widths from the footer (from scrolling)
			tmpTable.find('tfoot th, tfoot td').css('width', '');
	
			// Apply custom sizing to the cloned header
			headerCells = _fnGetUniqueThs( oSettings, tmpTable.find('thead')[0] );
	
			for ( i=0 ; i<visibleColumns.length ; i++ ) {
				column = columns[ visibleColumns[i] ];
	
				headerCells[i].style.width = column.sWidthOrig !== null && column.sWidthOrig !== '' ?
					_fnStringToCss( column.sWidthOrig ) :
					'';
	
				// For scrollX we need to force the column width otherwise the
				// browser will collapse it. If this width is smaller than the
				// width the column requires, then it will have no effect
				if ( column.sWidthOrig && scrollX ) {
					$( headerCells[i] ).append( $('<div/>').css( {
						width: column.sWidthOrig,
						margin: 0,
						padding: 0,
						border: 0,
						height: 1
					} ) );
				}
			}
	
			// Find the widest cell for each column and put it into the table
			if ( oSettings.aoData.length ) {
				for ( i=0 ; i<visibleColumns.length ; i++ ) {
					columnIdx = visibleColumns[i];
					column = columns[ columnIdx ];
	
					$( _fnGetWidestNode( oSettings, columnIdx ) )
						.clone( false )
						.append( column.sContentPadding )
						.appendTo( tr );
				}
			}
	
			// Tidy the temporary table - remove name attributes so there aren't
			// duplicated in the dom (radio elements for example)
			$('[name]', tmpTable).removeAttr('name');
	
			// Table has been built, attach to the document so we can work with it.
			// A holding element is used, positioned at the top of the container
			// with minimal height, so it has no effect on if the container scrolls
			// or not. Otherwise it might trigger scrolling when it actually isn't
			// needed
			var holder = $('<div/>').css( scrollX || scrollY ?
					{
						position: 'absolute',
						top: 0,
						left: 0,
						height: 1,
						right: 0,
						overflow: 'hidden'
					} :
					{}
				)
				.append( tmpTable )
				.appendTo( tableContainer );
	
			// When scrolling (X or Y) we want to set the width of the table as 
			// appropriate. However, when not scrolling leave the table width as it
			// is. This results in slightly different, but I think correct behaviour
			if ( scrollX && scrollXInner ) {
				tmpTable.width( scrollXInner );
			}
			else if ( scrollX ) {
				tmpTable.css( 'width', 'auto' );
				tmpTable.removeAttr('width');
	
				// If there is no width attribute or style, then allow the table to
				// collapse
				if ( tmpTable.width() < tableContainer.clientWidth && tableWidthAttr ) {
					tmpTable.width( tableContainer.clientWidth );
				}
			}
			else if ( scrollY ) {
				tmpTable.width( tableContainer.clientWidth );
			}
			else if ( tableWidthAttr ) {
				tmpTable.width( tableWidthAttr );
			}
	
			// Get the width of each column in the constructed table - we need to
			// know the inner width (so it can be assigned to the other table's
			// cells) and the outer width so we can calculate the full width of the
			// table. This is safe since DataTables requires a unique cell for each
			// column, but if ever a header can span multiple columns, this will
			// need to be modified.
			var total = 0;
			for ( i=0 ; i<visibleColumns.length ; i++ ) {
				var cell = $(headerCells[i]);
				var border = cell.outerWidth() - cell.width();
	
				// Use getBounding... where possible (not IE8-) because it can give
				// sub-pixel accuracy, which we then want to round up!
				var bounding = browser.bBounding ?
					Math.ceil( headerCells[i].getBoundingClientRect().width ) :
					cell.outerWidth();
	
				// Total is tracked to remove any sub-pixel errors as the outerWidth
				// of the table might not equal the total given here (IE!).
				total += bounding;
	
				// Width for each column to use
				columns[ visibleColumns[i] ].sWidth = _fnStringToCss( bounding - border );
			}
	
			table.style.width = _fnStringToCss( total );
	
			// Finished with the table - ditch it
			holder.remove();
		}
	
		// If there is a width attr, we want to attach an event listener which
		// allows the table sizing to automatically adjust when the window is
		// resized. Use the width attr rather than CSS, since we can't know if the
		// CSS is a relative value or absolute - DOM read is always px.
		if ( tableWidthAttr ) {
			table.style.width = _fnStringToCss( tableWidthAttr );
		}
	
		if ( (tableWidthAttr || scrollX) && ! oSettings._reszEvt ) {
			var bindResize = function () {
				$(window).on('resize.DT-'+oSettings.sInstance, _fnThrottle( function () {
					_fnAdjustColumnSizing( oSettings );
				} ) );
			};
	
			// IE6/7 will crash if we bind a resize event handler on page load.
			// To be removed in 1.11 which drops IE6/7 support
			if ( ie67 ) {
				setTimeout( bindResize, 1000 );
			}
			else {
				bindResize();
			}
	
			oSettings._reszEvt = true;
		}
	}
	
	
	/**
	 * Throttle the calls to a function. Arguments and context are maintained for
	 * the throttled function
	 *  @param {function} fn Function to be called
	 *  @param {int} [freq=200] call frequency in mS
	 *  @returns {function} wrapped function
	 *  @memberof DataTable#oApi
	 */
	var _fnThrottle = DataTable.util.throttle;
	
	
	/**
	 * Convert a CSS unit width to pixels (e.g. 2em)
	 *  @param {string} width width to be converted
	 *  @param {node} parent parent to get the with for (required for relative widths) - optional
	 *  @returns {int} width in pixels
	 *  @memberof DataTable#oApi
	 */
	function _fnConvertToWidth ( width, parent )
	{
		if ( ! width ) {
			return 0;
		}
	
		var n = $('<div/>')
			.css( 'width', _fnStringToCss( width ) )
			.appendTo( parent || document.body );
	
		var val = n[0].offsetWidth;
		n.remove();
	
		return val;
	}
	
	
	/**
	 * Get the widest node
	 *  @param {object} settings dataTables settings object
	 *  @param {int} colIdx column of interest
	 *  @returns {node} widest table node
	 *  @memberof DataTable#oApi
	 */
	function _fnGetWidestNode( settings, colIdx )
	{
		var idx = _fnGetMaxLenString( settings, colIdx );
		if ( idx < 0 ) {
			return null;
		}
	
		var data = settings.aoData[ idx ];
		return ! data.nTr ? // Might not have been created when deferred rendering
			$('<td/>').html( _fnGetCellData( settings, idx, colIdx, 'display' ) )[0] :
			data.anCells[ colIdx ];
	}
	
	
	/**
	 * Get the maximum strlen for each data column
	 *  @param {object} settings dataTables settings object
	 *  @param {int} colIdx column of interest
	 *  @returns {string} max string length for each column
	 *  @memberof DataTable#oApi
	 */
	function _fnGetMaxLenString( settings, colIdx )
	{
		var s, max=-1, maxIdx = -1;
	
		for ( var i=0, ien=settings.aoData.length ; i<ien ; i++ ) {
			s = _fnGetCellData( settings, i, colIdx, 'display' )+'';
			s = s.replace( __re_html_remove, '' );
			s = s.replace( /&nbsp;/g, ' ' );
	
			if ( s.length > max ) {
				max = s.length;
				maxIdx = i;
			}
		}
	
		return maxIdx;
	}
	
	
	/**
	 * Append a CSS unit (only if required) to a string
	 *  @param {string} value to css-ify
	 *  @returns {string} value with css unit
	 *  @memberof DataTable#oApi
	 */
	function _fnStringToCss( s )
	{
		if ( s === null ) {
			return '0px';
		}
	
		if ( typeof s == 'number' ) {
			return s < 0 ?
				'0px' :
				s+'px';
		}
	
		// Check it has a unit character already
		return s.match(/\d$/) ?
			s+'px' :
			s;
	}
	
	
	
	function _fnSortFlatten ( settings )
	{
		var
			i, iLen, k, kLen,
			aSort = [],
			aiOrig = [],
			aoColumns = settings.aoColumns,
			aDataSort, iCol, sType, srcCol,
			fixed = settings.aaSortingFixed,
			fixedObj = $.isPlainObject( fixed ),
			nestedSort = [],
			add = function ( a ) {
				if ( a.length && ! $.isArray( a[0] ) ) {
					// 1D array
					nestedSort.push( a );
				}
				else {
					// 2D array
					$.merge( nestedSort, a );
				}
			};
	
		// Build the sort array, with pre-fix and post-fix options if they have been
		// specified
		if ( $.isArray( fixed ) ) {
			add( fixed );
		}
	
		if ( fixedObj && fixed.pre ) {
			add( fixed.pre );
		}
	
		add( settings.aaSorting );
	
		if (fixedObj && fixed.post ) {
			add( fixed.post );
		}
	
		for ( i=0 ; i<nestedSort.length ; i++ )
		{
			srcCol = nestedSort[i][0];
			aDataSort = aoColumns[ srcCol ].aDataSort;
	
			for ( k=0, kLen=aDataSort.length ; k<kLen ; k++ )
			{
				iCol = aDataSort[k];
				sType = aoColumns[ iCol ].sType || 'string';
	
				if ( nestedSort[i]._idx === undefined ) {
					nestedSort[i]._idx = $.inArray( nestedSort[i][1], aoColumns[iCol].asSorting );
				}
	
				aSort.push( {
					src:       srcCol,
					col:       iCol,
					dir:       nestedSort[i][1],
					index:     nestedSort[i]._idx,
					type:      sType,
					formatter: DataTable.ext.type.order[ sType+"-pre" ]
				} );
			}
		}
	
		return aSort;
	}
	
	/**
	 * Change the order of the table
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 *  @todo This really needs split up!
	 */
	function _fnSort ( oSettings )
	{
		var
			i, ien, iLen, j, jLen, k, kLen,
			sDataType, nTh,
			aiOrig = [],
			oExtSort = DataTable.ext.type.order,
			aoData = oSettings.aoData,
			aoColumns = oSettings.aoColumns,
			aDataSort, data, iCol, sType, oSort,
			formatters = 0,
			sortCol,
			displayMaster = oSettings.aiDisplayMaster,
			aSort;
	
		// Resolve any column types that are unknown due to addition or invalidation
		// @todo Can this be moved into a 'data-ready' handler which is called when
		//   data is going to be used in the table?
		_fnColumnTypes( oSettings );
	
		aSort = _fnSortFlatten( oSettings );
	
		for ( i=0, ien=aSort.length ; i<ien ; i++ ) {
			sortCol = aSort[i];
	
			// Track if we can use the fast sort algorithm
			if ( sortCol.formatter ) {
				formatters++;
			}
	
			// Load the data needed for the sort, for each cell
			_fnSortData( oSettings, sortCol.col );
		}
	
		/* No sorting required if server-side or no sorting array */
		if ( _fnDataSource( oSettings ) != 'ssp' && aSort.length !== 0 )
		{
			// Create a value - key array of the current row positions such that we can use their
			// current position during the sort, if values match, in order to perform stable sorting
			for ( i=0, iLen=displayMaster.length ; i<iLen ; i++ ) {
				aiOrig[ displayMaster[i] ] = i;
			}
	
			/* Do the sort - here we want multi-column sorting based on a given data source (column)
			 * and sorting function (from oSort) in a certain direction. It's reasonably complex to
			 * follow on it's own, but this is what we want (example two column sorting):
			 *  fnLocalSorting = function(a,b){
			 *    var iTest;
			 *    iTest = oSort['string-asc']('data11', 'data12');
			 *      if (iTest !== 0)
			 *        return iTest;
			 *    iTest = oSort['numeric-desc']('data21', 'data22');
			 *    if (iTest !== 0)
			 *      return iTest;
			 *    return oSort['numeric-asc']( aiOrig[a], aiOrig[b] );
			 *  }
			 * Basically we have a test for each sorting column, if the data in that column is equal,
			 * test the next column. If all columns match, then we use a numeric sort on the row
			 * positions in the original data array to provide a stable sort.
			 *
			 * Note - I know it seems excessive to have two sorting methods, but the first is around
			 * 15% faster, so the second is only maintained for backwards compatibility with sorting
			 * methods which do not have a pre-sort formatting function.
			 */
			if ( formatters === aSort.length ) {
				// All sort types have formatting functions
				displayMaster.sort( function ( a, b ) {
					var
						x, y, k, test, sort,
						len=aSort.length,
						dataA = aoData[a]._aSortData,
						dataB = aoData[b]._aSortData;
	
					for ( k=0 ; k<len ; k++ ) {
						sort = aSort[k];
	
						x = dataA[ sort.col ];
						y = dataB[ sort.col ];
	
						test = x<y ? -1 : x>y ? 1 : 0;
						if ( test !== 0 ) {
							return sort.dir === 'asc' ? test : -test;
						}
					}
	
					x = aiOrig[a];
					y = aiOrig[b];
					return x<y ? -1 : x>y ? 1 : 0;
				} );
			}
			else {
				// Depreciated - remove in 1.11 (providing a plug-in option)
				// Not all sort types have formatting methods, so we have to call their sorting
				// methods.
				displayMaster.sort( function ( a, b ) {
					var
						x, y, k, l, test, sort, fn,
						len=aSort.length,
						dataA = aoData[a]._aSortData,
						dataB = aoData[b]._aSortData;
	
					for ( k=0 ; k<len ; k++ ) {
						sort = aSort[k];
	
						x = dataA[ sort.col ];
						y = dataB[ sort.col ];
	
						fn = oExtSort[ sort.type+"-"+sort.dir ] || oExtSort[ "string-"+sort.dir ];
						test = fn( x, y );
						if ( test !== 0 ) {
							return test;
						}
					}
	
					x = aiOrig[a];
					y = aiOrig[b];
					return x<y ? -1 : x>y ? 1 : 0;
				} );
			}
		}
	
		/* Tell the draw function that we have sorted the data */
		oSettings.bSorted = true;
	}
	
	
	function _fnSortAria ( settings )
	{
		var label;
		var nextSort;
		var columns = settings.aoColumns;
		var aSort = _fnSortFlatten( settings );
		var oAria = settings.oLanguage.oAria;
	
		// ARIA attributes - need to loop all columns, to update all (removing old
		// attributes as needed)
		for ( var i=0, iLen=columns.length ; i<iLen ; i++ )
		{
			var col = columns[i];
			var asSorting = col.asSorting;
			var sTitle = col.sTitle.replace( /<.*?>/g, "" );
			var th = col.nTh;
	
			// IE7 is throwing an error when setting these properties with jQuery's
			// attr() and removeAttr() methods...
			th.removeAttribute('aria-sort');
	
			/* In ARIA only the first sorting column can be marked as sorting - no multi-sort option */
			if ( col.bSortable ) {
				if ( aSort.length > 0 && aSort[0].col == i ) {
					th.setAttribute('aria-sort', aSort[0].dir=="asc" ? "ascending" : "descending" );
					nextSort = asSorting[ aSort[0].index+1 ] || asSorting[0];
				}
				else {
					nextSort = asSorting[0];
				}
	
				label = sTitle + ( nextSort === "asc" ?
					oAria.sSortAscending :
					oAria.sSortDescending
				);
			}
			else {
				label = sTitle;
			}
	
			th.setAttribute('aria-label', label);
		}
	}
	
	
	/**
	 * Function to run on user sort request
	 *  @param {object} settings dataTables settings object
	 *  @param {node} attachTo node to attach the handler to
	 *  @param {int} colIdx column sorting index
	 *  @param {boolean} [append=false] Append the requested sort to the existing
	 *    sort if true (i.e. multi-column sort)
	 *  @param {function} [callback] callback function
	 *  @memberof DataTable#oApi
	 */
	function _fnSortListener ( settings, colIdx, append, callback )
	{
		var col = settings.aoColumns[ colIdx ];
		var sorting = settings.aaSorting;
		var asSorting = col.asSorting;
		var nextSortIdx;
		var next = function ( a, overflow ) {
			var idx = a._idx;
			if ( idx === undefined ) {
				idx = $.inArray( a[1], asSorting );
			}
	
			return idx+1 < asSorting.length ?
				idx+1 :
				overflow ?
					null :
					0;
		};
	
		// Convert to 2D array if needed
		if ( typeof sorting[0] === 'number' ) {
			sorting = settings.aaSorting = [ sorting ];
		}
	
		// If appending the sort then we are multi-column sorting
		if ( append && settings.oFeatures.bSortMulti ) {
			// Are we already doing some kind of sort on this column?
			var sortIdx = $.inArray( colIdx, _pluck(sorting, '0') );
	
			if ( sortIdx !== -1 ) {
				// Yes, modify the sort
				nextSortIdx = next( sorting[sortIdx], true );
	
				if ( nextSortIdx === null && sorting.length === 1 ) {
					nextSortIdx = 0; // can't remove sorting completely
				}
	
				if ( nextSortIdx === null ) {
					sorting.splice( sortIdx, 1 );
				}
				else {
					sorting[sortIdx][1] = asSorting[ nextSortIdx ];
					sorting[sortIdx]._idx = nextSortIdx;
				}
			}
			else {
				// No sort on this column yet
				sorting.push( [ colIdx, asSorting[0], 0 ] );
				sorting[sorting.length-1]._idx = 0;
			}
		}
		else if ( sorting.length && sorting[0][0] == colIdx ) {
			// Single column - already sorting on this column, modify the sort
			nextSortIdx = next( sorting[0] );
	
			sorting.length = 1;
			sorting[0][1] = asSorting[ nextSortIdx ];
			sorting[0]._idx = nextSortIdx;
		}
		else {
			// Single column - sort only on this column
			sorting.length = 0;
			sorting.push( [ colIdx, asSorting[0] ] );
			sorting[0]._idx = 0;
		}
	
		// Run the sort by calling a full redraw
		_fnReDraw( settings );
	
		// callback used for async user interaction
		if ( typeof callback == 'function' ) {
			callback( settings );
		}
	}
	
	
	/**
	 * Attach a sort handler (click) to a node
	 *  @param {object} settings dataTables settings object
	 *  @param {node} attachTo node to attach the handler to
	 *  @param {int} colIdx column sorting index
	 *  @param {function} [callback] callback function
	 *  @memberof DataTable#oApi
	 */
	function _fnSortAttachListener ( settings, attachTo, colIdx, callback )
	{
		var col = settings.aoColumns[ colIdx ];
	
		_fnBindAction( attachTo, {}, function (e) {
			/* If the column is not sortable - don't to anything */
			if ( col.bSortable === false ) {
				return;
			}
	
			// If processing is enabled use a timeout to allow the processing
			// display to be shown - otherwise to it synchronously
			if ( settings.oFeatures.bProcessing ) {
				_fnProcessingDisplay( settings, true );
	
				setTimeout( function() {
					_fnSortListener( settings, colIdx, e.shiftKey, callback );
	
					// In server-side processing, the draw callback will remove the
					// processing display
					if ( _fnDataSource( settings ) !== 'ssp' ) {
						_fnProcessingDisplay( settings, false );
					}
				}, 0 );
			}
			else {
				_fnSortListener( settings, colIdx, e.shiftKey, callback );
			}
		} );
	}
	
	
	/**
	 * Set the sorting classes on table's body, Note: it is safe to call this function
	 * when bSort and bSortClasses are false
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnSortingClasses( settings )
	{
		var oldSort = settings.aLastSort;
		var sortClass = settings.oClasses.sSortColumn;
		var sort = _fnSortFlatten( settings );
		var features = settings.oFeatures;
		var i, ien, colIdx;
	
		if ( features.bSort && features.bSortClasses ) {
			// Remove old sorting classes
			for ( i=0, ien=oldSort.length ; i<ien ; i++ ) {
				colIdx = oldSort[i].src;
	
				// Remove column sorting
				$( _pluck( settings.aoData, 'anCells', colIdx ) )
					.removeClass( sortClass + (i<2 ? i+1 : 3) );
			}
	
			// Add new column sorting
			for ( i=0, ien=sort.length ; i<ien ; i++ ) {
				colIdx = sort[i].src;
	
				$( _pluck( settings.aoData, 'anCells', colIdx ) )
					.addClass( sortClass + (i<2 ? i+1 : 3) );
			}
		}
	
		settings.aLastSort = sort;
	}
	
	
	// Get the data to sort a column, be it from cache, fresh (populating the
	// cache), or from a sort formatter
	function _fnSortData( settings, idx )
	{
		// Custom sorting function - provided by the sort data type
		var column = settings.aoColumns[ idx ];
		var customSort = DataTable.ext.order[ column.sSortDataType ];
		var customData;
	
		if ( customSort ) {
			customData = customSort.call( settings.oInstance, settings, idx,
				_fnColumnIndexToVisible( settings, idx )
			);
		}
	
		// Use / populate cache
		var row, cellData;
		var formatter = DataTable.ext.type.order[ column.sType+"-pre" ];
	
		for ( var i=0, ien=settings.aoData.length ; i<ien ; i++ ) {
			row = settings.aoData[i];
	
			if ( ! row._aSortData ) {
				row._aSortData = [];
			}
	
			if ( ! row._aSortData[idx] || customSort ) {
				cellData = customSort ?
					customData[i] : // If there was a custom sort function, use data from there
					_fnGetCellData( settings, i, idx, 'sort' );
	
				row._aSortData[ idx ] = formatter ?
					formatter( cellData ) :
					cellData;
			}
		}
	}
	
	
	
	/**
	 * Save the state of a table
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnSaveState ( settings )
	{
		if ( !settings.oFeatures.bStateSave || settings.bDestroying )
		{
			return;
		}
	
		/* Store the interesting variables */
		var state = {
			time:    +new Date(),
			start:   settings._iDisplayStart,
			length:  settings._iDisplayLength,
			order:   $.extend( true, [], settings.aaSorting ),
			search:  _fnSearchToCamel( settings.oPreviousSearch ),
			columns: $.map( settings.aoColumns, function ( col, i ) {
				return {
					visible: col.bVisible,
					search: _fnSearchToCamel( settings.aoPreSearchCols[i] )
				};
			} )
		};
	
		_fnCallbackFire( settings, "aoStateSaveParams", 'stateSaveParams', [settings, state] );
	
		settings.oSavedState = state;
		settings.fnStateSaveCallback.call( settings.oInstance, settings, state );
	}
	
	
	/**
	 * Attempt to load a saved table state
	 *  @param {object} oSettings dataTables settings object
	 *  @param {object} oInit DataTables init object so we can override settings
	 *  @param {function} callback Callback to execute when the state has been loaded
	 *  @memberof DataTable#oApi
	 */
	function _fnLoadState ( settings, oInit, callback )
	{
		var i, ien;
		var columns = settings.aoColumns;
		var loaded = function ( s ) {
			if ( ! s || ! s.time ) {
				callback();
				return;
			}
	
			// Allow custom and plug-in manipulation functions to alter the saved data set and
			// cancelling of loading by returning false
			var abStateLoad = _fnCallbackFire( settings, 'aoStateLoadParams', 'stateLoadParams', [settings, s] );
			if ( $.inArray( false, abStateLoad ) !== -1 ) {
				callback();
				return;
			}
	
			// Reject old data
			var duration = settings.iStateDuration;
			if ( duration > 0 && s.time < +new Date() - (duration*1000) ) {
				callback();
				return;
			}
	
			// Number of columns have changed - all bets are off, no restore of settings
			if ( s.columns && columns.length !== s.columns.length ) {
				callback();
				return;
			}
	
			// Store the saved state so it might be accessed at any time
			settings.oLoadedState = $.extend( true, {}, s );
	
			// Restore key features - todo - for 1.11 this needs to be done by
			// subscribed events
			if ( s.start !== undefined ) {
				settings._iDisplayStart    = s.start;
				settings.iInitDisplayStart = s.start;
			}
			if ( s.length !== undefined ) {
				settings._iDisplayLength   = s.length;
			}
	
			// Order
			if ( s.order !== undefined ) {
				settings.aaSorting = [];
				$.each( s.order, function ( i, col ) {
					settings.aaSorting.push( col[0] >= columns.length ?
						[ 0, col[1] ] :
						col
					);
				} );
			}
	
			// Search
			if ( s.search !== undefined ) {
				$.extend( settings.oPreviousSearch, _fnSearchToHung( s.search ) );
			}
	
			// Columns
			//
			if ( s.columns ) {
				for ( i=0, ien=s.columns.length ; i<ien ; i++ ) {
					var col = s.columns[i];
	
					// Visibility
					if ( col.visible !== undefined ) {
						columns[i].bVisible = col.visible;
					}
	
					// Search
					if ( col.search !== undefined ) {
						$.extend( settings.aoPreSearchCols[i], _fnSearchToHung( col.search ) );
					}
				}
			}
	
			_fnCallbackFire( settings, 'aoStateLoaded', 'stateLoaded', [settings, s] );
			callback();
		}
	
		if ( ! settings.oFeatures.bStateSave ) {
			callback();
			return;
		}
	
		var state = settings.fnStateLoadCallback.call( settings.oInstance, settings, loaded );
	
		if ( state !== undefined ) {
			loaded( state );
		}
		// otherwise, wait for the loaded callback to be executed
	}
	
	
	/**
	 * Return the settings object for a particular table
	 *  @param {node} table table we are using as a dataTable
	 *  @returns {object} Settings object - or null if not found
	 *  @memberof DataTable#oApi
	 */
	function _fnSettingsFromNode ( table )
	{
		var settings = DataTable.settings;
		var idx = $.inArray( table, _pluck( settings, 'nTable' ) );
	
		return idx !== -1 ?
			settings[ idx ] :
			null;
	}
	
	
	/**
	 * Log an error message
	 *  @param {object} settings dataTables settings object
	 *  @param {int} level log error messages, or display them to the user
	 *  @param {string} msg error message
	 *  @param {int} tn Technical note id to get more information about the error.
	 *  @memberof DataTable#oApi
	 */
	function _fnLog( settings, level, msg, tn )
	{
		msg = 'DataTables warning: '+
			(settings ? 'table id='+settings.sTableId+' - ' : '')+msg;
	
		if ( tn ) {
			msg += '. For more information about this error, please see '+
			'http://datatables.net/tn/'+tn;
		}
	
		if ( ! level  ) {
			// Backwards compatibility pre 1.10
			var ext = DataTable.ext;
			var type = ext.sErrMode || ext.errMode;
	
			if ( settings ) {
				_fnCallbackFire( settings, null, 'error', [ settings, tn, msg ] );
			}
	
			if ( type == 'alert' ) {
				alert( msg );
			}
			else if ( type == 'throw' ) {
				throw new Error(msg);
			}
			else if ( typeof type == 'function' ) {
				type( settings, tn, msg );
			}
		}
		else if ( window.console && console.log ) {
			console.log( msg );
		}
	}
	
	
	/**
	 * See if a property is defined on one object, if so assign it to the other object
	 *  @param {object} ret target object
	 *  @param {object} src source object
	 *  @param {string} name property
	 *  @param {string} [mappedName] name to map too - optional, name used if not given
	 *  @memberof DataTable#oApi
	 */
	function _fnMap( ret, src, name, mappedName )
	{
		if ( $.isArray( name ) ) {
			$.each( name, function (i, val) {
				if ( $.isArray( val ) ) {
					_fnMap( ret, src, val[0], val[1] );
				}
				else {
					_fnMap( ret, src, val );
				}
			} );
	
			return;
		}
	
		if ( mappedName === undefined ) {
			mappedName = name;
		}
	
		if ( src[name] !== undefined ) {
			ret[mappedName] = src[name];
		}
	}
	
	
	/**
	 * Extend objects - very similar to jQuery.extend, but deep copy objects, and
	 * shallow copy arrays. The reason we need to do this, is that we don't want to
	 * deep copy array init values (such as aaSorting) since the dev wouldn't be
	 * able to override them, but we do want to deep copy arrays.
	 *  @param {object} out Object to extend
	 *  @param {object} extender Object from which the properties will be applied to
	 *      out
	 *  @param {boolean} breakRefs If true, then arrays will be sliced to take an
	 *      independent copy with the exception of the `data` or `aaData` parameters
	 *      if they are present. This is so you can pass in a collection to
	 *      DataTables and have that used as your data source without breaking the
	 *      references
	 *  @returns {object} out Reference, just for convenience - out === the return.
	 *  @memberof DataTable#oApi
	 *  @todo This doesn't take account of arrays inside the deep copied objects.
	 */
	function _fnExtend( out, extender, breakRefs )
	{
		var val;
	
		for ( var prop in extender ) {
			if ( extender.hasOwnProperty(prop) ) {
				val = extender[prop];
	
				if ( $.isPlainObject( val ) ) {
					if ( ! $.isPlainObject( out[prop] ) ) {
						out[prop] = {};
					}
					$.extend( true, out[prop], val );
				}
				else if ( breakRefs && prop !== 'data' && prop !== 'aaData' && $.isArray(val) ) {
					out[prop] = val.slice();
				}
				else {
					out[prop] = val;
				}
			}
		}
	
		return out;
	}
	
	
	/**
	 * Bind an event handers to allow a click or return key to activate the callback.
	 * This is good for accessibility since a return on the keyboard will have the
	 * same effect as a click, if the element has focus.
	 *  @param {element} n Element to bind the action to
	 *  @param {object} oData Data object to pass to the triggered function
	 *  @param {function} fn Callback function for when the event is triggered
	 *  @memberof DataTable#oApi
	 */
	function _fnBindAction( n, oData, fn )
	{
		$(n)
			.on( 'click.DT', oData, function (e) {
					$(n).blur(); // Remove focus outline for mouse users
					fn(e);
				} )
			.on( 'keypress.DT', oData, function (e){
					if ( e.which === 13 ) {
						e.preventDefault();
						fn(e);
					}
				} )
			.on( 'selectstart.DT', function () {
					/* Take the brutal approach to cancelling text selection */
					return false;
				} );
	}
	
	
	/**
	 * Register a callback function. Easily allows a callback function to be added to
	 * an array store of callback functions that can then all be called together.
	 *  @param {object} oSettings dataTables settings object
	 *  @param {string} sStore Name of the array storage for the callbacks in oSettings
	 *  @param {function} fn Function to be called back
	 *  @param {string} sName Identifying name for the callback (i.e. a label)
	 *  @memberof DataTable#oApi
	 */
	function _fnCallbackReg( oSettings, sStore, fn, sName )
	{
		if ( fn )
		{
			oSettings[sStore].push( {
				"fn": fn,
				"sName": sName
			} );
		}
	}
	
	
	/**
	 * Fire callback functions and trigger events. Note that the loop over the
	 * callback array store is done backwards! Further note that you do not want to
	 * fire off triggers in time sensitive applications (for example cell creation)
	 * as its slow.
	 *  @param {object} settings dataTables settings object
	 *  @param {string} callbackArr Name of the array storage for the callbacks in
	 *      oSettings
	 *  @param {string} eventName Name of the jQuery custom event to trigger. If
	 *      null no trigger is fired
	 *  @param {array} args Array of arguments to pass to the callback function /
	 *      trigger
	 *  @memberof DataTable#oApi
	 */
	function _fnCallbackFire( settings, callbackArr, eventName, args )
	{
		var ret = [];
	
		if ( callbackArr ) {
			ret = $.map( settings[callbackArr].slice().reverse(), function (val, i) {
				return val.fn.apply( settings.oInstance, args );
			} );
		}
	
		if ( eventName !== null ) {
			var e = $.Event( eventName+'.dt' );
	
			$(settings.nTable).trigger( e, args );
	
			ret.push( e.result );
		}
	
		return ret;
	}
	
	
	function _fnLengthOverflow ( settings )
	{
		var
			start = settings._iDisplayStart,
			end = settings.fnDisplayEnd(),
			len = settings._iDisplayLength;
	
		/* If we have space to show extra rows (backing up from the end point - then do so */
		if ( start >= end )
		{
			start = end - len;
		}
	
		// Keep the start record on the current page
		start -= (start % len);
	
		if ( len === -1 || start < 0 )
		{
			start = 0;
		}
	
		settings._iDisplayStart = start;
	}
	
	
	function _fnRenderer( settings, type )
	{
		var renderer = settings.renderer;
		var host = DataTable.ext.renderer[type];
	
		if ( $.isPlainObject( renderer ) && renderer[type] ) {
			// Specific renderer for this type. If available use it, otherwise use
			// the default.
			return host[renderer[type]] || host._;
		}
		else if ( typeof renderer === 'string' ) {
			// Common renderer - if there is one available for this type use it,
			// otherwise use the default
			return host[renderer] || host._;
		}
	
		// Use the default
		return host._;
	}
	
	
	/**
	 * Detect the data source being used for the table. Used to simplify the code
	 * a little (ajax) and to make it compress a little smaller.
	 *
	 *  @param {object} settings dataTables settings object
	 *  @returns {string} Data source
	 *  @memberof DataTable#oApi
	 */
	function _fnDataSource ( settings )
	{
		if ( settings.oFeatures.bServerSide ) {
			return 'ssp';
		}
		else if ( settings.ajax || settings.sAjaxSource ) {
			return 'ajax';
		}
		return 'dom';
	}
	

	
	
	/**
	 * Computed structure of the DataTables API, defined by the options passed to
	 * `DataTable.Api.register()` when building the API.
	 *
	 * The structure is built in order to speed creation and extension of the Api
	 * objects since the extensions are effectively pre-parsed.
	 *
	 * The array is an array of objects with the following structure, where this
	 * base array represents the Api prototype base:
	 *
	 *     [
	 *       {
	 *         name:      'data'                -- string   - Property name
	 *         val:       function () {},       -- function - Api method (or undefined if just an object
	 *         methodExt: [ ... ],              -- array    - Array of Api object definitions to extend the method result
	 *         propExt:   [ ... ]               -- array    - Array of Api object definitions to extend the property
	 *       },
	 *       {
	 *         name:     'row'
	 *         val:       {},
	 *         methodExt: [ ... ],
	 *         propExt:   [
	 *           {
	 *             name:      'data'
	 *             val:       function () {},
	 *             methodExt: [ ... ],
	 *             propExt:   [ ... ]
	 *           },
	 *           ...
	 *         ]
	 *       }
	 *     ]
	 *
	 * @type {Array}
	 * @ignore
	 */
	var __apiStruct = [];
	
	
	/**
	 * `Array.prototype` reference.
	 *
	 * @type object
	 * @ignore
	 */
	var __arrayProto = Array.prototype;
	
	
	/**
	 * Abstraction for `context` parameter of the `Api` constructor to allow it to
	 * take several different forms for ease of use.
	 *
	 * Each of the input parameter types will be converted to a DataTables settings
	 * object where possible.
	 *
	 * @param  {string|node|jQuery|object} mixed DataTable identifier. Can be one
	 *   of:
	 *
	 *   * `string` - jQuery selector. Any DataTables' matching the given selector
	 *     with be found and used.
	 *   * `node` - `TABLE` node which has already been formed into a DataTable.
	 *   * `jQuery` - A jQuery object of `TABLE` nodes.
	 *   * `object` - DataTables settings object
	 *   * `DataTables.Api` - API instance
	 * @return {array|null} Matching DataTables settings objects. `null` or
	 *   `undefined` is returned if no matching DataTable is found.
	 * @ignore
	 */
	var _toSettings = function ( mixed )
	{
		var idx, jq;
		var settings = DataTable.settings;
		var tables = $.map( settings, function (el, i) {
			return el.nTable;
		} );
	
		if ( ! mixed ) {
			return [];
		}
		else if ( mixed.nTable && mixed.oApi ) {
			// DataTables settings object
			return [ mixed ];
		}
		else if ( mixed.nodeName && mixed.nodeName.toLowerCase() === 'table' ) {
			// Table node
			idx = $.inArray( mixed, tables );
			return idx !== -1 ? [ settings[idx] ] : null;
		}
		else if ( mixed && typeof mixed.settings === 'function' ) {
			return mixed.settings().toArray();
		}
		else if ( typeof mixed === 'string' ) {
			// jQuery selector
			jq = $(mixed);
		}
		else if ( mixed instanceof $ ) {
			// jQuery object (also DataTables instance)
			jq = mixed;
		}
	
		if ( jq ) {
			return jq.map( function(i) {
				idx = $.inArray( this, tables );
				return idx !== -1 ? settings[idx] : null;
			} ).toArray();
		}
	};
	
	
	/**
	 * DataTables API class - used to control and interface with  one or more
	 * DataTables enhanced tables.
	 *
	 * The API class is heavily based on jQuery, presenting a chainable interface
	 * that you can use to interact with tables. Each instance of the API class has
	 * a "context" - i.e. the tables that it will operate on. This could be a single
	 * table, all tables on a page or a sub-set thereof.
	 *
	 * Additionally the API is designed to allow you to easily work with the data in
	 * the tables, retrieving and manipulating it as required. This is done by
	 * presenting the API class as an array like interface. The contents of the
	 * array depend upon the actions requested by each method (for example
	 * `rows().nodes()` will return an array of nodes, while `rows().data()` will
	 * return an array of objects or arrays depending upon your table's
	 * configuration). The API object has a number of array like methods (`push`,
	 * `pop`, `reverse` etc) as well as additional helper methods (`each`, `pluck`,
	 * `unique` etc) to assist your working with the data held in a table.
	 *
	 * Most methods (those which return an Api instance) are chainable, which means
	 * the return from a method call also has all of the methods available that the
	 * top level object had. For example, these two calls are equivalent:
	 *
	 *     // Not chained
	 *     api.row.add( {...} );
	 *     api.draw();
	 *
	 *     // Chained
	 *     api.row.add( {...} ).draw();
	 *
	 * @class DataTable.Api
	 * @param {array|object|string|jQuery} context DataTable identifier. This is
	 *   used to define which DataTables enhanced tables this API will operate on.
	 *   Can be one of:
	 *
	 *   * `string` - jQuery selector. Any DataTables' matching the given selector
	 *     with be found and used.
	 *   * `node` - `TABLE` node which has already been formed into a DataTable.
	 *   * `jQuery` - A jQuery object of `TABLE` nodes.
	 *   * `object` - DataTables settings object
	 * @param {array} [data] Data to initialise the Api instance with.
	 *
	 * @example
	 *   // Direct initialisation during DataTables construction
	 *   var api = $('#example').DataTable();
	 *
	 * @example
	 *   // Initialisation using a DataTables jQuery object
	 *   var api = $('#example').dataTable().api();
	 *
	 * @example
	 *   // Initialisation as a constructor
	 *   var api = new $.fn.DataTable.Api( 'table.dataTable' );
	 */
	_Api = function ( context, data )
	{
		if ( ! (this instanceof _Api) ) {
			return new _Api( context, data );
		}
	
		var settings = [];
		var ctxSettings = function ( o ) {
			var a = _toSettings( o );
			if ( a ) {
				settings = settings.concat( a );
			}
		};
	
		if ( $.isArray( context ) ) {
			for ( var i=0, ien=context.length ; i<ien ; i++ ) {
				ctxSettings( context[i] );
			}
		}
		else {
			ctxSettings( context );
		}
	
		// Remove duplicates
		this.context = _unique( settings );
	
		// Initial data
		if ( data ) {
			$.merge( this, data );
		}
	
		// selector
		this.selector = {
			rows: null,
			cols: null,
			opts: null
		};
	
		_Api.extend( this, this, __apiStruct );
	};
	
	DataTable.Api = _Api;
	
	// Don't destroy the existing prototype, just extend it. Required for jQuery 2's
	// isPlainObject.
	$.extend( _Api.prototype, {
		any: function ()
		{
			return this.count() !== 0;
		},
	
	
		concat:  __arrayProto.concat,
	
	
		context: [], // array of table settings objects
	
	
		count: function ()
		{
			return this.flatten().length;
		},
	
	
		each: function ( fn )
		{
			for ( var i=0, ien=this.length ; i<ien; i++ ) {
				fn.call( this, this[i], i, this );
			}
	
			return this;
		},
	
	
		eq: function ( idx )
		{
			var ctx = this.context;
	
			return ctx.length > idx ?
				new _Api( ctx[idx], this[idx] ) :
				null;
		},
	
	
		filter: function ( fn )
		{
			var a = [];
	
			if ( __arrayProto.filter ) {
				a = __arrayProto.filter.call( this, fn, this );
			}
			else {
				// Compatibility for browsers without EMCA-252-5 (JS 1.6)
				for ( var i=0, ien=this.length ; i<ien ; i++ ) {
					if ( fn.call( this, this[i], i, this ) ) {
						a.push( this[i] );
					}
				}
			}
	
			return new _Api( this.context, a );
		},
	
	
		flatten: function ()
		{
			var a = [];
			return new _Api( this.context, a.concat.apply( a, this.toArray() ) );
		},
	
	
		join:    __arrayProto.join,
	
	
		indexOf: __arrayProto.indexOf || function (obj, start)
		{
			for ( var i=(start || 0), ien=this.length ; i<ien ; i++ ) {
				if ( this[i] === obj ) {
					return i;
				}
			}
			return -1;
		},
	
		iterator: function ( flatten, type, fn, alwaysNew ) {
			var
				a = [], ret,
				i, ien, j, jen,
				context = this.context,
				rows, items, item,
				selector = this.selector;
	
			// Argument shifting
			if ( typeof flatten === 'string' ) {
				alwaysNew = fn;
				fn = type;
				type = flatten;
				flatten = false;
			}
	
			for ( i=0, ien=context.length ; i<ien ; i++ ) {
				var apiInst = new _Api( context[i] );
	
				if ( type === 'table' ) {
					ret = fn.call( apiInst, context[i], i );
	
					if ( ret !== undefined ) {
						a.push( ret );
					}
				}
				else if ( type === 'columns' || type === 'rows' ) {
					// this has same length as context - one entry for each table
					ret = fn.call( apiInst, context[i], this[i], i );
	
					if ( ret !== undefined ) {
						a.push( ret );
					}
				}
				else if ( type === 'column' || type === 'column-rows' || type === 'row' || type === 'cell' ) {
					// columns and rows share the same structure.
					// 'this' is an array of column indexes for each context
					items = this[i];
	
					if ( type === 'column-rows' ) {
						rows = _selector_row_indexes( context[i], selector.opts );
					}
	
					for ( j=0, jen=items.length ; j<jen ; j++ ) {
						item = items[j];
	
						if ( type === 'cell' ) {
							ret = fn.call( apiInst, context[i], item.row, item.column, i, j );
						}
						else {
							ret = fn.call( apiInst, context[i], item, i, j, rows );
						}
	
						if ( ret !== undefined ) {
							a.push( ret );
						}
					}
				}
			}
	
			if ( a.length || alwaysNew ) {
				var api = new _Api( context, flatten ? a.concat.apply( [], a ) : a );
				var apiSelector = api.selector;
				apiSelector.rows = selector.rows;
				apiSelector.cols = selector.cols;
				apiSelector.opts = selector.opts;
				return api;
			}
			return this;
		},
	
	
		lastIndexOf: __arrayProto.lastIndexOf || function (obj, start)
		{
			// Bit cheeky...
			return this.indexOf.apply( this.toArray.reverse(), arguments );
		},
	
	
		length:  0,
	
	
		map: function ( fn )
		{
			var a = [];
	
			if ( __arrayProto.map ) {
				a = __arrayProto.map.call( this, fn, this );
			}
			else {
				// Compatibility for browsers without EMCA-252-5 (JS 1.6)
				for ( var i=0, ien=this.length ; i<ien ; i++ ) {
					a.push( fn.call( this, this[i], i ) );
				}
			}
	
			return new _Api( this.context, a );
		},
	
	
		pluck: function ( prop )
		{
			return this.map( function ( el ) {
				return el[ prop ];
			} );
		},
	
		pop:     __arrayProto.pop,
	
	
		push:    __arrayProto.push,
	
	
		// Does not return an API instance
		reduce: __arrayProto.reduce || function ( fn, init )
		{
			return _fnReduce( this, fn, init, 0, this.length, 1 );
		},
	
	
		reduceRight: __arrayProto.reduceRight || function ( fn, init )
		{
			return _fnReduce( this, fn, init, this.length-1, -1, -1 );
		},
	
	
		reverse: __arrayProto.reverse,
	
	
		// Object with rows, columns and opts
		selector: null,
	
	
		shift:   __arrayProto.shift,
	
	
		slice: function () {
			return new _Api( this.context, this );
		},
	
	
		sort:    __arrayProto.sort, // ? name - order?
	
	
		splice:  __arrayProto.splice,
	
	
		toArray: function ()
		{
			return __arrayProto.slice.call( this );
		},
	
	
		to$: function ()
		{
			return $( this );
		},
	
	
		toJQuery: function ()
		{
			return $( this );
		},
	
	
		unique: function ()
		{
			return new _Api( this.context, _unique(this) );
		},
	
	
		unshift: __arrayProto.unshift
	} );
	
	
	_Api.extend = function ( scope, obj, ext )
	{
		// Only extend API instances and static properties of the API
		if ( ! ext.length || ! obj || ( ! (obj instanceof _Api) && ! obj.__dt_wrapper ) ) {
			return;
		}
	
		var
			i, ien,
			j, jen,
			struct, inner,
			methodScoping = function ( scope, fn, struc ) {
				return function () {
					var ret = fn.apply( scope, arguments );
	
					// Method extension
					_Api.extend( ret, ret, struc.methodExt );
					return ret;
				};
			};
	
		for ( i=0, ien=ext.length ; i<ien ; i++ ) {
			struct = ext[i];
	
			// Value
			obj[ struct.name ] = typeof struct.val === 'function' ?
				methodScoping( scope, struct.val, struct ) :
				$.isPlainObject( struct.val ) ?
					{} :
					struct.val;
	
			obj[ struct.name ].__dt_wrapper = true;
	
			// Property extension
			_Api.extend( scope, obj[ struct.name ], struct.propExt );
		}
	};
	
	
	// @todo - Is there need for an augment function?
	// _Api.augment = function ( inst, name )
	// {
	// 	// Find src object in the structure from the name
	// 	var parts = name.split('.');
	
	// 	_Api.extend( inst, obj );
	// };
	
	
	//     [
	//       {
	//         name:      'data'                -- string   - Property name
	//         val:       function () {},       -- function - Api method (or undefined if just an object
	//         methodExt: [ ... ],              -- array    - Array of Api object definitions to extend the method result
	//         propExt:   [ ... ]               -- array    - Array of Api object definitions to extend the property
	//       },
	//       {
	//         name:     'row'
	//         val:       {},
	//         methodExt: [ ... ],
	//         propExt:   [
	//           {
	//             name:      'data'
	//             val:       function () {},
	//             methodExt: [ ... ],
	//             propExt:   [ ... ]
	//           },
	//           ...
	//         ]
	//       }
	//     ]
	
	_Api.register = _api_register = function ( name, val )
	{
		if ( $.isArray( name ) ) {
			for ( var j=0, jen=name.length ; j<jen ; j++ ) {
				_Api.register( name[j], val );
			}
			return;
		}
	
		var
			i, ien,
			heir = name.split('.'),
			struct = __apiStruct,
			key, method;
	
		var find = function ( src, name ) {
			for ( var i=0, ien=src.length ; i<ien ; i++ ) {
				if ( src[i].name === name ) {
					return src[i];
				}
			}
			return null;
		};
	
		for ( i=0, ien=heir.length ; i<ien ; i++ ) {
			method = heir[i].indexOf('()') !== -1;
			key = method ?
				heir[i].replace('()', '') :
				heir[i];
	
			var src = find( struct, key );
			if ( ! src ) {
				src = {
					name:      key,
					val:       {},
					methodExt: [],
					propExt:   []
				};
				struct.push( src );
			}
	
			if ( i === ien-1 ) {
				src.val = val;
			}
			else {
				struct = method ?
					src.methodExt :
					src.propExt;
			}
		}
	};
	
	
	_Api.registerPlural = _api_registerPlural = function ( pluralName, singularName, val ) {
		_Api.register( pluralName, val );
	
		_Api.register( singularName, function () {
			var ret = val.apply( this, arguments );
	
			if ( ret === this ) {
				// Returned item is the API instance that was passed in, return it
				return this;
			}
			else if ( ret instanceof _Api ) {
				// New API instance returned, want the value from the first item
				// in the returned array for the singular result.
				return ret.length ?
					$.isArray( ret[0] ) ?
						new _Api( ret.context, ret[0] ) : // Array results are 'enhanced'
						ret[0] :
					undefined;
			}
	
			// Non-API return - just fire it back
			return ret;
		} );
	};
	
	
	/**
	 * Selector for HTML tables. Apply the given selector to the give array of
	 * DataTables settings objects.
	 *
	 * @param {string|integer} [selector] jQuery selector string or integer
	 * @param  {array} Array of DataTables settings objects to be filtered
	 * @return {array}
	 * @ignore
	 */
	var __table_selector = function ( selector, a )
	{
		// Integer is used to pick out a table by index
		if ( typeof selector === 'number' ) {
			return [ a[ selector ] ];
		}
	
		// Perform a jQuery selector on the table nodes
		var nodes = $.map( a, function (el, i) {
			return el.nTable;
		} );
	
		return $(nodes)
			.filter( selector )
			.map( function (i) {
				// Need to translate back from the table node to the settings
				var idx = $.inArray( this, nodes );
				return a[ idx ];
			} )
			.toArray();
	};
	
	
	
	/**
	 * Context selector for the API's context (i.e. the tables the API instance
	 * refers to.
	 *
	 * @name    DataTable.Api#tables
	 * @param {string|integer} [selector] Selector to pick which tables the iterator
	 *   should operate on. If not given, all tables in the current context are
	 *   used. This can be given as a jQuery selector (for example `':gt(0)'`) to
	 *   select multiple tables or as an integer to select a single table.
	 * @returns {DataTable.Api} Returns a new API instance if a selector is given.
	 */
	_api_register( 'tables()', function ( selector ) {
		// A new instance is created if there was a selector specified
		return selector ?
			new _Api( __table_selector( selector, this.context ) ) :
			this;
	} );
	
	
	_api_register( 'table()', function ( selector ) {
		var tables = this.tables( selector );
		var ctx = tables.context;
	
		// Truncate to the first matched table
		return ctx.length ?
			new _Api( ctx[0] ) :
			tables;
	} );
	
	
	_api_registerPlural( 'tables().nodes()', 'table().node()' , function () {
		return this.iterator( 'table', function ( ctx ) {
			return ctx.nTable;
		}, 1 );
	} );
	
	
	_api_registerPlural( 'tables().body()', 'table().body()' , function () {
		return this.iterator( 'table', function ( ctx ) {
			return ctx.nTBody;
		}, 1 );
	} );
	
	
	_api_registerPlural( 'tables().header()', 'table().header()' , function () {
		return this.iterator( 'table', function ( ctx ) {
			return ctx.nTHead;
		}, 1 );
	} );
	
	
	_api_registerPlural( 'tables().footer()', 'table().footer()' , function () {
		return this.iterator( 'table', function ( ctx ) {
			return ctx.nTFoot;
		}, 1 );
	} );
	
	
	_api_registerPlural( 'tables().containers()', 'table().container()' , function () {
		return this.iterator( 'table', function ( ctx ) {
			return ctx.nTableWrapper;
		}, 1 );
	} );
	
	
	
	/**
	 * Redraw the tables in the current context.
	 */
	_api_register( 'draw()', function ( paging ) {
		return this.iterator( 'table', function ( settings ) {
			if ( paging === 'page' ) {
				_fnDraw( settings );
			}
			else {
				if ( typeof paging === 'string' ) {
					paging = paging === 'full-hold' ?
						false :
						true;
				}
	
				_fnReDraw( settings, paging===false );
			}
		} );
	} );
	
	
	
	/**
	 * Get the current page index.
	 *
	 * @return {integer} Current page index (zero based)
	 *//**
	 * Set the current page.
	 *
	 * Note that if you attempt to show a page which does not exist, DataTables will
	 * not throw an error, but rather reset the paging.
	 *
	 * @param {integer|string} action The paging action to take. This can be one of:
	 *  * `integer` - The page index to jump to
	 *  * `string` - An action to take:
	 *    * `first` - Jump to first page.
	 *    * `next` - Jump to the next page
	 *    * `previous` - Jump to previous page
	 *    * `last` - Jump to the last page.
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'page()', function ( action ) {
		if ( action === undefined ) {
			return this.page.info().page; // not an expensive call
		}
	
		// else, have an action to take on all tables
		return this.iterator( 'table', function ( settings ) {
			_fnPageChange( settings, action );
		} );
	} );
	
	
	/**
	 * Paging information for the first table in the current context.
	 *
	 * If you require paging information for another table, use the `table()` method
	 * with a suitable selector.
	 *
	 * @return {object} Object with the following properties set:
	 *  * `page` - Current page index (zero based - i.e. the first page is `0`)
	 *  * `pages` - Total number of pages
	 *  * `start` - Display index for the first record shown on the current page
	 *  * `end` - Display index for the last record shown on the current page
	 *  * `length` - Display length (number of records). Note that generally `start
	 *    + length = end`, but this is not always true, for example if there are
	 *    only 2 records to show on the final page, with a length of 10.
	 *  * `recordsTotal` - Full data set length
	 *  * `recordsDisplay` - Data set length once the current filtering criterion
	 *    are applied.
	 */
	_api_register( 'page.info()', function ( action ) {
		if ( this.context.length === 0 ) {
			return undefined;
		}
	
		var
			settings   = this.context[0],
			start      = settings._iDisplayStart,
			len        = settings.oFeatures.bPaginate ? settings._iDisplayLength : -1,
			visRecords = settings.fnRecordsDisplay(),
			all        = len === -1;
	
		return {
			"page":           all ? 0 : Math.floor( start / len ),
			"pages":          all ? 1 : Math.ceil( visRecords / len ),
			"start":          start,
			"end":            settings.fnDisplayEnd(),
			"length":         len,
			"recordsTotal":   settings.fnRecordsTotal(),
			"recordsDisplay": visRecords,
			"serverSide":     _fnDataSource( settings ) === 'ssp'
		};
	} );
	
	
	/**
	 * Get the current page length.
	 *
	 * @return {integer} Current page length. Note `-1` indicates that all records
	 *   are to be shown.
	 *//**
	 * Set the current page length.
	 *
	 * @param {integer} Page length to set. Use `-1` to show all records.
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'page.len()', function ( len ) {
		// Note that we can't call this function 'length()' because `length`
		// is a Javascript property of functions which defines how many arguments
		// the function expects.
		if ( len === undefined ) {
			return this.context.length !== 0 ?
				this.context[0]._iDisplayLength :
				undefined;
		}
	
		// else, set the page length
		return this.iterator( 'table', function ( settings ) {
			_fnLengthChange( settings, len );
		} );
	} );
	
	
	
	var __reload = function ( settings, holdPosition, callback ) {
		// Use the draw event to trigger a callback
		if ( callback ) {
			var api = new _Api( settings );
	
			api.one( 'draw', function () {
				callback( api.ajax.json() );
			} );
		}
	
		if ( _fnDataSource( settings ) == 'ssp' ) {
			_fnReDraw( settings, holdPosition );
		}
		else {
			_fnProcessingDisplay( settings, true );
	
			// Cancel an existing request
			var xhr = settings.jqXHR;
			if ( xhr && xhr.readyState !== 4 ) {
				xhr.abort();
			}
	
			// Trigger xhr
			_fnBuildAjax( settings, [], function( json ) {
				_fnClearTable( settings );
	
				var data = _fnAjaxDataSrc( settings, json );
				for ( var i=0, ien=data.length ; i<ien ; i++ ) {
					_fnAddData( settings, data[i] );
				}
	
				_fnReDraw( settings, holdPosition );
				_fnProcessingDisplay( settings, false );
			} );
		}
	};
	
	
	/**
	 * Get the JSON response from the last Ajax request that DataTables made to the
	 * server. Note that this returns the JSON from the first table in the current
	 * context.
	 *
	 * @return {object} JSON received from the server.
	 */
	_api_register( 'ajax.json()', function () {
		var ctx = this.context;
	
		if ( ctx.length > 0 ) {
			return ctx[0].json;
		}
	
		// else return undefined;
	} );
	
	
	/**
	 * Get the data submitted in the last Ajax request
	 */
	_api_register( 'ajax.params()', function () {
		var ctx = this.context;
	
		if ( ctx.length > 0 ) {
			return ctx[0].oAjaxData;
		}
	
		// else return undefined;
	} );
	
	
	/**
	 * Reload tables from the Ajax data source. Note that this function will
	 * automatically re-draw the table when the remote data has been loaded.
	 *
	 * @param {boolean} [reset=true] Reset (default) or hold the current paging
	 *   position. A full re-sort and re-filter is performed when this method is
	 *   called, which is why the pagination reset is the default action.
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'ajax.reload()', function ( callback, resetPaging ) {
		return this.iterator( 'table', function (settings) {
			__reload( settings, resetPaging===false, callback );
		} );
	} );
	
	
	/**
	 * Get the current Ajax URL. Note that this returns the URL from the first
	 * table in the current context.
	 *
	 * @return {string} Current Ajax source URL
	 *//**
	 * Set the Ajax URL. Note that this will set the URL for all tables in the
	 * current context.
	 *
	 * @param {string} url URL to set.
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'ajax.url()', function ( url ) {
		var ctx = this.context;
	
		if ( url === undefined ) {
			// get
			if ( ctx.length === 0 ) {
				return undefined;
			}
			ctx = ctx[0];
	
			return ctx.ajax ?
				$.isPlainObject( ctx.ajax ) ?
					ctx.ajax.url :
					ctx.ajax :
				ctx.sAjaxSource;
		}
	
		// set
		return this.iterator( 'table', function ( settings ) {
			if ( $.isPlainObject( settings.ajax ) ) {
				settings.ajax.url = url;
			}
			else {
				settings.ajax = url;
			}
			// No need to consider sAjaxSource here since DataTables gives priority
			// to `ajax` over `sAjaxSource`. So setting `ajax` here, renders any
			// value of `sAjaxSource` redundant.
		} );
	} );
	
	
	/**
	 * Load data from the newly set Ajax URL. Note that this method is only
	 * available when `ajax.url()` is used to set a URL. Additionally, this method
	 * has the same effect as calling `ajax.reload()` but is provided for
	 * convenience when setting a new URL. Like `ajax.reload()` it will
	 * automatically redraw the table once the remote data has been loaded.
	 *
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'ajax.url().load()', function ( callback, resetPaging ) {
		// Same as a reload, but makes sense to present it for easy access after a
		// url change
		return this.iterator( 'table', function ( ctx ) {
			__reload( ctx, resetPaging===false, callback );
		} );
	} );
	
	
	
	
	var _selector_run = function ( type, selector, selectFn, settings, opts )
	{
		var
			out = [], res,
			a, i, ien, j, jen,
			selectorType = typeof selector;
	
		// Can't just check for isArray here, as an API or jQuery instance might be
		// given with their array like look
		if ( ! selector || selectorType === 'string' || selectorType === 'function' || selector.length === undefined ) {
			selector = [ selector ];
		}
	
		for ( i=0, ien=selector.length ; i<ien ; i++ ) {
			// Only split on simple strings - complex expressions will be jQuery selectors
			a = selector[i] && selector[i].split && ! selector[i].match(/[\[\(:]/) ?
				selector[i].split(',') :
				[ selector[i] ];
	
			for ( j=0, jen=a.length ; j<jen ; j++ ) {
				res = selectFn( typeof a[j] === 'string' ? $.trim(a[j]) : a[j] );
	
				if ( res && res.length ) {
					out = out.concat( res );
				}
			}
		}
	
		// selector extensions
		var ext = _ext.selector[ type ];
		if ( ext.length ) {
			for ( i=0, ien=ext.length ; i<ien ; i++ ) {
				out = ext[i]( settings, opts, out );
			}
		}
	
		return _unique( out );
	};
	
	
	var _selector_opts = function ( opts )
	{
		if ( ! opts ) {
			opts = {};
		}
	
		// Backwards compatibility for 1.9- which used the terminology filter rather
		// than search
		if ( opts.filter && opts.search === undefined ) {
			opts.search = opts.filter;
		}
	
		return $.extend( {
			search: 'none',
			order: 'current',
			page: 'all'
		}, opts );
	};
	
	
	var _selector_first = function ( inst )
	{
		// Reduce the API instance to the first item found
		for ( var i=0, ien=inst.length ; i<ien ; i++ ) {
			if ( inst[i].length > 0 ) {
				// Assign the first element to the first item in the instance
				// and truncate the instance and context
				inst[0] = inst[i];
				inst[0].length = 1;
				inst.length = 1;
				inst.context = [ inst.context[i] ];
	
				return inst;
			}
		}
	
		// Not found - return an empty instance
		inst.length = 0;
		return inst;
	};
	
	
	var _selector_row_indexes = function ( settings, opts )
	{
		var
			i, ien, tmp, a=[],
			displayFiltered = settings.aiDisplay,
			displayMaster = settings.aiDisplayMaster;
	
		var
			search = opts.search,  // none, applied, removed
			order  = opts.order,   // applied, current, index (original - compatibility with 1.9)
			page   = opts.page;    // all, current
	
		if ( _fnDataSource( settings ) == 'ssp' ) {
			// In server-side processing mode, most options are irrelevant since
			// rows not shown don't exist and the index order is the applied order
			// Removed is a special case - for consistency just return an empty
			// array
			return search === 'removed' ?
				[] :
				_range( 0, displayMaster.length );
		}
		else if ( page == 'current' ) {
			// Current page implies that order=current and fitler=applied, since it is
			// fairly senseless otherwise, regardless of what order and search actually
			// are
			for ( i=settings._iDisplayStart, ien=settings.fnDisplayEnd() ; i<ien ; i++ ) {
				a.push( displayFiltered[i] );
			}
		}
		else if ( order == 'current' || order == 'applied' ) {
			if ( search == 'none') {
				a = displayMaster.slice();
			}
			else if ( search == 'applied' ) {
				a = displayFiltered.slice();
			}
			else if ( search == 'removed' ) {
				// O(n+m) solution by creating a hash map
				var displayFilteredMap = {};
	
				for ( var i=0, ien=displayFiltered.length ; i<ien ; i++ ) {
					displayFilteredMap[displayFiltered[i]] = null;
				}
	
				a = $.map( displayMaster, function (el) {
					return ! displayFilteredMap.hasOwnProperty(el) ?
						el :
						null;
				} );
			}
		}
		else if ( order == 'index' || order == 'original' ) {
			for ( i=0, ien=settings.aoData.length ; i<ien ; i++ ) {
				if ( search == 'none' ) {
					a.push( i );
				}
				else { // applied | removed
					tmp = $.inArray( i, displayFiltered );
	
					if ((tmp === -1 && search == 'removed') ||
						(tmp >= 0   && search == 'applied') )
					{
						a.push( i );
					}
				}
			}
		}
	
		return a;
	};
	
	
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Rows
	 *
	 * {}          - no selector - use all available rows
	 * {integer}   - row aoData index
	 * {node}      - TR node
	 * {string}    - jQuery selector to apply to the TR elements
	 * {array}     - jQuery array of nodes, or simply an array of TR nodes
	 *
	 */
	var __row_selector = function ( settings, selector, opts )
	{
		var rows;
		var run = function ( sel ) {
			var selInt = _intVal( sel );
			var i, ien;
			var aoData = settings.aoData;
	
			// Short cut - selector is a number and no options provided (default is
			// all records, so no need to check if the index is in there, since it
			// must be - dev error if the index doesn't exist).
			if ( selInt !== null && ! opts ) {
				return [ selInt ];
			}
	
			if ( ! rows ) {
				rows = _selector_row_indexes( settings, opts );
			}
	
			if ( selInt !== null && $.inArray( selInt, rows ) !== -1 ) {
				// Selector - integer
				return [ selInt ];
			}
			else if ( sel === null || sel === undefined || sel === '' ) {
				// Selector - none
				return rows;
			}
	
			// Selector - function
			if ( typeof sel === 'function' ) {
				return $.map( rows, function (idx) {
					var row = aoData[ idx ];
					return sel( idx, row._aData, row.nTr ) ? idx : null;
				} );
			}
	
			// Selector - node
			if ( sel.nodeName ) {
				var rowIdx = sel._DT_RowIndex;  // Property added by DT for fast lookup
				var cellIdx = sel._DT_CellIndex;
	
				if ( rowIdx !== undefined ) {
					// Make sure that the row is actually still present in the table
					return aoData[ rowIdx ] && aoData[ rowIdx ].nTr === sel ?
						[ rowIdx ] :
						[];
				}
				else if ( cellIdx ) {
					return aoData[ cellIdx.row ] && aoData[ cellIdx.row ].nTr === sel ?
						[ cellIdx.row ] :
						[];
				}
				else {
					var host = $(sel).closest('*[data-dt-row]');
					return host.length ?
						[ host.data('dt-row') ] :
						[];
				}
			}
	
			// ID selector. Want to always be able to select rows by id, regardless
			// of if the tr element has been created or not, so can't rely upon
			// jQuery here - hence a custom implementation. This does not match
			// Sizzle's fast selector or HTML4 - in HTML5 the ID can be anything,
			// but to select it using a CSS selector engine (like Sizzle or
			// querySelect) it would need to need to be escaped for some characters.
			// DataTables simplifies this for row selectors since you can select
			// only a row. A # indicates an id any anything that follows is the id -
			// unescaped.
			if ( typeof sel === 'string' && sel.charAt(0) === '#' ) {
				// get row index from id
				var rowObj = settings.aIds[ sel.replace( /^#/, '' ) ];
				if ( rowObj !== undefined ) {
					return [ rowObj.idx ];
				}
	
				// need to fall through to jQuery in case there is DOM id that
				// matches
			}
			
			// Get nodes in the order from the `rows` array with null values removed
			var nodes = _removeEmpty(
				_pluck_order( settings.aoData, rows, 'nTr' )
			);
	
			// Selector - jQuery selector string, array of nodes or jQuery object/
			// As jQuery's .filter() allows jQuery objects to be passed in filter,
			// it also allows arrays, so this will cope with all three options
			return $(nodes)
				.filter( sel )
				.map( function () {
					return this._DT_RowIndex;
				} )
				.toArray();
		};
	
		return _selector_run( 'row', selector, run, settings, opts );
	};
	
	
	_api_register( 'rows()', function ( selector, opts ) {
		// argument shifting
		if ( selector === undefined ) {
			selector = '';
		}
		else if ( $.isPlainObject( selector ) ) {
			opts = selector;
			selector = '';
		}
	
		opts = _selector_opts( opts );
	
		var inst = this.iterator( 'table', function ( settings ) {
			return __row_selector( settings, selector, opts );
		}, 1 );
	
		// Want argument shifting here and in __row_selector?
		inst.selector.rows = selector;
		inst.selector.opts = opts;
	
		return inst;
	} );
	
	_api_register( 'rows().nodes()', function () {
		return this.iterator( 'row', function ( settings, row ) {
			return settings.aoData[ row ].nTr || undefined;
		}, 1 );
	} );
	
	_api_register( 'rows().data()', function () {
		return this.iterator( true, 'rows', function ( settings, rows ) {
			return _pluck_order( settings.aoData, rows, '_aData' );
		}, 1 );
	} );
	
	_api_registerPlural( 'rows().cache()', 'row().cache()', function ( type ) {
		return this.iterator( 'row', function ( settings, row ) {
			var r = settings.aoData[ row ];
			return type === 'search' ? r._aFilterData : r._aSortData;
		}, 1 );
	} );
	
	_api_registerPlural( 'rows().invalidate()', 'row().invalidate()', function ( src ) {
		return this.iterator( 'row', function ( settings, row ) {
			_fnInvalidate( settings, row, src );
		} );
	} );
	
	_api_registerPlural( 'rows().indexes()', 'row().index()', function () {
		return this.iterator( 'row', function ( settings, row ) {
			return row;
		}, 1 );
	} );
	
	_api_registerPlural( 'rows().ids()', 'row().id()', function ( hash ) {
		var a = [];
		var context = this.context;
	
		// `iterator` will drop undefined values, but in this case we want them
		for ( var i=0, ien=context.length ; i<ien ; i++ ) {
			for ( var j=0, jen=this[i].length ; j<jen ; j++ ) {
				var id = context[i].rowIdFn( context[i].aoData[ this[i][j] ]._aData );
				a.push( (hash === true ? '#' : '' )+ id );
			}
		}
	
		return new _Api( context, a );
	} );
	
	_api_registerPlural( 'rows().remove()', 'row().remove()', function () {
		var that = this;
	
		this.iterator( 'row', function ( settings, row, thatIdx ) {
			var data = settings.aoData;
			var rowData = data[ row ];
			var i, ien, j, jen;
			var loopRow, loopCells;
	
			data.splice( row, 1 );
	
			// Update the cached indexes
			for ( i=0, ien=data.length ; i<ien ; i++ ) {
				loopRow = data[i];
				loopCells = loopRow.anCells;
	
				// Rows
				if ( loopRow.nTr !== null ) {
					loopRow.nTr._DT_RowIndex = i;
				}
	
				// Cells
				if ( loopCells !== null ) {
					for ( j=0, jen=loopCells.length ; j<jen ; j++ ) {
						loopCells[j]._DT_CellIndex.row = i;
					}
				}
			}
	
			// Delete from the display arrays
			_fnDeleteIndex( settings.aiDisplayMaster, row );
			_fnDeleteIndex( settings.aiDisplay, row );
			_fnDeleteIndex( that[ thatIdx ], row, false ); // maintain local indexes
	
			// For server-side processing tables - subtract the deleted row from the count
			if ( settings._iRecordsDisplay > 0 ) {
				settings._iRecordsDisplay--;
			}
	
			// Check for an 'overflow' they case for displaying the table
			_fnLengthOverflow( settings );
	
			// Remove the row's ID reference if there is one
			var id = settings.rowIdFn( rowData._aData );
			if ( id !== undefined ) {
				delete settings.aIds[ id ];
			}
		} );
	
		this.iterator( 'table', function ( settings ) {
			for ( var i=0, ien=settings.aoData.length ; i<ien ; i++ ) {
				settings.aoData[i].idx = i;
			}
		} );
	
		return this;
	} );
	
	
	_api_register( 'rows.add()', function ( rows ) {
		var newRows = this.iterator( 'table', function ( settings ) {
				var row, i, ien;
				var out = [];
	
				for ( i=0, ien=rows.length ; i<ien ; i++ ) {
					row = rows[i];
	
					if ( row.nodeName && row.nodeName.toUpperCase() === 'TR' ) {
						out.push( _fnAddTr( settings, row )[0] );
					}
					else {
						out.push( _fnAddData( settings, row ) );
					}
				}
	
				return out;
			}, 1 );
	
		// Return an Api.rows() extended instance, so rows().nodes() etc can be used
		var modRows = this.rows( -1 );
		modRows.pop();
		$.merge( modRows, newRows );
	
		return modRows;
	} );
	
	
	
	
	
	/**
	 *
	 */
	_api_register( 'row()', function ( selector, opts ) {
		return _selector_first( this.rows( selector, opts ) );
	} );
	
	
	_api_register( 'row().data()', function ( data ) {
		var ctx = this.context;
	
		if ( data === undefined ) {
			// Get
			return ctx.length && this.length ?
				ctx[0].aoData[ this[0] ]._aData :
				undefined;
		}
	
		// Set
		var row = ctx[0].aoData[ this[0] ];
		row._aData = data;
	
		// If the DOM has an id, and the data source is an array
		if ( $.isArray( data ) && row.nTr.id ) {
			_fnSetObjectDataFn( ctx[0].rowId )( data, row.nTr.id );
		}
	
		// Automatically invalidate
		_fnInvalidate( ctx[0], this[0], 'data' );
	
		return this;
	} );
	
	
	_api_register( 'row().node()', function () {
		var ctx = this.context;
	
		return ctx.length && this.length ?
			ctx[0].aoData[ this[0] ].nTr || null :
			null;
	} );
	
	
	_api_register( 'row.add()', function ( row ) {
		// Allow a jQuery object to be passed in - only a single row is added from
		// it though - the first element in the set
		if ( row instanceof $ && row.length ) {
			row = row[0];
		}
	
		var rows = this.iterator( 'table', function ( settings ) {
			if ( row.nodeName && row.nodeName.toUpperCase() === 'TR' ) {
				return _fnAddTr( settings, row )[0];
			}
			return _fnAddData( settings, row );
		} );
	
		// Return an Api.rows() extended instance, with the newly added row selected
		return this.row( rows[0] );
	} );
	
	
	
	var __details_add = function ( ctx, row, data, klass )
	{
		// Convert to array of TR elements
		var rows = [];
		var addRow = function ( r, k ) {
			// Recursion to allow for arrays of jQuery objects
			if ( $.isArray( r ) || r instanceof $ ) {
				for ( var i=0, ien=r.length ; i<ien ; i++ ) {
					addRow( r[i], k );
				}
				return;
			}
	
			// If we get a TR element, then just add it directly - up to the dev
			// to add the correct number of columns etc
			if ( r.nodeName && r.nodeName.toLowerCase() === 'tr' ) {
				rows.push( r );
			}
			else {
				// Otherwise create a row with a wrapper
				var created = $('<tr><td/></tr>').addClass( k );
				$('td', created)
					.addClass( k )
					.html( r )
					[0].colSpan = _fnVisbleColumns( ctx );
	
				rows.push( created[0] );
			}
		};
	
		addRow( data, klass );
	
		if ( row._details ) {
			row._details.detach();
		}
	
		row._details = $(rows);
	
		// If the children were already shown, that state should be retained
		if ( row._detailsShow ) {
			row._details.insertAfter( row.nTr );
		}
	};
	
	
	var __details_remove = function ( api, idx )
	{
		var ctx = api.context;
	
		if ( ctx.length ) {
			var row = ctx[0].aoData[ idx !== undefined ? idx : api[0] ];
	
			if ( row && row._details ) {
				row._details.remove();
	
				row._detailsShow = undefined;
				row._details = undefined;
			}
		}
	};
	
	
	var __details_display = function ( api, show ) {
		var ctx = api.context;
	
		if ( ctx.length && api.length ) {
			var row = ctx[0].aoData[ api[0] ];
	
			if ( row._details ) {
				row._detailsShow = show;
	
				if ( show ) {
					row._details.insertAfter( row.nTr );
				}
				else {
					row._details.detach();
				}
	
				__details_events( ctx[0] );
			}
		}
	};
	
	
	var __details_events = function ( settings )
	{
		var api = new _Api( settings );
		var namespace = '.dt.DT_details';
		var drawEvent = 'draw'+namespace;
		var colvisEvent = 'column-visibility'+namespace;
		var destroyEvent = 'destroy'+namespace;
		var data = settings.aoData;
	
		api.off( drawEvent +' '+ colvisEvent +' '+ destroyEvent );
	
		if ( _pluck( data, '_details' ).length > 0 ) {
			// On each draw, insert the required elements into the document
			api.on( drawEvent, function ( e, ctx ) {
				if ( settings !== ctx ) {
					return;
				}
	
				api.rows( {page:'current'} ).eq(0).each( function (idx) {
					// Internal data grab
					var row = data[ idx ];
	
					if ( row._detailsShow ) {
						row._details.insertAfter( row.nTr );
					}
				} );
			} );
	
			// Column visibility change - update the colspan
			api.on( colvisEvent, function ( e, ctx, idx, vis ) {
				if ( settings !== ctx ) {
					return;
				}
	
				// Update the colspan for the details rows (note, only if it already has
				// a colspan)
				var row, visible = _fnVisbleColumns( ctx );
	
				for ( var i=0, ien=data.length ; i<ien ; i++ ) {
					row = data[i];
	
					if ( row._details ) {
						row._details.children('td[colspan]').attr('colspan', visible );
					}
				}
			} );
	
			// Table destroyed - nuke any child rows
			api.on( destroyEvent, function ( e, ctx ) {
				if ( settings !== ctx ) {
					return;
				}
	
				for ( var i=0, ien=data.length ; i<ien ; i++ ) {
					if ( data[i]._details ) {
						__details_remove( api, i );
					}
				}
			} );
		}
	};
	
	// Strings for the method names to help minification
	var _emp = '';
	var _child_obj = _emp+'row().child';
	var _child_mth = _child_obj+'()';
	
	// data can be:
	//  tr
	//  string
	//  jQuery or array of any of the above
	_api_register( _child_mth, function ( data, klass ) {
		var ctx = this.context;
	
		if ( data === undefined ) {
			// get
			return ctx.length && this.length ?
				ctx[0].aoData[ this[0] ]._details :
				undefined;
		}
		else if ( data === true ) {
			// show
			this.child.show();
		}
		else if ( data === false ) {
			// remove
			__details_remove( this );
		}
		else if ( ctx.length && this.length ) {
			// set
			__details_add( ctx[0], ctx[0].aoData[ this[0] ], data, klass );
		}
	
		return this;
	} );
	
	
	_api_register( [
		_child_obj+'.show()',
		_child_mth+'.show()' // only when `child()` was called with parameters (without
	], function ( show ) {   // it returns an object and this method is not executed)
		__details_display( this, true );
		return this;
	} );
	
	
	_api_register( [
		_child_obj+'.hide()',
		_child_mth+'.hide()' // only when `child()` was called with parameters (without
	], function () {         // it returns an object and this method is not executed)
		__details_display( this, false );
		return this;
	} );
	
	
	_api_register( [
		_child_obj+'.remove()',
		_child_mth+'.remove()' // only when `child()` was called with parameters (without
	], function () {           // it returns an object and this method is not executed)
		__details_remove( this );
		return this;
	} );
	
	
	_api_register( _child_obj+'.isShown()', function () {
		var ctx = this.context;
	
		if ( ctx.length && this.length ) {
			// _detailsShown as false or undefined will fall through to return false
			return ctx[0].aoData[ this[0] ]._detailsShow || false;
		}
		return false;
	} );
	
	
	
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Columns
	 *
	 * {integer}           - column index (>=0 count from left, <0 count from right)
	 * "{integer}:visIdx"  - visible column index (i.e. translate to column index)  (>=0 count from left, <0 count from right)
	 * "{integer}:visible" - alias for {integer}:visIdx  (>=0 count from left, <0 count from right)
	 * "{string}:name"     - column name
	 * "{string}"          - jQuery selector on column header nodes
	 *
	 */
	
	// can be an array of these items, comma separated list, or an array of comma
	// separated lists
	
	var __re_column_selector = /^([^:]+):(name|visIdx|visible)$/;
	
	
	// r1 and r2 are redundant - but it means that the parameters match for the
	// iterator callback in columns().data()
	var __columnData = function ( settings, column, r1, r2, rows ) {
		var a = [];
		for ( var row=0, ien=rows.length ; row<ien ; row++ ) {
			a.push( _fnGetCellData( settings, rows[row], column ) );
		}
		return a;
	};
	
	
	var __column_selector = function ( settings, selector, opts )
	{
		var
			columns = settings.aoColumns,
			names = _pluck( columns, 'sName' ),
			nodes = _pluck( columns, 'nTh' );
	
		var run = function ( s ) {
			var selInt = _intVal( s );
	
			// Selector - all
			if ( s === '' ) {
				return _range( columns.length );
			}
	
			// Selector - index
			if ( selInt !== null ) {
				return [ selInt >= 0 ?
					selInt : // Count from left
					columns.length + selInt // Count from right (+ because its a negative value)
				];
			}
	
			// Selector = function
			if ( typeof s === 'function' ) {
				var rows = _selector_row_indexes( settings, opts );
	
				return $.map( columns, function (col, idx) {
					return s(
							idx,
							__columnData( settings, idx, 0, 0, rows ),
							nodes[ idx ]
						) ? idx : null;
				} );
			}
	
			// jQuery or string selector
			var match = typeof s === 'string' ?
				s.match( __re_column_selector ) :
				'';
	
			if ( match ) {
				switch( match[2] ) {
					case 'visIdx':
					case 'visible':
						var idx = parseInt( match[1], 10 );
						// Visible index given, convert to column index
						if ( idx < 0 ) {
							// Counting from the right
							var visColumns = $.map( columns, function (col,i) {
								return col.bVisible ? i : null;
							} );
							return [ visColumns[ visColumns.length + idx ] ];
						}
						// Counting from the left
						return [ _fnVisibleToColumnIndex( settings, idx ) ];
	
					case 'name':
						// match by name. `names` is column index complete and in order
						return $.map( names, function (name, i) {
							return name === match[1] ? i : null;
						} );
	
					default:
						return [];
				}
			}
	
			// Cell in the table body
			if ( s.nodeName && s._DT_CellIndex ) {
				return [ s._DT_CellIndex.column ];
			}
	
			// jQuery selector on the TH elements for the columns
			var jqResult = $( nodes )
				.filter( s )
				.map( function () {
					return $.inArray( this, nodes ); // `nodes` is column index complete and in order
				} )
				.toArray();
	
			if ( jqResult.length || ! s.nodeName ) {
				return jqResult;
			}
	
			// Otherwise a node which might have a `dt-column` data attribute, or be
			// a child or such an element
			var host = $(s).closest('*[data-dt-column]');
			return host.length ?
				[ host.data('dt-column') ] :
				[];
		};
	
		return _selector_run( 'column', selector, run, settings, opts );
	};
	
	
	var __setColumnVis = function ( settings, column, vis ) {
		var
			cols = settings.aoColumns,
			col  = cols[ column ],
			data = settings.aoData,
			row, cells, i, ien, tr;
	
		// Get
		if ( vis === undefined ) {
			return col.bVisible;
		}
	
		// Set
		// No change
		if ( col.bVisible === vis ) {
			return;
		}
	
		if ( vis ) {
			// Insert column
			// Need to decide if we should use appendChild or insertBefore
			var insertBefore = $.inArray( true, _pluck(cols, 'bVisible'), column+1 );
	
			for ( i=0, ien=data.length ; i<ien ; i++ ) {
				tr = data[i].nTr;
				cells = data[i].anCells;
	
				if ( tr ) {
					// insertBefore can act like appendChild if 2nd arg is null
					tr.insertBefore( cells[ column ], cells[ insertBefore ] || null );
				}
			}
		}
		else {
			// Remove column
			$( _pluck( settings.aoData, 'anCells', column ) ).detach();
		}
	
		// Common actions
		col.bVisible = vis;
		_fnDrawHead( settings, settings.aoHeader );
		_fnDrawHead( settings, settings.aoFooter );
	
		// Update colspan for no records display. Child rows and extensions will use their own
		// listeners to do this - only need to update the empty table item here
		if ( ! settings.aiDisplay.length ) {
			$(settings.nTBody).find('td[colspan]').attr('colspan', _fnVisbleColumns(settings));
		}
	
		_fnSaveState( settings );
	};
	
	
	_api_register( 'columns()', function ( selector, opts ) {
		// argument shifting
		if ( selector === undefined ) {
			selector = '';
		}
		else if ( $.isPlainObject( selector ) ) {
			opts = selector;
			selector = '';
		}
	
		opts = _selector_opts( opts );
	
		var inst = this.iterator( 'table', function ( settings ) {
			return __column_selector( settings, selector, opts );
		}, 1 );
	
		// Want argument shifting here and in _row_selector?
		inst.selector.cols = selector;
		inst.selector.opts = opts;
	
		return inst;
	} );
	
	_api_registerPlural( 'columns().header()', 'column().header()', function ( selector, opts ) {
		return this.iterator( 'column', function ( settings, column ) {
			return settings.aoColumns[column].nTh;
		}, 1 );
	} );
	
	_api_registerPlural( 'columns().footer()', 'column().footer()', function ( selector, opts ) {
		return this.iterator( 'column', function ( settings, column ) {
			return settings.aoColumns[column].nTf;
		}, 1 );
	} );
	
	_api_registerPlural( 'columns().data()', 'column().data()', function () {
		return this.iterator( 'column-rows', __columnData, 1 );
	} );
	
	_api_registerPlural( 'columns().dataSrc()', 'column().dataSrc()', function () {
		return this.iterator( 'column', function ( settings, column ) {
			return settings.aoColumns[column].mData;
		}, 1 );
	} );
	
	_api_registerPlural( 'columns().cache()', 'column().cache()', function ( type ) {
		return this.iterator( 'column-rows', function ( settings, column, i, j, rows ) {
			return _pluck_order( settings.aoData, rows,
				type === 'search' ? '_aFilterData' : '_aSortData', column
			);
		}, 1 );
	} );
	
	_api_registerPlural( 'columns().nodes()', 'column().nodes()', function () {
		return this.iterator( 'column-rows', function ( settings, column, i, j, rows ) {
			return _pluck_order( settings.aoData, rows, 'anCells', column ) ;
		}, 1 );
	} );
	
	_api_registerPlural( 'columns().visible()', 'column().visible()', function ( vis, calc ) {
		var ret = this.iterator( 'column', function ( settings, column ) {
			if ( vis === undefined ) {
				return settings.aoColumns[ column ].bVisible;
			} // else
			__setColumnVis( settings, column, vis );
		} );
	
		// Group the column visibility changes
		if ( vis !== undefined ) {
			// Second loop once the first is done for events
			this.iterator( 'column', function ( settings, column ) {
				_fnCallbackFire( settings, null, 'column-visibility', [settings, column, vis, calc] );
			} );
	
			if ( calc === undefined || calc ) {
				this.columns.adjust();
			}
		}
	
		return ret;
	} );
	
	_api_registerPlural( 'columns().indexes()', 'column().index()', function ( type ) {
		return this.iterator( 'column', function ( settings, column ) {
			return type === 'visible' ?
				_fnColumnIndexToVisible( settings, column ) :
				column;
		}, 1 );
	} );
	
	_api_register( 'columns.adjust()', function () {
		return this.iterator( 'table', function ( settings ) {
			_fnAdjustColumnSizing( settings );
		}, 1 );
	} );
	
	_api_register( 'column.index()', function ( type, idx ) {
		if ( this.context.length !== 0 ) {
			var ctx = this.context[0];
	
			if ( type === 'fromVisible' || type === 'toData' ) {
				return _fnVisibleToColumnIndex( ctx, idx );
			}
			else if ( type === 'fromData' || type === 'toVisible' ) {
				return _fnColumnIndexToVisible( ctx, idx );
			}
		}
	} );
	
	_api_register( 'column()', function ( selector, opts ) {
		return _selector_first( this.columns( selector, opts ) );
	} );
	
	
	
	var __cell_selector = function ( settings, selector, opts )
	{
		var data = settings.aoData;
		var rows = _selector_row_indexes( settings, opts );
		var cells = _removeEmpty( _pluck_order( data, rows, 'anCells' ) );
		var allCells = $( [].concat.apply([], cells) );
		var row;
		var columns = settings.aoColumns.length;
		var a, i, ien, j, o, host;
	
		var run = function ( s ) {
			var fnSelector = typeof s === 'function';
	
			if ( s === null || s === undefined || fnSelector ) {
				// All cells and function selectors
				a = [];
	
				for ( i=0, ien=rows.length ; i<ien ; i++ ) {
					row = rows[i];
	
					for ( j=0 ; j<columns ; j++ ) {
						o = {
							row: row,
							column: j
						};
	
						if ( fnSelector ) {
							// Selector - function
							host = data[ row ];
	
							if ( s( o, _fnGetCellData(settings, row, j), host.anCells ? host.anCells[j] : null ) ) {
								a.push( o );
							}
						}
						else {
							// Selector - all
							a.push( o );
						}
					}
				}
	
				return a;
			}
			
			// Selector - index
			if ( $.isPlainObject( s ) ) {
				// Valid cell index and its in the array of selectable rows
				return s.column !== undefined && s.row !== undefined && $.inArray( s.row, rows ) !== -1 ?
					[s] :
					[];
			}
	
			// Selector - jQuery filtered cells
			var jqResult = allCells
				.filter( s )
				.map( function (i, el) {
					return { // use a new object, in case someone changes the values
						row:    el._DT_CellIndex.row,
						column: el._DT_CellIndex.column
	 				};
				} )
				.toArray();
	
			if ( jqResult.length || ! s.nodeName ) {
				return jqResult;
			}
	
			// Otherwise the selector is a node, and there is one last option - the
			// element might be a child of an element which has dt-row and dt-column
			// data attributes
			host = $(s).closest('*[data-dt-row]');
			return host.length ?
				[ {
					row: host.data('dt-row'),
					column: host.data('dt-column')
				} ] :
				[];
		};
	
		return _selector_run( 'cell', selector, run, settings, opts );
	};
	
	
	
	
	_api_register( 'cells()', function ( rowSelector, columnSelector, opts ) {
		// Argument shifting
		if ( $.isPlainObject( rowSelector ) ) {
			// Indexes
			if ( rowSelector.row === undefined ) {
				// Selector options in first parameter
				opts = rowSelector;
				rowSelector = null;
			}
			else {
				// Cell index objects in first parameter
				opts = columnSelector;
				columnSelector = null;
			}
		}
		if ( $.isPlainObject( columnSelector ) ) {
			opts = columnSelector;
			columnSelector = null;
		}
	
		// Cell selector
		if ( columnSelector === null || columnSelector === undefined ) {
			return this.iterator( 'table', function ( settings ) {
				return __cell_selector( settings, rowSelector, _selector_opts( opts ) );
			} );
		}
	
		// Row + column selector
		var columns = this.columns( columnSelector );
		var rows = this.rows( rowSelector );
		var a, i, ien, j, jen;
	
		this.iterator( 'table', function ( settings, idx ) {
			a = [];
	
			for ( i=0, ien=rows[idx].length ; i<ien ; i++ ) {
				for ( j=0, jen=columns[idx].length ; j<jen ; j++ ) {
					a.push( {
						row:    rows[idx][i],
						column: columns[idx][j]
					} );
				}
			}
		}, 1 );
	
	    // Now pass through the cell selector for options
	    var cells = this.cells( a, opts );
	
		$.extend( cells.selector, {
			cols: columnSelector,
			rows: rowSelector,
			opts: opts
		} );
	
		return cells;
	} );
	
	
	_api_registerPlural( 'cells().nodes()', 'cell().node()', function () {
		return this.iterator( 'cell', function ( settings, row, column ) {
			var data = settings.aoData[ row ];
	
			return data && data.anCells ?
				data.anCells[ column ] :
				undefined;
		}, 1 );
	} );
	
	
	_api_register( 'cells().data()', function () {
		return this.iterator( 'cell', function ( settings, row, column ) {
			return _fnGetCellData( settings, row, column );
		}, 1 );
	} );
	
	
	_api_registerPlural( 'cells().cache()', 'cell().cache()', function ( type ) {
		type = type === 'search' ? '_aFilterData' : '_aSortData';
	
		return this.iterator( 'cell', function ( settings, row, column ) {
			return settings.aoData[ row ][ type ][ column ];
		}, 1 );
	} );
	
	
	_api_registerPlural( 'cells().render()', 'cell().render()', function ( type ) {
		return this.iterator( 'cell', function ( settings, row, column ) {
			return _fnGetCellData( settings, row, column, type );
		}, 1 );
	} );
	
	
	_api_registerPlural( 'cells().indexes()', 'cell().index()', function () {
		return this.iterator( 'cell', function ( settings, row, column ) {
			return {
				row: row,
				column: column,
				columnVisible: _fnColumnIndexToVisible( settings, column )
			};
		}, 1 );
	} );
	
	
	_api_registerPlural( 'cells().invalidate()', 'cell().invalidate()', function ( src ) {
		return this.iterator( 'cell', function ( settings, row, column ) {
			_fnInvalidate( settings, row, src, column );
		} );
	} );
	
	
	
	_api_register( 'cell()', function ( rowSelector, columnSelector, opts ) {
		return _selector_first( this.cells( rowSelector, columnSelector, opts ) );
	} );
	
	
	_api_register( 'cell().data()', function ( data ) {
		var ctx = this.context;
		var cell = this[0];
	
		if ( data === undefined ) {
			// Get
			return ctx.length && cell.length ?
				_fnGetCellData( ctx[0], cell[0].row, cell[0].column ) :
				undefined;
		}
	
		// Set
		_fnSetCellData( ctx[0], cell[0].row, cell[0].column, data );
		_fnInvalidate( ctx[0], cell[0].row, 'data', cell[0].column );
	
		return this;
	} );
	
	
	
	/**
	 * Get current ordering (sorting) that has been applied to the table.
	 *
	 * @returns {array} 2D array containing the sorting information for the first
	 *   table in the current context. Each element in the parent array represents
	 *   a column being sorted upon (i.e. multi-sorting with two columns would have
	 *   2 inner arrays). The inner arrays may have 2 or 3 elements. The first is
	 *   the column index that the sorting condition applies to, the second is the
	 *   direction of the sort (`desc` or `asc`) and, optionally, the third is the
	 *   index of the sorting order from the `column.sorting` initialisation array.
	 *//**
	 * Set the ordering for the table.
	 *
	 * @param {integer} order Column index to sort upon.
	 * @param {string} direction Direction of the sort to be applied (`asc` or `desc`)
	 * @returns {DataTables.Api} this
	 *//**
	 * Set the ordering for the table.
	 *
	 * @param {array} order 1D array of sorting information to be applied.
	 * @param {array} [...] Optional additional sorting conditions
	 * @returns {DataTables.Api} this
	 *//**
	 * Set the ordering for the table.
	 *
	 * @param {array} order 2D array of sorting information to be applied.
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'order()', function ( order, dir ) {
		var ctx = this.context;
	
		if ( order === undefined ) {
			// get
			return ctx.length !== 0 ?
				ctx[0].aaSorting :
				undefined;
		}
	
		// set
		if ( typeof order === 'number' ) {
			// Simple column / direction passed in
			order = [ [ order, dir ] ];
		}
		else if ( order.length && ! $.isArray( order[0] ) ) {
			// Arguments passed in (list of 1D arrays)
			order = Array.prototype.slice.call( arguments );
		}
		// otherwise a 2D array was passed in
	
		return this.iterator( 'table', function ( settings ) {
			settings.aaSorting = order.slice();
		} );
	} );
	
	
	/**
	 * Attach a sort listener to an element for a given column
	 *
	 * @param {node|jQuery|string} node Identifier for the element(s) to attach the
	 *   listener to. This can take the form of a single DOM node, a jQuery
	 *   collection of nodes or a jQuery selector which will identify the node(s).
	 * @param {integer} column the column that a click on this node will sort on
	 * @param {function} [callback] callback function when sort is run
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'order.listener()', function ( node, column, callback ) {
		return this.iterator( 'table', function ( settings ) {
			_fnSortAttachListener( settings, node, column, callback );
		} );
	} );
	
	
	_api_register( 'order.fixed()', function ( set ) {
		if ( ! set ) {
			var ctx = this.context;
			var fixed = ctx.length ?
				ctx[0].aaSortingFixed :
				undefined;
	
			return $.isArray( fixed ) ?
				{ pre: fixed } :
				fixed;
		}
	
		return this.iterator( 'table', function ( settings ) {
			settings.aaSortingFixed = $.extend( true, {}, set );
		} );
	} );
	
	
	// Order by the selected column(s)
	_api_register( [
		'columns().order()',
		'column().order()'
	], function ( dir ) {
		var that = this;
	
		return this.iterator( 'table', function ( settings, i ) {
			var sort = [];
	
			$.each( that[i], function (j, col) {
				sort.push( [ col, dir ] );
			} );
	
			settings.aaSorting = sort;
		} );
	} );
	
	
	
	_api_register( 'search()', function ( input, regex, smart, caseInsen ) {
		var ctx = this.context;
	
		if ( input === undefined ) {
			// get
			return ctx.length !== 0 ?
				ctx[0].oPreviousSearch.sSearch :
				undefined;
		}
	
		// set
		return this.iterator( 'table', function ( settings ) {
			if ( ! settings.oFeatures.bFilter ) {
				return;
			}
	
			_fnFilterComplete( settings, $.extend( {}, settings.oPreviousSearch, {
				"sSearch": input+"",
				"bRegex":  regex === null ? false : regex,
				"bSmart":  smart === null ? true  : smart,
				"bCaseInsensitive": caseInsen === null ? true : caseInsen
			} ), 1 );
		} );
	} );
	
	
	_api_registerPlural(
		'columns().search()',
		'column().search()',
		function ( input, regex, smart, caseInsen ) {
			return this.iterator( 'column', function ( settings, column ) {
				var preSearch = settings.aoPreSearchCols;
	
				if ( input === undefined ) {
					// get
					return preSearch[ column ].sSearch;
				}
	
				// set
				if ( ! settings.oFeatures.bFilter ) {
					return;
				}
	
				$.extend( preSearch[ column ], {
					"sSearch": input+"",
					"bRegex":  regex === null ? false : regex,
					"bSmart":  smart === null ? true  : smart,
					"bCaseInsensitive": caseInsen === null ? true : caseInsen
				} );
	
				_fnFilterComplete( settings, settings.oPreviousSearch, 1 );
			} );
		}
	);
	
	/*
	 * State API methods
	 */
	
	_api_register( 'state()', function () {
		return this.context.length ?
			this.context[0].oSavedState :
			null;
	} );
	
	
	_api_register( 'state.clear()', function () {
		return this.iterator( 'table', function ( settings ) {
			// Save an empty object
			settings.fnStateSaveCallback.call( settings.oInstance, settings, {} );
		} );
	} );
	
	
	_api_register( 'state.loaded()', function () {
		return this.context.length ?
			this.context[0].oLoadedState :
			null;
	} );
	
	
	_api_register( 'state.save()', function () {
		return this.iterator( 'table', function ( settings ) {
			_fnSaveState( settings );
		} );
	} );
	
	
	
	/**
	 * Provide a common method for plug-ins to check the version of DataTables being
	 * used, in order to ensure compatibility.
	 *
	 *  @param {string} version Version string to check for, in the format "X.Y.Z".
	 *    Note that the formats "X" and "X.Y" are also acceptable.
	 *  @returns {boolean} true if this version of DataTables is greater or equal to
	 *    the required version, or false if this version of DataTales is not
	 *    suitable
	 *  @static
	 *  @dtopt API-Static
	 *
	 *  @example
	 *    alert( $.fn.dataTable.versionCheck( '1.9.0' ) );
	 */
	DataTable.versionCheck = DataTable.fnVersionCheck = function( version )
	{
		var aThis = DataTable.version.split('.');
		var aThat = version.split('.');
		var iThis, iThat;
	
		for ( var i=0, iLen=aThat.length ; i<iLen ; i++ ) {
			iThis = parseInt( aThis[i], 10 ) || 0;
			iThat = parseInt( aThat[i], 10 ) || 0;
	
			// Parts are the same, keep comparing
			if (iThis === iThat) {
				continue;
			}
	
			// Parts are different, return immediately
			return iThis > iThat;
		}
	
		return true;
	};
	
	
	/**
	 * Check if a `<table>` node is a DataTable table already or not.
	 *
	 *  @param {node|jquery|string} table Table node, jQuery object or jQuery
	 *      selector for the table to test. Note that if more than more than one
	 *      table is passed on, only the first will be checked
	 *  @returns {boolean} true the table given is a DataTable, or false otherwise
	 *  @static
	 *  @dtopt API-Static
	 *
	 *  @example
	 *    if ( ! $.fn.DataTable.isDataTable( '#example' ) ) {
	 *      $('#example').dataTable();
	 *    }
	 */
	DataTable.isDataTable = DataTable.fnIsDataTable = function ( table )
	{
		var t = $(table).get(0);
		var is = false;
	
		if ( table instanceof DataTable.Api ) {
			return true;
		}
	
		$.each( DataTable.settings, function (i, o) {
			var head = o.nScrollHead ? $('table', o.nScrollHead)[0] : null;
			var foot = o.nScrollFoot ? $('table', o.nScrollFoot)[0] : null;
	
			if ( o.nTable === t || head === t || foot === t ) {
				is = true;
			}
		} );
	
		return is;
	};
	
	
	/**
	 * Get all DataTable tables that have been initialised - optionally you can
	 * select to get only currently visible tables.
	 *
	 *  @param {boolean} [visible=false] Flag to indicate if you want all (default)
	 *    or visible tables only.
	 *  @returns {array} Array of `table` nodes (not DataTable instances) which are
	 *    DataTables
	 *  @static
	 *  @dtopt API-Static
	 *
	 *  @example
	 *    $.each( $.fn.dataTable.tables(true), function () {
	 *      $(table).DataTable().columns.adjust();
	 *    } );
	 */
	DataTable.tables = DataTable.fnTables = function ( visible )
	{
		var api = false;
	
		if ( $.isPlainObject( visible ) ) {
			api = visible.api;
			visible = visible.visible;
		}
	
		var a = $.map( DataTable.settings, function (o) {
			if ( !visible || (visible && $(o.nTable).is(':visible')) ) {
				return o.nTable;
			}
		} );
	
		return api ?
			new _Api( a ) :
			a;
	};
	
	
	/**
	 * Convert from camel case parameters to Hungarian notation. This is made public
	 * for the extensions to provide the same ability as DataTables core to accept
	 * either the 1.9 style Hungarian notation, or the 1.10+ style camelCase
	 * parameters.
	 *
	 *  @param {object} src The model object which holds all parameters that can be
	 *    mapped.
	 *  @param {object} user The object to convert from camel case to Hungarian.
	 *  @param {boolean} force When set to `true`, properties which already have a
	 *    Hungarian value in the `user` object will be overwritten. Otherwise they
	 *    won't be.
	 */
	DataTable.camelToHungarian = _fnCamelToHungarian;
	
	
	
	/**
	 *
	 */
	_api_register( '$()', function ( selector, opts ) {
		var
			rows   = this.rows( opts ).nodes(), // Get all rows
			jqRows = $(rows);
	
		return $( [].concat(
			jqRows.filter( selector ).toArray(),
			jqRows.find( selector ).toArray()
		) );
	} );
	
	
	// jQuery functions to operate on the tables
	$.each( [ 'on', 'one', 'off' ], function (i, key) {
		_api_register( key+'()', function ( /* event, handler */ ) {
			var args = Array.prototype.slice.call(arguments);
	
			// Add the `dt` namespace automatically if it isn't already present
			args[0] = $.map( args[0].split( /\s/ ), function ( e ) {
				return ! e.match(/\.dt\b/) ?
					e+'.dt' :
					e;
				} ).join( ' ' );
	
			var inst = $( this.tables().nodes() );
			inst[key].apply( inst, args );
			return this;
		} );
	} );
	
	
	_api_register( 'clear()', function () {
		return this.iterator( 'table', function ( settings ) {
			_fnClearTable( settings );
		} );
	} );
	
	
	_api_register( 'settings()', function () {
		return new _Api( this.context, this.context );
	} );
	
	
	_api_register( 'init()', function () {
		var ctx = this.context;
		return ctx.length ? ctx[0].oInit : null;
	} );
	
	
	_api_register( 'data()', function () {
		return this.iterator( 'table', function ( settings ) {
			return _pluck( settings.aoData, '_aData' );
		} ).flatten();
	} );
	
	
	_api_register( 'destroy()', function ( remove ) {
		remove = remove || false;
	
		return this.iterator( 'table', function ( settings ) {
			var orig      = settings.nTableWrapper.parentNode;
			var classes   = settings.oClasses;
			var table     = settings.nTable;
			var tbody     = settings.nTBody;
			var thead     = settings.nTHead;
			var tfoot     = settings.nTFoot;
			var jqTable   = $(table);
			var jqTbody   = $(tbody);
			var jqWrapper = $(settings.nTableWrapper);
			var rows      = $.map( settings.aoData, function (r) { return r.nTr; } );
			var i, ien;
	
			// Flag to note that the table is currently being destroyed - no action
			// should be taken
			settings.bDestroying = true;
	
			// Fire off the destroy callbacks for plug-ins etc
			_fnCallbackFire( settings, "aoDestroyCallback", "destroy", [settings] );
	
			// If not being removed from the document, make all columns visible
			if ( ! remove ) {
				new _Api( settings ).columns().visible( true );
			}
	
			// Blitz all `DT` namespaced events (these are internal events, the
			// lowercase, `dt` events are user subscribed and they are responsible
			// for removing them
			jqWrapper.off('.DT').find(':not(tbody *)').off('.DT');
			$(window).off('.DT-'+settings.sInstance);
	
			// When scrolling we had to break the table up - restore it
			if ( table != thead.parentNode ) {
				jqTable.children('thead').detach();
				jqTable.append( thead );
			}
	
			if ( tfoot && table != tfoot.parentNode ) {
				jqTable.children('tfoot').detach();
				jqTable.append( tfoot );
			}
	
			settings.aaSorting = [];
			settings.aaSortingFixed = [];
			_fnSortingClasses( settings );
	
			$( rows ).removeClass( settings.asStripeClasses.join(' ') );
	
			$('th, td', thead).removeClass( classes.sSortable+' '+
				classes.sSortableAsc+' '+classes.sSortableDesc+' '+classes.sSortableNone
			);
	
			// Add the TR elements back into the table in their original order
			jqTbody.children().detach();
			jqTbody.append( rows );
	
			// Remove the DataTables generated nodes, events and classes
			var removedMethod = remove ? 'remove' : 'detach';
			jqTable[ removedMethod ]();
			jqWrapper[ removedMethod ]();
	
			// If we need to reattach the table to the document
			if ( ! remove && orig ) {
				// insertBefore acts like appendChild if !arg[1]
				orig.insertBefore( table, settings.nTableReinsertBefore );
	
				// Restore the width of the original table - was read from the style property,
				// so we can restore directly to that
				jqTable
					.css( 'width', settings.sDestroyWidth )
					.removeClass( classes.sTable );
	
				// If the were originally stripe classes - then we add them back here.
				// Note this is not fool proof (for example if not all rows had stripe
				// classes - but it's a good effort without getting carried away
				ien = settings.asDestroyStripes.length;
	
				if ( ien ) {
					jqTbody.children().each( function (i) {
						$(this).addClass( settings.asDestroyStripes[i % ien] );
					} );
				}
			}
	
			/* Remove the settings object from the settings array */
			var idx = $.inArray( settings, DataTable.settings );
			if ( idx !== -1 ) {
				DataTable.settings.splice( idx, 1 );
			}
		} );
	} );
	
	
	// Add the `every()` method for rows, columns and cells in a compact form
	$.each( [ 'column', 'row', 'cell' ], function ( i, type ) {
		_api_register( type+'s().every()', function ( fn ) {
			var opts = this.selector.opts;
			var api = this;
	
			return this.iterator( type, function ( settings, arg1, arg2, arg3, arg4 ) {
				// Rows and columns:
				//  arg1 - index
				//  arg2 - table counter
				//  arg3 - loop counter
				//  arg4 - undefined
				// Cells:
				//  arg1 - row index
				//  arg2 - column index
				//  arg3 - table counter
				//  arg4 - loop counter
				fn.call(
					api[ type ](
						arg1,
						type==='cell' ? arg2 : opts,
						type==='cell' ? opts : undefined
					),
					arg1, arg2, arg3, arg4
				);
			} );
		} );
	} );
	
	
	// i18n method for extensions to be able to use the language object from the
	// DataTable
	_api_register( 'i18n()', function ( token, def, plural ) {
		var ctx = this.context[0];
		var resolved = _fnGetObjectDataFn( token )( ctx.oLanguage );
	
		if ( resolved === undefined ) {
			resolved = def;
		}
	
		if ( plural !== undefined && $.isPlainObject( resolved ) ) {
			resolved = resolved[ plural ] !== undefined ?
				resolved[ plural ] :
				resolved._;
		}
	
		return resolved.replace( '%d', plural ); // nb: plural might be undefined,
	} );

	/**
	 * Version string for plug-ins to check compatibility. Allowed format is
	 * `a.b.c-d` where: a:int, b:int, c:int, d:string(dev|beta|alpha). `d` is used
	 * only for non-release builds. See http://semver.org/ for more information.
	 *  @member
	 *  @type string
	 *  @default Version number
	 */
	DataTable.version = "1.10.18";

	/**
	 * Private data store, containing all of the settings objects that are
	 * created for the tables on a given page.
	 *
	 * Note that the `DataTable.settings` object is aliased to
	 * `jQuery.fn.dataTableExt` through which it may be accessed and
	 * manipulated, or `jQuery.fn.dataTable.settings`.
	 *  @member
	 *  @type array
	 *  @default []
	 *  @private
	 */
	DataTable.settings = [];

	/**
	 * Object models container, for the various models that DataTables has
	 * available to it. These models define the objects that are used to hold
	 * the active state and configuration of the table.
	 *  @namespace
	 */
	DataTable.models = {};
	
	
	
	/**
	 * Template object for the way in which DataTables holds information about
	 * search information for the global filter and individual column filters.
	 *  @namespace
	 */
	DataTable.models.oSearch = {
		/**
		 * Flag to indicate if the filtering should be case insensitive or not
		 *  @type boolean
		 *  @default true
		 */
		"bCaseInsensitive": true,
	
		/**
		 * Applied search term
		 *  @type string
		 *  @default <i>Empty string</i>
		 */
		"sSearch": "",
	
		/**
		 * Flag to indicate if the search term should be interpreted as a
		 * regular expression (true) or not (false) and therefore and special
		 * regex characters escaped.
		 *  @type boolean
		 *  @default false
		 */
		"bRegex": false,
	
		/**
		 * Flag to indicate if DataTables is to use its smart filtering or not.
		 *  @type boolean
		 *  @default true
		 */
		"bSmart": true
	};
	
	
	
	
	/**
	 * Template object for the way in which DataTables holds information about
	 * each individual row. This is the object format used for the settings
	 * aoData array.
	 *  @namespace
	 */
	DataTable.models.oRow = {
		/**
		 * TR element for the row
		 *  @type node
		 *  @default null
		 */
		"nTr": null,
	
		/**
		 * Array of TD elements for each row. This is null until the row has been
		 * created.
		 *  @type array nodes
		 *  @default []
		 */
		"anCells": null,
	
		/**
		 * Data object from the original data source for the row. This is either
		 * an array if using the traditional form of DataTables, or an object if
		 * using mData options. The exact type will depend on the passed in
		 * data from the data source, or will be an array if using DOM a data
		 * source.
		 *  @type array|object
		 *  @default []
		 */
		"_aData": [],
	
		/**
		 * Sorting data cache - this array is ostensibly the same length as the
		 * number of columns (although each index is generated only as it is
		 * needed), and holds the data that is used for sorting each column in the
		 * row. We do this cache generation at the start of the sort in order that
		 * the formatting of the sort data need be done only once for each cell
		 * per sort. This array should not be read from or written to by anything
		 * other than the master sorting methods.
		 *  @type array
		 *  @default null
		 *  @private
		 */
		"_aSortData": null,
	
		/**
		 * Per cell filtering data cache. As per the sort data cache, used to
		 * increase the performance of the filtering in DataTables
		 *  @type array
		 *  @default null
		 *  @private
		 */
		"_aFilterData": null,
	
		/**
		 * Filtering data cache. This is the same as the cell filtering cache, but
		 * in this case a string rather than an array. This is easily computed with
		 * a join on `_aFilterData`, but is provided as a cache so the join isn't
		 * needed on every search (memory traded for performance)
		 *  @type array
		 *  @default null
		 *  @private
		 */
		"_sFilterRow": null,
	
		/**
		 * Cache of the class name that DataTables has applied to the row, so we
		 * can quickly look at this variable rather than needing to do a DOM check
		 * on className for the nTr property.
		 *  @type string
		 *  @default <i>Empty string</i>
		 *  @private
		 */
		"_sRowStripe": "",
	
		/**
		 * Denote if the original data source was from the DOM, or the data source
		 * object. This is used for invalidating data, so DataTables can
		 * automatically read data from the original source, unless uninstructed
		 * otherwise.
		 *  @type string
		 *  @default null
		 *  @private
		 */
		"src": null,
	
		/**
		 * Index in the aoData array. This saves an indexOf lookup when we have the
		 * object, but want to know the index
		 *  @type integer
		 *  @default -1
		 *  @private
		 */
		"idx": -1
	};
	
	
	/**
	 * Template object for the column information object in DataTables. This object
	 * is held in the settings aoColumns array and contains all the information that
	 * DataTables needs about each individual column.
	 *
	 * Note that this object is related to {@link DataTable.defaults.column}
	 * but this one is the internal data store for DataTables's cache of columns.
	 * It should NOT be manipulated outside of DataTables. Any configuration should
	 * be done through the initialisation options.
	 *  @namespace
	 */
	DataTable.models.oColumn = {
		/**
		 * Column index. This could be worked out on-the-fly with $.inArray, but it
		 * is faster to just hold it as a variable
		 *  @type integer
		 *  @default null
		 */
		"idx": null,
	
		/**
		 * A list of the columns that sorting should occur on when this column
		 * is sorted. That this property is an array allows multi-column sorting
		 * to be defined for a column (for example first name / last name columns
		 * would benefit from this). The values are integers pointing to the
		 * columns to be sorted on (typically it will be a single integer pointing
		 * at itself, but that doesn't need to be the case).
		 *  @type array
		 */
		"aDataSort": null,
	
		/**
		 * Define the sorting directions that are applied to the column, in sequence
		 * as the column is repeatedly sorted upon - i.e. the first value is used
		 * as the sorting direction when the column if first sorted (clicked on).
		 * Sort it again (click again) and it will move on to the next index.
		 * Repeat until loop.
		 *  @type array
		 */
		"asSorting": null,
	
		/**
		 * Flag to indicate if the column is searchable, and thus should be included
		 * in the filtering or not.
		 *  @type boolean
		 */
		"bSearchable": null,
	
		/**
		 * Flag to indicate if the column is sortable or not.
		 *  @type boolean
		 */
		"bSortable": null,
	
		/**
		 * Flag to indicate if the column is currently visible in the table or not
		 *  @type boolean
		 */
		"bVisible": null,
	
		/**
		 * Store for manual type assignment using the `column.type` option. This
		 * is held in store so we can manipulate the column's `sType` property.
		 *  @type string
		 *  @default null
		 *  @private
		 */
		"_sManualType": null,
	
		/**
		 * Flag to indicate if HTML5 data attributes should be used as the data
		 * source for filtering or sorting. True is either are.
		 *  @type boolean
		 *  @default false
		 *  @private
		 */
		"_bAttrSrc": false,
	
		/**
		 * Developer definable function that is called whenever a cell is created (Ajax source,
		 * etc) or processed for input (DOM source). This can be used as a compliment to mRender
		 * allowing you to modify the DOM element (add background colour for example) when the
		 * element is available.
		 *  @type function
		 *  @param {element} nTd The TD node that has been created
		 *  @param {*} sData The Data for the cell
		 *  @param {array|object} oData The data for the whole row
		 *  @param {int} iRow The row index for the aoData data store
		 *  @default null
		 */
		"fnCreatedCell": null,
	
		/**
		 * Function to get data from a cell in a column. You should <b>never</b>
		 * access data directly through _aData internally in DataTables - always use
		 * the method attached to this property. It allows mData to function as
		 * required. This function is automatically assigned by the column
		 * initialisation method
		 *  @type function
		 *  @param {array|object} oData The data array/object for the array
		 *    (i.e. aoData[]._aData)
		 *  @param {string} sSpecific The specific data type you want to get -
		 *    'display', 'type' 'filter' 'sort'
		 *  @returns {*} The data for the cell from the given row's data
		 *  @default null
		 */
		"fnGetData": null,
	
		/**
		 * Function to set data for a cell in the column. You should <b>never</b>
		 * set the data directly to _aData internally in DataTables - always use
		 * this method. It allows mData to function as required. This function
		 * is automatically assigned by the column initialisation method
		 *  @type function
		 *  @param {array|object} oData The data array/object for the array
		 *    (i.e. aoData[]._aData)
		 *  @param {*} sValue Value to set
		 *  @default null
		 */
		"fnSetData": null,
	
		/**
		 * Property to read the value for the cells in the column from the data
		 * source array / object. If null, then the default content is used, if a
		 * function is given then the return from the function is used.
		 *  @type function|int|string|null
		 *  @default null
		 */
		"mData": null,
	
		/**
		 * Partner property to mData which is used (only when defined) to get
		 * the data - i.e. it is basically the same as mData, but without the
		 * 'set' option, and also the data fed to it is the result from mData.
		 * This is the rendering method to match the data method of mData.
		 *  @type function|int|string|null
		 *  @default null
		 */
		"mRender": null,
	
		/**
		 * Unique header TH/TD element for this column - this is what the sorting
		 * listener is attached to (if sorting is enabled.)
		 *  @type node
		 *  @default null
		 */
		"nTh": null,
	
		/**
		 * Unique footer TH/TD element for this column (if there is one). Not used
		 * in DataTables as such, but can be used for plug-ins to reference the
		 * footer for each column.
		 *  @type node
		 *  @default null
		 */
		"nTf": null,
	
		/**
		 * The class to apply to all TD elements in the table's TBODY for the column
		 *  @type string
		 *  @default null
		 */
		"sClass": null,
	
		/**
		 * When DataTables calculates the column widths to assign to each column,
		 * it finds the longest string in each column and then constructs a
		 * temporary table and reads the widths from that. The problem with this
		 * is that "mmm" is much wider then "iiii", but the latter is a longer
		 * string - thus the calculation can go wrong (doing it properly and putting
		 * it into an DOM object and measuring that is horribly(!) slow). Thus as
		 * a "work around" we provide this option. It will append its value to the
		 * text that is found to be the longest string for the column - i.e. padding.
		 *  @type string
		 */
		"sContentPadding": null,
	
		/**
		 * Allows a default value to be given for a column's data, and will be used
		 * whenever a null data source is encountered (this can be because mData
		 * is set to null, or because the data source itself is null).
		 *  @type string
		 *  @default null
		 */
		"sDefaultContent": null,
	
		/**
		 * Name for the column, allowing reference to the column by name as well as
		 * by index (needs a lookup to work by name).
		 *  @type string
		 */
		"sName": null,
	
		/**
		 * Custom sorting data type - defines which of the available plug-ins in
		 * afnSortData the custom sorting will use - if any is defined.
		 *  @type string
		 *  @default std
		 */
		"sSortDataType": 'std',
	
		/**
		 * Class to be applied to the header element when sorting on this column
		 *  @type string
		 *  @default null
		 */
		"sSortingClass": null,
	
		/**
		 * Class to be applied to the header element when sorting on this column -
		 * when jQuery UI theming is used.
		 *  @type string
		 *  @default null
		 */
		"sSortingClassJUI": null,
	
		/**
		 * Title of the column - what is seen in the TH element (nTh).
		 *  @type string
		 */
		"sTitle": null,
	
		/**
		 * Column sorting and filtering type
		 *  @type string
		 *  @default null
		 */
		"sType": null,
	
		/**
		 * Width of the column
		 *  @type string
		 *  @default null
		 */
		"sWidth": null,
	
		/**
		 * Width of the column when it was first "encountered"
		 *  @type string
		 *  @default null
		 */
		"sWidthOrig": null
	};
	
	
	/*
	 * Developer note: The properties of the object below are given in Hungarian
	 * notation, that was used as the interface for DataTables prior to v1.10, however
	 * from v1.10 onwards the primary interface is camel case. In order to avoid
	 * breaking backwards compatibility utterly with this change, the Hungarian
	 * version is still, internally the primary interface, but is is not documented
	 * - hence the @name tags in each doc comment. This allows a Javascript function
	 * to create a map from Hungarian notation to camel case (going the other direction
	 * would require each property to be listed, which would at around 3K to the size
	 * of DataTables, while this method is about a 0.5K hit.
	 *
	 * Ultimately this does pave the way for Hungarian notation to be dropped
	 * completely, but that is a massive amount of work and will break current
	 * installs (therefore is on-hold until v2).
	 */
	
	/**
	 * Initialisation options that can be given to DataTables at initialisation
	 * time.
	 *  @namespace
	 */
	DataTable.defaults = {
		/**
		 * An array of data to use for the table, passed in at initialisation which
		 * will be used in preference to any data which is already in the DOM. This is
		 * particularly useful for constructing tables purely in Javascript, for
		 * example with a custom Ajax call.
		 *  @type array
		 *  @default null
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.data
		 *
		 *  @example
		 *    // Using a 2D array data source
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "data": [
		 *          ['Trident', 'Internet Explorer 4.0', 'Win 95+', 4, 'X'],
		 *          ['Trident', 'Internet Explorer 5.0', 'Win 95+', 5, 'C'],
		 *        ],
		 *        "columns": [
		 *          { "title": "Engine" },
		 *          { "title": "Browser" },
		 *          { "title": "Platform" },
		 *          { "title": "Version" },
		 *          { "title": "Grade" }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using an array of objects as a data source (`data`)
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "data": [
		 *          {
		 *            "engine":   "Trident",
		 *            "browser":  "Internet Explorer 4.0",
		 *            "platform": "Win 95+",
		 *            "version":  4,
		 *            "grade":    "X"
		 *          },
		 *          {
		 *            "engine":   "Trident",
		 *            "browser":  "Internet Explorer 5.0",
		 *            "platform": "Win 95+",
		 *            "version":  5,
		 *            "grade":    "C"
		 *          }
		 *        ],
		 *        "columns": [
		 *          { "title": "Engine",   "data": "engine" },
		 *          { "title": "Browser",  "data": "browser" },
		 *          { "title": "Platform", "data": "platform" },
		 *          { "title": "Version",  "data": "version" },
		 *          { "title": "Grade",    "data": "grade" }
		 *        ]
		 *      } );
		 *    } );
		 */
		"aaData": null,
	
	
		/**
		 * If ordering is enabled, then DataTables will perform a first pass sort on
		 * initialisation. You can define which column(s) the sort is performed
		 * upon, and the sorting direction, with this variable. The `sorting` array
		 * should contain an array for each column to be sorted initially containing
		 * the column's index and a direction string ('asc' or 'desc').
		 *  @type array
		 *  @default [[0,'asc']]
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.order
		 *
		 *  @example
		 *    // Sort by 3rd column first, and then 4th column
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "order": [[2,'asc'], [3,'desc']]
		 *      } );
		 *    } );
		 *
		 *    // No initial sorting
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "order": []
		 *      } );
		 *    } );
		 */
		"aaSorting": [[0,'asc']],
	
	
		/**
		 * This parameter is basically identical to the `sorting` parameter, but
		 * cannot be overridden by user interaction with the table. What this means
		 * is that you could have a column (visible or hidden) which the sorting
		 * will always be forced on first - any sorting after that (from the user)
		 * will then be performed as required. This can be useful for grouping rows
		 * together.
		 *  @type array
		 *  @default null
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.orderFixed
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "orderFixed": [[0,'asc']]
		 *      } );
		 *    } )
		 */
		"aaSortingFixed": [],
	
	
		/**
		 * DataTables can be instructed to load data to display in the table from a
		 * Ajax source. This option defines how that Ajax call is made and where to.
		 *
		 * The `ajax` property has three different modes of operation, depending on
		 * how it is defined. These are:
		 *
		 * * `string` - Set the URL from where the data should be loaded from.
		 * * `object` - Define properties for `jQuery.ajax`.
		 * * `function` - Custom data get function
		 *
		 * `string`
		 * --------
		 *
		 * As a string, the `ajax` property simply defines the URL from which
		 * DataTables will load data.
		 *
		 * `object`
		 * --------
		 *
		 * As an object, the parameters in the object are passed to
		 * [jQuery.ajax](http://api.jquery.com/jQuery.ajax/) allowing fine control
		 * of the Ajax request. DataTables has a number of default parameters which
		 * you can override using this option. Please refer to the jQuery
		 * documentation for a full description of the options available, although
		 * the following parameters provide additional options in DataTables or
		 * require special consideration:
		 *
		 * * `data` - As with jQuery, `data` can be provided as an object, but it
		 *   can also be used as a function to manipulate the data DataTables sends
		 *   to the server. The function takes a single parameter, an object of
		 *   parameters with the values that DataTables has readied for sending. An
		 *   object may be returned which will be merged into the DataTables
		 *   defaults, or you can add the items to the object that was passed in and
		 *   not return anything from the function. This supersedes `fnServerParams`
		 *   from DataTables 1.9-.
		 *
		 * * `dataSrc` - By default DataTables will look for the property `data` (or
		 *   `aaData` for compatibility with DataTables 1.9-) when obtaining data
		 *   from an Ajax source or for server-side processing - this parameter
		 *   allows that property to be changed. You can use Javascript dotted
		 *   object notation to get a data source for multiple levels of nesting, or
		 *   it my be used as a function. As a function it takes a single parameter,
		 *   the JSON returned from the server, which can be manipulated as
		 *   required, with the returned value being that used by DataTables as the
		 *   data source for the table. This supersedes `sAjaxDataProp` from
		 *   DataTables 1.9-.
		 *
		 * * `success` - Should not be overridden it is used internally in
		 *   DataTables. To manipulate / transform the data returned by the server
		 *   use `ajax.dataSrc`, or use `ajax` as a function (see below).
		 *
		 * `function`
		 * ----------
		 *
		 * As a function, making the Ajax call is left up to yourself allowing
		 * complete control of the Ajax request. Indeed, if desired, a method other
		 * than Ajax could be used to obtain the required data, such as Web storage
		 * or an AIR database.
		 *
		 * The function is given four parameters and no return is required. The
		 * parameters are:
		 *
		 * 1. _object_ - Data to send to the server
		 * 2. _function_ - Callback function that must be executed when the required
		 *    data has been obtained. That data should be passed into the callback
		 *    as the only parameter
		 * 3. _object_ - DataTables settings object for the table
		 *
		 * Note that this supersedes `fnServerData` from DataTables 1.9-.
		 *
		 *  @type string|object|function
		 *  @default null
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.ajax
		 *  @since 1.10.0
		 *
		 * @example
		 *   // Get JSON data from a file via Ajax.
		 *   // Note DataTables expects data in the form `{ data: [ ...data... ] }` by default).
		 *   $('#example').dataTable( {
		 *     "ajax": "data.json"
		 *   } );
		 *
		 * @example
		 *   // Get JSON data from a file via Ajax, using `dataSrc` to change
		 *   // `data` to `tableData` (i.e. `{ tableData: [ ...data... ] }`)
		 *   $('#example').dataTable( {
		 *     "ajax": {
		 *       "url": "data.json",
		 *       "dataSrc": "tableData"
		 *     }
		 *   } );
		 *
		 * @example
		 *   // Get JSON data from a file via Ajax, using `dataSrc` to read data
		 *   // from a plain array rather than an array in an object
		 *   $('#example').dataTable( {
		 *     "ajax": {
		 *       "url": "data.json",
		 *       "dataSrc": ""
		 *     }
		 *   } );
		 *
		 * @example
		 *   // Manipulate the data returned from the server - add a link to data
		 *   // (note this can, should, be done using `render` for the column - this
		 *   // is just a simple example of how the data can be manipulated).
		 *   $('#example').dataTable( {
		 *     "ajax": {
		 *       "url": "data.json",
		 *       "dataSrc": function ( json ) {
		 *         for ( var i=0, ien=json.length ; i<ien ; i++ ) {
		 *           json[i][0] = '<a href="/message/'+json[i][0]+'>View message</a>';
		 *         }
		 *         return json;
		 *       }
		 *     }
		 *   } );
		 *
		 * @example
		 *   // Add data to the request
		 *   $('#example').dataTable( {
		 *     "ajax": {
		 *       "url": "data.json",
		 *       "data": function ( d ) {
		 *         return {
		 *           "extra_search": $('#extra').val()
		 *         };
		 *       }
		 *     }
		 *   } );
		 *
		 * @example
		 *   // Send request as POST
		 *   $('#example').dataTable( {
		 *     "ajax": {
		 *       "url": "data.json",
		 *       "type": "POST"
		 *     }
		 *   } );
		 *
		 * @example
		 *   // Get the data from localStorage (could interface with a form for
		 *   // adding, editing and removing rows).
		 *   $('#example').dataTable( {
		 *     "ajax": function (data, callback, settings) {
		 *       callback(
		 *         JSON.parse( localStorage.getItem('dataTablesData') )
		 *       );
		 *     }
		 *   } );
		 */
		"ajax": null,
	
	
		/**
		 * This parameter allows you to readily specify the entries in the length drop
		 * down menu that DataTables shows when pagination is enabled. It can be
		 * either a 1D array of options which will be used for both the displayed
		 * option and the value, or a 2D array which will use the array in the first
		 * position as the value, and the array in the second position as the
		 * displayed options (useful for language strings such as 'All').
		 *
		 * Note that the `pageLength` property will be automatically set to the
		 * first value given in this array, unless `pageLength` is also provided.
		 *  @type array
		 *  @default [ 10, 25, 50, 100 ]
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.lengthMenu
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]]
		 *      } );
		 *    } );
		 */
		"aLengthMenu": [ 10, 25, 50, 100 ],
	
	
		/**
		 * The `columns` option in the initialisation parameter allows you to define
		 * details about the way individual columns behave. For a full list of
		 * column options that can be set, please see
		 * {@link DataTable.defaults.column}. Note that if you use `columns` to
		 * define your columns, you must have an entry in the array for every single
		 * column that you have in your table (these can be null if you don't which
		 * to specify any options).
		 *  @member
		 *
		 *  @name DataTable.defaults.column
		 */
		"aoColumns": null,
	
		/**
		 * Very similar to `columns`, `columnDefs` allows you to target a specific
		 * column, multiple columns, or all columns, using the `targets` property of
		 * each object in the array. This allows great flexibility when creating
		 * tables, as the `columnDefs` arrays can be of any length, targeting the
		 * columns you specifically want. `columnDefs` may use any of the column
		 * options available: {@link DataTable.defaults.column}, but it _must_
		 * have `targets` defined in each object in the array. Values in the `targets`
		 * array may be:
		 *   <ul>
		 *     <li>a string - class name will be matched on the TH for the column</li>
		 *     <li>0 or a positive integer - column index counting from the left</li>
		 *     <li>a negative integer - column index counting from the right</li>
		 *     <li>the string "_all" - all columns (i.e. assign a default)</li>
		 *   </ul>
		 *  @member
		 *
		 *  @name DataTable.defaults.columnDefs
		 */
		"aoColumnDefs": null,
	
	
		/**
		 * Basically the same as `search`, this parameter defines the individual column
		 * filtering state at initialisation time. The array must be of the same size
		 * as the number of columns, and each element be an object with the parameters
		 * `search` and `escapeRegex` (the latter is optional). 'null' is also
		 * accepted and the default will be used.
		 *  @type array
		 *  @default []
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.searchCols
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "searchCols": [
		 *          null,
		 *          { "search": "My filter" },
		 *          null,
		 *          { "search": "^[0-9]", "escapeRegex": false }
		 *        ]
		 *      } );
		 *    } )
		 */
		"aoSearchCols": [],
	
	
		/**
		 * An array of CSS classes that should be applied to displayed rows. This
		 * array may be of any length, and DataTables will apply each class
		 * sequentially, looping when required.
		 *  @type array
		 *  @default null <i>Will take the values determined by the `oClasses.stripe*`
		 *    options</i>
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.stripeClasses
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stripeClasses": [ 'strip1', 'strip2', 'strip3' ]
		 *      } );
		 *    } )
		 */
		"asStripeClasses": null,
	
	
		/**
		 * Enable or disable automatic column width calculation. This can be disabled
		 * as an optimisation (it takes some time to calculate the widths) if the
		 * tables widths are passed in using `columns`.
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.autoWidth
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "autoWidth": false
		 *      } );
		 *    } );
		 */
		"bAutoWidth": true,
	
	
		/**
		 * Deferred rendering can provide DataTables with a huge speed boost when you
		 * are using an Ajax or JS data source for the table. This option, when set to
		 * true, will cause DataTables to defer the creation of the table elements for
		 * each row until they are needed for a draw - saving a significant amount of
		 * time.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.deferRender
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "ajax": "sources/arrays.txt",
		 *        "deferRender": true
		 *      } );
		 *    } );
		 */
		"bDeferRender": false,
	
	
		/**
		 * Replace a DataTable which matches the given selector and replace it with
		 * one which has the properties of the new initialisation object passed. If no
		 * table matches the selector, then the new DataTable will be constructed as
		 * per normal.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.destroy
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "srollY": "200px",
		 *        "paginate": false
		 *      } );
		 *
		 *      // Some time later....
		 *      $('#example').dataTable( {
		 *        "filter": false,
		 *        "destroy": true
		 *      } );
		 *    } );
		 */
		"bDestroy": false,
	
	
		/**
		 * Enable or disable filtering of data. Filtering in DataTables is "smart" in
		 * that it allows the end user to input multiple words (space separated) and
		 * will match a row containing those words, even if not in the order that was
		 * specified (this allow matching across multiple columns). Note that if you
		 * wish to use filtering in DataTables this must remain 'true' - to remove the
		 * default filtering input box and retain filtering abilities, please use
		 * {@link DataTable.defaults.dom}.
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.searching
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "searching": false
		 *      } );
		 *    } );
		 */
		"bFilter": true,
	
	
		/**
		 * Enable or disable the table information display. This shows information
		 * about the data that is currently visible on the page, including information
		 * about filtered data if that action is being performed.
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.info
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "info": false
		 *      } );
		 *    } );
		 */
		"bInfo": true,
	
	
		/**
		 * Allows the end user to select the size of a formatted page from a select
		 * menu (sizes are 10, 25, 50 and 100). Requires pagination (`paginate`).
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.lengthChange
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "lengthChange": false
		 *      } );
		 *    } );
		 */
		"bLengthChange": true,
	
	
		/**
		 * Enable or disable pagination.
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.paging
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "paging": false
		 *      } );
		 *    } );
		 */
		"bPaginate": true,
	
	
		/**
		 * Enable or disable the display of a 'processing' indicator when the table is
		 * being processed (e.g. a sort). This is particularly useful for tables with
		 * large amounts of data where it can take a noticeable amount of time to sort
		 * the entries.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.processing
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "processing": true
		 *      } );
		 *    } );
		 */
		"bProcessing": false,
	
	
		/**
		 * Retrieve the DataTables object for the given selector. Note that if the
		 * table has already been initialised, this parameter will cause DataTables
		 * to simply return the object that has already been set up - it will not take
		 * account of any changes you might have made to the initialisation object
		 * passed to DataTables (setting this parameter to true is an acknowledgement
		 * that you understand this). `destroy` can be used to reinitialise a table if
		 * you need.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.retrieve
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      initTable();
		 *      tableActions();
		 *    } );
		 *
		 *    function initTable ()
		 *    {
		 *      return $('#example').dataTable( {
		 *        "scrollY": "200px",
		 *        "paginate": false,
		 *        "retrieve": true
		 *      } );
		 *    }
		 *
		 *    function tableActions ()
		 *    {
		 *      var table = initTable();
		 *      // perform API operations with oTable
		 *    }
		 */
		"bRetrieve": false,
	
	
		/**
		 * When vertical (y) scrolling is enabled, DataTables will force the height of
		 * the table's viewport to the given height at all times (useful for layout).
		 * However, this can look odd when filtering data down to a small data set,
		 * and the footer is left "floating" further down. This parameter (when
		 * enabled) will cause DataTables to collapse the table's viewport down when
		 * the result set will fit within the given Y height.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.scrollCollapse
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "scrollY": "200",
		 *        "scrollCollapse": true
		 *      } );
		 *    } );
		 */
		"bScrollCollapse": false,
	
	
		/**
		 * Configure DataTables to use server-side processing. Note that the
		 * `ajax` parameter must also be given in order to give DataTables a
		 * source to obtain the required data for each draw.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Features
		 *  @dtopt Server-side
		 *  @name DataTable.defaults.serverSide
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "serverSide": true,
		 *        "ajax": "xhr.php"
		 *      } );
		 *    } );
		 */
		"bServerSide": false,
	
	
		/**
		 * Enable or disable sorting of columns. Sorting of individual columns can be
		 * disabled by the `sortable` option for each column.
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.ordering
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "ordering": false
		 *      } );
		 *    } );
		 */
		"bSort": true,
	
	
		/**
		 * Enable or display DataTables' ability to sort multiple columns at the
		 * same time (activated by shift-click by the user).
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.orderMulti
		 *
		 *  @example
		 *    // Disable multiple column sorting ability
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "orderMulti": false
		 *      } );
		 *    } );
		 */
		"bSortMulti": true,
	
	
		/**
		 * Allows control over whether DataTables should use the top (true) unique
		 * cell that is found for a single column, or the bottom (false - default).
		 * This is useful when using complex headers.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.orderCellsTop
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "orderCellsTop": true
		 *      } );
		 *    } );
		 */
		"bSortCellsTop": false,
	
	
		/**
		 * Enable or disable the addition of the classes `sorting\_1`, `sorting\_2` and
		 * `sorting\_3` to the columns which are currently being sorted on. This is
		 * presented as a feature switch as it can increase processing time (while
		 * classes are removed and added) so for large data sets you might want to
		 * turn this off.
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.orderClasses
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "orderClasses": false
		 *      } );
		 *    } );
		 */
		"bSortClasses": true,
	
	
		/**
		 * Enable or disable state saving. When enabled HTML5 `localStorage` will be
		 * used to save table display information such as pagination information,
		 * display length, filtering and sorting. As such when the end user reloads
		 * the page the display display will match what thy had previously set up.
		 *
		 * Due to the use of `localStorage` the default state saving is not supported
		 * in IE6 or 7. If state saving is required in those browsers, use
		 * `stateSaveCallback` to provide a storage solution such as cookies.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.stateSave
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "stateSave": true
		 *      } );
		 *    } );
		 */
		"bStateSave": false,
	
	
		/**
		 * This function is called when a TR element is created (and all TD child
		 * elements have been inserted), or registered if using a DOM source, allowing
		 * manipulation of the TR element (adding classes etc).
		 *  @type function
		 *  @param {node} row "TR" element for the current row
		 *  @param {array} data Raw data array for this row
		 *  @param {int} dataIndex The index of this row in the internal aoData array
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.createdRow
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "createdRow": function( row, data, dataIndex ) {
		 *          // Bold the grade for all 'A' grade browsers
		 *          if ( data[4] == "A" )
		 *          {
		 *            $('td:eq(4)', row).html( '<b>A</b>' );
		 *          }
		 *        }
		 *      } );
		 *    } );
		 */
		"fnCreatedRow": null,
	
	
		/**
		 * This function is called on every 'draw' event, and allows you to
		 * dynamically modify any aspect you want about the created DOM.
		 *  @type function
		 *  @param {object} settings DataTables settings object
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.drawCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "drawCallback": function( settings ) {
		 *          alert( 'DataTables has redrawn the table' );
		 *        }
		 *      } );
		 *    } );
		 */
		"fnDrawCallback": null,
	
	
		/**
		 * Identical to fnHeaderCallback() but for the table footer this function
		 * allows you to modify the table footer on every 'draw' event.
		 *  @type function
		 *  @param {node} foot "TR" element for the footer
		 *  @param {array} data Full table data (as derived from the original HTML)
		 *  @param {int} start Index for the current display starting point in the
		 *    display array
		 *  @param {int} end Index for the current display ending point in the
		 *    display array
		 *  @param {array int} display Index array to translate the visual position
		 *    to the full data array
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.footerCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "footerCallback": function( tfoot, data, start, end, display ) {
		 *          tfoot.getElementsByTagName('th')[0].innerHTML = "Starting index is "+start;
		 *        }
		 *      } );
		 *    } )
		 */
		"fnFooterCallback": null,
	
	
		/**
		 * When rendering large numbers in the information element for the table
		 * (i.e. "Showing 1 to 10 of 57 entries") DataTables will render large numbers
		 * to have a comma separator for the 'thousands' units (e.g. 1 million is
		 * rendered as "1,000,000") to help readability for the end user. This
		 * function will override the default method DataTables uses.
		 *  @type function
		 *  @member
		 *  @param {int} toFormat number to be formatted
		 *  @returns {string} formatted string for DataTables to show the number
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.formatNumber
		 *
		 *  @example
		 *    // Format a number using a single quote for the separator (note that
		 *    // this can also be done with the language.thousands option)
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "formatNumber": function ( toFormat ) {
		 *          return toFormat.toString().replace(
		 *            /\B(?=(\d{3})+(?!\d))/g, "'"
		 *          );
		 *        };
		 *      } );
		 *    } );
		 */
		"fnFormatNumber": function ( toFormat ) {
			return toFormat.toString().replace(
				/\B(?=(\d{3})+(?!\d))/g,
				this.oLanguage.sThousands
			);
		},
	
	
		/**
		 * This function is called on every 'draw' event, and allows you to
		 * dynamically modify the header row. This can be used to calculate and
		 * display useful information about the table.
		 *  @type function
		 *  @param {node} head "TR" element for the header
		 *  @param {array} data Full table data (as derived from the original HTML)
		 *  @param {int} start Index for the current display starting point in the
		 *    display array
		 *  @param {int} end Index for the current display ending point in the
		 *    display array
		 *  @param {array int} display Index array to translate the visual position
		 *    to the full data array
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.headerCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "fheaderCallback": function( head, data, start, end, display ) {
		 *          head.getElementsByTagName('th')[0].innerHTML = "Displaying "+(end-start)+" records";
		 *        }
		 *      } );
		 *    } )
		 */
		"fnHeaderCallback": null,
	
	
		/**
		 * The information element can be used to convey information about the current
		 * state of the table. Although the internationalisation options presented by
		 * DataTables are quite capable of dealing with most customisations, there may
		 * be times where you wish to customise the string further. This callback
		 * allows you to do exactly that.
		 *  @type function
		 *  @param {object} oSettings DataTables settings object
		 *  @param {int} start Starting position in data for the draw
		 *  @param {int} end End position in data for the draw
		 *  @param {int} max Total number of rows in the table (regardless of
		 *    filtering)
		 *  @param {int} total Total number of rows in the data set, after filtering
		 *  @param {string} pre The string that DataTables has formatted using it's
		 *    own rules
		 *  @returns {string} The string to be displayed in the information element.
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.infoCallback
		 *
		 *  @example
		 *    $('#example').dataTable( {
		 *      "infoCallback": function( settings, start, end, max, total, pre ) {
		 *        return start +" to "+ end;
		 *      }
		 *    } );
		 */
		"fnInfoCallback": null,
	
	
		/**
		 * Called when the table has been initialised. Normally DataTables will
		 * initialise sequentially and there will be no need for this function,
		 * however, this does not hold true when using external language information
		 * since that is obtained using an async XHR call.
		 *  @type function
		 *  @param {object} settings DataTables settings object
		 *  @param {object} json The JSON object request from the server - only
		 *    present if client-side Ajax sourced data is used
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.initComplete
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "initComplete": function(settings, json) {
		 *          alert( 'DataTables has finished its initialisation.' );
		 *        }
		 *      } );
		 *    } )
		 */
		"fnInitComplete": null,
	
	
		/**
		 * Called at the very start of each table draw and can be used to cancel the
		 * draw by returning false, any other return (including undefined) results in
		 * the full draw occurring).
		 *  @type function
		 *  @param {object} settings DataTables settings object
		 *  @returns {boolean} False will cancel the draw, anything else (including no
		 *    return) will allow it to complete.
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.preDrawCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "preDrawCallback": function( settings ) {
		 *          if ( $('#test').val() == 1 ) {
		 *            return false;
		 *          }
		 *        }
		 *      } );
		 *    } );
		 */
		"fnPreDrawCallback": null,
	
	
		/**
		 * This function allows you to 'post process' each row after it have been
		 * generated for each table draw, but before it is rendered on screen. This
		 * function might be used for setting the row class name etc.
		 *  @type function
		 *  @param {node} row "TR" element for the current row
		 *  @param {array} data Raw data array for this row
		 *  @param {int} displayIndex The display index for the current table draw
		 *  @param {int} displayIndexFull The index of the data in the full list of
		 *    rows (after filtering)
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.rowCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "rowCallback": function( row, data, displayIndex, displayIndexFull ) {
		 *          // Bold the grade for all 'A' grade browsers
		 *          if ( data[4] == "A" ) {
		 *            $('td:eq(4)', row).html( '<b>A</b>' );
		 *          }
		 *        }
		 *      } );
		 *    } );
		 */
		"fnRowCallback": null,
	
	
		/**
		 * __Deprecated__ The functionality provided by this parameter has now been
		 * superseded by that provided through `ajax`, which should be used instead.
		 *
		 * This parameter allows you to override the default function which obtains
		 * the data from the server so something more suitable for your application.
		 * For example you could use POST data, or pull information from a Gears or
		 * AIR database.
		 *  @type function
		 *  @member
		 *  @param {string} source HTTP source to obtain the data from (`ajax`)
		 *  @param {array} data A key/value pair object containing the data to send
		 *    to the server
		 *  @param {function} callback to be called on completion of the data get
		 *    process that will draw the data on the page.
		 *  @param {object} settings DataTables settings object
		 *
		 *  @dtopt Callbacks
		 *  @dtopt Server-side
		 *  @name DataTable.defaults.serverData
		 *
		 *  @deprecated 1.10. Please use `ajax` for this functionality now.
		 */
		"fnServerData": null,
	
	
		/**
		 * __Deprecated__ The functionality provided by this parameter has now been
		 * superseded by that provided through `ajax`, which should be used instead.
		 *
		 *  It is often useful to send extra data to the server when making an Ajax
		 * request - for example custom filtering information, and this callback
		 * function makes it trivial to send extra information to the server. The
		 * passed in parameter is the data set that has been constructed by
		 * DataTables, and you can add to this or modify it as you require.
		 *  @type function
		 *  @param {array} data Data array (array of objects which are name/value
		 *    pairs) that has been constructed by DataTables and will be sent to the
		 *    server. In the case of Ajax sourced data with server-side processing
		 *    this will be an empty array, for server-side processing there will be a
		 *    significant number of parameters!
		 *  @returns {undefined} Ensure that you modify the data array passed in,
		 *    as this is passed by reference.
		 *
		 *  @dtopt Callbacks
		 *  @dtopt Server-side
		 *  @name DataTable.defaults.serverParams
		 *
		 *  @deprecated 1.10. Please use `ajax` for this functionality now.
		 */
		"fnServerParams": null,
	
	
		/**
		 * Load the table state. With this function you can define from where, and how, the
		 * state of a table is loaded. By default DataTables will load from `localStorage`
		 * but you might wish to use a server-side database or cookies.
		 *  @type function
		 *  @member
		 *  @param {object} settings DataTables settings object
		 *  @param {object} callback Callback that can be executed when done. It
		 *    should be passed the loaded state object.
		 *  @return {object} The DataTables state object to be loaded
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.stateLoadCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateSave": true,
		 *        "stateLoadCallback": function (settings, callback) {
		 *          $.ajax( {
		 *            "url": "/state_load",
		 *            "dataType": "json",
		 *            "success": function (json) {
		 *              callback( json );
		 *            }
		 *          } );
		 *        }
		 *      } );
		 *    } );
		 */
		"fnStateLoadCallback": function ( settings ) {
			try {
				return JSON.parse(
					(settings.iStateDuration === -1 ? sessionStorage : localStorage).getItem(
						'DataTables_'+settings.sInstance+'_'+location.pathname
					)
				);
			} catch (e) {}
		},
	
	
		/**
		 * Callback which allows modification of the saved state prior to loading that state.
		 * This callback is called when the table is loading state from the stored data, but
		 * prior to the settings object being modified by the saved state. Note that for
		 * plug-in authors, you should use the `stateLoadParams` event to load parameters for
		 * a plug-in.
		 *  @type function
		 *  @param {object} settings DataTables settings object
		 *  @param {object} data The state object that is to be loaded
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.stateLoadParams
		 *
		 *  @example
		 *    // Remove a saved filter, so filtering is never loaded
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateSave": true,
		 *        "stateLoadParams": function (settings, data) {
		 *          data.oSearch.sSearch = "";
		 *        }
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Disallow state loading by returning false
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateSave": true,
		 *        "stateLoadParams": function (settings, data) {
		 *          return false;
		 *        }
		 *      } );
		 *    } );
		 */
		"fnStateLoadParams": null,
	
	
		/**
		 * Callback that is called when the state has been loaded from the state saving method
		 * and the DataTables settings object has been modified as a result of the loaded state.
		 *  @type function
		 *  @param {object} settings DataTables settings object
		 *  @param {object} data The state object that was loaded
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.stateLoaded
		 *
		 *  @example
		 *    // Show an alert with the filtering value that was saved
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateSave": true,
		 *        "stateLoaded": function (settings, data) {
		 *          alert( 'Saved filter was: '+data.oSearch.sSearch );
		 *        }
		 *      } );
		 *    } );
		 */
		"fnStateLoaded": null,
	
	
		/**
		 * Save the table state. This function allows you to define where and how the state
		 * information for the table is stored By default DataTables will use `localStorage`
		 * but you might wish to use a server-side database or cookies.
		 *  @type function
		 *  @member
		 *  @param {object} settings DataTables settings object
		 *  @param {object} data The state object to be saved
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.stateSaveCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateSave": true,
		 *        "stateSaveCallback": function (settings, data) {
		 *          // Send an Ajax request to the server with the state object
		 *          $.ajax( {
		 *            "url": "/state_save",
		 *            "data": data,
		 *            "dataType": "json",
		 *            "method": "POST"
		 *            "success": function () {}
		 *          } );
		 *        }
		 *      } );
		 *    } );
		 */
		"fnStateSaveCallback": function ( settings, data ) {
			try {
				(settings.iStateDuration === -1 ? sessionStorage : localStorage).setItem(
					'DataTables_'+settings.sInstance+'_'+location.pathname,
					JSON.stringify( data )
				);
			} catch (e) {}
		},
	
	
		/**
		 * Callback which allows modification of the state to be saved. Called when the table
		 * has changed state a new state save is required. This method allows modification of
		 * the state saving object prior to actually doing the save, including addition or
		 * other state properties or modification. Note that for plug-in authors, you should
		 * use the `stateSaveParams` event to save parameters for a plug-in.
		 *  @type function
		 *  @param {object} settings DataTables settings object
		 *  @param {object} data The state object to be saved
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.stateSaveParams
		 *
		 *  @example
		 *    // Remove a saved filter, so filtering is never saved
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateSave": true,
		 *        "stateSaveParams": function (settings, data) {
		 *          data.oSearch.sSearch = "";
		 *        }
		 *      } );
		 *    } );
		 */
		"fnStateSaveParams": null,
	
	
		/**
		 * Duration for which the saved state information is considered valid. After this period
		 * has elapsed the state will be returned to the default.
		 * Value is given in seconds.
		 *  @type int
		 *  @default 7200 <i>(2 hours)</i>
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.stateDuration
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateDuration": 60*60*24; // 1 day
		 *      } );
		 *    } )
		 */
		"iStateDuration": 7200,
	
	
		/**
		 * When enabled DataTables will not make a request to the server for the first
		 * page draw - rather it will use the data already on the page (no sorting etc
		 * will be applied to it), thus saving on an XHR at load time. `deferLoading`
		 * is used to indicate that deferred loading is required, but it is also used
		 * to tell DataTables how many records there are in the full table (allowing
		 * the information element and pagination to be displayed correctly). In the case
		 * where a filtering is applied to the table on initial load, this can be
		 * indicated by giving the parameter as an array, where the first element is
		 * the number of records available after filtering and the second element is the
		 * number of records without filtering (allowing the table information element
		 * to be shown correctly).
		 *  @type int | array
		 *  @default null
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.deferLoading
		 *
		 *  @example
		 *    // 57 records available in the table, no filtering applied
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "serverSide": true,
		 *        "ajax": "scripts/server_processing.php",
		 *        "deferLoading": 57
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // 57 records after filtering, 100 without filtering (an initial filter applied)
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "serverSide": true,
		 *        "ajax": "scripts/server_processing.php",
		 *        "deferLoading": [ 57, 100 ],
		 *        "search": {
		 *          "search": "my_filter"
		 *        }
		 *      } );
		 *    } );
		 */
		"iDeferLoading": null,
	
	
		/**
		 * Number of rows to display on a single page when using pagination. If
		 * feature enabled (`lengthChange`) then the end user will be able to override
		 * this to a custom setting using a pop-up menu.
		 *  @type int
		 *  @default 10
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.pageLength
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "pageLength": 50
		 *      } );
		 *    } )
		 */
		"iDisplayLength": 10,
	
	
		/**
		 * Define the starting point for data display when using DataTables with
		 * pagination. Note that this parameter is the number of records, rather than
		 * the page number, so if you have 10 records per page and want to start on
		 * the third page, it should be "20".
		 *  @type int
		 *  @default 0
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.displayStart
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "displayStart": 20
		 *      } );
		 *    } )
		 */
		"iDisplayStart": 0,
	
	
		/**
		 * By default DataTables allows keyboard navigation of the table (sorting, paging,
		 * and filtering) by adding a `tabindex` attribute to the required elements. This
		 * allows you to tab through the controls and press the enter key to activate them.
		 * The tabindex is default 0, meaning that the tab follows the flow of the document.
		 * You can overrule this using this parameter if you wish. Use a value of -1 to
		 * disable built-in keyboard navigation.
		 *  @type int
		 *  @default 0
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.tabIndex
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "tabIndex": 1
		 *      } );
		 *    } );
		 */
		"iTabIndex": 0,
	
	
		/**
		 * Classes that DataTables assigns to the various components and features
		 * that it adds to the HTML table. This allows classes to be configured
		 * during initialisation in addition to through the static
		 * {@link DataTable.ext.oStdClasses} object).
		 *  @namespace
		 *  @name DataTable.defaults.classes
		 */
		"oClasses": {},
	
	
		/**
		 * All strings that DataTables uses in the user interface that it creates
		 * are defined in this object, allowing you to modified them individually or
		 * completely replace them all as required.
		 *  @namespace
		 *  @name DataTable.defaults.language
		 */
		"oLanguage": {
			/**
			 * Strings that are used for WAI-ARIA labels and controls only (these are not
			 * actually visible on the page, but will be read by screenreaders, and thus
			 * must be internationalised as well).
			 *  @namespace
			 *  @name DataTable.defaults.language.aria
			 */
			"oAria": {
				/**
				 * ARIA label that is added to the table headers when the column may be
				 * sorted ascending by activing the column (click or return when focused).
				 * Note that the column header is prefixed to this string.
				 *  @type string
				 *  @default : activate to sort column ascending
				 *
				 *  @dtopt Language
				 *  @name DataTable.defaults.language.aria.sortAscending
				 *
				 *  @example
				 *    $(document).ready( function() {
				 *      $('#example').dataTable( {
				 *        "language": {
				 *          "aria": {
				 *            "sortAscending": " - click/return to sort ascending"
				 *          }
				 *        }
				 *      } );
				 *    } );
				 */
				"sSortAscending": ": activate to sort column ascending",
	
				/**
				 * ARIA label that is added to the table headers when the column may be
				 * sorted descending by activing the column (click or return when focused).
				 * Note that the column header is prefixed to this string.
				 *  @type string
				 *  @default : activate to sort column ascending
				 *
				 *  @dtopt Language
				 *  @name DataTable.defaults.language.aria.sortDescending
				 *
				 *  @example
				 *    $(document).ready( function() {
				 *      $('#example').dataTable( {
				 *        "language": {
				 *          "aria": {
				 *            "sortDescending": " - click/return to sort descending"
				 *          }
				 *        }
				 *      } );
				 *    } );
				 */
				"sSortDescending": ": activate to sort column descending"
			},
	
			/**
			 * Pagination string used by DataTables for the built-in pagination
			 * control types.
			 *  @namespace
			 *  @name DataTable.defaults.language.paginate
			 */
			"oPaginate": {
				/**
				 * Text to use when using the 'full_numbers' type of pagination for the
				 * button to take the user to the first page.
				 *  @type string
				 *  @default First
				 *
				 *  @dtopt Language
				 *  @name DataTable.defaults.language.paginate.first
				 *
				 *  @example
				 *    $(document).ready( function() {
				 *      $('#example').dataTable( {
				 *        "language": {
				 *          "paginate": {
				 *            "first": "First page"
				 *          }
				 *        }
				 *      } );
				 *    } );
				 */
				"sFirst": "First",
	
	
				/**
				 * Text to use when using the 'full_numbers' type of pagination for the
				 * button to take the user to the last page.
				 *  @type string
				 *  @default Last
				 *
				 *  @dtopt Language
				 *  @name DataTable.defaults.language.paginate.last
				 *
				 *  @example
				 *    $(document).ready( function() {
				 *      $('#example').dataTable( {
				 *        "language": {
				 *          "paginate": {
				 *            "last": "Last page"
				 *          }
				 *        }
				 *      } );
				 *    } );
				 */
				"sLast": "Last",
	
	
				/**
				 * Text to use for the 'next' pagination button (to take the user to the
				 * next page).
				 *  @type string
				 *  @default Next
				 *
				 *  @dtopt Language
				 *  @name DataTable.defaults.language.paginate.next
				 *
				 *  @example
				 *    $(document).ready( function() {
				 *      $('#example').dataTable( {
				 *        "language": {
				 *          "paginate": {
				 *            "next": "Next page"
				 *          }
				 *        }
				 *      } );
				 *    } );
				 */
				"sNext": "Next",
	
	
				/**
				 * Text to use for the 'previous' pagination button (to take the user to
				 * the previous page).
				 *  @type string
				 *  @default Previous
				 *
				 *  @dtopt Language
				 *  @name DataTable.defaults.language.paginate.previous
				 *
				 *  @example
				 *    $(document).ready( function() {
				 *      $('#example').dataTable( {
				 *        "language": {
				 *          "paginate": {
				 *            "previous": "Previous page"
				 *          }
				 *        }
				 *      } );
				 *    } );
				 */
				"sPrevious": "Previous"
			},
	
			/**
			 * This string is shown in preference to `zeroRecords` when the table is
			 * empty of data (regardless of filtering). Note that this is an optional
			 * parameter - if it is not given, the value of `zeroRecords` will be used
			 * instead (either the default or given value).
			 *  @type string
			 *  @default No data available in table
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.emptyTable
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "emptyTable": "No data available in table"
			 *        }
			 *      } );
			 *    } );
			 */
			"sEmptyTable": "No data available in table",
	
	
			/**
			 * This string gives information to the end user about the information
			 * that is current on display on the page. The following tokens can be
			 * used in the string and will be dynamically replaced as the table
			 * display updates. This tokens can be placed anywhere in the string, or
			 * removed as needed by the language requires:
			 *
			 * * `\_START\_` - Display index of the first record on the current page
			 * * `\_END\_` - Display index of the last record on the current page
			 * * `\_TOTAL\_` - Number of records in the table after filtering
			 * * `\_MAX\_` - Number of records in the table without filtering
			 * * `\_PAGE\_` - Current page number
			 * * `\_PAGES\_` - Total number of pages of data in the table
			 *
			 *  @type string
			 *  @default Showing _START_ to _END_ of _TOTAL_ entries
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.info
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "info": "Showing page _PAGE_ of _PAGES_"
			 *        }
			 *      } );
			 *    } );
			 */
			"sInfo": "Showing _START_ to _END_ of _TOTAL_ entries",
	
	
			/**
			 * Display information string for when the table is empty. Typically the
			 * format of this string should match `info`.
			 *  @type string
			 *  @default Showing 0 to 0 of 0 entries
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.infoEmpty
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "infoEmpty": "No entries to show"
			 *        }
			 *      } );
			 *    } );
			 */
			"sInfoEmpty": "Showing 0 to 0 of 0 entries",
	
	
			/**
			 * When a user filters the information in a table, this string is appended
			 * to the information (`info`) to give an idea of how strong the filtering
			 * is. The variable _MAX_ is dynamically updated.
			 *  @type string
			 *  @default (filtered from _MAX_ total entries)
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.infoFiltered
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "infoFiltered": " - filtering from _MAX_ records"
			 *        }
			 *      } );
			 *    } );
			 */
			"sInfoFiltered": "(filtered from _MAX_ total entries)",
	
	
			/**
			 * If can be useful to append extra information to the info string at times,
			 * and this variable does exactly that. This information will be appended to
			 * the `info` (`infoEmpty` and `infoFiltered` in whatever combination they are
			 * being used) at all times.
			 *  @type string
			 *  @default <i>Empty string</i>
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.infoPostFix
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "infoPostFix": "All records shown are derived from real information."
			 *        }
			 *      } );
			 *    } );
			 */
			"sInfoPostFix": "",
	
	
			/**
			 * This decimal place operator is a little different from the other
			 * language options since DataTables doesn't output floating point
			 * numbers, so it won't ever use this for display of a number. Rather,
			 * what this parameter does is modify the sort methods of the table so
			 * that numbers which are in a format which has a character other than
			 * a period (`.`) as a decimal place will be sorted numerically.
			 *
			 * Note that numbers with different decimal places cannot be shown in
			 * the same table and still be sortable, the table must be consistent.
			 * However, multiple different tables on the page can use different
			 * decimal place characters.
			 *  @type string
			 *  @default 
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.decimal
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "decimal": ","
			 *          "thousands": "."
			 *        }
			 *      } );
			 *    } );
			 */
			"sDecimal": "",
	
	
			/**
			 * DataTables has a build in number formatter (`formatNumber`) which is
			 * used to format large numbers that are used in the table information.
			 * By default a comma is used, but this can be trivially changed to any
			 * character you wish with this parameter.
			 *  @type string
			 *  @default ,
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.thousands
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "thousands": "'"
			 *        }
			 *      } );
			 *    } );
			 */
			"sThousands": ",",
	
	
			/**
			 * Detail the action that will be taken when the drop down menu for the
			 * pagination length option is changed. The '_MENU_' variable is replaced
			 * with a default select list of 10, 25, 50 and 100, and can be replaced
			 * with a custom select box if required.
			 *  @type string
			 *  @default Show _MENU_ entries
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.lengthMenu
			 *
			 *  @example
			 *    // Language change only
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "lengthMenu": "Display _MENU_ records"
			 *        }
			 *      } );
			 *    } );
			 *
			 *  @example
			 *    // Language and options change
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "lengthMenu": 'Display <select>'+
			 *            '<option value="10">10</option>'+
			 *            '<option value="20">20</option>'+
			 *            '<option value="30">30</option>'+
			 *            '<option value="40">40</option>'+
			 *            '<option value="50">50</option>'+
			 *            '<option value="-1">All</option>'+
			 *            '</select> records'
			 *        }
			 *      } );
			 *    } );
			 */
			"sLengthMenu": "Show _MENU_ entries",
	
	
			/**
			 * When using Ajax sourced data and during the first draw when DataTables is
			 * gathering the data, this message is shown in an empty row in the table to
			 * indicate to the end user the the data is being loaded. Note that this
			 * parameter is not used when loading data by server-side processing, just
			 * Ajax sourced data with client-side processing.
			 *  @type string
			 *  @default Loading...
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.loadingRecords
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "loadingRecords": "Please wait - loading..."
			 *        }
			 *      } );
			 *    } );
			 */
			"sLoadingRecords": "Loading...",
	
	
			/**
			 * Text which is displayed when the table is processing a user action
			 * (usually a sort command or similar).
			 *  @type string
			 *  @default Processing...
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.processing
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "processing": "DataTables is currently busy"
			 *        }
			 *      } );
			 *    } );
			 */
			"sProcessing": "Processing...",
	
	
			/**
			 * Details the actions that will be taken when the user types into the
			 * filtering input text box. The variable "_INPUT_", if used in the string,
			 * is replaced with the HTML text box for the filtering input allowing
			 * control over where it appears in the string. If "_INPUT_" is not given
			 * then the input box is appended to the string automatically.
			 *  @type string
			 *  @default Search:
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.search
			 *
			 *  @example
			 *    // Input text box will be appended at the end automatically
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "search": "Filter records:"
			 *        }
			 *      } );
			 *    } );
			 *
			 *  @example
			 *    // Specify where the filter should appear
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "search": "Apply filter _INPUT_ to table"
			 *        }
			 *      } );
			 *    } );
			 */
			"sSearch": "Search:",
	
	
			/**
			 * Assign a `placeholder` attribute to the search `input` element
			 *  @type string
			 *  @default 
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.searchPlaceholder
			 */
			"sSearchPlaceholder": "",
	
	
			/**
			 * All of the language information can be stored in a file on the
			 * server-side, which DataTables will look up if this parameter is passed.
			 * It must store the URL of the language file, which is in a JSON format,
			 * and the object has the same properties as the oLanguage object in the
			 * initialiser object (i.e. the above parameters). Please refer to one of
			 * the example language files to see how this works in action.
			 *  @type string
			 *  @default <i>Empty string - i.e. disabled</i>
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.url
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "url": "http://www.sprymedia.co.uk/dataTables/lang.txt"
			 *        }
			 *      } );
			 *    } );
			 */
			"sUrl": "",
	
	
			/**
			 * Text shown inside the table records when the is no information to be
			 * displayed after filtering. `emptyTable` is shown when there is simply no
			 * information in the table at all (regardless of filtering).
			 *  @type string
			 *  @default No matching records found
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.zeroRecords
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "zeroRecords": "No records to display"
			 *        }
			 *      } );
			 *    } );
			 */
			"sZeroRecords": "No matching records found"
		},
	
	
		/**
		 * This parameter allows you to have define the global filtering state at
		 * initialisation time. As an object the `search` parameter must be
		 * defined, but all other parameters are optional. When `regex` is true,
		 * the search string will be treated as a regular expression, when false
		 * (default) it will be treated as a straight string. When `smart`
		 * DataTables will use it's smart filtering methods (to word match at
		 * any point in the data), when false this will not be done.
		 *  @namespace
		 *  @extends DataTable.models.oSearch
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.search
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "search": {"search": "Initial search"}
		 *      } );
		 *    } )
		 */
		"oSearch": $.extend( {}, DataTable.models.oSearch ),
	
	
		/**
		 * __Deprecated__ The functionality provided by this parameter has now been
		 * superseded by that provided through `ajax`, which should be used instead.
		 *
		 * By default DataTables will look for the property `data` (or `aaData` for
		 * compatibility with DataTables 1.9-) when obtaining data from an Ajax
		 * source or for server-side processing - this parameter allows that
		 * property to be changed. You can use Javascript dotted object notation to
		 * get a data source for multiple levels of nesting.
		 *  @type string
		 *  @default data
		 *
		 *  @dtopt Options
		 *  @dtopt Server-side
		 *  @name DataTable.defaults.ajaxDataProp
		 *
		 *  @deprecated 1.10. Please use `ajax` for this functionality now.
		 */
		"sAjaxDataProp": "data",
	
	
		/**
		 * __Deprecated__ The functionality provided by this parameter has now been
		 * superseded by that provided through `ajax`, which should be used instead.
		 *
		 * You can instruct DataTables to load data from an external
		 * source using this parameter (use aData if you want to pass data in you
		 * already have). Simply provide a url a JSON object can be obtained from.
		 *  @type string
		 *  @default null
		 *
		 *  @dtopt Options
		 *  @dtopt Server-side
		 *  @name DataTable.defaults.ajaxSource
		 *
		 *  @deprecated 1.10. Please use `ajax` for this functionality now.
		 */
		"sAjaxSource": null,
	
	
		/**
		 * This initialisation variable allows you to specify exactly where in the
		 * DOM you want DataTables to inject the various controls it adds to the page
		 * (for example you might want the pagination controls at the top of the
		 * table). DIV elements (with or without a custom class) can also be added to
		 * aid styling. The follow syntax is used:
		 *   <ul>
		 *     <li>The following options are allowed:
		 *       <ul>
		 *         <li>'l' - Length changing</li>
		 *         <li>'f' - Filtering input</li>
		 *         <li>'t' - The table!</li>
		 *         <li>'i' - Information</li>
		 *         <li>'p' - Pagination</li>
		 *         <li>'r' - pRocessing</li>
		 *       </ul>
		 *     </li>
		 *     <li>The following constants are allowed:
		 *       <ul>
		 *         <li>'H' - jQueryUI theme "header" classes ('fg-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix')</li>
		 *         <li>'F' - jQueryUI theme "footer" classes ('fg-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix')</li>
		 *       </ul>
		 *     </li>
		 *     <li>The following syntax is expected:
		 *       <ul>
		 *         <li>'&lt;' and '&gt;' - div elements</li>
		 *         <li>'&lt;"class" and '&gt;' - div with a class</li>
		 *         <li>'&lt;"#id" and '&gt;' - div with an ID</li>
		 *       </ul>
		 *     </li>
		 *     <li>Examples:
		 *       <ul>
		 *         <li>'&lt;"wrapper"flipt&gt;'</li>
		 *         <li>'&lt;lf&lt;t&gt;ip&gt;'</li>
		 *       </ul>
		 *     </li>
		 *   </ul>
		 *  @type string
		 *  @default lfrtip <i>(when `jQueryUI` is false)</i> <b>or</b>
		 *    <"H"lfr>t<"F"ip> <i>(when `jQueryUI` is true)</i>
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.dom
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "dom": '&lt;"top"i&gt;rt&lt;"bottom"flp&gt;&lt;"clear"&gt;'
		 *      } );
		 *    } );
		 */
		"sDom": "lfrtip",
	
	
		/**
		 * Search delay option. This will throttle full table searches that use the
		 * DataTables provided search input element (it does not effect calls to
		 * `dt-api search()`, providing a delay before the search is made.
		 *  @type integer
		 *  @default 0
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.searchDelay
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "searchDelay": 200
		 *      } );
		 *    } )
		 */
		"searchDelay": null,
	
	
		/**
		 * DataTables features six different built-in options for the buttons to
		 * display for pagination control:
		 *
		 * * `numbers` - Page number buttons only
		 * * `simple` - 'Previous' and 'Next' buttons only
		 * * 'simple_numbers` - 'Previous' and 'Next' buttons, plus page numbers
		 * * `full` - 'First', 'Previous', 'Next' and 'Last' buttons
		 * * `full_numbers` - 'First', 'Previous', 'Next' and 'Last' buttons, plus page numbers
		 * * `first_last_numbers` - 'First' and 'Last' buttons, plus page numbers
		 *  
		 * Further methods can be added using {@link DataTable.ext.oPagination}.
		 *  @type string
		 *  @default simple_numbers
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.pagingType
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "pagingType": "full_numbers"
		 *      } );
		 *    } )
		 */
		"sPaginationType": "simple_numbers",
	
	
		/**
		 * Enable horizontal scrolling. When a table is too wide to fit into a
		 * certain layout, or you have a large number of columns in the table, you
		 * can enable x-scrolling to show the table in a viewport, which can be
		 * scrolled. This property can be `true` which will allow the table to
		 * scroll horizontally when needed, or any CSS unit, or a number (in which
		 * case it will be treated as a pixel measurement). Setting as simply `true`
		 * is recommended.
		 *  @type boolean|string
		 *  @default <i>blank string - i.e. disabled</i>
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.scrollX
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "scrollX": true,
		 *        "scrollCollapse": true
		 *      } );
		 *    } );
		 */
		"sScrollX": "",
	
	
		/**
		 * This property can be used to force a DataTable to use more width than it
		 * might otherwise do when x-scrolling is enabled. For example if you have a
		 * table which requires to be well spaced, this parameter is useful for
		 * "over-sizing" the table, and thus forcing scrolling. This property can by
		 * any CSS unit, or a number (in which case it will be treated as a pixel
		 * measurement).
		 *  @type string
		 *  @default <i>blank string - i.e. disabled</i>
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.scrollXInner
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "scrollX": "100%",
		 *        "scrollXInner": "110%"
		 *      } );
		 *    } );
		 */
		"sScrollXInner": "",
	
	
		/**
		 * Enable vertical scrolling. Vertical scrolling will constrain the DataTable
		 * to the given height, and enable scrolling for any data which overflows the
		 * current viewport. This can be used as an alternative to paging to display
		 * a lot of data in a small area (although paging and scrolling can both be
		 * enabled at the same time). This property can be any CSS unit, or a number
		 * (in which case it will be treated as a pixel measurement).
		 *  @type string
		 *  @default <i>blank string - i.e. disabled</i>
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.scrollY
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "scrollY": "200px",
		 *        "paginate": false
		 *      } );
		 *    } );
		 */
		"sScrollY": "",
	
	
		/**
		 * __Deprecated__ The functionality provided by this parameter has now been
		 * superseded by that provided through `ajax`, which should be used instead.
		 *
		 * Set the HTTP method that is used to make the Ajax call for server-side
		 * processing or Ajax sourced data.
		 *  @type string
		 *  @default GET
		 *
		 *  @dtopt Options
		 *  @dtopt Server-side
		 *  @name DataTable.defaults.serverMethod
		 *
		 *  @deprecated 1.10. Please use `ajax` for this functionality now.
		 */
		"sServerMethod": "GET",
	
	
		/**
		 * DataTables makes use of renderers when displaying HTML elements for
		 * a table. These renderers can be added or modified by plug-ins to
		 * generate suitable mark-up for a site. For example the Bootstrap
		 * integration plug-in for DataTables uses a paging button renderer to
		 * display pagination buttons in the mark-up required by Bootstrap.
		 *
		 * For further information about the renderers available see
		 * DataTable.ext.renderer
		 *  @type string|object
		 *  @default null
		 *
		 *  @name DataTable.defaults.renderer
		 *
		 */
		"renderer": null,
	
	
		/**
		 * Set the data property name that DataTables should use to get a row's id
		 * to set as the `id` property in the node.
		 *  @type string
		 *  @default DT_RowId
		 *
		 *  @name DataTable.defaults.rowId
		 */
		"rowId": "DT_RowId"
	};
	
	_fnHungarianMap( DataTable.defaults );
	
	
	
	/*
	 * Developer note - See note in model.defaults.js about the use of Hungarian
	 * notation and camel case.
	 */
	
	/**
	 * Column options that can be given to DataTables at initialisation time.
	 *  @namespace
	 */
	DataTable.defaults.column = {
		/**
		 * Define which column(s) an order will occur on for this column. This
		 * allows a column's ordering to take multiple columns into account when
		 * doing a sort or use the data from a different column. For example first
		 * name / last name columns make sense to do a multi-column sort over the
		 * two columns.
		 *  @type array|int
		 *  @default null <i>Takes the value of the column index automatically</i>
		 *
		 *  @name DataTable.defaults.column.orderData
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "orderData": [ 0, 1 ], "targets": [ 0 ] },
		 *          { "orderData": [ 1, 0 ], "targets": [ 1 ] },
		 *          { "orderData": 2, "targets": [ 2 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "orderData": [ 0, 1 ] },
		 *          { "orderData": [ 1, 0 ] },
		 *          { "orderData": 2 },
		 *          null,
		 *          null
		 *        ]
		 *      } );
		 *    } );
		 */
		"aDataSort": null,
		"iDataSort": -1,
	
	
		/**
		 * You can control the default ordering direction, and even alter the
		 * behaviour of the sort handler (i.e. only allow ascending ordering etc)
		 * using this parameter.
		 *  @type array
		 *  @default [ 'asc', 'desc' ]
		 *
		 *  @name DataTable.defaults.column.orderSequence
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "orderSequence": [ "asc" ], "targets": [ 1 ] },
		 *          { "orderSequence": [ "desc", "asc", "asc" ], "targets": [ 2 ] },
		 *          { "orderSequence": [ "desc" ], "targets": [ 3 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          null,
		 *          { "orderSequence": [ "asc" ] },
		 *          { "orderSequence": [ "desc", "asc", "asc" ] },
		 *          { "orderSequence": [ "desc" ] },
		 *          null
		 *        ]
		 *      } );
		 *    } );
		 */
		"asSorting": [ 'asc', 'desc' ],
	
	
		/**
		 * Enable or disable filtering on the data in this column.
		 *  @type boolean
		 *  @default true
		 *
		 *  @name DataTable.defaults.column.searchable
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "searchable": false, "targets": [ 0 ] }
		 *        ] } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "searchable": false },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ] } );
		 *    } );
		 */
		"bSearchable": true,
	
	
		/**
		 * Enable or disable ordering on this column.
		 *  @type boolean
		 *  @default true
		 *
		 *  @name DataTable.defaults.column.orderable
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "orderable": false, "targets": [ 0 ] }
		 *        ] } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "orderable": false },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ] } );
		 *    } );
		 */
		"bSortable": true,
	
	
		/**
		 * Enable or disable the display of this column.
		 *  @type boolean
		 *  @default true
		 *
		 *  @name DataTable.defaults.column.visible
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "visible": false, "targets": [ 0 ] }
		 *        ] } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "visible": false },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ] } );
		 *    } );
		 */
		"bVisible": true,
	
	
		/**
		 * Developer definable function that is called whenever a cell is created (Ajax source,
		 * etc) or processed for input (DOM source). This can be used as a compliment to mRender
		 * allowing you to modify the DOM element (add background colour for example) when the
		 * element is available.
		 *  @type function
		 *  @param {element} td The TD node that has been created
		 *  @param {*} cellData The Data for the cell
		 *  @param {array|object} rowData The data for the whole row
		 *  @param {int} row The row index for the aoData data store
		 *  @param {int} col The column index for aoColumns
		 *
		 *  @name DataTable.defaults.column.createdCell
		 *  @dtopt Columns
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [3],
		 *          "createdCell": function (td, cellData, rowData, row, col) {
		 *            if ( cellData == "1.7" ) {
		 *              $(td).css('color', 'blue')
		 *            }
		 *          }
		 *        } ]
		 *      });
		 *    } );
		 */
		"fnCreatedCell": null,
	
	
		/**
		 * This parameter has been replaced by `data` in DataTables to ensure naming
		 * consistency. `dataProp` can still be used, as there is backwards
		 * compatibility in DataTables for this option, but it is strongly
		 * recommended that you use `data` in preference to `dataProp`.
		 *  @name DataTable.defaults.column.dataProp
		 */
	
	
		/**
		 * This property can be used to read data from any data source property,
		 * including deeply nested objects / properties. `data` can be given in a
		 * number of different ways which effect its behaviour:
		 *
		 * * `integer` - treated as an array index for the data source. This is the
		 *   default that DataTables uses (incrementally increased for each column).
		 * * `string` - read an object property from the data source. There are
		 *   three 'special' options that can be used in the string to alter how
		 *   DataTables reads the data from the source object:
		 *    * `.` - Dotted Javascript notation. Just as you use a `.` in
		 *      Javascript to read from nested objects, so to can the options
		 *      specified in `data`. For example: `browser.version` or
		 *      `browser.name`. If your object parameter name contains a period, use
		 *      `\\` to escape it - i.e. `first\\.name`.
		 *    * `[]` - Array notation. DataTables can automatically combine data
		 *      from and array source, joining the data with the characters provided
		 *      between the two brackets. For example: `name[, ]` would provide a
		 *      comma-space separated list from the source array. If no characters
		 *      are provided between the brackets, the original array source is
		 *      returned.
		 *    * `()` - Function notation. Adding `()` to the end of a parameter will
		 *      execute a function of the name given. For example: `browser()` for a
		 *      simple function on the data source, `browser.version()` for a
		 *      function in a nested property or even `browser().version` to get an
		 *      object property if the function called returns an object. Note that
		 *      function notation is recommended for use in `render` rather than
		 *      `data` as it is much simpler to use as a renderer.
		 * * `null` - use the original data source for the row rather than plucking
		 *   data directly from it. This action has effects on two other
		 *   initialisation options:
		 *    * `defaultContent` - When null is given as the `data` option and
		 *      `defaultContent` is specified for the column, the value defined by
		 *      `defaultContent` will be used for the cell.
		 *    * `render` - When null is used for the `data` option and the `render`
		 *      option is specified for the column, the whole data source for the
		 *      row is used for the renderer.
		 * * `function` - the function given will be executed whenever DataTables
		 *   needs to set or get the data for a cell in the column. The function
		 *   takes three parameters:
		 *    * Parameters:
		 *      * `{array|object}` The data source for the row
		 *      * `{string}` The type call data requested - this will be 'set' when
		 *        setting data or 'filter', 'display', 'type', 'sort' or undefined
		 *        when gathering data. Note that when `undefined` is given for the
		 *        type DataTables expects to get the raw data for the object back<
		 *      * `{*}` Data to set when the second parameter is 'set'.
		 *    * Return:
		 *      * The return value from the function is not required when 'set' is
		 *        the type of call, but otherwise the return is what will be used
		 *        for the data requested.
		 *
		 * Note that `data` is a getter and setter option. If you just require
		 * formatting of data for output, you will likely want to use `render` which
		 * is simply a getter and thus simpler to use.
		 *
		 * Note that prior to DataTables 1.9.2 `data` was called `mDataProp`. The
		 * name change reflects the flexibility of this property and is consistent
		 * with the naming of mRender. If 'mDataProp' is given, then it will still
		 * be used by DataTables, as it automatically maps the old name to the new
		 * if required.
		 *
		 *  @type string|int|function|null
		 *  @default null <i>Use automatically calculated column index</i>
		 *
		 *  @name DataTable.defaults.column.data
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Read table data from objects
		 *    // JSON structure for each row:
		 *    //   {
		 *    //      "engine": {value},
		 *    //      "browser": {value},
		 *    //      "platform": {value},
		 *    //      "version": {value},
		 *    //      "grade": {value}
		 *    //   }
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "ajaxSource": "sources/objects.txt",
		 *        "columns": [
		 *          { "data": "engine" },
		 *          { "data": "browser" },
		 *          { "data": "platform" },
		 *          { "data": "version" },
		 *          { "data": "grade" }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Read information from deeply nested objects
		 *    // JSON structure for each row:
		 *    //   {
		 *    //      "engine": {value},
		 *    //      "browser": {value},
		 *    //      "platform": {
		 *    //         "inner": {value}
		 *    //      },
		 *    //      "details": [
		 *    //         {value}, {value}
		 *    //      ]
		 *    //   }
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "ajaxSource": "sources/deep.txt",
		 *        "columns": [
		 *          { "data": "engine" },
		 *          { "data": "browser" },
		 *          { "data": "platform.inner" },
		 *          { "data": "details.0" },
		 *          { "data": "details.1" }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `data` as a function to provide different information for
		 *    // sorting, filtering and display. In this case, currency (price)
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "data": function ( source, type, val ) {
		 *            if (type === 'set') {
		 *              source.price = val;
		 *              // Store the computed dislay and filter values for efficiency
		 *              source.price_display = val=="" ? "" : "$"+numberFormat(val);
		 *              source.price_filter  = val=="" ? "" : "$"+numberFormat(val)+" "+val;
		 *              return;
		 *            }
		 *            else if (type === 'display') {
		 *              return source.price_display;
		 *            }
		 *            else if (type === 'filter') {
		 *              return source.price_filter;
		 *            }
		 *            // 'sort', 'type' and undefined all just use the integer
		 *            return source.price;
		 *          }
		 *        } ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using default content
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "data": null,
		 *          "defaultContent": "Click to edit"
		 *        } ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using array notation - outputting a list from an array
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "data": "name[, ]"
		 *        } ]
		 *      } );
		 *    } );
		 *
		 */
		"mData": null,
	
	
		/**
		 * This property is the rendering partner to `data` and it is suggested that
		 * when you want to manipulate data for display (including filtering,
		 * sorting etc) without altering the underlying data for the table, use this
		 * property. `render` can be considered to be the the read only companion to
		 * `data` which is read / write (then as such more complex). Like `data`
		 * this option can be given in a number of different ways to effect its
		 * behaviour:
		 *
		 * * `integer` - treated as an array index for the data source. This is the
		 *   default that DataTables uses (incrementally increased for each column).
		 * * `string` - read an object property from the data source. There are
		 *   three 'special' options that can be used in the string to alter how
		 *   DataTables reads the data from the source object:
		 *    * `.` - Dotted Javascript notation. Just as you use a `.` in
		 *      Javascript to read from nested objects, so to can the options
		 *      specified in `data`. For example: `browser.version` or
		 *      `browser.name`. If your object parameter name contains a period, use
		 *      `\\` to escape it - i.e. `first\\.name`.
		 *    * `[]` - Array notation. DataTables can automatically combine data
		 *      from and array source, joining the data with the characters provided
		 *      between the two brackets. For example: `name[, ]` would provide a
		 *      comma-space separated list from the source array. If no characters
		 *      are provided between the brackets, the original array source is
		 *      returned.
		 *    * `()` - Function notation. Adding `()` to the end of a parameter will
		 *      execute a function of the name given. For example: `browser()` for a
		 *      simple function on the data source, `browser.version()` for a
		 *      function in a nested property or even `browser().version` to get an
		 *      object property if the function called returns an object.
		 * * `object` - use different data for the different data types requested by
		 *   DataTables ('filter', 'display', 'type' or 'sort'). The property names
		 *   of the object is the data type the property refers to and the value can
		 *   defined using an integer, string or function using the same rules as
		 *   `render` normally does. Note that an `_` option _must_ be specified.
		 *   This is the default value to use if you haven't specified a value for
		 *   the data type requested by DataTables.
		 * * `function` - the function given will be executed whenever DataTables
		 *   needs to set or get the data for a cell in the column. The function
		 *   takes three parameters:
		 *    * Parameters:
		 *      * {array|object} The data source for the row (based on `data`)
		 *      * {string} The type call data requested - this will be 'filter',
		 *        'display', 'type' or 'sort'.
		 *      * {array|object} The full data source for the row (not based on
		 *        `data`)
		 *    * Return:
		 *      * The return value from the function is what will be used for the
		 *        data requested.
		 *
		 *  @type string|int|function|object|null
		 *  @default null Use the data source value.
		 *
		 *  @name DataTable.defaults.column.render
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Create a comma separated list from an array of objects
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "ajaxSource": "sources/deep.txt",
		 *        "columns": [
		 *          { "data": "engine" },
		 *          { "data": "browser" },
		 *          {
		 *            "data": "platform",
		 *            "render": "[, ].name"
		 *          }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Execute a function to obtain data
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "data": null, // Use the full data source object for the renderer's source
		 *          "render": "browserName()"
		 *        } ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // As an object, extracting different data for the different types
		 *    // This would be used with a data source such as:
		 *    //   { "phone": 5552368, "phone_filter": "5552368 555-2368", "phone_display": "555-2368" }
		 *    // Here the `phone` integer is used for sorting and type detection, while `phone_filter`
		 *    // (which has both forms) is used for filtering for if a user inputs either format, while
		 *    // the formatted phone number is the one that is shown in the table.
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "data": null, // Use the full data source object for the renderer's source
		 *          "render": {
		 *            "_": "phone",
		 *            "filter": "phone_filter",
		 *            "display": "phone_display"
		 *          }
		 *        } ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Use as a function to create a link from the data source
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "data": "download_link",
		 *          "render": function ( data, type, full ) {
		 *            return '<a href="'+data+'">Download</a>';
		 *          }
		 *        } ]
		 *      } );
		 *    } );
		 */
		"mRender": null,
	
	
		/**
		 * Change the cell type created for the column - either TD cells or TH cells. This
		 * can be useful as TH cells have semantic meaning in the table body, allowing them
		 * to act as a header for a row (you may wish to add scope='row' to the TH elements).
		 *  @type string
		 *  @default td
		 *
		 *  @name DataTable.defaults.column.cellType
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Make the first column use TH cells
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "cellType": "th"
		 *        } ]
		 *      } );
		 *    } );
		 */
		"sCellType": "td",
	
	
		/**
		 * Class to give to each cell in this column.
		 *  @type string
		 *  @default <i>Empty string</i>
		 *
		 *  @name DataTable.defaults.column.class
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "class": "my_class", "targets": [ 0 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "class": "my_class" },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ]
		 *      } );
		 *    } );
		 */
		"sClass": "",
	
		/**
		 * When DataTables calculates the column widths to assign to each column,
		 * it finds the longest string in each column and then constructs a
		 * temporary table and reads the widths from that. The problem with this
		 * is that "mmm" is much wider then "iiii", but the latter is a longer
		 * string - thus the calculation can go wrong (doing it properly and putting
		 * it into an DOM object and measuring that is horribly(!) slow). Thus as
		 * a "work around" we provide this option. It will append its value to the
		 * text that is found to be the longest string for the column - i.e. padding.
		 * Generally you shouldn't need this!
		 *  @type string
		 *  @default <i>Empty string<i>
		 *
		 *  @name DataTable.defaults.column.contentPadding
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          null,
		 *          null,
		 *          null,
		 *          {
		 *            "contentPadding": "mmm"
		 *          }
		 *        ]
		 *      } );
		 *    } );
		 */
		"sContentPadding": "",
	
	
		/**
		 * Allows a default value to be given for a column's data, and will be used
		 * whenever a null data source is encountered (this can be because `data`
		 * is set to null, or because the data source itself is null).
		 *  @type string
		 *  @default null
		 *
		 *  @name DataTable.defaults.column.defaultContent
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          {
		 *            "data": null,
		 *            "defaultContent": "Edit",
		 *            "targets": [ -1 ]
		 *          }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          null,
		 *          null,
		 *          null,
		 *          {
		 *            "data": null,
		 *            "defaultContent": "Edit"
		 *          }
		 *        ]
		 *      } );
		 *    } );
		 */
		"sDefaultContent": null,
	
	
		/**
		 * This parameter is only used in DataTables' server-side processing. It can
		 * be exceptionally useful to know what columns are being displayed on the
		 * client side, and to map these to database fields. When defined, the names
		 * also allow DataTables to reorder information from the server if it comes
		 * back in an unexpected order (i.e. if you switch your columns around on the
		 * client-side, your server-side code does not also need updating).
		 *  @type string
		 *  @default <i>Empty string</i>
		 *
		 *  @name DataTable.defaults.column.name
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "name": "engine", "targets": [ 0 ] },
		 *          { "name": "browser", "targets": [ 1 ] },
		 *          { "name": "platform", "targets": [ 2 ] },
		 *          { "name": "version", "targets": [ 3 ] },
		 *          { "name": "grade", "targets": [ 4 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "name": "engine" },
		 *          { "name": "browser" },
		 *          { "name": "platform" },
		 *          { "name": "version" },
		 *          { "name": "grade" }
		 *        ]
		 *      } );
		 *    } );
		 */
		"sName": "",
	
	
		/**
		 * Defines a data source type for the ordering which can be used to read
		 * real-time information from the table (updating the internally cached
		 * version) prior to ordering. This allows ordering to occur on user
		 * editable elements such as form inputs.
		 *  @type string
		 *  @default std
		 *
		 *  @name DataTable.defaults.column.orderDataType
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "orderDataType": "dom-text", "targets": [ 2, 3 ] },
		 *          { "type": "numeric", "targets": [ 3 ] },
		 *          { "orderDataType": "dom-select", "targets": [ 4 ] },
		 *          { "orderDataType": "dom-checkbox", "targets": [ 5 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          null,
		 *          null,
		 *          { "orderDataType": "dom-text" },
		 *          { "orderDataType": "dom-text", "type": "numeric" },
		 *          { "orderDataType": "dom-select" },
		 *          { "orderDataType": "dom-checkbox" }
		 *        ]
		 *      } );
		 *    } );
		 */
		"sSortDataType": "std",
	
	
		/**
		 * The title of this column.
		 *  @type string
		 *  @default null <i>Derived from the 'TH' value for this column in the
		 *    original HTML table.</i>
		 *
		 *  @name DataTable.defaults.column.title
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "title": "My column title", "targets": [ 0 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "title": "My column title" },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ]
		 *      } );
		 *    } );
		 */
		"sTitle": null,
	
	
		/**
		 * The type allows you to specify how the data for this column will be
		 * ordered. Four types (string, numeric, date and html (which will strip
		 * HTML tags before ordering)) are currently available. Note that only date
		 * formats understood by Javascript's Date() object will be accepted as type
		 * date. For example: "Mar 26, 2008 5:03 PM". May take the values: 'string',
		 * 'numeric', 'date' or 'html' (by default). Further types can be adding
		 * through plug-ins.
		 *  @type string
		 *  @default null <i>Auto-detected from raw data</i>
		 *
		 *  @name DataTable.defaults.column.type
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "type": "html", "targets": [ 0 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "type": "html" },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ]
		 *      } );
		 *    } );
		 */
		"sType": null,
	
	
		/**
		 * Defining the width of the column, this parameter may take any CSS value
		 * (3em, 20px etc). DataTables applies 'smart' widths to columns which have not
		 * been given a specific width through this interface ensuring that the table
		 * remains readable.
		 *  @type string
		 *  @default null <i>Automatic</i>
		 *
		 *  @name DataTable.defaults.column.width
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "width": "20%", "targets": [ 0 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "width": "20%" },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ]
		 *      } );
		 *    } );
		 */
		"sWidth": null
	};
	
	_fnHungarianMap( DataTable.defaults.column );
	
	
	
	/**
	 * DataTables settings object - this holds all the information needed for a
	 * given table, including configuration, data and current application of the
	 * table options. DataTables does not have a single instance for each DataTable
	 * with the settings attached to that instance, but rather instances of the
	 * DataTable "class" are created on-the-fly as needed (typically by a
	 * $().dataTable() call) and the settings object is then applied to that
	 * instance.
	 *
	 * Note that this object is related to {@link DataTable.defaults} but this
	 * one is the internal data store for DataTables's cache of columns. It should
	 * NOT be manipulated outside of DataTables. Any configuration should be done
	 * through the initialisation options.
	 *  @namespace
	 *  @todo Really should attach the settings object to individual instances so we
	 *    don't need to create new instances on each $().dataTable() call (if the
	 *    table already exists). It would also save passing oSettings around and
	 *    into every single function. However, this is a very significant
	 *    architecture change for DataTables and will almost certainly break
	 *    backwards compatibility with older installations. This is something that
	 *    will be done in 2.0.
	 */
	DataTable.models.oSettings = {
		/**
		 * Primary features of DataTables and their enablement state.
		 *  @namespace
		 */
		"oFeatures": {
	
			/**
			 * Flag to say if DataTables should automatically try to calculate the
			 * optimum table and columns widths (true) or not (false).
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bAutoWidth": null,
	
			/**
			 * Delay the creation of TR and TD elements until they are actually
			 * needed by a driven page draw. This can give a significant speed
			 * increase for Ajax source and Javascript source data, but makes no
			 * difference at all fro DOM and server-side processing tables.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bDeferRender": null,
	
			/**
			 * Enable filtering on the table or not. Note that if this is disabled
			 * then there is no filtering at all on the table, including fnFilter.
			 * To just remove the filtering input use sDom and remove the 'f' option.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bFilter": null,
	
			/**
			 * Table information element (the 'Showing x of y records' div) enable
			 * flag.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bInfo": null,
	
			/**
			 * Present a user control allowing the end user to change the page size
			 * when pagination is enabled.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bLengthChange": null,
	
			/**
			 * Pagination enabled or not. Note that if this is disabled then length
			 * changing must also be disabled.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bPaginate": null,
	
			/**
			 * Processing indicator enable flag whenever DataTables is enacting a
			 * user request - typically an Ajax request for server-side processing.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bProcessing": null,
	
			/**
			 * Server-side processing enabled flag - when enabled DataTables will
			 * get all data from the server for every draw - there is no filtering,
			 * sorting or paging done on the client-side.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bServerSide": null,
	
			/**
			 * Sorting enablement flag.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bSort": null,
	
			/**
			 * Multi-column sorting
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bSortMulti": null,
	
			/**
			 * Apply a class to the columns which are being sorted to provide a
			 * visual highlight or not. This can slow things down when enabled since
			 * there is a lot of DOM interaction.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bSortClasses": null,
	
			/**
			 * State saving enablement flag.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bStateSave": null
		},
	
	
		/**
		 * Scrolling settings for a table.
		 *  @namespace
		 */
		"oScroll": {
			/**
			 * When the table is shorter in height than sScrollY, collapse the
			 * table container down to the height of the table (when true).
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bCollapse": null,
	
			/**
			 * Width of the scrollbar for the web-browser's platform. Calculated
			 * during table initialisation.
			 *  @type int
			 *  @default 0
			 */
			"iBarWidth": 0,
	
			/**
			 * Viewport width for horizontal scrolling. Horizontal scrolling is
			 * disabled if an empty string.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type string
			 */
			"sX": null,
	
			/**
			 * Width to expand the table to when using x-scrolling. Typically you
			 * should not need to use this.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type string
			 *  @deprecated
			 */
			"sXInner": null,
	
			/**
			 * Viewport height for vertical scrolling. Vertical scrolling is disabled
			 * if an empty string.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type string
			 */
			"sY": null
		},
	
		/**
		 * Language information for the table.
		 *  @namespace
		 *  @extends DataTable.defaults.oLanguage
		 */
		"oLanguage": {
			/**
			 * Information callback function. See
			 * {@link DataTable.defaults.fnInfoCallback}
			 *  @type function
			 *  @default null
			 */
			"fnInfoCallback": null
		},
	
		/**
		 * Browser support parameters
		 *  @namespace
		 */
		"oBrowser": {
			/**
			 * Indicate if the browser incorrectly calculates width:100% inside a
			 * scrolling element (IE6/7)
			 *  @type boolean
			 *  @default false
			 */
			"bScrollOversize": false,
	
			/**
			 * Determine if the vertical scrollbar is on the right or left of the
			 * scrolling container - needed for rtl language layout, although not
			 * all browsers move the scrollbar (Safari).
			 *  @type boolean
			 *  @default false
			 */
			"bScrollbarLeft": false,
	
			/**
			 * Flag for if `getBoundingClientRect` is fully supported or not
			 *  @type boolean
			 *  @default false
			 */
			"bBounding": false,
	
			/**
			 * Browser scrollbar width
			 *  @type integer
			 *  @default 0
			 */
			"barWidth": 0
		},
	
	
		"ajax": null,
	
	
		/**
		 * Array referencing the nodes which are used for the features. The
		 * parameters of this object match what is allowed by sDom - i.e.
		 *   <ul>
		 *     <li>'l' - Length changing</li>
		 *     <li>'f' - Filtering input</li>
		 *     <li>'t' - The table!</li>
		 *     <li>'i' - Information</li>
		 *     <li>'p' - Pagination</li>
		 *     <li>'r' - pRocessing</li>
		 *   </ul>
		 *  @type array
		 *  @default []
		 */
		"aanFeatures": [],
	
		/**
		 * Store data information - see {@link DataTable.models.oRow} for detailed
		 * information.
		 *  @type array
		 *  @default []
		 */
		"aoData": [],
	
		/**
		 * Array of indexes which are in the current display (after filtering etc)
		 *  @type array
		 *  @default []
		 */
		"aiDisplay": [],
	
		/**
		 * Array of indexes for display - no filtering
		 *  @type array
		 *  @default []
		 */
		"aiDisplayMaster": [],
	
		/**
		 * Map of row ids to data indexes
		 *  @type object
		 *  @default {}
		 */
		"aIds": {},
	
		/**
		 * Store information about each column that is in use
		 *  @type array
		 *  @default []
		 */
		"aoColumns": [],
	
		/**
		 * Store information about the table's header
		 *  @type array
		 *  @default []
		 */
		"aoHeader": [],
	
		/**
		 * Store information about the table's footer
		 *  @type array
		 *  @default []
		 */
		"aoFooter": [],
	
		/**
		 * Store the applied global search information in case we want to force a
		 * research or compare the old search to a new one.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @namespace
		 *  @extends DataTable.models.oSearch
		 */
		"oPreviousSearch": {},
	
		/**
		 * Store the applied search for each column - see
		 * {@link DataTable.models.oSearch} for the format that is used for the
		 * filtering information for each column.
		 *  @type array
		 *  @default []
		 */
		"aoPreSearchCols": [],
	
		/**
		 * Sorting that is applied to the table. Note that the inner arrays are
		 * used in the following manner:
		 * <ul>
		 *   <li>Index 0 - column number</li>
		 *   <li>Index 1 - current sorting direction</li>
		 * </ul>
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type array
		 *  @todo These inner arrays should really be objects
		 */
		"aaSorting": null,
	
		/**
		 * Sorting that is always applied to the table (i.e. prefixed in front of
		 * aaSorting).
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type array
		 *  @default []
		 */
		"aaSortingFixed": [],
	
		/**
		 * Classes to use for the striping of a table.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type array
		 *  @default []
		 */
		"asStripeClasses": null,
	
		/**
		 * If restoring a table - we should restore its striping classes as well
		 *  @type array
		 *  @default []
		 */
		"asDestroyStripes": [],
	
		/**
		 * If restoring a table - we should restore its width
		 *  @type int
		 *  @default 0
		 */
		"sDestroyWidth": 0,
	
		/**
		 * Callback functions array for every time a row is inserted (i.e. on a draw).
		 *  @type array
		 *  @default []
		 */
		"aoRowCallback": [],
	
		/**
		 * Callback functions for the header on each draw.
		 *  @type array
		 *  @default []
		 */
		"aoHeaderCallback": [],
	
		/**
		 * Callback function for the footer on each draw.
		 *  @type array
		 *  @default []
		 */
		"aoFooterCallback": [],
	
		/**
		 * Array of callback functions for draw callback functions
		 *  @type array
		 *  @default []
		 */
		"aoDrawCallback": [],
	
		/**
		 * Array of callback functions for row created function
		 *  @type array
		 *  @default []
		 */
		"aoRowCreatedCallback": [],
	
		/**
		 * Callback functions for just before the table is redrawn. A return of
		 * false will be used to cancel the draw.
		 *  @type array
		 *  @default []
		 */
		"aoPreDrawCallback": [],
	
		/**
		 * Callback functions for when the table has been initialised.
		 *  @type array
		 *  @default []
		 */
		"aoInitComplete": [],
	
	
		/**
		 * Callbacks for modifying the settings to be stored for state saving, prior to
		 * saving state.
		 *  @type array
		 *  @default []
		 */
		"aoStateSaveParams": [],
	
		/**
		 * Callbacks for modifying the settings that have been stored for state saving
		 * prior to using the stored values to restore the state.
		 *  @type array
		 *  @default []
		 */
		"aoStateLoadParams": [],
	
		/**
		 * Callbacks for operating on the settings object once the saved state has been
		 * loaded
		 *  @type array
		 *  @default []
		 */
		"aoStateLoaded": [],
	
		/**
		 * Cache the table ID for quick access
		 *  @type string
		 *  @default <i>Empty string</i>
		 */
		"sTableId": "",
	
		/**
		 * The TABLE node for the main table
		 *  @type node
		 *  @default null
		 */
		"nTable": null,
	
		/**
		 * Permanent ref to the thead element
		 *  @type node
		 *  @default null
		 */
		"nTHead": null,
	
		/**
		 * Permanent ref to the tfoot element - if it exists
		 *  @type node
		 *  @default null
		 */
		"nTFoot": null,
	
		/**
		 * Permanent ref to the tbody element
		 *  @type node
		 *  @default null
		 */
		"nTBody": null,
	
		/**
		 * Cache the wrapper node (contains all DataTables controlled elements)
		 *  @type node
		 *  @default null
		 */
		"nTableWrapper": null,
	
		/**
		 * Indicate if when using server-side processing the loading of data
		 * should be deferred until the second draw.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type boolean
		 *  @default false
		 */
		"bDeferLoading": false,
	
		/**
		 * Indicate if all required information has been read in
		 *  @type boolean
		 *  @default false
		 */
		"bInitialised": false,
	
		/**
		 * Information about open rows. Each object in the array has the parameters
		 * 'nTr' and 'nParent'
		 *  @type array
		 *  @default []
		 */
		"aoOpenRows": [],
	
		/**
		 * Dictate the positioning of DataTables' control elements - see
		 * {@link DataTable.model.oInit.sDom}.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type string
		 *  @default null
		 */
		"sDom": null,
	
		/**
		 * Search delay (in mS)
		 *  @type integer
		 *  @default null
		 */
		"searchDelay": null,
	
		/**
		 * Which type of pagination should be used.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type string
		 *  @default two_button
		 */
		"sPaginationType": "two_button",
	
		/**
		 * The state duration (for `stateSave`) in seconds.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type int
		 *  @default 0
		 */
		"iStateDuration": 0,
	
		/**
		 * Array of callback functions for state saving. Each array element is an
		 * object with the following parameters:
		 *   <ul>
		 *     <li>function:fn - function to call. Takes two parameters, oSettings
		 *       and the JSON string to save that has been thus far created. Returns
		 *       a JSON string to be inserted into a json object
		 *       (i.e. '"param": [ 0, 1, 2]')</li>
		 *     <li>string:sName - name of callback</li>
		 *   </ul>
		 *  @type array
		 *  @default []
		 */
		"aoStateSave": [],
	
		/**
		 * Array of callback functions for state loading. Each array element is an
		 * object with the following parameters:
		 *   <ul>
		 *     <li>function:fn - function to call. Takes two parameters, oSettings
		 *       and the object stored. May return false to cancel state loading</li>
		 *     <li>string:sName - name of callback</li>
		 *   </ul>
		 *  @type array
		 *  @default []
		 */
		"aoStateLoad": [],
	
		/**
		 * State that was saved. Useful for back reference
		 *  @type object
		 *  @default null
		 */
		"oSavedState": null,
	
		/**
		 * State that was loaded. Useful for back reference
		 *  @type object
		 *  @default null
		 */
		"oLoadedState": null,
	
		/**
		 * Source url for AJAX data for the table.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type string
		 *  @default null
		 */
		"sAjaxSource": null,
	
		/**
		 * Property from a given object from which to read the table data from. This
		 * can be an empty string (when not server-side processing), in which case
		 * it is  assumed an an array is given directly.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type string
		 */
		"sAjaxDataProp": null,
	
		/**
		 * Note if draw should be blocked while getting data
		 *  @type boolean
		 *  @default true
		 */
		"bAjaxDataGet": true,
	
		/**
		 * The last jQuery XHR object that was used for server-side data gathering.
		 * This can be used for working with the XHR information in one of the
		 * callbacks
		 *  @type object
		 *  @default null
		 */
		"jqXHR": null,
	
		/**
		 * JSON returned from the server in the last Ajax request
		 *  @type object
		 *  @default undefined
		 */
		"json": undefined,
	
		/**
		 * Data submitted as part of the last Ajax request
		 *  @type object
		 *  @default undefined
		 */
		"oAjaxData": undefined,
	
		/**
		 * Function to get the server-side data.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type function
		 */
		"fnServerData": null,
	
		/**
		 * Functions which are called prior to sending an Ajax request so extra
		 * parameters can easily be sent to the server
		 *  @type array
		 *  @default []
		 */
		"aoServerParams": [],
	
		/**
		 * Send the XHR HTTP method - GET or POST (could be PUT or DELETE if
		 * required).
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type string
		 */
		"sServerMethod": null,
	
		/**
		 * Format numbers for display.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type function
		 */
		"fnFormatNumber": null,
	
		/**
		 * List of options that can be used for the user selectable length menu.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type array
		 *  @default []
		 */
		"aLengthMenu": null,
	
		/**
		 * Counter for the draws that the table does. Also used as a tracker for
		 * server-side processing
		 *  @type int
		 *  @default 0
		 */
		"iDraw": 0,
	
		/**
		 * Indicate if a redraw is being done - useful for Ajax
		 *  @type boolean
		 *  @default false
		 */
		"bDrawing": false,
	
		/**
		 * Draw index (iDraw) of the last error when parsing the returned data
		 *  @type int
		 *  @default -1
		 */
		"iDrawError": -1,
	
		/**
		 * Paging display length
		 *  @type int
		 *  @default 10
		 */
		"_iDisplayLength": 10,
	
		/**
		 * Paging start point - aiDisplay index
		 *  @type int
		 *  @default 0
		 */
		"_iDisplayStart": 0,
	
		/**
		 * Server-side processing - number of records in the result set
		 * (i.e. before filtering), Use fnRecordsTotal rather than
		 * this property to get the value of the number of records, regardless of
		 * the server-side processing setting.
		 *  @type int
		 *  @default 0
		 *  @private
		 */
		"_iRecordsTotal": 0,
	
		/**
		 * Server-side processing - number of records in the current display set
		 * (i.e. after filtering). Use fnRecordsDisplay rather than
		 * this property to get the value of the number of records, regardless of
		 * the server-side processing setting.
		 *  @type boolean
		 *  @default 0
		 *  @private
		 */
		"_iRecordsDisplay": 0,
	
		/**
		 * The classes to use for the table
		 *  @type object
		 *  @default {}
		 */
		"oClasses": {},
	
		/**
		 * Flag attached to the settings object so you can check in the draw
		 * callback if filtering has been done in the draw. Deprecated in favour of
		 * events.
		 *  @type boolean
		 *  @default false
		 *  @deprecated
		 */
		"bFiltered": false,
	
		/**
		 * Flag attached to the settings object so you can check in the draw
		 * callback if sorting has been done in the draw. Deprecated in favour of
		 * events.
		 *  @type boolean
		 *  @default false
		 *  @deprecated
		 */
		"bSorted": false,
	
		/**
		 * Indicate that if multiple rows are in the header and there is more than
		 * one unique cell per column, if the top one (true) or bottom one (false)
		 * should be used for sorting / title by DataTables.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type boolean
		 */
		"bSortCellsTop": null,
	
		/**
		 * Initialisation object that is used for the table
		 *  @type object
		 *  @default null
		 */
		"oInit": null,
	
		/**
		 * Destroy callback functions - for plug-ins to attach themselves to the
		 * destroy so they can clean up markup and events.
		 *  @type array
		 *  @default []
		 */
		"aoDestroyCallback": [],
	
	
		/**
		 * Get the number of records in the current record set, before filtering
		 *  @type function
		 */
		"fnRecordsTotal": function ()
		{
			return _fnDataSource( this ) == 'ssp' ?
				this._iRecordsTotal * 1 :
				this.aiDisplayMaster.length;
		},
	
		/**
		 * Get the number of records in the current record set, after filtering
		 *  @type function
		 */
		"fnRecordsDisplay": function ()
		{
			return _fnDataSource( this ) == 'ssp' ?
				this._iRecordsDisplay * 1 :
				this.aiDisplay.length;
		},
	
		/**
		 * Get the display end point - aiDisplay index
		 *  @type function
		 */
		"fnDisplayEnd": function ()
		{
			var
				len      = this._iDisplayLength,
				start    = this._iDisplayStart,
				calc     = start + len,
				records  = this.aiDisplay.length,
				features = this.oFeatures,
				paginate = features.bPaginate;
	
			if ( features.bServerSide ) {
				return paginate === false || len === -1 ?
					start + records :
					Math.min( start+len, this._iRecordsDisplay );
			}
			else {
				return ! paginate || calc>records || len===-1 ?
					records :
					calc;
			}
		},
	
		/**
		 * The DataTables object for this table
		 *  @type object
		 *  @default null
		 */
		"oInstance": null,
	
		/**
		 * Unique identifier for each instance of the DataTables object. If there
		 * is an ID on the table node, then it takes that value, otherwise an
		 * incrementing internal counter is used.
		 *  @type string
		 *  @default null
		 */
		"sInstance": null,
	
		/**
		 * tabindex attribute value that is added to DataTables control elements, allowing
		 * keyboard navigation of the table and its controls.
		 */
		"iTabIndex": 0,
	
		/**
		 * DIV container for the footer scrolling table if scrolling
		 */
		"nScrollHead": null,
	
		/**
		 * DIV container for the footer scrolling table if scrolling
		 */
		"nScrollFoot": null,
	
		/**
		 * Last applied sort
		 *  @type array
		 *  @default []
		 */
		"aLastSort": [],
	
		/**
		 * Stored plug-in instances
		 *  @type object
		 *  @default {}
		 */
		"oPlugins": {},
	
		/**
		 * Function used to get a row's id from the row's data
		 *  @type function
		 *  @default null
		 */
		"rowIdFn": null,
	
		/**
		 * Data location where to store a row's id
		 *  @type string
		 *  @default null
		 */
		"rowId": null
	};

	/**
	 * Extension object for DataTables that is used to provide all extension
	 * options.
	 *
	 * Note that the `DataTable.ext` object is available through
	 * `jQuery.fn.dataTable.ext` where it may be accessed and manipulated. It is
	 * also aliased to `jQuery.fn.dataTableExt` for historic reasons.
	 *  @namespace
	 *  @extends DataTable.models.ext
	 */
	
	
	/**
	 * DataTables extensions
	 * 
	 * This namespace acts as a collection area for plug-ins that can be used to
	 * extend DataTables capabilities. Indeed many of the build in methods
	 * use this method to provide their own capabilities (sorting methods for
	 * example).
	 *
	 * Note that this namespace is aliased to `jQuery.fn.dataTableExt` for legacy
	 * reasons
	 *
	 *  @namespace
	 */
	DataTable.ext = _ext = {
		/**
		 * Buttons. For use with the Buttons extension for DataTables. This is
		 * defined here so other extensions can define buttons regardless of load
		 * order. It is _not_ used by DataTables core.
		 *
		 *  @type object
		 *  @default {}
		 */
		buttons: {},
	
	
		/**
		 * Element class names
		 *
		 *  @type object
		 *  @default {}
		 */
		classes: {},
	
	
		/**
		 * DataTables build type (expanded by the download builder)
		 *
		 *  @type string
		 */
		build:"dt/dt-1.10.18/e-1.8.1/af-2.3.2/fc-3.2.5/fh-3.1.4/sl-1.2.6",
	
	
		/**
		 * Error reporting.
		 * 
		 * How should DataTables report an error. Can take the value 'alert',
		 * 'throw', 'none' or a function.
		 *
		 *  @type string|function
		 *  @default alert
		 */
		errMode: "alert",
	
	
		/**
		 * Feature plug-ins.
		 * 
		 * This is an array of objects which describe the feature plug-ins that are
		 * available to DataTables. These feature plug-ins are then available for
		 * use through the `dom` initialisation option.
		 * 
		 * Each feature plug-in is described by an object which must have the
		 * following properties:
		 * 
		 * * `fnInit` - function that is used to initialise the plug-in,
		 * * `cFeature` - a character so the feature can be enabled by the `dom`
		 *   instillation option. This is case sensitive.
		 *
		 * The `fnInit` function has the following input parameters:
		 *
		 * 1. `{object}` DataTables settings object: see
		 *    {@link DataTable.models.oSettings}
		 *
		 * And the following return is expected:
		 * 
		 * * {node|null} The element which contains your feature. Note that the
		 *   return may also be void if your plug-in does not require to inject any
		 *   DOM elements into DataTables control (`dom`) - for example this might
		 *   be useful when developing a plug-in which allows table control via
		 *   keyboard entry
		 *
		 *  @type array
		 *
		 *  @example
		 *    $.fn.dataTable.ext.features.push( {
		 *      "fnInit": function( oSettings ) {
		 *        return new TableTools( { "oDTSettings": oSettings } );
		 *      },
		 *      "cFeature": "T"
		 *    } );
		 */
		feature: [],
	
	
		/**
		 * Row searching.
		 * 
		 * This method of searching is complimentary to the default type based
		 * searching, and a lot more comprehensive as it allows you complete control
		 * over the searching logic. Each element in this array is a function
		 * (parameters described below) that is called for every row in the table,
		 * and your logic decides if it should be included in the searching data set
		 * or not.
		 *
		 * Searching functions have the following input parameters:
		 *
		 * 1. `{object}` DataTables settings object: see
		 *    {@link DataTable.models.oSettings}
		 * 2. `{array|object}` Data for the row to be processed (same as the
		 *    original format that was passed in as the data source, or an array
		 *    from a DOM data source
		 * 3. `{int}` Row index ({@link DataTable.models.oSettings.aoData}), which
		 *    can be useful to retrieve the `TR` element if you need DOM interaction.
		 *
		 * And the following return is expected:
		 *
		 * * {boolean} Include the row in the searched result set (true) or not
		 *   (false)
		 *
		 * Note that as with the main search ability in DataTables, technically this
		 * is "filtering", since it is subtractive. However, for consistency in
		 * naming we call it searching here.
		 *
		 *  @type array
		 *  @default []
		 *
		 *  @example
		 *    // The following example shows custom search being applied to the
		 *    // fourth column (i.e. the data[3] index) based on two input values
		 *    // from the end-user, matching the data in a certain range.
		 *    $.fn.dataTable.ext.search.push(
		 *      function( settings, data, dataIndex ) {
		 *        var min = document.getElementById('min').value * 1;
		 *        var max = document.getElementById('max').value * 1;
		 *        var version = data[3] == "-" ? 0 : data[3]*1;
		 *
		 *        if ( min == "" && max == "" ) {
		 *          return true;
		 *        }
		 *        else if ( min == "" && version < max ) {
		 *          return true;
		 *        }
		 *        else if ( min < version && "" == max ) {
		 *          return true;
		 *        }
		 *        else if ( min < version && version < max ) {
		 *          return true;
		 *        }
		 *        return false;
		 *      }
		 *    );
		 */
		search: [],
	
	
		/**
		 * Selector extensions
		 *
		 * The `selector` option can be used to extend the options available for the
		 * selector modifier options (`selector-modifier` object data type) that
		 * each of the three built in selector types offer (row, column and cell +
		 * their plural counterparts). For example the Select extension uses this
		 * mechanism to provide an option to select only rows, columns and cells
		 * that have been marked as selected by the end user (`{selected: true}`),
		 * which can be used in conjunction with the existing built in selector
		 * options.
		 *
		 * Each property is an array to which functions can be pushed. The functions
		 * take three attributes:
		 *
		 * * Settings object for the host table
		 * * Options object (`selector-modifier` object type)
		 * * Array of selected item indexes
		 *
		 * The return is an array of the resulting item indexes after the custom
		 * selector has been applied.
		 *
		 *  @type object
		 */
		selector: {
			cell: [],
			column: [],
			row: []
		},
	
	
		/**
		 * Internal functions, exposed for used in plug-ins.
		 * 
		 * Please note that you should not need to use the internal methods for
		 * anything other than a plug-in (and even then, try to avoid if possible).
		 * The internal function may change between releases.
		 *
		 *  @type object
		 *  @default {}
		 */
		internal: {},
	
	
		/**
		 * Legacy configuration options. Enable and disable legacy options that
		 * are available in DataTables.
		 *
		 *  @type object
		 */
		legacy: {
			/**
			 * Enable / disable DataTables 1.9 compatible server-side processing
			 * requests
			 *
			 *  @type boolean
			 *  @default null
			 */
			ajax: null
		},
	
	
		/**
		 * Pagination plug-in methods.
		 * 
		 * Each entry in this object is a function and defines which buttons should
		 * be shown by the pagination rendering method that is used for the table:
		 * {@link DataTable.ext.renderer.pageButton}. The renderer addresses how the
		 * buttons are displayed in the document, while the functions here tell it
		 * what buttons to display. This is done by returning an array of button
		 * descriptions (what each button will do).
		 *
		 * Pagination types (the four built in options and any additional plug-in
		 * options defined here) can be used through the `paginationType`
		 * initialisation parameter.
		 *
		 * The functions defined take two parameters:
		 *
		 * 1. `{int} page` The current page index
		 * 2. `{int} pages` The number of pages in the table
		 *
		 * Each function is expected to return an array where each element of the
		 * array can be one of:
		 *
		 * * `first` - Jump to first page when activated
		 * * `last` - Jump to last page when activated
		 * * `previous` - Show previous page when activated
		 * * `next` - Show next page when activated
		 * * `{int}` - Show page of the index given
		 * * `{array}` - A nested array containing the above elements to add a
		 *   containing 'DIV' element (might be useful for styling).
		 *
		 * Note that DataTables v1.9- used this object slightly differently whereby
		 * an object with two functions would be defined for each plug-in. That
		 * ability is still supported by DataTables 1.10+ to provide backwards
		 * compatibility, but this option of use is now decremented and no longer
		 * documented in DataTables 1.10+.
		 *
		 *  @type object
		 *  @default {}
		 *
		 *  @example
		 *    // Show previous, next and current page buttons only
		 *    $.fn.dataTableExt.oPagination.current = function ( page, pages ) {
		 *      return [ 'previous', page, 'next' ];
		 *    };
		 */
		pager: {},
	
	
		renderer: {
			pageButton: {},
			header: {}
		},
	
	
		/**
		 * Ordering plug-ins - custom data source
		 * 
		 * The extension options for ordering of data available here is complimentary
		 * to the default type based ordering that DataTables typically uses. It
		 * allows much greater control over the the data that is being used to
		 * order a column, but is necessarily therefore more complex.
		 * 
		 * This type of ordering is useful if you want to do ordering based on data
		 * live from the DOM (for example the contents of an 'input' element) rather
		 * than just the static string that DataTables knows of.
		 * 
		 * The way these plug-ins work is that you create an array of the values you
		 * wish to be ordering for the column in question and then return that
		 * array. The data in the array much be in the index order of the rows in
		 * the table (not the currently ordering order!). Which order data gathering
		 * function is run here depends on the `dt-init columns.orderDataType`
		 * parameter that is used for the column (if any).
		 *
		 * The functions defined take two parameters:
		 *
		 * 1. `{object}` DataTables settings object: see
		 *    {@link DataTable.models.oSettings}
		 * 2. `{int}` Target column index
		 *
		 * Each function is expected to return an array:
		 *
		 * * `{array}` Data for the column to be ordering upon
		 *
		 *  @type array
		 *
		 *  @example
		 *    // Ordering using `input` node values
		 *    $.fn.dataTable.ext.order['dom-text'] = function  ( settings, col )
		 *    {
		 *      return this.api().column( col, {order:'index'} ).nodes().map( function ( td, i ) {
		 *        return $('input', td).val();
		 *      } );
		 *    }
		 */
		order: {},
	
	
		/**
		 * Type based plug-ins.
		 *
		 * Each column in DataTables has a type assigned to it, either by automatic
		 * detection or by direct assignment using the `type` option for the column.
		 * The type of a column will effect how it is ordering and search (plug-ins
		 * can also make use of the column type if required).
		 *
		 * @namespace
		 */
		type: {
			/**
			 * Type detection functions.
			 *
			 * The functions defined in this object are used to automatically detect
			 * a column's type, making initialisation of DataTables super easy, even
			 * when complex data is in the table.
			 *
			 * The functions defined take two parameters:
			 *
		     *  1. `{*}` Data from the column cell to be analysed
		     *  2. `{settings}` DataTables settings object. This can be used to
		     *     perform context specific type detection - for example detection
		     *     based on language settings such as using a comma for a decimal
		     *     place. Generally speaking the options from the settings will not
		     *     be required
			 *
			 * Each function is expected to return:
			 *
			 * * `{string|null}` Data type detected, or null if unknown (and thus
			 *   pass it on to the other type detection functions.
			 *
			 *  @type array
			 *
			 *  @example
			 *    // Currency type detection plug-in:
			 *    $.fn.dataTable.ext.type.detect.push(
			 *      function ( data, settings ) {
			 *        // Check the numeric part
			 *        if ( ! data.substring(1).match(/[0-9]/) ) {
			 *          return null;
			 *        }
			 *
			 *        // Check prefixed by currency
			 *        if ( data.charAt(0) == '$' || data.charAt(0) == '&pound;' ) {
			 *          return 'currency';
			 *        }
			 *        return null;
			 *      }
			 *    );
			 */
			detect: [],
	
	
			/**
			 * Type based search formatting.
			 *
			 * The type based searching functions can be used to pre-format the
			 * data to be search on. For example, it can be used to strip HTML
			 * tags or to de-format telephone numbers for numeric only searching.
			 *
			 * Note that is a search is not defined for a column of a given type,
			 * no search formatting will be performed.
			 * 
			 * Pre-processing of searching data plug-ins - When you assign the sType
			 * for a column (or have it automatically detected for you by DataTables
			 * or a type detection plug-in), you will typically be using this for
			 * custom sorting, but it can also be used to provide custom searching
			 * by allowing you to pre-processing the data and returning the data in
			 * the format that should be searched upon. This is done by adding
			 * functions this object with a parameter name which matches the sType
			 * for that target column. This is the corollary of <i>afnSortData</i>
			 * for searching data.
			 *
			 * The functions defined take a single parameter:
			 *
		     *  1. `{*}` Data from the column cell to be prepared for searching
			 *
			 * Each function is expected to return:
			 *
			 * * `{string|null}` Formatted string that will be used for the searching.
			 *
			 *  @type object
			 *  @default {}
			 *
			 *  @example
			 *    $.fn.dataTable.ext.type.search['title-numeric'] = function ( d ) {
			 *      return d.replace(/\n/g," ").replace( /<.*?>/g, "" );
			 *    }
			 */
			search: {},
	
	
			/**
			 * Type based ordering.
			 *
			 * The column type tells DataTables what ordering to apply to the table
			 * when a column is sorted upon. The order for each type that is defined,
			 * is defined by the functions available in this object.
			 *
			 * Each ordering option can be described by three properties added to
			 * this object:
			 *
			 * * `{type}-pre` - Pre-formatting function
			 * * `{type}-asc` - Ascending order function
			 * * `{type}-desc` - Descending order function
			 *
			 * All three can be used together, only `{type}-pre` or only
			 * `{type}-asc` and `{type}-desc` together. It is generally recommended
			 * that only `{type}-pre` is used, as this provides the optimal
			 * implementation in terms of speed, although the others are provided
			 * for compatibility with existing Javascript sort functions.
			 *
			 * `{type}-pre`: Functions defined take a single parameter:
			 *
		     *  1. `{*}` Data from the column cell to be prepared for ordering
			 *
			 * And return:
			 *
			 * * `{*}` Data to be sorted upon
			 *
			 * `{type}-asc` and `{type}-desc`: Functions are typical Javascript sort
			 * functions, taking two parameters:
			 *
		     *  1. `{*}` Data to compare to the second parameter
		     *  2. `{*}` Data to compare to the first parameter
			 *
			 * And returning:
			 *
			 * * `{*}` Ordering match: <0 if first parameter should be sorted lower
			 *   than the second parameter, ===0 if the two parameters are equal and
			 *   >0 if the first parameter should be sorted height than the second
			 *   parameter.
			 * 
			 *  @type object
			 *  @default {}
			 *
			 *  @example
			 *    // Numeric ordering of formatted numbers with a pre-formatter
			 *    $.extend( $.fn.dataTable.ext.type.order, {
			 *      "string-pre": function(x) {
			 *        a = (a === "-" || a === "") ? 0 : a.replace( /[^\d\-\.]/g, "" );
			 *        return parseFloat( a );
			 *      }
			 *    } );
			 *
			 *  @example
			 *    // Case-sensitive string ordering, with no pre-formatting method
			 *    $.extend( $.fn.dataTable.ext.order, {
			 *      "string-case-asc": function(x,y) {
			 *        return ((x < y) ? -1 : ((x > y) ? 1 : 0));
			 *      },
			 *      "string-case-desc": function(x,y) {
			 *        return ((x < y) ? 1 : ((x > y) ? -1 : 0));
			 *      }
			 *    } );
			 */
			order: {}
		},
	
		/**
		 * Unique DataTables instance counter
		 *
		 * @type int
		 * @private
		 */
		_unique: 0,
	
	
		//
		// Depreciated
		// The following properties are retained for backwards compatiblity only.
		// The should not be used in new projects and will be removed in a future
		// version
		//
	
		/**
		 * Version check function.
		 *  @type function
		 *  @depreciated Since 1.10
		 */
		fnVersionCheck: DataTable.fnVersionCheck,
	
	
		/**
		 * Index for what 'this' index API functions should use
		 *  @type int
		 *  @deprecated Since v1.10
		 */
		iApiIndex: 0,
	
	
		/**
		 * jQuery UI class container
		 *  @type object
		 *  @deprecated Since v1.10
		 */
		oJUIClasses: {},
	
	
		/**
		 * Software version
		 *  @type string
		 *  @deprecated Since v1.10
		 */
		sVersion: DataTable.version
	};
	
	
	//
	// Backwards compatibility. Alias to pre 1.10 Hungarian notation counter parts
	//
	$.extend( _ext, {
		afnFiltering: _ext.search,
		aTypes:       _ext.type.detect,
		ofnSearch:    _ext.type.search,
		oSort:        _ext.type.order,
		afnSortData:  _ext.order,
		aoFeatures:   _ext.feature,
		oApi:         _ext.internal,
		oStdClasses:  _ext.classes,
		oPagination:  _ext.pager
	} );
	
	
	$.extend( DataTable.ext.classes, {
		"sTable": "dataTable",
		"sNoFooter": "no-footer",
	
		/* Paging buttons */
		"sPageButton": "paginate_button",
		"sPageButtonActive": "current",
		"sPageButtonDisabled": "disabled",
	
		/* Striping classes */
		"sStripeOdd": "odd",
		"sStripeEven": "even",
	
		/* Empty row */
		"sRowEmpty": "dataTables_empty",
	
		/* Features */
		"sWrapper": "dataTables_wrapper",
		"sFilter": "dataTables_filter",
		"sInfo": "dataTables_info",
		"sPaging": "dataTables_paginate paging_", /* Note that the type is postfixed */
		"sLength": "dataTables_length",
		"sProcessing": "dataTables_processing",
	
		/* Sorting */
		"sSortAsc": "sorting_asc",
		"sSortDesc": "sorting_desc",
		"sSortable": "sorting", /* Sortable in both directions */
		"sSortableAsc": "sorting_asc_disabled",
		"sSortableDesc": "sorting_desc_disabled",
		"sSortableNone": "sorting_disabled",
		"sSortColumn": "sorting_", /* Note that an int is postfixed for the sorting order */
	
		/* Filtering */
		"sFilterInput": "",
	
		/* Page length */
		"sLengthSelect": "",
	
		/* Scrolling */
		"sScrollWrapper": "dataTables_scroll",
		"sScrollHead": "dataTables_scrollHead",
		"sScrollHeadInner": "dataTables_scrollHeadInner",
		"sScrollBody": "dataTables_scrollBody",
		"sScrollFoot": "dataTables_scrollFoot",
		"sScrollFootInner": "dataTables_scrollFootInner",
	
		/* Misc */
		"sHeaderTH": "",
		"sFooterTH": "",
	
		// Deprecated
		"sSortJUIAsc": "",
		"sSortJUIDesc": "",
		"sSortJUI": "",
		"sSortJUIAscAllowed": "",
		"sSortJUIDescAllowed": "",
		"sSortJUIWrapper": "",
		"sSortIcon": "",
		"sJUIHeader": "",
		"sJUIFooter": ""
	} );
	
	
	var extPagination = DataTable.ext.pager;
	
	function _numbers ( page, pages ) {
		var
			numbers = [],
			buttons = extPagination.numbers_length,
			half = Math.floor( buttons / 2 ),
			i = 1;
	
		if ( pages <= buttons ) {
			numbers = _range( 0, pages );
		}
		else if ( page <= half ) {
			numbers = _range( 0, buttons-2 );
			numbers.push( 'ellipsis' );
			numbers.push( pages-1 );
		}
		else if ( page >= pages - 1 - half ) {
			numbers = _range( pages-(buttons-2), pages );
			numbers.splice( 0, 0, 'ellipsis' ); // no unshift in ie6
			numbers.splice( 0, 0, 0 );
		}
		else {
			numbers = _range( page-half+2, page+half-1 );
			numbers.push( 'ellipsis' );
			numbers.push( pages-1 );
			numbers.splice( 0, 0, 'ellipsis' );
			numbers.splice( 0, 0, 0 );
		}
	
		numbers.DT_el = 'span';
		return numbers;
	}
	
	
	$.extend( extPagination, {
		simple: function ( page, pages ) {
			return [ 'previous', 'next' ];
		},
	
		full: function ( page, pages ) {
			return [  'first', 'previous', 'next', 'last' ];
		},
	
		numbers: function ( page, pages ) {
			return [ _numbers(page, pages) ];
		},
	
		simple_numbers: function ( page, pages ) {
			return [ 'previous', _numbers(page, pages), 'next' ];
		},
	
		full_numbers: function ( page, pages ) {
			return [ 'first', 'previous', _numbers(page, pages), 'next', 'last' ];
		},
		
		first_last_numbers: function (page, pages) {
	 		return ['first', _numbers(page, pages), 'last'];
	 	},
	
		// For testing and plug-ins to use
		_numbers: _numbers,
	
		// Number of number buttons (including ellipsis) to show. _Must be odd!_
		numbers_length: 7
	} );
	
	
	$.extend( true, DataTable.ext.renderer, {
		pageButton: {
			_: function ( settings, host, idx, buttons, page, pages ) {
				var classes = settings.oClasses;
				var lang = settings.oLanguage.oPaginate;
				var aria = settings.oLanguage.oAria.paginate || {};
				var btnDisplay, btnClass, counter=0;
	
				var attach = function( container, buttons ) {
					var i, ien, node, button;
					var clickHandler = function ( e ) {
						_fnPageChange( settings, e.data.action, true );
					};
	
					for ( i=0, ien=buttons.length ; i<ien ; i++ ) {
						button = buttons[i];
	
						if ( $.isArray( button ) ) {
							var inner = $( '<'+(button.DT_el || 'div')+'/>' )
								.appendTo( container );
							attach( inner, button );
						}
						else {
							btnDisplay = null;
							btnClass = '';
	
							switch ( button ) {
								case 'ellipsis':
									container.append('<span class="ellipsis">&#x2026;</span>');
									break;
	
								case 'first':
									btnDisplay = lang.sFirst;
									btnClass = button + (page > 0 ?
										'' : ' '+classes.sPageButtonDisabled);
									break;
	
								case 'previous':
									btnDisplay = lang.sPrevious;
									btnClass = button + (page > 0 ?
										'' : ' '+classes.sPageButtonDisabled);
									break;
	
								case 'next':
									btnDisplay = lang.sNext;
									btnClass = button + (page < pages-1 ?
										'' : ' '+classes.sPageButtonDisabled);
									break;
	
								case 'last':
									btnDisplay = lang.sLast;
									btnClass = button + (page < pages-1 ?
										'' : ' '+classes.sPageButtonDisabled);
									break;
	
								default:
									btnDisplay = button + 1;
									btnClass = page === button ?
										classes.sPageButtonActive : '';
									break;
							}
	
							if ( btnDisplay !== null ) {
								node = $('<a>', {
										'class': classes.sPageButton+' '+btnClass,
										'aria-controls': settings.sTableId,
										'aria-label': aria[ button ],
										'data-dt-idx': counter,
										'tabindex': settings.iTabIndex,
										'id': idx === 0 && typeof button === 'string' ?
											settings.sTableId +'_'+ button :
											null
									} )
									.html( btnDisplay )
									.appendTo( container );
	
								_fnBindAction(
									node, {action: button}, clickHandler
								);
	
								counter++;
							}
						}
					}
				};
	
				// IE9 throws an 'unknown error' if document.activeElement is used
				// inside an iframe or frame. Try / catch the error. Not good for
				// accessibility, but neither are frames.
				var activeEl;
	
				try {
					// Because this approach is destroying and recreating the paging
					// elements, focus is lost on the select button which is bad for
					// accessibility. So we want to restore focus once the draw has
					// completed
					activeEl = $(host).find(document.activeElement).data('dt-idx');
				}
				catch (e) {}
	
				attach( $(host).empty(), buttons );
	
				if ( activeEl !== undefined ) {
					$(host).find( '[data-dt-idx='+activeEl+']' ).focus();
				}
			}
		}
	} );
	
	
	
	// Built in type detection. See model.ext.aTypes for information about
	// what is required from this methods.
	$.extend( DataTable.ext.type.detect, [
		// Plain numbers - first since V8 detects some plain numbers as dates
		// e.g. Date.parse('55') (but not all, e.g. Date.parse('22')...).
		function ( d, settings )
		{
			var decimal = settings.oLanguage.sDecimal;
			return _isNumber( d, decimal ) ? 'num'+decimal : null;
		},
	
		// Dates (only those recognised by the browser's Date.parse)
		function ( d, settings )
		{
			// V8 tries _very_ hard to make a string passed into `Date.parse()`
			// valid, so we need to use a regex to restrict date formats. Use a
			// plug-in for anything other than ISO8601 style strings
			if ( d && !(d instanceof Date) && ! _re_date.test(d) ) {
				return null;
			}
			var parsed = Date.parse(d);
			return (parsed !== null && !isNaN(parsed)) || _empty(d) ? 'date' : null;
		},
	
		// Formatted numbers
		function ( d, settings )
		{
			var decimal = settings.oLanguage.sDecimal;
			return _isNumber( d, decimal, true ) ? 'num-fmt'+decimal : null;
		},
	
		// HTML numeric
		function ( d, settings )
		{
			var decimal = settings.oLanguage.sDecimal;
			return _htmlNumeric( d, decimal ) ? 'html-num'+decimal : null;
		},
	
		// HTML numeric, formatted
		function ( d, settings )
		{
			var decimal = settings.oLanguage.sDecimal;
			return _htmlNumeric( d, decimal, true ) ? 'html-num-fmt'+decimal : null;
		},
	
		// HTML (this is strict checking - there must be html)
		function ( d, settings )
		{
			return _empty( d ) || (typeof d === 'string' && d.indexOf('<') !== -1) ?
				'html' : null;
		}
	] );
	
	
	
	// Filter formatting functions. See model.ext.ofnSearch for information about
	// what is required from these methods.
	// 
	// Note that additional search methods are added for the html numbers and
	// html formatted numbers by `_addNumericSort()` when we know what the decimal
	// place is
	
	
	$.extend( DataTable.ext.type.search, {
		html: function ( data ) {
			return _empty(data) ?
				data :
				typeof data === 'string' ?
					data
						.replace( _re_new_lines, " " )
						.replace( _re_html, "" ) :
					'';
		},
	
		string: function ( data ) {
			return _empty(data) ?
				data :
				typeof data === 'string' ?
					data.replace( _re_new_lines, " " ) :
					data;
		}
	} );
	
	
	
	var __numericReplace = function ( d, decimalPlace, re1, re2 ) {
		if ( d !== 0 && (!d || d === '-') ) {
			return -Infinity;
		}
	
		// If a decimal place other than `.` is used, it needs to be given to the
		// function so we can detect it and replace with a `.` which is the only
		// decimal place Javascript recognises - it is not locale aware.
		if ( decimalPlace ) {
			d = _numToDecimal( d, decimalPlace );
		}
	
		if ( d.replace ) {
			if ( re1 ) {
				d = d.replace( re1, '' );
			}
	
			if ( re2 ) {
				d = d.replace( re2, '' );
			}
		}
	
		return d * 1;
	};
	
	
	// Add the numeric 'deformatting' functions for sorting and search. This is done
	// in a function to provide an easy ability for the language options to add
	// additional methods if a non-period decimal place is used.
	function _addNumericSort ( decimalPlace ) {
		$.each(
			{
				// Plain numbers
				"num": function ( d ) {
					return __numericReplace( d, decimalPlace );
				},
	
				// Formatted numbers
				"num-fmt": function ( d ) {
					return __numericReplace( d, decimalPlace, _re_formatted_numeric );
				},
	
				// HTML numeric
				"html-num": function ( d ) {
					return __numericReplace( d, decimalPlace, _re_html );
				},
	
				// HTML numeric, formatted
				"html-num-fmt": function ( d ) {
					return __numericReplace( d, decimalPlace, _re_html, _re_formatted_numeric );
				}
			},
			function ( key, fn ) {
				// Add the ordering method
				_ext.type.order[ key+decimalPlace+'-pre' ] = fn;
	
				// For HTML types add a search formatter that will strip the HTML
				if ( key.match(/^html\-/) ) {
					_ext.type.search[ key+decimalPlace ] = _ext.type.search.html;
				}
			}
		);
	}
	
	
	// Default sort methods
	$.extend( _ext.type.order, {
		// Dates
		"date-pre": function ( d ) {
			var ts = Date.parse( d );
			return isNaN(ts) ? -Infinity : ts;
		},
	
		// html
		"html-pre": function ( a ) {
			return _empty(a) ?
				'' :
				a.replace ?
					a.replace( /<.*?>/g, "" ).toLowerCase() :
					a+'';
		},
	
		// string
		"string-pre": function ( a ) {
			// This is a little complex, but faster than always calling toString,
			// http://jsperf.com/tostring-v-check
			return _empty(a) ?
				'' :
				typeof a === 'string' ?
					a.toLowerCase() :
					! a.toString ?
						'' :
						a.toString();
		},
	
		// string-asc and -desc are retained only for compatibility with the old
		// sort methods
		"string-asc": function ( x, y ) {
			return ((x < y) ? -1 : ((x > y) ? 1 : 0));
		},
	
		"string-desc": function ( x, y ) {
			return ((x < y) ? 1 : ((x > y) ? -1 : 0));
		}
	} );
	
	
	// Numeric sorting types - order doesn't matter here
	_addNumericSort( '' );
	
	
	$.extend( true, DataTable.ext.renderer, {
		header: {
			_: function ( settings, cell, column, classes ) {
				// No additional mark-up required
				// Attach a sort listener to update on sort - note that using the
				// `DT` namespace will allow the event to be removed automatically
				// on destroy, while the `dt` namespaced event is the one we are
				// listening for
				$(settings.nTable).on( 'order.dt.DT', function ( e, ctx, sorting, columns ) {
					if ( settings !== ctx ) { // need to check this this is the host
						return;               // table, not a nested one
					}
	
					var colIdx = column.idx;
	
					cell
						.removeClass(
							column.sSortingClass +' '+
							classes.sSortAsc +' '+
							classes.sSortDesc
						)
						.addClass( columns[ colIdx ] == 'asc' ?
							classes.sSortAsc : columns[ colIdx ] == 'desc' ?
								classes.sSortDesc :
								column.sSortingClass
						);
				} );
			},
	
			jqueryui: function ( settings, cell, column, classes ) {
				$('<div/>')
					.addClass( classes.sSortJUIWrapper )
					.append( cell.contents() )
					.append( $('<span/>')
						.addClass( classes.sSortIcon+' '+column.sSortingClassJUI )
					)
					.appendTo( cell );
	
				// Attach a sort listener to update on sort
				$(settings.nTable).on( 'order.dt.DT', function ( e, ctx, sorting, columns ) {
					if ( settings !== ctx ) {
						return;
					}
	
					var colIdx = column.idx;
	
					cell
						.removeClass( classes.sSortAsc +" "+classes.sSortDesc )
						.addClass( columns[ colIdx ] == 'asc' ?
							classes.sSortAsc : columns[ colIdx ] == 'desc' ?
								classes.sSortDesc :
								column.sSortingClass
						);
	
					cell
						.find( 'span.'+classes.sSortIcon )
						.removeClass(
							classes.sSortJUIAsc +" "+
							classes.sSortJUIDesc +" "+
							classes.sSortJUI +" "+
							classes.sSortJUIAscAllowed +" "+
							classes.sSortJUIDescAllowed
						)
						.addClass( columns[ colIdx ] == 'asc' ?
							classes.sSortJUIAsc : columns[ colIdx ] == 'desc' ?
								classes.sSortJUIDesc :
								column.sSortingClassJUI
						);
				} );
			}
		}
	} );
	
	/*
	 * Public helper functions. These aren't used internally by DataTables, or
	 * called by any of the options passed into DataTables, but they can be used
	 * externally by developers working with DataTables. They are helper functions
	 * to make working with DataTables a little bit easier.
	 */
	
	var __htmlEscapeEntities = function ( d ) {
		return typeof d === 'string' ?
			d.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;') :
			d;
	};
	
	/**
	 * Helpers for `columns.render`.
	 *
	 * The options defined here can be used with the `columns.render` initialisation
	 * option to provide a display renderer. The following functions are defined:
	 *
	 * * `number` - Will format numeric data (defined by `columns.data`) for
	 *   display, retaining the original unformatted data for sorting and filtering.
	 *   It takes 5 parameters:
	 *   * `string` - Thousands grouping separator
	 *   * `string` - Decimal point indicator
	 *   * `integer` - Number of decimal points to show
	 *   * `string` (optional) - Prefix.
	 *   * `string` (optional) - Postfix (/suffix).
	 * * `text` - Escape HTML to help prevent XSS attacks. It has no optional
	 *   parameters.
	 *
	 * @example
	 *   // Column definition using the number renderer
	 *   {
	 *     data: "salary",
	 *     render: $.fn.dataTable.render.number( '\'', '.', 0, '$' )
	 *   }
	 *
	 * @namespace
	 */
	DataTable.render = {
		number: function ( thousands, decimal, precision, prefix, postfix ) {
			return {
				display: function ( d ) {
					if ( typeof d !== 'number' && typeof d !== 'string' ) {
						return d;
					}
	
					var negative = d < 0 ? '-' : '';
					var flo = parseFloat( d );
	
					// If NaN then there isn't much formatting that we can do - just
					// return immediately, escaping any HTML (this was supposed to
					// be a number after all)
					if ( isNaN( flo ) ) {
						return __htmlEscapeEntities( d );
					}
	
					flo = flo.toFixed( precision );
					d = Math.abs( flo );
	
					var intPart = parseInt( d, 10 );
					var floatPart = precision ?
						decimal+(d - intPart).toFixed( precision ).substring( 2 ):
						'';
	
					return negative + (prefix||'') +
						intPart.toString().replace(
							/\B(?=(\d{3})+(?!\d))/g, thousands
						) +
						floatPart +
						(postfix||'');
				}
			};
		},
	
		text: function () {
			return {
				display: __htmlEscapeEntities
			};
		}
	};
	
	
	/*
	 * This is really a good bit rubbish this method of exposing the internal methods
	 * publicly... - To be fixed in 2.0 using methods on the prototype
	 */
	
	
	/**
	 * Create a wrapper function for exporting an internal functions to an external API.
	 *  @param {string} fn API function name
	 *  @returns {function} wrapped function
	 *  @memberof DataTable#internal
	 */
	function _fnExternApiFunc (fn)
	{
		return function() {
			var args = [_fnSettingsFromNode( this[DataTable.ext.iApiIndex] )].concat(
				Array.prototype.slice.call(arguments)
			);
			return DataTable.ext.internal[fn].apply( this, args );
		};
	}
	
	
	/**
	 * Reference to internal functions for use by plug-in developers. Note that
	 * these methods are references to internal functions and are considered to be
	 * private. If you use these methods, be aware that they are liable to change
	 * between versions.
	 *  @namespace
	 */
	$.extend( DataTable.ext.internal, {
		_fnExternApiFunc: _fnExternApiFunc,
		_fnBuildAjax: _fnBuildAjax,
		_fnAjaxUpdate: _fnAjaxUpdate,
		_fnAjaxParameters: _fnAjaxParameters,
		_fnAjaxUpdateDraw: _fnAjaxUpdateDraw,
		_fnAjaxDataSrc: _fnAjaxDataSrc,
		_fnAddColumn: _fnAddColumn,
		_fnColumnOptions: _fnColumnOptions,
		_fnAdjustColumnSizing: _fnAdjustColumnSizing,
		_fnVisibleToColumnIndex: _fnVisibleToColumnIndex,
		_fnColumnIndexToVisible: _fnColumnIndexToVisible,
		_fnVisbleColumns: _fnVisbleColumns,
		_fnGetColumns: _fnGetColumns,
		_fnColumnTypes: _fnColumnTypes,
		_fnApplyColumnDefs: _fnApplyColumnDefs,
		_fnHungarianMap: _fnHungarianMap,
		_fnCamelToHungarian: _fnCamelToHungarian,
		_fnLanguageCompat: _fnLanguageCompat,
		_fnBrowserDetect: _fnBrowserDetect,
		_fnAddData: _fnAddData,
		_fnAddTr: _fnAddTr,
		_fnNodeToDataIndex: _fnNodeToDataIndex,
		_fnNodeToColumnIndex: _fnNodeToColumnIndex,
		_fnGetCellData: _fnGetCellData,
		_fnSetCellData: _fnSetCellData,
		_fnSplitObjNotation: _fnSplitObjNotation,
		_fnGetObjectDataFn: _fnGetObjectDataFn,
		_fnSetObjectDataFn: _fnSetObjectDataFn,
		_fnGetDataMaster: _fnGetDataMaster,
		_fnClearTable: _fnClearTable,
		_fnDeleteIndex: _fnDeleteIndex,
		_fnInvalidate: _fnInvalidate,
		_fnGetRowElements: _fnGetRowElements,
		_fnCreateTr: _fnCreateTr,
		_fnBuildHead: _fnBuildHead,
		_fnDrawHead: _fnDrawHead,
		_fnDraw: _fnDraw,
		_fnReDraw: _fnReDraw,
		_fnAddOptionsHtml: _fnAddOptionsHtml,
		_fnDetectHeader: _fnDetectHeader,
		_fnGetUniqueThs: _fnGetUniqueThs,
		_fnFeatureHtmlFilter: _fnFeatureHtmlFilter,
		_fnFilterComplete: _fnFilterComplete,
		_fnFilterCustom: _fnFilterCustom,
		_fnFilterColumn: _fnFilterColumn,
		_fnFilter: _fnFilter,
		_fnFilterCreateSearch: _fnFilterCreateSearch,
		_fnEscapeRegex: _fnEscapeRegex,
		_fnFilterData: _fnFilterData,
		_fnFeatureHtmlInfo: _fnFeatureHtmlInfo,
		_fnUpdateInfo: _fnUpdateInfo,
		_fnInfoMacros: _fnInfoMacros,
		_fnInitialise: _fnInitialise,
		_fnInitComplete: _fnInitComplete,
		_fnLengthChange: _fnLengthChange,
		_fnFeatureHtmlLength: _fnFeatureHtmlLength,
		_fnFeatureHtmlPaginate: _fnFeatureHtmlPaginate,
		_fnPageChange: _fnPageChange,
		_fnFeatureHtmlProcessing: _fnFeatureHtmlProcessing,
		_fnProcessingDisplay: _fnProcessingDisplay,
		_fnFeatureHtmlTable: _fnFeatureHtmlTable,
		_fnScrollDraw: _fnScrollDraw,
		_fnApplyToChildren: _fnApplyToChildren,
		_fnCalculateColumnWidths: _fnCalculateColumnWidths,
		_fnThrottle: _fnThrottle,
		_fnConvertToWidth: _fnConvertToWidth,
		_fnGetWidestNode: _fnGetWidestNode,
		_fnGetMaxLenString: _fnGetMaxLenString,
		_fnStringToCss: _fnStringToCss,
		_fnSortFlatten: _fnSortFlatten,
		_fnSort: _fnSort,
		_fnSortAria: _fnSortAria,
		_fnSortListener: _fnSortListener,
		_fnSortAttachListener: _fnSortAttachListener,
		_fnSortingClasses: _fnSortingClasses,
		_fnSortData: _fnSortData,
		_fnSaveState: _fnSaveState,
		_fnLoadState: _fnLoadState,
		_fnSettingsFromNode: _fnSettingsFromNode,
		_fnLog: _fnLog,
		_fnMap: _fnMap,
		_fnBindAction: _fnBindAction,
		_fnCallbackReg: _fnCallbackReg,
		_fnCallbackFire: _fnCallbackFire,
		_fnLengthOverflow: _fnLengthOverflow,
		_fnRenderer: _fnRenderer,
		_fnDataSource: _fnDataSource,
		_fnRowAttributes: _fnRowAttributes,
		_fnExtend: _fnExtend,
		_fnCalculateEnd: function () {} // Used by a lot of plug-ins, but redundant
		                                // in 1.10, so this dead-end function is
		                                // added to prevent errors
	} );
	

	// jQuery access
	$.fn.dataTable = DataTable;

	// Provide access to the host jQuery object (circular reference)
	DataTable.$ = $;

	// Legacy aliases
	$.fn.dataTableSettings = DataTable.settings;
	$.fn.dataTableExt = DataTable.ext;

	// With a capital `D` we return a DataTables API instance rather than a
	// jQuery object
	$.fn.DataTable = function ( opts ) {
		return $(this).dataTable( opts ).api();
	};

	// All properties that are available to $.fn.dataTable should also be
	// available on $.fn.DataTable
	$.each( DataTable, function ( prop, val ) {
		$.fn.DataTable[ prop ] = val;
	} );


	// Information about events fired by DataTables - for documentation.
	/**
	 * Draw event, fired whenever the table is redrawn on the page, at the same
	 * point as fnDrawCallback. This may be useful for binding events or
	 * performing calculations when the table is altered at all.
	 *  @name DataTable#draw.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 */

	/**
	 * Search event, fired when the searching applied to the table (using the
	 * built-in global search, or column filters) is altered.
	 *  @name DataTable#search.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 */

	/**
	 * Page change event, fired when the paging of the table is altered.
	 *  @name DataTable#page.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 */

	/**
	 * Order event, fired when the ordering applied to the table is altered.
	 *  @name DataTable#order.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 */

	/**
	 * DataTables initialisation complete event, fired when the table is fully
	 * drawn, including Ajax data loaded, if Ajax data is required.
	 *  @name DataTable#init.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} oSettings DataTables settings object
	 *  @param {object} json The JSON object request from the server - only
	 *    present if client-side Ajax sourced data is used</li></ol>
	 */

	/**
	 * State save event, fired when the table has changed state a new state save
	 * is required. This event allows modification of the state saving object
	 * prior to actually doing the save, including addition or other state
	 * properties (for plug-ins) or modification of a DataTables core property.
	 *  @name DataTable#stateSaveParams.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} oSettings DataTables settings object
	 *  @param {object} json The state information to be saved
	 */

	/**
	 * State load event, fired when the table is loading state from the stored
	 * data, but prior to the settings object being modified by the saved state
	 * - allowing modification of the saved state is required or loading of
	 * state for a plug-in.
	 *  @name DataTable#stateLoadParams.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} oSettings DataTables settings object
	 *  @param {object} json The saved state information
	 */

	/**
	 * State loaded event, fired when state has been loaded from stored data and
	 * the settings object has been modified by the loaded data.
	 *  @name DataTable#stateLoaded.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} oSettings DataTables settings object
	 *  @param {object} json The saved state information
	 */

	/**
	 * Processing event, fired when DataTables is doing some kind of processing
	 * (be it, order, searcg or anything else). It can be used to indicate to
	 * the end user that there is something happening, or that something has
	 * finished.
	 *  @name DataTable#processing.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} oSettings DataTables settings object
	 *  @param {boolean} bShow Flag for if DataTables is doing processing or not
	 */

	/**
	 * Ajax (XHR) event, fired whenever an Ajax request is completed from a
	 * request to made to the server for new data. This event is called before
	 * DataTables processed the returned data, so it can also be used to pre-
	 * process the data returned from the server, if needed.
	 *
	 * Note that this trigger is called in `fnServerData`, if you override
	 * `fnServerData` and which to use this event, you need to trigger it in you
	 * success function.
	 *  @name DataTable#xhr.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 *  @param {object} json JSON returned from the server
	 *
	 *  @example
	 *     // Use a custom property returned from the server in another DOM element
	 *     $('#table').dataTable().on('xhr.dt', function (e, settings, json) {
	 *       $('#status').html( json.status );
	 *     } );
	 *
	 *  @example
	 *     // Pre-process the data returned from the server
	 *     $('#table').dataTable().on('xhr.dt', function (e, settings, json) {
	 *       for ( var i=0, ien=json.aaData.length ; i<ien ; i++ ) {
	 *         json.aaData[i].sum = json.aaData[i].one + json.aaData[i].two;
	 *       }
	 *       // Note no return - manipulate the data directly in the JSON object.
	 *     } );
	 */

	/**
	 * Destroy event, fired when the DataTable is destroyed by calling fnDestroy
	 * or passing the bDestroy:true parameter in the initialisation object. This
	 * can be used to remove bound events, added DOM nodes, etc.
	 *  @name DataTable#destroy.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 */

	/**
	 * Page length change event, fired when number of records to show on each
	 * page (the length) is changed.
	 *  @name DataTable#length.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 *  @param {integer} len New length
	 */

	/**
	 * Column sizing has changed.
	 *  @name DataTable#column-sizing.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 */

	/**
	 * Column visibility has changed.
	 *  @name DataTable#column-visibility.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 *  @param {int} column Column index
	 *  @param {bool} vis `false` if column now hidden, or `true` if visible
	 */

	return $.fn.dataTable;
}));


/*!
 * File:        dataTables.editor.min.js
 * Version:     1.8.1
 * Author:      SpryMedia (www.sprymedia.co.uk)
 * Info:        http://editor.datatables.net
 * 
 * Copyright 2012-2018 SpryMedia Limited, all rights reserved.
 * License: DataTables Editor - http://editor.datatables.net/license
 */

 // Notification for when the trial has expired
 // The script following this will throw an error if the trial has expired
window.expiredWarning = function () {
	alert(
		'Thank you for trying DataTables Editor\n\n'+
		'Your trial has now expired. To purchase a license '+
		'for Editor, please see https://editor.datatables.net/purchase'
	);
};

W1RR.s8=function (){return typeof W1RR.w8.U==='function'?W1RR.w8.U.apply(W1RR.w8,arguments):W1RR.w8.U;};W1RR.c7n="6";W1RR.K7n="";W1RR.b7n="obj";W1RR.w8=function(z6,u){function j6(I6){var y6=2;while(y6!==15){switch(y6){case 20:T6=I6-Y6>s6&&d6-I6>s6;y6=19;break;case 5:Q6=S[u[4]];y6=4;break;case 10:y6=Y6>=0&&d6>=0?20:18;break;case 9:y6=!p--?8:7;break;case 11:Y6=(Z6||Z6===0)&&Q6(Z6,s6);y6=10;break;case 6:d6=C6&&Q6(C6,s6);y6=14;break;case 12:y6=!p--?11:10;break;case 13:Z6=u[7];y6=12;break;case 14:y6=!p--?13:12;break;case 2:var T6,s6,C6,d6,Z6,Y6,Q6;y6=1;break;case 8:C6=u[6];y6=7;break;case 4:y6=!p--?3:9;break;case 3:s6=35;y6=9;break;case 7:y6=!p--?6:14;break;case 1:y6=!p--?5:4;break;case 19:return T6;break;case 18:y6=Y6>=0?17:16;break;case 16:T6=d6-I6>s6;y6=19;break;case 17:T6=I6-Y6>s6;y6=19;break;}}}var H6=2;while(H6!==10){switch(H6){case 1:H6=!p--?5:4;break;case 12:j6=j6(new S[u[0]]()[u[1]]());H6=11;break;case 8:H6=!p--?7:6;break;case 13:H6=!p--?12:11;break;case 11:return{U:function(L6,e6){var z8=2;while(z8!==16){switch(z8){case 4:var g6=j6;z8=3;break;case 5:var v6,V6=0;z8=4;break;case 14:V6++;z8=3;break;case 3:z8=V6<L6[u[5]]?9:12;break;case 7:z8=V6===0?6:13;break;case 18:R6=1;z8=10;break;case 12:z8=!g6?11:17;break;case 13:v6=v6^m6;z8=14;break;case 2:z8=!p--?1:5;break;case 11:var R6=2;z8=10;break;case 6:v6=m6;z8=14;break;case 10:z8=R6!==1?20:17;break;case 20:z8=R6===2?19:10;break;case 1:e6=S[u[4]];z8=5;break;case 19:(function(){var F8=2;while(F8!==60){switch(F8){case 30:F8=D6===13?29:41;break;case 12:D6=8;F8=1;break;case 48:F8=D6===25?47:1;break;case 43:var G6="e";F8=42;break;case 17:D6=13;F8=1;break;case 29:var o6="_";F8=28;break;case 47:b6+=p6;b6+=a6;b6+=G6;b6+=c6;var x6=q6;F8=63;break;case 54:F8=D6===33?53:48;break;case 4:var i6="X";var B6="O";F8=9;break;case 53:x6+=G6;x6+=S6;x6+=p6;x6+=a6;F8=49;break;case 16:F8=D6===20?15:23;break;case 33:F8=D6===43?32:30;break;case 24:D6=16;F8=1;break;case 5:F8=D6===2?4:8;break;case 23:F8=D6===29?22:33;break;case 40:b6+=a6;b6+=c6;b6+=G6;b6+=S6;F8=36;break;case 36:D6=25;F8=1;break;case 9:D6=5;F8=1;break;case 28:var p6="i";var S6="f";F8=43;break;case 32:try{var M8=2;while(M8!==52){switch(M8){case 41:l6=15;M8=1;break;case 21:M8=l6===14?35:30;break;case 39:l6=!k6[X6]?20:33;M8=1;break;case 20:var X6=o6;X6+=J6;X6+=r6;X6+=U6;M8=16;break;case 5:M8=l6===22?4:6;break;case 44:E6+=r6;E6+=U6;E6+=G6;M8=41;break;case 22:l6=14;M8=1;break;case 29:var E6=o6;E6+=J6;M8=44;break;case 31:l6=10;M8=1;break;case 38:M8=l6===25?37:1;break;case 14:E6+=N6;E6+=u6;E6+=K6;M8=11;break;case 2:var l6=2;M8=1;break;case 1:M8=l6!==33?5:52;break;case 10:M8=l6===2?20:15;break;case 16:l6=3;M8=1;break;case 34:X6+=P6;X6+=B6;X6+=i6;M8=31;break;case 15:M8=l6===3?27:21;break;case 4:E6+=B6;E6+=i6;expiredWarning();k6[E6]=function(){};M8=7;break;case 23:X6+=q6;M8=22;break;case 6:M8=l6===15?14:10;break;case 40:M8=l6===10?39:38;break;case 27:X6+=G6;X6+=N6;X6+=u6;X6+=K6;M8=23;break;case 11:l6=25;M8=1;break;case 37:E6+=q6;E6+=n6;E6+=P6;M8=53;break;case 35:X6+=n6;M8=34;break;case 7:l6=33;M8=1;break;case 30:M8=l6===20?29:40;break;case 53:l6=22;M8=1;break;}}}catch(t6){}F8=31;break;case 2:var D6=2;F8=1;break;case 22:x6+=G6;x6+=c6;var k6=typeof window!==x6?window:typeof global!==b6?global:this;F8=34;break;case 63:x6+=a6;x6+=c6;F8=61;break;case 31:D6=42;F8=1;break;case 42:D6=20;F8=1;break;case 8:F8=D6===5?7:11;break;case 11:F8=D6===8?10:16;break;case 61:D6=33;F8=1;break;case 1:F8=D6!==42?5:60;break;case 15:var c6="d";var a6="n";var q6="u";var b6=q6;F8=24;break;case 7:var P6="W";var n6="J";var K6="z";F8=13;break;case 13:var u6="s";F8=12;break;case 49:D6=29;F8=1;break;case 41:F8=D6===16?40:54;break;case 34:D6=43;F8=1;break;case 10:var N6="6";var U6="q";var r6="H";var J6="x";F8=17;break;}}}());z8=18;break;case 17:return v6?g6:!g6;break;case 9:var W6=e6(L6[u[2]](V6),16)[u[3]](2);var m6=W6[u[2]](W6[u[5]]-1);z8=7;break;}}}};break;case 7:M6=F6.replace(new S[A6]("^['-|]"),'S');H6=6;break;case 3:F6=typeof z6;H6=9;break;case 9:var w6='fromCharCode',A6='RegExp';H6=8;break;case 6:H6=!p--?14:13;break;case 2:var S,F6,M6,p;H6=1;break;case 14:u=u.map(function(h6){var j8=2;while(j8!==13){switch(j8){case 5:f6='';j8=4;break;case 2:var f6;j8=1;break;case 1:j8=!p--?5:4;break;case 3:j8=O6<h6.length?9:7;break;case 9:f6+=S[M6][w6](h6[O6]+101);j8=8;break;case 4:var O6=0;j8=3;break;case 8:O6++;j8=3;break;case 7:j8=!f6?6:14;break;case 6:return;break;case 14:return f6;break;}}});H6=13;break;case 4:H6=!p--?3:9;break;case 5:S=u.filter.constructor(z6)();H6=4;break;}}}('return this',[[-33,-4,15,0],[2,0,15,-17,4,8,0],[-2,3,-4,13,-36,15],[15,10,-18,15,13,4,9,2],[11,-4,13,14,0,-28,9,15],[7,0,9,2,15,3],[10,-52,-44,-1,-53,-50,6,-53],[9,5,-53,8,7,-45,16,11]]);W1RR.N7n="tion";W1RR.G7n="ec";W1RR.A8=function (){return typeof W1RR.w8.U==='function'?W1RR.w8.U.apply(W1RR.w8,arguments):W1RR.w8.U;};W1RR.r7n="fun";function W1RR(){}W1RR.U7n="c";W1RR.q7n="t";W1RR.q3=function(G3){if(W1RR&&G3)return W1RR.s8(G3);};W1RR.H1=function(t1){if(W1RR&&t1)return W1RR.A8(t1);};W1RR.K9=function(u9){if(W1RR)return W1RR.s8(u9);};W1RR.n8=function(c8){if(W1RR&&c8)return W1RR.s8(c8);};(function(factory){var g13=W1RR;var J7n="8352";var k7n="amd";var u7n="69a";var n7n="7";var a7n="32";var C0=g13.b7n;C0+=g13.G7n;C0+=g13.q7n;var Z0=a7n;Z0+=g13.c7n;Z0+=n7n;var d0=g13.r7n;d0+=g13.U7n;d0+=g13.N7n;var T0=u7n;T0+=n7n;g13.C9=function(Z9){if(g13)return g13.A8(Z9);};if(typeof define===(g13.n8(T0)?g13.K7n:d0)&&define[g13.C9(Z0)?g13.K7n:k7n]){define(['jquery','datatables.net'],function($){return factory($,window,document);});}else if(typeof exports===(g13.K9(J7n)?g13.K7n:C0)){module.exports=function(root,$){if(!root){root=window;}if(!$||!$.fn.dataTable){$=require('datatables.net')(root,$).$;}return factory($,root,root.document);};}else{factory(jQuery,window,document);}}(function($,window,document,undefined){var R13=W1RR;var f13="1.8.1";var Q13="version";var C13="eldTypes";var Z13="orFiel";var d13="editorFields";var J0u='YYYY-MM-DD';var k0u='editor-datetime';var K0u="_instance";var b0u="inpu";var Q0u="_h";var s0u="_optionSet";var z0u="nth";var i3u="contain";var K3u="max";var r3u="_opt";var e3u='select.';var L3u="ai";var v3u="<op";var f3u="pty";var A3u="tD";var r4u="_co";var V4u="<t";var h4u='</td>';var M4u="cted";var t5u='scroll.';var p5u="_pad";var k5u="UT";var K5u="ear";var n5u="getUTCFullYear";var b5u='day';var e5u="change";var O5u="getU";var C5u="getUTCMonth";var Z5u="Mon";var d5u="setUT";var T5u="ispla";var A5u="put";var w5u='-iconLeft';var M5u='disabled';var j5u="ha";var p1u="_position";var k1u="setUTCMinutes";var N1u="setUTCHours";var l1u="play";var Q1u="nput";var M1u='pm';var F1u='ampm';var z1u="_options";var y7u="hours12";var p7u='span';var k7u="classPrefix";var b7u="setUTCDate";var l7u="_dateToUtc";var D7u="_writeOutput";var E7u="U";var R7u="Da";var d7u="_setCalander";var T7u="minDate";var I7u="_optionsTitle";var s7u="_hide";var w7u="empty";var M7u="npu";var k9u="an>";var a9u="<div ";var l9u="-i";var D9u="n>";var E9u="format";var e9u="moment";var L9u="ul";var g9u="Y";var s9u="</div";var z9u="/>";var y8u="v ";var t8u="cale";var i8u="time";var S8u="-";var p8u="urs";var P8u="seco";var a8u="_in";var G8u="tc";var b8u="mat";var X8u="DateTime";var E8u="fieldTypes";var h8u="fir";var O8u="ace";var A8u='selected';var F8u="itle";var n6u="select";var b6u="dit";var g6u="sel";var f6u="lect";var T6u="sele";var I6u="DTE_Bubble_Background";var Y6u="DTE_Bubble_Table";var s6u="DTE_Bubble_Liner";var A6u="DTE DTE_Bubble";var w6u="DTE_Inline_Buttons";var M6u="DTE_Action_Edit";var F6u="multi-restore";var z6u="multi-value";var j6u="DTE_Field_Message";var y2y="DTE_Field_Error";var H2y="DTE_Field_Input";var t2y="DTE_Label";var i2y="DTE_Field";var S2y="DTE_Form_Buttons";var p2y="DTE_Form_Error";var o2y="DTE_Form";var B2y="DTE_Header_Content";var P2y="DTE_Header";var J2y="DTE";var H0y="ance";var o0y="ve";var l0y="isArr";var O0y="nodeName";var d0y="indexes";var M0y="mns";var t3y="exe";var U3y="formOptions";var r3y='am';var n3y='Fri';var c3y='Thu';var a3y='Wed';var q3y='Tue';var G3y='Sun';var b3y='December';var x3y='October';var l3y='September';var D3y='July';var X3y='May';var E3y='March';var e3y='January';var L3y='Previous';var m3y="The selected items contain different values for this input. To edit and set all items for this input to the same value, click or tap here, otherwise they will retain their individual values.";var W3y="Delete";var R3y="Update";var g3y="Create";var v3y="Create new entry";var V3y="New";var h3y='lightbox';var O3y="defaults";var Q3y="su";var t4y="submi";var S4y="submitC";var p4y="_submitError";var P4y="ll";var D4y="Data";var X4y="cr";var T4y="pts";var y5y="eat";var H5y="fier";var k5y="ov";var U5y="_submitTable";var q5y="omplete";var l5y="isEmptyObject";var e5y="[";var Z5y="_a";var A5y="os";var j5y='preOpen';var S1y="parents";var N1y="_ev";var l1y="pi";var X1y="_message";var E1y="options";var L1y="_optionsUpdate";var W1y='send';var v1y='keydown';var C1y="prev";var w1y="sub";var z1y="can";var S7y="parent";var K7y="pre";var R7y="onComplete";var I7y="ke";var z7y="_focus";var H9y="ing";var P9y="triggerHandler";var N9y="xO";var r9y="gt";var l9y="displayFields";var e9y="yFields";var L9y="sc";var W9y="editData";var h9y="appen";var M9y="dataSource";var i8y="_crudArgs";var p8y='focus.editor-focus';var J8y="closeIcb";var u8y="_e";var N8y="los";var c8y="_clearDynamicInfo";var G8y="ubm";var D8y="mate";var E8y="an";var L8y="par";var g8y="io";var O8y="lete";var f8y="mp";var Q8y="rr";var Z8y="indexOf";var T8y="split";var J6y="rc";var K6y="nct";var N6y="C";var a6y="opti";var l6y="bodyContent";var L6y="ly";var g6y="utto";var Q6y="eac";var Z6y="remo";var M6y="dataSources";var F6y="idSrc";var t2J="axUrl";var P2J="ss=\"";var E2J="<div";var L2J="rm";var R2J="Ta";var v2J="bod";var h2J="fie";var I2J="_constructor";var Y2J="splice";var z2J="am";var j2J="status";var y0J="fieldErrors";var i0J="ess";var G0J="uploa";var l0J="read";var D0J="ngt";var e0J="plo";var v0J="aj";var F0J="rea";var z0J="safeId";var j0J="attr";var y3J='value';var H3J="pairs";var P3J="namespace";var J3J='files()';var r3J='rows().delete()';var n3J='remove';var c3J="emove";var a3J='edit';var q3J='create';var x3J="confirm";var D3J="as";var E3J="_editor";var e3J="editor";var m3J="register";var W3J="Api";var O3J="_processing";var Q3J="sin";var Z3J="show";var I3J="ton";var w3J="_event";var M3J='node';var F3J="_actionClass";var z3J="_tidy";var y4J="ven";var t4J="remove";var o4J='-';var B4J="join";var k4J="editOpts";var K4J='main';var n4J="pla";var c4J="ope";var a4J="_eventName";var G4J="_even";var l4J="Object";var E4J="rray";var e4J="ield";var m4J="multiGet";var V4J="_postopen";var h4J="addBack";var I4J='click';var Y4J="nts";var w4J='div.';var M4J='.';var F4J="find";var y5J="_preopen";var H5J="tac";var J5J="cu";var k5J="_fo";var K5J='inline';var q5J="att";var l5J="indivi";var X5J="ormE";var L5J="inError";var m5J='#';var W5J="Fields";var Q5J="fiel";var Z5J="enable";var T5J='fields';var A5J="ain";var F5J="map";var z5J="displayed";var j5J="ach";var t1J="disable";var o1J="template";var k1J="destroy";var K1J="url";var N1J="then";var U1J="rows";var r1J='data';var n1J="rge";var q1J="va";var L1J="ea";var g1J="ssi";var v1J="proce";var V1J='change';var h1J='json';var O1J='POST';var Q1J="ngth";var C1J="end";var Z1J="no";var d1J="maybeOpen";var T1J="_assembleMain";var I1J="pt";var Y1J="multiSet";var s1J="Fi";var A1J="ed";var w1J="elds";var M1J="editFields";var F1J="fields";var H7J="act";var S7J="_eve";var o7J="create";var B7J="_fieldNames";var P7J="lic";var J7J="sp";var r7J='string';var n7J="clear";var c7J="ult";var q7J="event";var G7J="preventDefault";var l7J="keyCode";var E7J="ring";var e7J="xt";var L7J="butto";var W7J="tio";var R7J="ndex";var v7J="up";var V7J="key";var h7J='_basic';var f7J="em";var T7J="dClass";var I7J="outerWidth";var A7J="left";var w7J="get";var M7J="ft";var j7J="tt";var o9J="ength";var B9J="gth";var k9J="includeFields";var K9J="_cl";var c9J="_an";var a9J="_closeReg";var q9J="buttons";var b9J="formInfo";var x9J="prepend";var l9J="form";var D9J="eq";var X9J="To";var E9J="ppendTo";var e9J='" />';var m9J='<div class="';var h9J="_formOptions";var C9J="clas";var s9J="div";var A9J="oin";var w9J="mEr";var M9J="for";var z9J="mes";var j9J="ns";var y8J="butt";var S8J="ub";var p8J='bubble';var o8J="_edit";var B8J="_dataSource";var P8J="isPlainObject";var J8J="_tid";var K8J="tions";var N8J="dual";var r8J="bubble";var n8J="_blur";var c8J='submit';var a8J="ur";var b8J="edi";var x8J="ajax";var l8J="_displayReorder";var D8J="inArray";var X8J="unshift";var E8J="order";var m8J="ta";var W8J="da";var v8J="multiReset";var h8J="field";var s8J="lds";var A8J="dd";var w8J="eng";var M8J="isArray";var z8J="add";var j8J="row";var S6J="node";var p6J="modifier";var o6J="ow";var B6J="abl";var P6J="action";var J6J="header";var k6J="attach";var K6J="reate";var N6J="ba";var q6J="ff";var G6J="clos";var x6J="ind";var X6J="outerHeight";var e6J="height";var R6J="co";var f6J="div.";var Q6J="ght";var A6J="lose";var z6J="offset";var t2Z="top";var S2Z="width";var J2Z="splay";var u2Z="fin";var a2Z="off";var D2Z="ppe";var L2Z="lo";var h2Z=".";var f2Z='block';var Q2Z="display";var C2Z="style";var T2Z="body";var t0Z="hide";var S0Z="ent";var B0Z="pend";var J0Z="init";var k0Z="displayController";var U0Z="<di";var r0Z="dataTable";var G0Z='resize.DTED_Lightbox';var b0Z="unbind";var E0Z="appendTo";var e0Z="ldr";var W0Z="at";var f0Z="of";var Q0Z="ani";var I0Z="div.DTED_";var Y0Z="unb";var w0Z="cli";var M0Z='div.DTE_Body_Content';var z0Z="ight";var j0Z="he";var i3Z="der";var o3Z="ou";var B3Z="pp";var P3Z="wra";var u3Z="ma";var r3Z="backgr";var b3Z="_d";var l3Z="target";var D3Z='div.DTED_Lightbox_Content_Wrapper';var e3Z='click.DTED_Lightbox';var L3Z="bind";var v3Z="div.D";var V3Z="background";var h3Z="_animate";var O3Z="_heightCalc";var f3Z="conf";var Z3Z="ht";var I3Z="ie";var A3Z="dy";var w3Z="bo";var M3Z="app";var j3Z="back";var H4Z="wr";var B4Z="per";var P4Z="_Lightbox";var K4Z="bi";var r4Z="resize";var n4Z="ody";var q4Z="content";var G4Z="_ready";var D4Z="rappe";var X4Z="und";var W4Z="dte";var R4Z="hi";var v4Z="_show";var V4Z="close";var h4Z="append";var O4Z="children";var f4Z="_dom";var Q4Z="_dte";var C4Z="tent";var Z4Z="appe";var I4Z="disp";var A4Z="troller";var F4Z="<div cla";var z4Z="</di";var t5Z="displa";var i5Z="nf";var S5Z='focus';var p5Z='close';var o5Z='blur';var B5Z="button";var P5Z="fieldType";var J5Z="settings";var k5Z="text";var K5Z="ho";var u5Z="slice";var U5Z="un";var r5Z="opt";var n5Z="_multiInfo";var x5Z="op";var l5Z="ue";var D5Z="rn";var X5Z="tu";var e5Z="oc";var L5Z="Info";var g5Z="tml";var h5Z="ml";var f5Z="table";var Q5Z="host";var C5Z="ction";var Z5Z="pa";var d5Z="im";var I5Z="submit";var w5Z="ock";var j5Z="iner";var y1Z="onta";var i1Z="ge";var S1Z="lay";var o1Z="st";var K1Z="len";var u1Z="ra";var a1Z='&';var b1Z="replace";var l1Z="repla";var D1Z="repl";var X1Z="ac";var E1Z="pl";var e1Z="alue";var W1Z="cessi";var R1Z="isplay";var g1Z="ck";var v1Z="blo";var V1Z="one";var h1Z="do";var O1Z="containe";var Q1Z="sh";var C1Z="pu";var Z1Z="rra";var d1Z="inA";var T1Z="multiValues";var s1Z="isP";var M1Z="tiValue";var F1Z="isMultiVal";var H7Z="lti";var t7Z="isMu";var p7Z="ab";var o7Z="html";var B7Z="detach";var J7Z="en";var k7Z="ap";var K7Z='none';var u7Z="slideUp";var N7Z="ost";var U7Z="displ";var a7Z="ocus";var q7Z="us";var b7Z='input';var x7Z="ut";var l7Z="inp";var E7Z="error";var e7Z="ds";var m7Z="leng";var W7Z="_msg";var g7Z="removeClass";var v7Z="ner";var V7Z="contai";var h7Z="rror";var O7Z="addClass";var Q7Z="con";var d7Z="_m";var T7Z="om";var s7Z="cont";var w7Z="lass";var M7Z="led";var F7Z="dis";var z7Z="disabled";var j7Z="container";var y9Z="ass";var H9Z="oveCl";var t9Z="rem";var i9Z="classe";var S9Z="bl";var p9Z='body';var B9Z="paren";var P9Z="h";var J9Z="classes";var k9Z="ine";var K9Z="conta";var u9Z="ss";var U9Z="is";var c9Z="ion";var q9Z="fu";var G9Z="prototype";var b9Z="apply";var x9Z="_typeFn";var l9Z="call";var D9Z="sl";var e9Z='function';var R9Z="focus";var g9Z="val";var v9Z='readonly';var V9Z="hasClass";var h9Z="multiEditable";var O9Z="opts";var f9Z="ble";var Q9Z="isa";var T9Z="models";var I9Z="dom";var Y9Z='display';var s9Z="css";var A9Z="ntrol";var F9Z=null;var j9Z="processing";var t8Z="message";var o8Z='</span>';var B8Z="multiInfo";var J8Z="title";var k8Z="multiValue";var u8Z='"/>';var r8Z="input";var n8Z='</label>';var c8Z='</div>';var q8Z='">';var G8Z="label";var l8Z=' ';var D8Z="wrapper";var X8Z="_fnSetObjectDataFn";var e8Z="_fnGetObjectDataFn";var L8Z="edit";var m8Z="valFromData";var W8Z="oApi";var R8Z="data";var g8Z="na";var v8Z="ata";var h8Z="dat";var Q8Z="name";var C8Z="extend";var I8Z="multi";var Y8Z="i18n";var s8Z="xten";var A8Z="fault";var w8Z="ypes";var M8Z="T";var F8Z="eld";var z8Z="fi";var j8Z="gs";var y6Z="in";var H6Z="set";var i6Z="<div clas";var S6Z="nam";var o6Z="cla";var B6Z="ass=\"";var K6Z="Id";var N6Z="lab";var n6Z="class=\"";var q6Z="iv>";var G6Z="</";var b6Z="=\"";var l6Z="<div data-dte";var e6Z="/d";var m6Z="v>";var R6Z="s=\"";var g6Z="las";var O6Z=">";var f6Z="iv";var Q6Z="</d";var C6Z="div>";var Z6Z="/";var d6Z="<";var I6Z="_t";var Y6Z="ate";var s6Z="cre";var M6Z="essage";var j6Z="mult";var i2n="mul";var S2n="lt";var p2n="ick";var o2n="ic";var B2n=true;var J2n=false;var k2n="length";var K2n="ct";var u2n="ect";var U2n="th";var r2n="ng";var x2n="files";var l2n="push";var D2n="each";var E2n="]";var e2n="\"";var L2n="DataTable";var g2n="on";var v2n="_c";var V2n='Editor requires DataTables 1.10.7 or newer';var h2n='1.10.7';var O2n="versionCheck";var f2n="fn";var Q2n='s';var C2n='';var I2n="og";var A2n="re";var w2n=" ";var t0n="2";var J0n="ca";var k0n="ce";var r0n="g";var q0n="dataTab";var G0n="k";var b0n="hec";var x0n="versionC";var l0n="ito";var D0n="ditor";var X0n="Fiel";var E0n="ld";var e0n="Fie";var L0n="els";var m0n="el";var W0n="F";var R0n="ults";var g0n="def";var v0n="odel";var V0n="ls";var h0n="oller";var O0n="tr";var f0n="splayCon";var Q0n="dels";var C0n="mo";var Z0n="mod";var d0n="ions";var T0n="rmOpt";var I0n="bmit";var Y0n="se";var s0n="cl";var A0n="al";var w0n="la";var M0n="isp";var F0n="nd";var z0n="ackgro";var j0n="blu";var y3n="bblePosition";var H3n="bu";var t3n="pende";var i3n="toty";var S3n="ay";var p3n="spl";var o3n="yNod";var B3n="spla";var P3n="ile";var J3n="prototyp";var k3n="id";var K3n="line";var u3n="ssage";var N3n="pro";var U3n="modifi";var r3n="otyp";var n3n="rot";var c3n="od";var a3n="pen";var q3n="ord";var G3n="et";var b3n="bm";var x3n="templa";var l3n="le";var D3n="ti";var X3n="rototype";var E3n="itor(";var e3n="eate()";var L3n="ow.cr";var m3n="dit()";var W3n=".e";var R3n="w()";var g3n="t()";var v3n="s().edi";var V3n="w";var h3n="row().delete(";var O3n=".edit()";var f3n="l(";var Q3n="cel";var C3n="ells().edit()";var Z3n=")";var d3n="file(";var T3n=".d";var I3n="hr";var Y3n="ror";var s3n="er";var A3n="ad";var w3n="uplo";var M3n="ty";var F3n="ax";var z3n="j";var j3n="otype";var y4n="totype";var H4n="Reg";var t4n="_close";var i4n="prototy";var S4n="_dataSou";var p4n="ot";var o4n="prot";var B4n="entName";var P4n="v";var J4n="to";var k4n="ro";var K4n="de";var u4n="FromNo";var N4n="_field";var U4n="ototype";var r4n="yp";var n4n="jax";var c4n="_legacyA";var a4n="pe";var q4n="rotot";var G4n="tiInfo";var b4n="_mul";var x4n="reop";var l4n="_p";var D4n="ype";var X4n="protot";var E4n="mit";var e4n="_sub";var L4n="ubmitSuccess";var m4n="_s";var W4n="ototyp";var R4n="pr";var g4n="type";var v4n="proto";var V4n="Arra";var h4n="_weakI";var O4n="wI";var f4n="T_Ro";var Q4n="try";var C4n="it en";var Z4n="Dele";var d4n="ete";var T4n="Del";var I4n="ete %d rows?";var Y4n="u wish to de";var s4n="Are you sure yo";var A4n="you wish to delete 1 row?";var w4n="Are you sure ";var M4n="\">More information</a>).";var F4n="atatables.net/tn/12";var z4n="A system error has occurred (<a target=\"_blank\" href=\"//d";var j4n="lue";var y5n="Multiple va";var H5n="es";var t5n="Undo chang";var i5n=" group.";var S5n="put can be edited individually, but not part of a";var p5n="This in";var o5n="ext";var B5n="N";var P5n="ry";var J5n="brua";var k5n="Fe";var K5n="il";var u5n="J";var N5n="gust";var U5n="A";var r5n="ber";var n5n="Nove";var c5n="M";var a5n="p";var q5n="tend";var G5n="ex";var b5n="mode";var x5n="ormOptions";var l5n="f";var D5n="cha";var X5n="ons";var E5n="rmOpti";var e5n="nged";var L5n="ch";var m5n="d";var W5n="exten";var R5n="ndicat";var g5n="g_I";var v5n="DTE_Processin";var V5n="y";var h5n="E_Bod";var O5n="Con";var f5n="DTE_Body_";var Q5n="Footer";var C5n="_";var Z5n="nt";var d5n="te";var T5n="DTE_Footer_Con";var I5n="Content";var Y5n="Form_";var s5n="rm_Info";var A5n="Fo";var w5n="E_";var M5n="b";var F5n="TE_Field_Type_";var z5n="me_";var j5n="E_Field_N";var y1n="trol";var H1n="tCon";var t1n="DTE_Field_Inpu";var i1n="or";var S1n="d_StateErr";var p1n="iel";var o1n="DTE_F";var B1n="fo";var P1n="Label_In";var J1n="_Info";var k1n="DTE_";var K1n="info";var u1n="lti-";var N1n="mu";var U1n="ti-noEdit";var r1n="u";var n1n="m";var c1n="bled";var a1n="a";var q1n="s";var G1n="di";var b1n="icator";var x1n="DTE_Processing_Ind";var l1n="eate";var D1n="DTE_Action_Cr";var X1n="ove";var E1n="_Action_Rem";var e1n="E";var L1n="DT";var m1n="ne";var W1n="TE DTE_Inli";var R1n="D";var g1n="Field";var v1n="DTE_Inline_";var V1n="ose";var h1n="l";var O1n="n c";var f1n="ico";var Q1n="_Triangle";var C1n="DTE_Bubble";var Z1n="me";var d1n="Ti";var T1n="Date";var I1n="Time";var Y1n="Dat";var s1n="ts";var A1n="defaul";var w1n="8";var M1n="1";var F1n="i";var z1n="tim";var j1n="date";var y7n="n";var H7n="x";var t7n="e";var i7n="S";var S7n="CLA";var p7n="r";var o7n="o";var B7n="it";var P7n="Ed";var e7n=500;var L7n=400;var W7n=100;var g7n=60;var T7n=27;var Y7n=24;var A7n=20;var F7n=13;var z7n=12;var j7n=11;var y9n=10;var t9n=7;var i9n=4;var S9n=3;var p9n=2;var o9n=1;var B9n=0;var P9n=P7n;P9n+=B7n;P9n+=o7n;P9n+=p7n;var J9n=S7n;J9n+=i7n;J9n+=i7n;var k9n=t7n;k9n+=H7n;k9n+=R13.q7n;var U9n=t7n;U9n+=H7n;U9n+=R13.q7n;var V4x=t7n;V4x+=y7n;var h4x=j1n;h4x+=z1n;h4x+=t7n;var O4x=F1n;O4x+=M1n;O4x+=w1n;O4x+=y7n;var f4x=A1n;f4x+=s1n;var Q4x=Y1n;Q4x+=t7n;Q4x+=I1n;var M8x=T1n;M8x+=d1n;M8x+=Z1n;var I0h=C1n;I0h+=Q1n;var Y0h=f1n;Y0h+=O1n;Y0h+=h1n;Y0h+=V1n;var s0h=v1n;s0h+=g1n;var A0h=R1n;A0h+=W1n;A0h+=m1n;var w0h=L1n;w0h+=e1n;w0h+=E1n;w0h+=X1n;var M0h=D1n;M0h+=l1n;var F0h=x1n;F0h+=b1n;var z0h=G1n;z0h+=q1n;z0h+=a1n;z0h+=c1n;var j0h=n1n;j0h+=r1n;j0h+=h1n;j0h+=U1n;var y3h=N1n;y3h+=u1n;y3h+=K1n;var H3h=k1n;H3h+=g1n;H3h+=J1n;var t3h=k1n;t3h+=P1n;t3h+=B1n;var i3h=o1n;i3h+=p1n;i3h+=S1n;i3h+=i1n;var S3h=t1n;S3h+=H1n;S3h+=y1n;var p3h=L1n;p3h+=j5n;p3h+=a1n;p3h+=z5n;var o3h=R1n;o3h+=F5n;var B3h=M5n;B3h+=R13.q7n;B3h+=y7n;var P3h=L1n;P3h+=w5n;P3h+=A5n;P3h+=s5n;var J3h=k1n;J3h+=Y5n;J3h+=I5n;var k3h=T5n;k3h+=d5n;k3h+=Z5n;var K3h=L1n;K3h+=e1n;K3h+=C5n;K3h+=Q5n;var u3h=f5n;u3h+=O5n;u3h+=d5n;u3h+=Z5n;var N3h=L1n;N3h+=h5n;N3h+=V5n;var U3h=v5n;U3h+=g5n;U3h+=R5n;U3h+=i1n;var H1h=W5n;H1h+=m5n;var t1h=L5n;t1h+=a1n;t1h+=e5n;var i1h=B1n;i1h+=E5n;i1h+=X5n;var S1h=D5n;S1h+=e5n;var p1h=l5n;p1h+=x5n;var o1h=b5n;o1h+=h1n;o1h+=q1n;var B1h=G5n;B1h+=q5n;var P1h=a5n;P1h+=n1n;var J1h=i7n;J1h+=a1n;J1h+=R13.q7n;var k1h=c5n;k1h+=o7n;k1h+=y7n;var K1h=n5n;K1h+=n1n;K1h+=r5n;var u1h=U5n;u1h+=r1n;u1h+=N5n;var N1h=u5n;N1h+=r1n;N1h+=y7n;N1h+=t7n;var U1h=U5n;U1h+=a5n;U1h+=p7n;U1h+=K5n;var r1h=k5n;r1h+=J5n;r1h+=P5n;var n1h=B5n;n1h+=o5n;var c1h=p5n;c1h+=S5n;c1h+=i5n;var a1h=t5n;a1h+=H5n;var q1h=y5n;q1h+=j4n;q1h+=q1n;var G1h=z4n;G1h+=F4n;G1h+=M4n;var b1h=w4n;b1h+=A4n;var x1h=s4n;x1h+=Y4n;x1h+=h1n;x1h+=I4n;var l1h=T4n;l1h+=d4n;var D1h=Z4n;D1h+=d5n;var X1h=P7n;X1h+=C4n;X1h+=Q4n;var E1h=e1n;E1h+=G1n;E1h+=R13.q7n;var e1h=R1n;e1h+=f4n;e1h+=O4n;e1h+=m5n;var m1h=h4n;m1h+=y7n;m1h+=V4n;m1h+=V5n;var W1h=v4n;W1h+=g4n;var w1h=R4n;w1h+=W4n;w1h+=t7n;var S9h=m4n;S9h+=L4n;var i8h=e4n;i8h+=E4n;var S8h=X4n;S8h+=D4n;var n8h=l4n;n8h+=x4n;n8h+=t7n;n8h+=y7n;var h8h=b4n;h8h+=G4n;var O8h=a5n;O8h+=q4n;O8h+=V5n;O8h+=a4n;var u6h=c4n;u6h+=n4n;var N6h=v4n;N6h+=R13.q7n;N6h+=r4n;N6h+=t7n;var n2c=R4n;n2c+=U4n;var G2c=N4n;G2c+=u4n;G2c+=K4n;var b2c=a5n;b2c+=k4n;b2c+=J4n;b2c+=g4n;var X2c=C5n;X2c+=t7n;X2c+=P4n;X2c+=B4n;var E2c=o4n;E2c+=p4n;E2c+=D4n;var Q2c=o4n;Q2c+=p4n;Q2c+=D4n;var e0c=S4n;e0c+=p7n;e0c+=R13.U7n;e0c+=t7n;var L0c=i4n;L0c+=a4n;var h0c=t4n;h0c+=H4n;var M0c=o4n;M0c+=o7n;M0c+=g4n;var k3c=R4n;k3c+=o7n;k3c+=y4n;var c3c=a5n;c3c+=k4n;c3c+=R13.q7n;c3c+=j3n;var y4c=C5n;y4c+=a1n;y4c+=z3n;y4c+=F3n;var H4c=v4n;H4c+=M3n;H4c+=a5n;H4c+=t7n;var E1c=w3n;E1c+=A3n;var g1c=s3n;g1c+=Y3n;var h1c=H7n;h1c+=I3n;h1c+=T3n;h1c+=R13.q7n;var O1c=d3n;O1c+=Z3n;var Q1c=R13.U7n;Q1c+=C3n;var T1c=Q3n;T1c+=f3n;T1c+=Z3n;T1c+=O3n;var s1c=h3n;s1c+=Z3n;var A1c=k4n;A1c+=V3n;A1c+=v3n;A1c+=g3n;var w1c=k4n;w1c+=R3n;w1c+=W3n;w1c+=m3n;var M1c=p7n;M1c+=L3n;M1c+=e3n;var F1c=t7n;F1c+=m5n;F1c+=E3n;F1c+=Z3n;var B7c=P4n;B7c+=a1n;B7c+=h1n;var P7c=a5n;P7c+=X3n;var c7c=D3n;c7c+=R13.q7n;c7c+=l3n;var q7c=x3n;q7c+=d5n;var L7c=q1n;L7c+=r1n;L7c+=b3n;L7c+=B7n;var v7c=q1n;v7c+=G3n;var H9c=q3n;H9c+=s3n;var t9c=R4n;t9c+=W4n;t9c+=t7n;var u9c=o7n;u9c+=a3n;var N9c=R4n;N9c+=o7n;N9c+=y4n;var U9c=a5n;U9c+=p7n;U9c+=U4n;var r9c=X4n;r9c+=D4n;var c9c=a5n;c9c+=X3n;var x9c=y7n;x9c+=c3n;x9c+=t7n;var E9c=a5n;E9c+=n3n;E9c+=r3n;E9c+=t7n;var R9c=v4n;R9c+=g4n;var v9c=U3n;v9c+=s3n;var V9c=v4n;V9c+=g4n;var O9c=N3n;O9c+=R13.q7n;O9c+=o7n;O9c+=g4n;var C9c=n1n;C9c+=t7n;C9c+=u3n;var L8c=F1n;L8c+=y7n;L8c+=K3n;var m8c=o4n;m8c+=o7n;m8c+=g4n;var V8c=o4n;V8c+=o7n;V8c+=R13.q7n;V8c+=D4n;var f8c=k3n;f8c+=q1n;var d8c=J3n;d8c+=t7n;var Y8c=l5n;Y8c+=P3n;var w8c=i4n;w8c+=a4n;var t6c=X4n;t6c+=V5n;t6c+=a4n;var P6c=i4n;P6c+=a4n;var K6c=G1n;K6c+=B3n;K6c+=o3n;K6c+=t7n;var u6c=R4n;u6c+=o7n;u6c+=y4n;var r6c=m5n;r6c+=F1n;r6c+=p3n;r6c+=S3n;var n6c=a5n;n6c+=k4n;n6c+=i3n;n6c+=a4n;var e6c=a5n;e6c+=q4n;e6c+=r4n;e6c+=t7n;var o2Y=m5n;o2Y+=t7n;o2Y+=t3n;o2Y+=Z5n;var D2Y=a5n;D2Y+=X3n;var X2Y=N3n;X2Y+=R13.q7n;X2Y+=j3n;var m0Y=H3n;m0Y+=y3n;var X3Y=j0n;X3Y+=p7n;var W3Y=M5n;W3Y+=z0n;W3Y+=r1n;W3Y+=F0n;var z3Y=l5n;z3Y+=y7n;var u1Y=l5n;u1Y+=y7n;var F7Y=m5n;F7Y+=M0n;F7Y+=w0n;F7Y+=V5n;var z7Y=p7n;z7Y+=o7n;z7Y+=V3n;var j7Y=A0n;j7Y+=h1n;var y9Y=s0n;y9Y+=o7n;y9Y+=Y0n;var H9Y=q1n;H9Y+=r1n;H9Y+=I0n;var t9Y=B1n;t9Y+=T0n;t9Y+=d0n;var i9Y=Z0n;i9Y+=t7n;i9Y+=h1n;i9Y+=q1n;var S9Y=C0n;S9Y+=Q0n;var p9Y=G1n;p9Y+=f0n;p9Y+=O0n;p9Y+=h0n;var o9Y=C0n;o9Y+=K4n;o9Y+=V0n;var B9Y=m5n;B9Y+=o7n;B9Y+=n1n;var P9Y=n1n;P9Y+=v0n;P9Y+=q1n;var J9Y=g0n;J9Y+=a1n;J9Y+=R0n;var k9Y=W0n;k9Y+=F1n;k9Y+=m0n;k9Y+=m5n;var K9Y=Z0n;K9Y+=L0n;var u9Y=e0n;u9Y+=E0n;var z6Y=W0n;z6Y+=F1n;z6Y+=t7n;z6Y+=E0n;var c0=X0n;c0+=m5n;var l0=e1n;l0+=D0n;var D0=e1n;D0+=m5n;D0+=l0n;D0+=p7n;var E0=x0n;E0+=b0n;E0+=G0n;var e0=q0n;e0+=l3n;'use strict';R13.x4=function(l4){if(R13)return R13.s8(l4);};R13.X5=function(E5){if(R13)return R13.A8(E5);};(function(){var Z2n=' day';var d2n="1a7f";var T2n='DataTables Editor trial info - ';var Y2n="d3";var s2n="maining";var M2n='Editor - Trial expired';var F2n='Your trial has now expired. To purchase a license ';var z2n='Thank you for trying DataTables Editor\n\n';var j2n="bles.net/purchase";var y0n=" please see https://editor.datata";var H0n="for Editor,";var i0n="4";var S0n="7f9a";var p0n="684b";var o0n="getTime";var B0n=4345204320;var P0n="8f2a";var K0n="9f";var u0n="31";var N0n="5";var U0n="etTim";var n0n="81";var c0n="3";var a0n="9";var x7n=1546473600;var D7n=1000;var R7n=69;var h7n=49;var v0=a0n;v0+=c0n;v0+=n0n;var V0=r0n;V0+=U0n;V0+=t7n;var h0=w1n;h0+=N0n;h0+=u0n;var O0=M5n;O0+=K0n;O0+=M1n;var f0=k0n;f0+=K5n;var Q0=J0n;Q0+=M1n;Q0+=N0n;R13.I0=function(Y0){if(R13)return R13.A8(Y0);};R13.w3=function(M3){if(R13)return R13.A8(M3);};R13.F4=function(z4){if(R13&&z4)return R13.A8(z4);};R13.R1=function(g1){if(R13&&g1)return R13.A8(g1);};R13.o7=function(B7){if(R13&&B7)return R13.A8(B7);};R13.h7=function(O7){if(R13)return R13.s8(O7);};var remaining=Math[R13.h7(Q0)?R13.K7n:f0]((new Date((R13.o7(P0n)?x7n:B0n)*D7n)[R13.R1(O0)?R13.K7n:o0n]()-new Date()[R13.H1(h0)?V0:R13.K7n]())/(D7n*(R13.X5(p0n)?g7n:R7n)*(R13.F4(S0n)?g7n:h7n)*Y7n));if(remaining<=(R13.x4(v0)?B9n:p9n)){var R0=i0n;R0+=t0n;R0+=R13.c7n;R0+=R13.U7n;var g0=H0n;g0+=y0n;g0+=j2n;alert(z2n+F2n+g0);throw R13.w3(R0)?R13.K7n:M2n;}else if(remaining<=t9n){var L0=w2n;L0+=A2n;L0+=s2n;var m0=l5n;m0+=Y2n;m0+=R13.U7n;var W0=h1n;W0+=I2n;console[W0]((R13.q3(m0)?T2n:R13.K7n)+remaining+(R13.I0(d2n)?Z2n:R13.K7n)+(remaining===o9n?C2n:Q2n)+L0);}}());var DataTable=$[f2n][e0];if(!DataTable||!DataTable[O2n]||!DataTable[E0](h2n)){throw V2n;}var Editor=function(opts){var m2n="DataTables Editor must be initialised as a 'new' instance'";var W2n="ctor";var R2n="stru";var X0=v2n;X0+=g2n;X0+=R2n;X0+=W2n;if(!(this instanceof Editor)){alert(m2n);}this[X0](opts);};DataTable[D0]=Editor;$[f2n][L2n][l0]=Editor;var _editor_el=function(dis,ctx){var X2n='*[data-dte-e="';var x0=e2n;x0+=E2n;if(ctx===undefined){ctx=document;}return $(X2n+dis+x0,ctx);};var __inlineCounter=B9n;var _pluck=function(a,prop){var out=[];$[D2n](a,function(idx,el){out[l2n](el[prop]);});return out;};var _api_file=function(name,id){var a2n=' in table ';var q2n="file id ";var G2n="n ";var b2n="Unknow";var table=this[x2n](name);var file=table[id];if(!file){var b0=b2n;b0+=G2n;b0+=q2n;throw b0+id+a2n+name;}return table[id];};var _api_files=function(name){var c2n='Unknown file table name: ';if(!name){return Editor[x2n];}var table=Editor[x2n][name];if(!table){throw c2n+name;}return table;};var _objectKeys=function(o){var n2n="hasOwnProperty";var out=[];for(var key in o){if(o[n2n](key)){out[l2n](key);}}return out;};var _deepCompare=function(o1,o2){var P2n='object';var N2n="ob";var a0=h1n;a0+=t7n;a0+=r2n;a0+=U2n;var q0=N2n;q0+=z3n;q0+=u2n;var G0=N2n;G0+=z3n;G0+=t7n;G0+=K2n;if(typeof o1!==G0||typeof o2!==q0){return o1==o2;}var o1Props=_objectKeys(o1);var o2Props=_objectKeys(o2);if(o1Props[k2n]!==o2Props[a0]){return J2n;}for(var i=B9n,ien=o1Props[k2n];i<ien;i++){var propName=o1Props[i];if(typeof o1[propName]===P2n){if(!_deepCompare(o1[propName],o2[propName])){return J2n;}}else if(o1[propName]!=o2[propName]){return J2n;}}return B2n;};Editor[c0]=function(opts,classes,host){var W9Z="multiReturn";var C9Z='field-processing';var Z9Z='msg-error';var d9Z='msg-label';var w9Z="input-co";var M9Z="epen";var z9Z='"><span/></div>';var y8Z='<div data-dte-e="field-processing" class="';var H8Z='msg-info';var i8Z='msg-message';var S8Z='<div data-dte-e="msg-message" class="';var p8Z='<div data-dte-e="msg-error" class="';var P8Z='<span data-dte-e="multi-info" class="';var K8Z='<div data-dte-e="multi-value" class="';var N8Z="inputControl";var U8Z='<div data-dte-e="input-control" class="';var a8Z='<div data-dte-e="msg-label" class="';var b8Z="namePrefix";var x8Z="typePrefix";var E8Z="valToData";var V8Z="aPro";var O8Z="dataProp";var f8Z="DTE_Fie";var Z8Z="eld - unknown field type ";var d8Z="ng fi";var T8Z="Error addi";var t6Z="eldType";var p6Z="ssNam";var P6Z="abel\" cl";var J6Z="<label data-dte-e=\"l";var k6Z=" for=\"";var u6Z="afe";var U6Z="In";var r6Z="abel";var c6Z="t\" ";var a6Z="<div data-dte-e=\"inpu";var x6Z="-e=\"msg-multi\" class";var D6Z="estor";var X6Z="ltiR";var E6Z="esto";var L6Z="msg-";var W6Z="\"></";var v6Z="g-info\" c";var V6Z="<div data-dte-e=\"ms";var h6Z="fieldIn";var T6Z="Fn";var A6Z="nput-control";var w6Z="abe";var F6Z="g-";var z6Z="i-val";var y2n="-mul";var H2n="ms";var t2n="ti-in";var t2=R13.q7n;t2+=D4n;var S2=s0n;S2+=o2n;S2+=G0n;var p2=o7n;p2+=y7n;var B2=m5n;B2+=o7n;B2+=n1n;var k2=s0n;k2+=p2n;var K2=o7n;K2+=y7n;var u2=N1n;u2+=S2n;u2+=F1n;var N2=i2n;N2+=t2n;N2+=B1n;var U2=H2n;U2+=r0n;U2+=y2n;U2+=D3n;var r2=j6Z;r2+=z6Z;r2+=r1n;r2+=t7n;var n2=H2n;n2+=F6Z;n2+=n1n;n2+=M6Z;var c2=h1n;c2+=w6Z;c2+=h1n;var a2=F1n;a2+=A6Z;var q2=m5n;q2+=o7n;q2+=n1n;var l2=s6Z;l2+=Y6Z;var D2=I6Z;D2+=D4n;D2+=T6Z;var X2=d6Z;X2+=Z6Z;X2+=C6Z;var E2=Q6Z;E2+=f6Z;E2+=O6Z;var e2=h6Z;e2+=B1n;var L2=V6Z;L2+=v6Z;L2+=g6Z;L2+=R6Z;var m2=W6Z;m2+=G1n;m2+=m6Z;var W2=L6Z;W2+=s3n;W2+=Y3n;var R2=d6Z;R2+=e6Z;R2+=F1n;R2+=m6Z;var g2=p7n;g2+=E6Z;g2+=p7n;g2+=t7n;var v2=e2n;v2+=O6Z;var V2=N1n;V2+=X6Z;V2+=D6Z;V2+=t7n;var h2=l6Z;h2+=x6Z;h2+=b6Z;var O2=G6Z;O2+=m5n;O2+=q6Z;var f2=e2n;f2+=O6Z;var Q2=a6Z;Q2+=c6Z;Q2+=n6Z;var C2=h1n;C2+=r6Z;C2+=U6Z;C2+=B1n;var Z2=e2n;Z2+=O6Z;var d2=L6Z;d2+=N6Z;d2+=t7n;d2+=h1n;var T2=F1n;T2+=m5n;var I2=q1n;I2+=u6Z;I2+=K6Z;var Y2=e2n;Y2+=k6Z;var s2=J6Z;s2+=P6Z;s2+=B6Z;var A2=e2n;A2+=O6Z;var w2=o6Z;w2+=p6Z;w2+=t7n;var M2=S6Z;M2+=t7n;var F2=M3n;F2+=a4n;var z2=i6Z;z2+=R6Z;var H0=t7n;H0+=H7n;H0+=R13.q7n;var i0=m5n;i0+=a1n;i0+=R13.q7n;i0+=a1n;var P0=F1n;P0+=m5n;var J0=R13.q7n;J0+=V5n;J0+=a4n;var k0=l5n;k0+=F1n;k0+=t6Z;k0+=q1n;var K0=H6Z;K0+=R13.q7n;K0+=y6Z;K0+=j8Z;var N0=z8Z;N0+=F8Z;N0+=M8Z;N0+=w8Z;var U0=m5n;U0+=t7n;U0+=A8Z;U0+=q1n;var r0=W0n;r0+=F1n;r0+=t7n;r0+=E0n;var n0=t7n;n0+=s8Z;n0+=m5n;var that=this;var multiI18n=host[Y8Z][I8Z];opts=$[n0](B2n,{},Editor[r0][U0],opts);if(!Editor[N0][opts[g4n]]){var u0=T8Z;u0+=d8Z;u0+=Z8Z;throw u0+opts[g4n];}this[q1n]=$[C8Z]({},Editor[g1n][K0],{type:Editor[k0][opts[J0]],name:opts[Q8Z],classes:classes,host:host,opts:opts,multiValue:J2n});if(!opts[P0]){var o0=y7n;o0+=a1n;o0+=n1n;o0+=t7n;var B0=f8Z;B0+=h1n;B0+=m5n;B0+=C5n;opts[k3n]=B0+opts[o0];}if(opts[O8Z]){var S0=h8Z;S0+=V8Z;S0+=a5n;var p0=m5n;p0+=v8Z;opts[p0]=opts[S0];}if(opts[i0]===C2n){var t0=g8Z;t0+=Z1n;opts[R8Z]=opts[t0];}var dtPrivateApi=DataTable[H0][W8Z];this[m8Z]=function(d){var j2=L8Z;j2+=o7n;j2+=p7n;var y0=m5n;y0+=v8Z;return dtPrivateApi[e8Z](opts[y0])(d,j2);};this[E8Z]=dtPrivateApi[X8Z](opts[R8Z]);var template=$(z2+classes[D8Z]+l8Z+classes[x8Z]+opts[F2]+l8Z+classes[b8Z]+opts[M2]+l8Z+opts[w2]+A2+s2+classes[G8Z]+Y2+Editor[I2](opts[T2])+q8Z+opts[G8Z]+a8Z+classes[d2]+Z2+opts[C2]+c8Z+n8Z+Q2+classes[r8Z]+q8Z+U8Z+classes[N8Z]+u8Z+K8Z+classes[k8Z]+q8Z+multiI18n[J8Z]+P8Z+classes[B8Z]+f2+multiI18n[K1n]+o8Z+O2+h2+classes[V2]+v2+multiI18n[g2]+R2+p8Z+classes[W2]+m2+S8Z+classes[i8Z]+q8Z+opts[t8Z]+c8Z+L2+classes[H8Z]+q8Z+opts[e2]+c8Z+E2+y8Z+classes[j9Z]+z9Z+X2);var input=this[D2](l2,opts);if(input!==F9Z){var b2=R4n;b2+=M9Z;b2+=m5n;var x2=w9Z;x2+=A9Z;_editor_el(x2,template)[b2](input);}else{var G2=y7n;G2+=o7n;G2+=y7n;G2+=t7n;template[s9Z](Y9Z,G2);}this[I9Z]=$[C8Z](B2n,{},Editor[g1n][T9Z][q2],{container:template,inputControl:_editor_el(a2,template),label:_editor_el(c2,template),fieldInfo:_editor_el(H8Z,template),labelInfo:_editor_el(d9Z,template),fieldError:_editor_el(Z9Z,template),fieldMessage:_editor_el(n2,template),multi:_editor_el(r2,template),multiReturn:_editor_el(U2,template),multiInfo:_editor_el(N2,template),processing:_editor_el(C9Z,template)});this[I9Z][u2][K2](k2,function(){var P2=R13.q7n;P2+=D4n;var J2=m5n;J2+=Q9Z;J2+=f9Z;J2+=m5n;if(that[q1n][O9Z][h9Z]&&!template[V9Z](classes[J2])&&opts[P2]!==v9Z){that[g9Z](C2n);that[R9Z]();}});this[B2][W9Z][p2](S2,function(){var L9Z="store";var m9Z="ltiRe";var i2=N1n;i2+=m9Z;i2+=L9Z;that[i2]();});$[D2n](this[q1n][t2],function(name,fn){if(typeof fn===e9Z&&that[name]===undefined){that[name]=function(){var X9Z="if";var E9Z="unsh";var j6Y=E9Z;j6Y+=X9Z;j6Y+=R13.q7n;var y2=D9Z;y2+=o2n;y2+=t7n;var H2=R4n;H2+=U4n;var args=Array[H2][y2][l9Z](arguments);args[j6Y](name);var ret=that[x9Z][b9Z](that,args);return ret===undefined?that:ret;};}});};Editor[z6Y][G9Z]={def:function(set){var n9Z='default';var a9Z="nc";var A6Y=m5n;A6Y+=t7n;A6Y+=l5n;var F6Y=o7n;F6Y+=a5n;F6Y+=s1n;var opts=this[q1n][F6Y];if(set===undefined){var w6Y=q9Z;w6Y+=a9Z;w6Y+=R13.q7n;w6Y+=c9Z;var M6Y=K4n;M6Y+=A8Z;var def=opts[n9Z]!==undefined?opts[M6Y]:opts[g0n];return typeof def===w6Y?def():def;}opts[A6Y]=set;return this;},disable:function(){var N9Z="addC";var r9Z="disabl";var T6Y=r9Z;T6Y+=t7n;var I6Y=m5n;I6Y+=U9Z;I6Y+=a1n;I6Y+=c1n;var Y6Y=N9Z;Y6Y+=h1n;Y6Y+=a1n;Y6Y+=u9Z;var s6Y=K9Z;s6Y+=k9Z;s6Y+=p7n;this[I9Z][s6Y][Y6Y](this[q1n][J9Z][I6Y]);this[x9Z](T6Y);return this;},displayed:function(){var o9Z="ontai";var f6Y=y7n;f6Y+=o7n;f6Y+=y7n;f6Y+=t7n;var Q6Y=R13.U7n;Q6Y+=u9Z;var C6Y=l3n;C6Y+=r2n;C6Y+=R13.q7n;C6Y+=P9Z;var Z6Y=B9Z;Z6Y+=s1n;var d6Y=R13.U7n;d6Y+=o9Z;d6Y+=m1n;d6Y+=p7n;var container=this[I9Z][d6Y];return container[Z6Y](p9Z)[C6Y]&&container[Q6Y](Y9Z)!=f6Y?B2n:J2n;},enable:function(){var V6Y=t7n;V6Y+=g8Z;V6Y+=S9Z;V6Y+=t7n;var h6Y=i9Z;h6Y+=q1n;var O6Y=t9Z;O6Y+=H9Z;O6Y+=y9Z;this[I9Z][j7Z][O6Y](this[q1n][h6Y][z7Z]);this[x9Z](V6Y);return this;},enabled:function(){var Y7Z="aine";var A7Z="hasCl";var m6Y=F7Z;m6Y+=a1n;m6Y+=M5n;m6Y+=M7Z;var W6Y=R13.U7n;W6Y+=w7Z;W6Y+=H5n;var R6Y=A7Z;R6Y+=a1n;R6Y+=q1n;R6Y+=q1n;var g6Y=s7Z;g6Y+=Y7Z;g6Y+=p7n;var v6Y=m5n;v6Y+=o7n;v6Y+=n1n;return this[v6Y][g6Y][R6Y](this[q1n][W6Y][m6Y])===J2n;},error:function(msg,fn){var f7Z="tainer";var C7Z="sses";var Z7Z="errorMessag";var I7Z="eldErro";var q6Y=l5n;q6Y+=F1n;q6Y+=I7Z;q6Y+=p7n;var G6Y=m5n;G6Y+=T7Z;var b6Y=d7Z;b6Y+=q1n;b6Y+=r0n;var x6Y=Z7Z;x6Y+=t7n;var L6Y=s0n;L6Y+=a1n;L6Y+=C7Z;var classes=this[q1n][L6Y];if(msg){var X6Y=s3n;X6Y+=Y3n;var E6Y=Q7Z;E6Y+=f7Z;var e6Y=m5n;e6Y+=o7n;e6Y+=n1n;this[e6Y][E6Y][O7Z](classes[X6Y]);}else{var l6Y=t7n;l6Y+=h7Z;var D6Y=V7Z;D6Y+=v7Z;this[I9Z][D6Y][g7Z](classes[l6Y]);}this[x9Z](x6Y,msg);return this[b6Y](this[G6Y][q6Y],msg,fn);},fieldInfo:function(msg){var R7Z="eldIn";var a6Y=z8Z;a6Y+=R7Z;a6Y+=B1n;return this[W7Z](this[I9Z][a6Y],msg);},isMultiValue:function(){var L7Z="ltiI";var n6Y=m7Z;n6Y+=R13.q7n;n6Y+=P9Z;var c6Y=N1n;c6Y+=L7Z;c6Y+=e7Z;return this[q1n][k8Z]&&this[q1n][c6Y][n6Y]!==o9n;},inError:function(){var U6Y=V7Z;U6Y+=v7Z;var r6Y=m5n;r6Y+=o7n;r6Y+=n1n;return this[r6Y][U6Y][V9Z](this[q1n][J9Z][E7Z]);},input:function(){var D7Z="ect, textarea";var X7Z="input, sel";var k6Y=K9Z;k6Y+=k9Z;k6Y+=p7n;var K6Y=X7Z;K6Y+=D7Z;var u6Y=l7Z;u6Y+=x7Z;var N6Y=R13.q7n;N6Y+=r4n;N6Y+=t7n;return this[q1n][N6Y][u6Y]?this[x9Z](b7Z):$(K6Y,this[I9Z][k6Y]);},focus:function(){var c7Z='input, select, textarea';var G7Z="foc";var J6Y=G7Z;J6Y+=q7Z;if(this[q1n][g4n][J6Y]){var P6Y=l5n;P6Y+=a7Z;this[x9Z](P6Y);}else{var o6Y=l5n;o6Y+=o7n;o6Y+=R13.U7n;o6Y+=q7Z;var B6Y=m5n;B6Y+=o7n;B6Y+=n1n;$(c7Z,this[B6Y][j7Z])[o6Y]();}return this;},get:function(){var r7Z="ultiValue";var n7Z="isM";var S6Y=r0n;S6Y+=t7n;S6Y+=R13.q7n;var p6Y=n7Z;p6Y+=r7Z;if(this[p6Y]()){return undefined;}var val=this[x9Z](S6Y);return val!==undefined?val:this[g0n]();},hide:function(animate){var H6Y=l5n;H6Y+=y7n;var t6Y=U7Z;t6Y+=S3n;var i6Y=P9Z;i6Y+=N7Z;var el=this[I9Z][j7Z];if(animate===undefined){animate=B2n;}if(this[q1n][i6Y][t6Y]()&&animate&&$[H6Y][u7Z]){el[u7Z]();}else{el[s9Z](Y9Z,K7Z);}return this;},label:function(str){var P7Z="labelInfo";var j8Y=k7Z;j8Y+=a5n;j8Y+=J7Z;j8Y+=m5n;var y6Y=m5n;y6Y+=o7n;y6Y+=n1n;var label=this[y6Y][G8Z];var labelInfo=this[I9Z][P7Z][B7Z]();if(str===undefined){return label[o7Z]();}label[o7Z](str);label[j8Y](labelInfo);return this;},labelInfo:function(msg){var S7Z="elInfo";var M8Y=h1n;M8Y+=p7Z;M8Y+=S7Z;var F8Y=m5n;F8Y+=o7n;F8Y+=n1n;var z8Y=C5n;z8Y+=n1n;z8Y+=q1n;z8Y+=r0n;return this[z8Y](this[F8Y][M8Y],msg);},message:function(msg,fn){var i7Z="fieldM";var w8Y=i7Z;w8Y+=M6Z;return this[W7Z](this[I9Z][w8Y],msg,fn);},multiGet:function(id){var z1Z="ultiValues";var j1Z="ltiIds";var y7Z="Value";var T8Y=t7Z;T8Y+=H7Z;T8Y+=y7Z;var s8Y=N1n;s8Y+=j1Z;var A8Y=n1n;A8Y+=z1Z;var value;var multiValues=this[q1n][A8Y];var multiIds=this[q1n][s8Y];if(id===undefined){value={};for(var i=B9n;i<multiIds[k2n];i++){var I8Y=P4n;I8Y+=a1n;I8Y+=h1n;var Y8Y=F1Z;Y8Y+=r1n;Y8Y+=t7n;value[multiIds[i]]=this[Y8Y]()?multiValues[multiIds[i]]:this[I8Y]();}}else if(this[T8Y]()){value=multiValues[id];}else{value=this[g9Z]();}return value;},multiRestore:function(){var A1Z="iValu";var w1Z="Chec";var Z8Y=b4n;Z8Y+=M1Z;Z8Y+=w1Z;Z8Y+=G0n;var d8Y=j6Z;d8Y+=A1Z;d8Y+=t7n;this[q1n][d8Y]=B2n;this[Z8Y]();},multiSet:function(id,val){var f1Z="_multiValueCheck";var I1Z="iIds";var Y1Z="inObject";var O8Y=s1Z;O8Y+=w0n;O8Y+=Y1Z;var C8Y=i2n;C8Y+=R13.q7n;C8Y+=I1Z;var multiValues=this[q1n][T1Z];var multiIds=this[q1n][C8Y];if(val===undefined){val=id;id=undefined;}var set=function(idSrc,val){var Q8Y=d1Z;Q8Y+=Z1Z;Q8Y+=V5n;if($[Q8Y](multiIds)===-o9n){var f8Y=C1Z;f8Y+=Q1Z;multiIds[f8Y](idSrc);}multiValues[idSrc]=val;};if($[O8Y](val)&&id===undefined){$[D2n](val,function(idSrc,innerVal){set(idSrc,innerVal);});}else if(id===undefined){$[D2n](multiIds,function(i,idSrc){set(idSrc,val);});}else{set(id,val);}this[q1n][k8Z]=B2n;this[f1Z]();return this;},name:function(){var V8Y=g8Z;V8Y+=Z1n;var h8Y=o7n;h8Y+=a5n;h8Y+=R13.q7n;h8Y+=q1n;return this[q1n][h8Y][V8Y];},node:function(){var g8Y=O1Z;g8Y+=p7n;var v8Y=h1Z;v8Y+=n1n;return this[v8Y][g8Y][B9n];},processing:function(set){var L8Y=y7n;L8Y+=V1Z;var m8Y=v1Z;m8Y+=g1Z;var W8Y=m5n;W8Y+=R1Z;var R8Y=R4n;R8Y+=o7n;R8Y+=W1Z;R8Y+=r2n;this[I9Z][R8Y][s9Z](W8Y,set?m8Y:L8Y);return this;},set:function(val,multiCheck){var J1Z="ultiValueCheck";var k1Z='set';var N1Z="isAr";var U1Z="entityDecode";var L1Z="multiV";var m1Z="ypeF";var G8Y=I6Z;G8Y+=m1Z;G8Y+=y7n;var l8Y=L1Z;l8Y+=e1Z;var decodeFn=function(d){var r1Z='\n';var n1Z='\'';var c1Z='"';var q1Z='<';var G1Z='>';var x1Z="strin";var D8Y=A2n;D8Y+=E1Z;D8Y+=X1Z;D8Y+=t7n;var X8Y=D1Z;X8Y+=a1n;X8Y+=k0n;var E8Y=l1Z;E8Y+=k0n;var e8Y=x1Z;e8Y+=r0n;return typeof d!==e8Y?d:d[b1Z](/&gt;/g,G1Z)[b1Z](/&lt;/g,q1Z)[E8Y](/&amp;/g,a1Z)[X8Y](/&quot;/g,c1Z)[b1Z](/&#39;/g,n1Z)[D8Y](/&#10;/g,r1Z);};this[q1n][l8Y]=J2n;var decode=this[q1n][O9Z][U1Z];if(decode===undefined||decode===B2n){var x8Y=N1Z;x8Y+=u1Z;x8Y+=V5n;if($[x8Y](val)){var b8Y=K1Z;b8Y+=r0n;b8Y+=R13.q7n;b8Y+=P9Z;for(var i=B9n,ien=val[b8Y];i<ien;i++){val[i]=decodeFn(val[i]);}}else{val=decodeFn(val);}}this[G8Y](k1Z,val);if(multiCheck===undefined||multiCheck===B2n){var q8Y=d7Z;q8Y+=J1Z;this[q8Y]();}return this;},show:function(animate){var p1Z="slideDown";var B1Z="wn";var P1Z="deDo";var r8Y=D9Z;r8Y+=F1n;r8Y+=P1Z;r8Y+=B1Z;var n8Y=G1n;n8Y+=p3n;n8Y+=S3n;var c8Y=P9Z;c8Y+=o7n;c8Y+=o1Z;var a8Y=m5n;a8Y+=o7n;a8Y+=n1n;var el=this[a8Y][j7Z];if(animate===undefined){animate=B2n;}if(this[q1n][c8Y][n8Y]()&&animate&&$[f2n][r8Y]){el[p1Z]();}else{var N8Y=m5n;N8Y+=M0n;N8Y+=S1Z;var U8Y=R13.U7n;U8Y+=q1n;U8Y+=q1n;el[U8Y](N8Y,C2n);}return this;},val:function(val){var K8Y=q1n;K8Y+=t7n;K8Y+=R13.q7n;var u8Y=i1Z;u8Y+=R13.q7n;return val===undefined?this[u8Y]():this[K8Y](val);},compare:function(value,original){var t1Z="compare";var k8Y=o7n;k8Y+=a5n;k8Y+=R13.q7n;k8Y+=q1n;var compare=this[q1n][k8Y][t1Z]||_deepCompare;return compare(value,original);},dataSrc:function(){var J8Y=o7n;J8Y+=a5n;J8Y+=R13.q7n;J8Y+=q1n;return this[q1n][J8Y][R8Z];},destroy:function(){var z5Z='destroy';var H1Z="eFn";var o8Y=I6Z;o8Y+=r4n;o8Y+=H1Z;var B8Y=t9Z;B8Y+=o7n;B8Y+=P4n;B8Y+=t7n;var P8Y=R13.U7n;P8Y+=y1Z;P8Y+=j5Z;this[I9Z][P8Y][B8Y]();this[o8Y](z5Z);return this;},multiEditable:function(){var F5Z="tiEditable";var p8Y=i2n;p8Y+=F5Z;return this[q1n][O9Z][p8Y];},multiIds:function(){var M5Z="multiId";var S8Y=M5Z;S8Y+=q1n;return this[q1n][S8Y];},multiInfoShown:function(show){var t8Y=S9Z;t8Y+=w5Z;var i8Y=R13.U7n;i8Y+=q1n;i8Y+=q1n;this[I9Z][B8Z][i8Y]({display:show?t8Y:K7Z});},multiReset:function(){var Y5Z="multiI";var s5Z="ues";var A5Z="tiVa";var y8Y=i2n;y8Y+=A5Z;y8Y+=h1n;y8Y+=s5Z;var H8Y=Y5Z;H8Y+=e7Z;this[q1n][H8Y]=[];this[q1n][y8Y]={};},submittable:function(){return this[q1n][O9Z][I5Z];},valFromData:F9Z,valToData:F9Z,_errorNode:function(){var T5Z="fieldError";var j9Y=m5n;j9Y+=T7Z;return this[j9Y][T5Z];},_msg:function(el,msg,fn){var v5Z="own";var V5Z="sli";var O5Z=":visible";var A9Y=a1n;A9Y+=y7n;A9Y+=d5Z;A9Y+=Y6Z;var w9Y=l5n;w9Y+=y7n;var M9Y=Z5Z;M9Y+=p7n;M9Y+=t7n;M9Y+=Z5n;var z9Y=R13.r7n;z9Y+=C5Z;if(msg===undefined){return el[o7Z]();}if(typeof msg===z9Y){var F9Y=U5n;F9Y+=a5n;F9Y+=F1n;var editor=this[q1n][Q5Z];msg=msg(editor,new DataTable[F9Y](editor[q1n][f5Z]));}if(el[M9Y]()[U9Z](O5Z)&&$[w9Y][A9Y]){var s9Y=P9Z;s9Y+=R13.q7n;s9Y+=h5Z;el[s9Y](msg);if(msg){var Y9Y=V5Z;Y9Y+=K4n;Y9Y+=R1n;Y9Y+=v5Z;el[Y9Y](fn);}else{el[u7Z](fn);}}else{var Z9Y=y7n;Z9Y+=o7n;Z9Y+=y7n;Z9Y+=t7n;var d9Y=S9Z;d9Y+=w5Z;var T9Y=R13.U7n;T9Y+=q1n;T9Y+=q1n;var I9Y=P9Z;I9Y+=g5Z;el[I9Y](msg||C2n)[T9Y](Y9Z,msg?d9Y:Z9Y);if(fn){fn();}}return this;},_multiValueCheck:function(){var c5Z="noMulti";var a5Z="ol";var q5Z="inputCon";var G5Z="inputCo";var b5Z="ultiIds";var E5Z="iRe";var m5Z="tm";var W5Z="toggleClas";var R5Z="tiNoEdit";var a9Y=i2n;a9Y+=R5Z;var q9Y=W5Z;q9Y+=q1n;var G9Y=F1n;G9Y+=y7n;G9Y+=l5n;G9Y+=o7n;var b9Y=P9Z;b9Y+=m5Z;b9Y+=h1n;var x9Y=N1n;x9Y+=H7Z;x9Y+=L5Z;var l9Y=y7n;l9Y+=V1Z;var D9Y=M5n;D9Y+=h1n;D9Y+=e5Z;D9Y+=G0n;var X9Y=j6Z;X9Y+=E5Z;X9Y+=X5Z;X9Y+=D5Z;var f9Y=F1Z;f9Y+=l5Z;var Q9Y=x5Z;Q9Y+=R13.q7n;Q9Y+=q1n;var C9Y=n1n;C9Y+=b5Z;var last;var ids=this[q1n][C9Y];var values=this[q1n][T1Z];var isMultiValue=this[q1n][k8Z];var isMultiEditable=this[q1n][Q9Y][h9Z];var val;var different=J2n;if(ids){for(var i=B9n;i<ids[k2n];i++){val=values[ids[i]];if(i>B9n&&!_deepCompare(val,last)){different=B2n;break;}last=val;}}if(different&&isMultiValue||!isMultiEditable&&this[f9Y]()){var g9Y=S9Z;g9Y+=w5Z;var v9Y=j6Z;v9Y+=F1n;var V9Y=m5n;V9Y+=o7n;V9Y+=n1n;var h9Y=G5Z;h9Y+=A9Z;var O9Y=m5n;O9Y+=T7Z;this[O9Y][h9Y][s9Z]({display:K7Z});this[V9Y][v9Y][s9Z]({display:g9Y});}else{var e9Y=j6Z;e9Y+=F1n;var L9Y=m5n;L9Y+=T7Z;var m9Y=S9Z;m9Y+=o7n;m9Y+=R13.U7n;m9Y+=G0n;var W9Y=R13.U7n;W9Y+=q1n;W9Y+=q1n;var R9Y=q5Z;R9Y+=O0n;R9Y+=a5Z;this[I9Z][R9Y][W9Y]({display:m9Y});this[L9Y][e9Y][s9Z]({display:K7Z});if(isMultiValue&&!different){var E9Y=Y0n;E9Y+=R13.q7n;this[E9Y](last,J2n);}}this[I9Z][X9Y][s9Z]({display:ids&&ids[k2n]>o9n&&different&&!isMultiValue?D9Y:l9Y});var i18n=this[q1n][Q5Z][Y8Z][I8Z];this[I9Z][x9Y][b9Y](isMultiEditable?i18n[G9Y]:i18n[c5Z]);this[I9Z][I8Z][q9Y](this[q1n][J9Z][a9Y],!isMultiEditable);this[q1n][Q5Z][n5Z]();return B2n;},_typeFn:function(name){var N5Z="hift";var U9Y=M3n;U9Y+=a5n;U9Y+=t7n;var r9Y=r5Z;r9Y+=q1n;var n9Y=U5Z;n9Y+=q1n;n9Y+=N5Z;var c9Y=q1n;c9Y+=N5Z;var args=Array[G9Z][u5Z][l9Z](arguments);args[c9Y]();args[n9Y](this[q1n][r9Y]);var fn=this[q1n][U9Y][name];if(fn){var N9Y=K5Z;N9Y+=o1Z;return fn[b9Z](this[q1n][N9Y],args);}}};Editor[u9Y][K9Y]={};Editor[k9Y][J9Y]={"className":R13.K7n,"data":R13.K7n,"def":R13.K7n,"fieldInfo":R13.K7n,"id":R13.K7n,"label":R13.K7n,"labelInfo":R13.K7n,"name":F9Z,"type":k5Z,"message":R13.K7n,"multiEditable":B2n,"submit":B2n};Editor[g1n][P9Y][J5Z]={type:F9Z,name:F9Z,classes:F9Z,opts:F9Z,host:F9Z};Editor[g1n][T9Z][B9Y]={container:F9Z,label:F9Z,labelInfo:F9Z,fieldInfo:F9Z,fieldError:F9Z,fieldMessage:F9Z};Editor[o9Y]={};Editor[T9Z][p9Y]={"init":function(dte){},"open":function(dte,append,fn){},"close":function(dte,fn){}};Editor[S9Y][P5Z]={"create":function(conf){},"get":function(conf){},"set":function(conf,val){},"enable":function(conf){},"disable":function(conf){}};Editor[T9Z][J5Z]={"ajaxUrl":F9Z,"ajax":F9Z,"dataSource":F9Z,"domTable":F9Z,"opts":F9Z,"displayController":F9Z,"fields":{},"order":[],"id":-o9n,"displayed":J2n,"processing":J2n,"modifier":F9Z,"action":F9Z,"idSrc":F9Z,"unique":B9n};Editor[T9Z][B5Z]={"label":F9Z,"fn":F9Z,"className":F9Z};Editor[i9Y][t9Y]={onReturn:H9Y,onBlur:y9Y,onBackground:o5Z,onComplete:p5Z,onEsc:p5Z,onFieldError:S5Z,submit:j7Y,focus:B9n,buttons:B2n,title:B2n,message:B2n,drawType:J2n,scope:z7Y};Editor[F7Y]={};(function(window,document,$,DataTable){var n0Z="lightbox";var c0Z='<div class="DTED_Lightbox_Content_Wrapper">';var a0Z='<div class="DTED_Lightbox_Container">';var q0Z='<div class="DTED DTED_Lightbox_Wrapper">';var y3Z="onf";var N3Z='div.DTED_Lightbox_Shown';var J4Z="ED";var k4Z="click.DT";var u4Z="ghtbox";var e4Z="bac";var Y4Z="box";var s4Z="ligh";var w4Z="playCon";var M4Z="ss=\"DTED_Lightbox_Content\">";var j4Z="ghtbox_Background\"><div/></div>";var y5Z="<div class=\"DTED_Li";var H5Z="<div class=\"DTED_Lightbox_Close\"></di";var I7n=25;var N1Y=R13.U7n;N1Y+=o7n;N1Y+=i5Z;var U1Y=t5Z;U1Y+=V5n;var r1Y=H5Z;r1Y+=m6Z;var n1Y=y5Z;n1Y+=j4Z;var c1Y=G6Z;c1Y+=m5n;c1Y+=f6Z;c1Y+=O6Z;var a1Y=d6Z;a1Y+=e6Z;a1Y+=q6Z;var q1Y=z4Z;q1Y+=m6Z;var G1Y=F4Z;G1Y+=M4Z;var A7Y=m5n;A7Y+=U9Z;A7Y+=w4Z;A7Y+=A4Z;var w7Y=s4Z;w7Y+=R13.q7n;w7Y+=Y4Z;var M7Y=I4Z;M7Y+=h1n;M7Y+=S3n;var self;Editor[M7Y][w7Y]=$[C8Z](B2n,{},Editor[T9Z][A7Y],{"init":function(dte){var T4Z="_init";self[T4Z]();return self;},"open":function(dte,append,callback){var d4Z="how";var T7Y=m4n;T7Y+=d4Z;T7Y+=y7n;var I7Y=Z4Z;I7Y+=y7n;I7Y+=m5n;var Y7Y=Q7Z;Y7Y+=C4Z;var s7Y=C5n;s7Y+=q1n;s7Y+=d4Z;s7Y+=y7n;if(self[s7Y]){if(callback){callback();}return;}self[Q4Z]=dte;var content=self[f4Z][Y7Y];content[O4Z]()[B7Z]();content[I7Y](append)[h4Z](self[f4Z][V4Z]);self[T7Y]=B2n;self[v4Z](callback);},"close":function(dte,callback){var g4Z="shown";var Q7Y=C5n;Q7Y+=g4Z;var C7Y=C5n;C7Y+=R4Z;C7Y+=K4n;var Z7Y=C5n;Z7Y+=W4Z;var d7Y=v4Z;d7Y+=y7n;if(!self[d7Y]){if(callback){callback();}return;}self[Z7Y]=dte;self[C7Y](callback);self[Q7Y]=J2n;},node:function(dte){var f7Y=C5n;f7Y+=I9Z;return self[f7Y][D8Z][B9n];},"_init":function(){var a4Z='opacity';var b4Z="ontent";var x4Z="TED_Lightbox_C";var l4Z="iv.D";var E4Z="gro";var L4Z="cs";var m4Z="pacity";var R7Y=o7n;R7Y+=m4Z;var g7Y=L4Z;g7Y+=q1n;var v7Y=e4Z;v7Y+=G0n;v7Y+=E4Z;v7Y+=X4Z;var V7Y=R13.U7n;V7Y+=q1n;V7Y+=q1n;var h7Y=V3n;h7Y+=D4Z;h7Y+=p7n;var O7Y=m5n;O7Y+=l4Z;O7Y+=x4Z;O7Y+=b4Z;if(self[G4Z]){return;}var dom=self[f4Z];dom[q4Z]=$(O7Y,self[f4Z][D8Z]);dom[h7Y][V7Y](a4Z,B9n);dom[v7Y][g7Y](R7Y,B9n);},"_show":function(callback){var U3Z="oun";var n3Z="s=\"DTED_Lightbox_Shown\"/>";var c3Z="orientation";var a3Z="_scrollTop";var Q3Z='auto';var C3Z="box_Mobil";var d3Z="DTED_Lig";var T3Z="tation";var Y3Z="igh";var s3Z="offsetAn";var F3Z="ound";var z3Z="gr";var y4Z="apper";var t4Z="tbox";var i4Z="_Ligh";var S4Z="click.DTED";var p4Z="backgrou";var o4Z="bin";var N4Z="Li";var U4Z=".DTED_";var c4Z="scrollT";var t7Y=c4Z;t7Y+=x5Z;var i7Y=M5n;i7Y+=n4Z;var p7Y=r4Z;p7Y+=U4Z;p7Y+=N4Z;p7Y+=u4Z;var o7Y=K4Z;o7Y+=F0n;var P7Y=k4Z;P7Y+=J4Z;P7Y+=P4Z;var J7Y=M5n;J7Y+=y6Z;J7Y+=m5n;var k7Y=V3n;k7Y+=u1Z;k7Y+=a5n;k7Y+=B4Z;var u7Y=o4Z;u7Y+=m5n;var N7Y=p4Z;N7Y+=F0n;var r7Y=S4Z;r7Y+=i4Z;r7Y+=t4Z;var a7Y=H4Z;a7Y+=y4Z;var q7Y=C5n;q7Y+=I9Z;var G7Y=j3Z;G7Y+=z3Z;G7Y+=F3Z;var b7Y=C5n;b7Y+=m5n;b7Y+=o7n;b7Y+=n1n;var x7Y=M3Z;x7Y+=J7Z;x7Y+=m5n;var l7Y=w3Z;l7Y+=A3Z;var D7Y=s3Z;D7Y+=F1n;var X7Y=R13.U7n;X7Y+=q1n;X7Y+=q1n;var E7Y=P9Z;E7Y+=t7n;E7Y+=Y3Z;E7Y+=R13.q7n;var m7Y=i1n;m7Y+=I3Z;m7Y+=y7n;m7Y+=T3Z;var W7Y=C5n;W7Y+=m5n;W7Y+=o7n;W7Y+=n1n;var that=this;var dom=self[W7Y];if(window[m7Y]!==undefined){var e7Y=d3Z;e7Y+=Z3Z;e7Y+=C3Z;e7Y+=t7n;var L7Y=w3Z;L7Y+=m5n;L7Y+=V5n;$(L7Y)[O7Z](e7Y);}dom[q4Z][s9Z](E7Y,Q3Z);dom[D8Z][X7Y]({top:-self[f3Z][D7Y]});$(l7Y)[x7Y](self[b7Y][G7Y])[h4Z](self[q7Y][a7Y]);self[O3Z]();self[Q4Z][h3Z](dom[D8Z],{opacity:o9n,top:B9n},callback);self[Q4Z][h3Z](dom[V3Z],{opacity:o9n});setTimeout(function(){var m3Z='text-indent';var W3Z="ter";var R3Z="oo";var g3Z="TE_F";var n7Y=R13.U7n;n7Y+=q1n;n7Y+=q1n;var c7Y=v3Z;c7Y+=g3Z;c7Y+=R3Z;c7Y+=W3Z;$(c7Y)[n7Y](m3Z,-o9n);},y9n);dom[V4Z][L3Z](r7Y,function(e){var U7Y=C5n;U7Y+=W4Z;self[U7Y][V4Z]();});dom[N7Y][u7Y](e3Z,function(e){var X3Z="round";var E3Z="ckg";var K7Y=M5n;K7Y+=a1n;K7Y+=E3Z;K7Y+=X3Z;self[Q4Z][K7Y]();});$(D3Z,dom[k7Y])[J7Y](P7Y,function(e){var x3Z='DTED_Lightbox_Content_Wrapper';if($(e[l3Z])[V9Z](x3Z)){var B7Y=b3Z;B7Y+=R13.q7n;B7Y+=t7n;self[B7Y][V3Z]();}});$(window)[o7Y](p7Y,function(){var q3Z="alc";var G3Z="_heightC";var S7Y=G3Z;S7Y+=q3Z;self[S7Y]();});self[a3Z]=$(i7Y)[t7Y]();if(window[c3Z]!==undefined){var z1Y=i6Z;z1Y+=n3Z;var j1Y=y7n;j1Y+=o7n;j1Y+=R13.q7n;var y7Y=r3Z;y7Y+=U3Z;y7Y+=m5n;var H7Y=y7n;H7Y+=p4n;var kids=$(p9Z)[O4Z]()[H7Y](dom[y7Y])[j1Y](dom[D8Z]);$(p9Z)[h4Z](z1Y);$(N3Z)[h4Z](kids);}},"_heightCalc":function(){var F0Z='div.DTE_Footer';var H3Z="wPadding";var t3Z="windo";var S3Z="TE_Hea";var p3Z="terHeight";var J3Z="rHeight";var k3Z="oute";var K3Z="xHeight";var d1Y=u3Z;d1Y+=K3Z;var T1Y=k3Z;T1Y+=J3Z;var I1Y=P3Z;I1Y+=B3Z;I1Y+=s3n;var Y1Y=o3Z;Y1Y+=p3Z;var s1Y=V3n;s1Y+=p7n;s1Y+=M3Z;s1Y+=s3n;var A1Y=v3Z;A1Y+=S3Z;A1Y+=i3Z;var w1Y=t3Z;w1Y+=H3Z;var M1Y=R13.U7n;M1Y+=y3Z;var F1Y=j0Z;F1Y+=z0Z;var dom=self[f4Z];var maxHeight=$(window)[F1Y]()-self[M1Y][w1Y]*p9n-$(A1Y,dom[s1Y])[Y1Y]()-$(F0Z,dom[I1Y])[T1Y]();$(M0Z,dom[D8Z])[s9Z](d1Y,maxHeight);},"_hide":function(callback){var D0Z="scrollTop";var X0Z='DTED_Lightbox_Mobile';var L0Z="chi";var m0Z="move";var R0Z="orient";var g0Z="llTop";var v0Z="_scro";var V0Z="_anim";var h0Z="rap";var O0Z="fsetAni";var C0Z="kground";var Z0Z="unbin";var d0Z="Content_Wrapp";var T0Z="Lightbox_";var s0Z="ED_Li";var A0Z="ck.DT";var b1Y=w0Z;b1Y+=A0Z;b1Y+=s0Z;b1Y+=u4Z;var x1Y=Y0Z;x1Y+=y6Z;x1Y+=m5n;var l1Y=I0Z;l1Y+=T0Z;l1Y+=d0Z;l1Y+=s3n;var D1Y=k4Z;D1Y+=J4Z;D1Y+=P4Z;var X1Y=Z0Z;X1Y+=m5n;var E1Y=s0n;E1Y+=o7n;E1Y+=q1n;E1Y+=t7n;var L1Y=e4Z;L1Y+=C0Z;var m1Y=C5n;m1Y+=Q0Z;m1Y+=u3Z;m1Y+=d5n;var R1Y=f0Z;R1Y+=O0Z;var g1Y=R13.U7n;g1Y+=y3Z;var v1Y=V3n;v1Y+=h0Z;v1Y+=a5n;v1Y+=s3n;var V1Y=V0Z;V1Y+=Y6Z;var h1Y=C5n;h1Y+=m5n;h1Y+=R13.q7n;h1Y+=t7n;var O1Y=v0Z;O1Y+=g0Z;var f1Y=M5n;f1Y+=c3n;f1Y+=V5n;var Z1Y=R0Z;Z1Y+=W0Z;Z1Y+=F1n;Z1Y+=g2n;var dom=self[f4Z];if(!callback){callback=function(){};}if(window[Z1Y]!==undefined){var Q1Y=A2n;Q1Y+=m0Z;var C1Y=L0Z;C1Y+=e0Z;C1Y+=t7n;C1Y+=y7n;var show=$(N3Z);show[C1Y]()[E0Z](p9Z);show[Q1Y]();}$(f1Y)[g7Z](X0Z)[D0Z](self[O1Y]);self[h1Y][V1Y](dom[v1Y],{opacity:B9n,top:self[g1Y][R1Y]},function(){var l0Z="det";var W1Y=l0Z;W1Y+=a1n;W1Y+=R13.U7n;W1Y+=P9Z;$(this)[W1Y]();callback();});self[Q4Z][m1Y](dom[L1Y],{opacity:B9n},function(){var x0Z="eta";var e1Y=m5n;e1Y+=x0Z;e1Y+=L5n;$(this)[e1Y]();});dom[E1Y][X1Y](e3Z);dom[V3Z][b0Z](D1Y);$(l1Y,dom[D8Z])[x1Y](b1Y);$(window)[b0Z](G0Z);},"_dte":F9Z,"_ready":J2n,"_shown":J2n,"_dom":{"wrapper":$(q0Z+a0Z+c0Z+G1Y+q1Y+a1Y+c1Y+c8Z),"background":$(n1Y),"close":$(r1Y),"content":F9Z}});self=Editor[U1Y][n0Z];self[N1Y]={"offsetAni":I7n,"windowPadding":I7n};}(window,document,jQuery,jQuery[u1Y][r0Z]));(function(window,document,$,DataTable){var y6J='<div class="DTED_Envelope_Close">&times;</div>';var H6J='<div class="DTED_Envelope_Background"><div/></div>';var t6J='<div class="DTED_Envelope_Container"></div>';var i6J='<div class="DTED_Envelope_Shadow"></div>';var E2Z="fa";var W2Z="k.DTE";var R2Z="clic";var O2Z="_cssBackgroundOpacity";var F2Z="ground";var p0Z="_do";var K0Z="envelope";var u0Z="nvelope_Wrapper\">";var N0Z="v class=\"DTED DTED_E";var X7n=600;var V7n=50;var j3Y=z4Z;j3Y+=m6Z;var y4Y=U0Z;y4Y+=N0Z;y4Y+=u0Z;var k1Y=C0n;k1Y+=Q0n;var K1Y=F7Z;K1Y+=E1Z;K1Y+=S3n;var self;Editor[K1Y][K0Z]=$[C8Z](B2n,{},Editor[k1Y][k0Z],{"init":function(dte){var J1Y=C5n;J1Y+=J0Z;self[Q4Z]=dte;self[J1Y]();return self;},"open":function(dte,append,callback){var i0Z="appendChild";var o0Z="Child";var P0Z="_sh";var i1Y=P0Z;i1Y+=o7n;i1Y+=V3n;var S1Y=k7Z;S1Y+=B0Z;S1Y+=o0Z;var p1Y=p0Z;p1Y+=n1n;var o1Y=s7Z;o1Y+=S0Z;var B1Y=C5n;B1Y+=m5n;B1Y+=o7n;B1Y+=n1n;var P1Y=C5n;P1Y+=m5n;P1Y+=R13.q7n;P1Y+=t7n;self[P1Y]=dte;$(self[f4Z][q4Z])[O4Z]()[B7Z]();self[B1Y][o1Y][i0Z](append);self[p1Y][q4Z][S1Y](self[f4Z][V4Z]);self[i1Y](callback);},"close":function(dte,callback){var t1Y=C5n;t1Y+=t0Z;self[Q4Z]=dte;self[t1Y](callback);},node:function(dte){var H1Y=C5n;H1Y+=m5n;H1Y+=T7Z;return self[H1Y][D8Z][B9n];},"_init":function(){var Z2Z='hidden';var d2Z="visbility";var I2Z="velope_Container";var Y2Z="En";var s2Z="dChild";var A2Z="dChil";var w2Z="ppen";var M2Z="styl";var z2Z="city";var j2Z="lit";var y0Z="visb";var H0Z="visi";var Q5Y=H0Z;Q5Y+=M5n;Q5Y+=l3n;var C5Y=y0Z;C5Y+=F1n;C5Y+=j2Z;C5Y+=V5n;var Z5Y=x5Z;Z5Y+=a1n;Z5Y+=z2Z;var d5Y=R13.U7n;d5Y+=q1n;d5Y+=q1n;var T5Y=j3Z;T5Y+=F2Z;var I5Y=C5n;I5Y+=m5n;I5Y+=T7Z;var Y5Y=M2Z;Y5Y+=t7n;var s5Y=C5n;s5Y+=h1Z;s5Y+=n1n;var A5Y=p0Z;A5Y+=n1n;var w5Y=a1n;w5Y+=w2Z;w5Y+=A2Z;w5Y+=m5n;var M5Y=j3Z;M5Y+=F2Z;var F5Y=C5n;F5Y+=m5n;F5Y+=o7n;F5Y+=n1n;var z5Y=a1n;z5Y+=w2Z;z5Y+=s2Z;var j5Y=C5n;j5Y+=m5n;j5Y+=T7Z;var y1Y=I0Z;y1Y+=Y2Z;y1Y+=I2Z;if(self[G4Z]){return;}self[f4Z][q4Z]=$(y1Y,self[j5Y][D8Z])[B9n];document[T2Z][z5Y](self[F5Y][M5Y]);document[T2Z][w5Y](self[A5Y][D8Z]);self[s5Y][V3Z][Y5Y][d2Z]=Z2Z;self[f4Z][V3Z][C2Z][Q2Z]=f2Z;self[O2Z]=$(self[I5Y][T5Y])[d5Y](Z5Y);self[f4Z][V3Z][C2Z][Q2Z]=K7Z;self[f4Z][V3Z][C2Z][C5Y]=Q5Y;},"_show":function(callback){var s6J='click.DTED_Envelope';var w6J="nima";var M6J="windowPadding";var F6J="offsetHeight";var j6J=",";var y2Z="ni";var H2Z="animate";var i2Z="px";var p2Z="offsetWidth";var o2Z="opacity";var B2Z="conte";var P2Z="gh";var k2Z="hRow";var K2Z="dAttac";var N2Z="opac";var U2Z="Left";var r2Z="rg";var n2Z="ffs";var c2Z="setHeigh";var q2Z="tyle";var G2Z="ity";var b2Z="backgro";var x2Z="tyl";var l2Z="orma";var X2Z="eIn";var e2Z="indowScroll";var m2Z="D_Envelope";var g2Z="elope";var v2Z="click.DTED_Env";var V2Z="ED_Envelope";var O4Y=r4Z;O4Y+=h2Z;O4Y+=L1n;O4Y+=V2Z;var Z4Y=v2Z;Z4Y+=g2Z;var d4Y=p0Z;d4Y+=n1n;var I4Y=K4Z;I4Y+=y7n;I4Y+=m5n;var Y4Y=C5n;Y4Y+=m5n;Y4Y+=o7n;Y4Y+=n1n;var A4Y=R2Z;A4Y+=W2Z;A4Y+=m2Z;var w4Y=R13.U7n;w4Y+=L2Z;w4Y+=Y0n;var M4Y=b3Z;M4Y+=o7n;M4Y+=n1n;var i5Y=V3n;i5Y+=e2Z;var S5Y=R13.U7n;S5Y+=o7n;S5Y+=y7n;S5Y+=l5n;var p5Y=E2Z;p5Y+=m5n;p5Y+=X2Z;var o5Y=P3Z;o5Y+=D2Z;o5Y+=p7n;var B5Y=y7n;B5Y+=l2Z;B5Y+=h1n;var P5Y=j3Z;P5Y+=F2Z;var J5Y=C5n;J5Y+=h1Z;J5Y+=n1n;var k5Y=v1Z;k5Y+=R13.U7n;k5Y+=G0n;var K5Y=q1n;K5Y+=x2Z;K5Y+=t7n;var u5Y=b2Z;u5Y+=U5Z;u5Y+=m5n;var N5Y=x5Z;N5Y+=X1Z;N5Y+=G2Z;var U5Y=q1n;U5Y+=q2Z;var r5Y=C5n;r5Y+=m5n;r5Y+=o7n;r5Y+=n1n;var n5Y=a5n;n5Y+=H7n;var c5Y=C5n;c5Y+=m5n;c5Y+=o7n;c5Y+=n1n;var a5Y=a2Z;a5Y+=c2Z;a5Y+=R13.q7n;var q5Y=o7n;q5Y+=n2Z;q5Y+=G3n;var G5Y=R13.q7n;G5Y+=o7n;G5Y+=a5n;var b5Y=q1n;b5Y+=R13.q7n;b5Y+=V5n;b5Y+=l3n;var x5Y=V3n;x5Y+=p7n;x5Y+=M3Z;x5Y+=s3n;var l5Y=C5n;l5Y+=I9Z;var D5Y=u3Z;D5Y+=r2Z;D5Y+=y6Z;D5Y+=U2Z;var X5Y=q1n;X5Y+=q2Z;var E5Y=P3Z;E5Y+=a5n;E5Y+=a4n;E5Y+=p7n;var e5Y=p0Z;e5Y+=n1n;var L5Y=P3Z;L5Y+=D2Z;L5Y+=p7n;var m5Y=N2Z;m5Y+=F1n;m5Y+=R13.q7n;m5Y+=V5n;var W5Y=y7n;W5Y+=o7n;W5Y+=y7n;W5Y+=t7n;var R5Y=t5Z;R5Y+=V5n;var g5Y=C5n;g5Y+=u2Z;g5Y+=K2Z;g5Y+=k2Z;var v5Y=m5n;v5Y+=F1n;v5Y+=J2Z;var V5Y=C5n;V5Y+=m5n;V5Y+=T7Z;var h5Y=a1n;h5Y+=r1n;h5Y+=R13.q7n;h5Y+=o7n;var O5Y=j0Z;O5Y+=F1n;O5Y+=P2Z;O5Y+=R13.q7n;var f5Y=B2Z;f5Y+=Z5n;var that=this;var formHeight;if(!callback){callback=function(){};}self[f4Z][f5Y][C2Z][O5Y]=h5Y;var style=self[V5Y][D8Z][C2Z];style[o2Z]=B9n;style[v5Y]=f2Z;var targetRow=self[g5Y]();var height=self[O3Z]();var width=targetRow[p2Z];style[R5Y]=W5Y;style[m5Y]=o9n;self[f4Z][L5Y][C2Z][S2Z]=width+i2Z;self[e5Y][E5Y][X5Y][D5Y]=-(width/p9n)+i2Z;self[l5Y][x5Y][b5Y][G5Y]=$(targetRow)[q5Y]()[t2Z]+targetRow[a5Y]+i2Z;self[c5Y][q4Z][C2Z][t2Z]=-o9n*height-A7n+n5Y;self[r5Y][V3Z][U5Y][N5Y]=B9n;self[f4Z][u5Y][K5Y][Q2Z]=k5Y;$(self[J5Y][P5Y])[H2Z]({'opacity':self[O2Z]},B5Y);$(self[f4Z][o5Y])[p5Y]();if(self[S5Y][i5Y]){var y5Y=R13.U7n;y5Y+=o7n;y5Y+=i5Z;var H5Y=a1n;H5Y+=y2Z;H5Y+=u3Z;H5Y+=d5n;var t5Y=o7Z;t5Y+=j6J;t5Y+=M5n;t5Y+=n4Z;$(t5Y)[H5Y]({"scrollTop":$(targetRow)[z6J]()[t2Z]+targetRow[F6J]-self[y5Y][M6J]},function(){var z4Y=Q0Z;z4Y+=n1n;z4Y+=a1n;z4Y+=d5n;var j4Y=C5n;j4Y+=h1Z;j4Y+=n1n;$(self[j4Y][q4Z])[z4Y]({"top":B9n},X7n,callback);});}else{var F4Y=a1n;F4Y+=w6J;F4Y+=d5n;$(self[f4Z][q4Z])[F4Y]({"top":B9n},X7n,callback);}$(self[M4Y][w4Y])[L3Z](A4Y,function(e){var s4Y=R13.U7n;s4Y+=A6J;self[Q4Z][s4Y]();});$(self[Y4Y][V3Z])[I4Y](s6J,function(e){var I6J="grou";var Y6J="ack";var T4Y=M5n;T4Y+=Y6J;T4Y+=I6J;T4Y+=F0n;self[Q4Z][T4Y]();});$(D3Z,self[d4Y][D8Z])[L3Z](Z4Y,function(e){var d6J='DTED_Envelope_Content_Wrapper';var T6J="arget";var C4Y=R13.q7n;C4Y+=T6J;if($(e[C4Y])[V9Z](d6J)){var f4Y=r3Z;f4Y+=o3Z;f4Y+=F0n;var Q4Y=C5n;Q4Y+=W4Z;self[Q4Y][f4Y]();}});$(window)[L3Z](O4Y,function(){self[O3Z]();});},"_heightCalc":function(){var D6J='maxHeight';var E6J='div.DTE_Header';var L6J="heightCalc";var m6J="Calc";var W6J="eig";var g6J="ei";var v6J="owPaddi";var V6J="wind";var h6J="oter";var O6J="DTE_Fo";var C6J="erHe";var Z6J="out";var l4Y=Z6J;l4Y+=C6J;l4Y+=F1n;l4Y+=Q6J;var D4Y=H4Z;D4Y+=k7Z;D4Y+=a5n;D4Y+=s3n;var X4Y=C5n;X4Y+=m5n;X4Y+=o7n;X4Y+=n1n;var E4Y=f6J;E4Y+=O6J;E4Y+=h6J;var e4Y=C5n;e4Y+=m5n;e4Y+=o7n;e4Y+=n1n;var L4Y=V6J;L4Y+=v6J;L4Y+=r2n;var m4Y=R13.U7n;m4Y+=g2n;m4Y+=l5n;var W4Y=P9Z;W4Y+=g6J;W4Y+=r0n;W4Y+=Z3Z;var R4Y=R13.U7n;R4Y+=R4Z;R4Y+=e0Z;R4Y+=J7Z;var g4Y=R6J;g4Y+=y7n;g4Y+=C4Z;var v4Y=b3Z;v4Y+=o7n;v4Y+=n1n;var V4Y=R13.U7n;V4Y+=o7n;V4Y+=y7n;V4Y+=l5n;var h4Y=P9Z;h4Y+=W6J;h4Y+=Z3Z;h4Y+=m6J;var formHeight;formHeight=self[f3Z][h4Y]?self[V4Y][L6J](self[f4Z][D8Z]):$(self[v4Y][g4Y])[R4Y]()[W4Y]();var maxHeight=$(window)[e6J]()-self[m4Y][L4Y]*p9n-$(E6J,self[e4Y][D8Z])[X6J]()-$(E4Y,self[f4Z][D8Z])[X6J]();$(M0Z,self[X4Y][D8Z])[s9Z](D6J,maxHeight);return $(self[Q4Z][I9Z][D4Y])[l4Y]();},"_hide":function(callback){var n6J="conten";var c6J="anima";var a6J="setHeight";var b6J="k.DTED_Lightbox";var l6J="unbi";var B4Y=R2Z;B4Y+=W2Z;B4Y+=R1n;B4Y+=P4Z;var P4Y=l6J;P4Y+=F0n;var J4Y=Y0Z;J4Y+=x6J;var k4Y=b3Z;k4Y+=o7n;k4Y+=n1n;var K4Y=R2Z;K4Y+=b6J;var u4Y=G6J;u4Y+=t7n;var N4Y=C5n;N4Y+=m5n;N4Y+=T7Z;var q4Y=o7n;q4Y+=q6J;q4Y+=a6J;var G4Y=c6J;G4Y+=d5n;var b4Y=n6J;b4Y+=R13.q7n;var x4Y=C5n;x4Y+=m5n;x4Y+=o7n;x4Y+=n1n;if(!callback){callback=function(){};}$(self[x4Y][b4Y])[G4Y]({"top":-(self[f4Z][q4Z][q4Y]+V7n)},X7n,function(){var u6J="ckgro";var U6J="deOut";var r6J="mal";var U4Y=y7n;U4Y+=o7n;U4Y+=p7n;U4Y+=r6J;var r4Y=E2Z;r4Y+=U6J;var n4Y=N6J;n4Y+=u6J;n4Y+=X4Z;var c4Y=C5n;c4Y+=m5n;c4Y+=o7n;c4Y+=n1n;var a4Y=C5n;a4Y+=m5n;a4Y+=T7Z;$([self[a4Y][D8Z],self[c4Y][n4Y]])[r4Y](U4Y,callback);});$(self[N4Y][u4Y])[b0Z](K4Y);$(self[k4Y][V3Z])[J4Y](e3Z);$(D3Z,self[f4Z][D8Z])[P4Y](B4Y);$(window)[b0Z](G0Z);},"_findAttachRow":function(){var S4Y=R13.U7n;S4Y+=K6J;var p4Y=P9Z;p4Y+=t7n;p4Y+=a1n;p4Y+=m5n;var o4Y=R6J;o4Y+=i5Z;var dt=$(self[Q4Z][q1n][f5Z])[L2n]();if(self[o4Y][k6J]===p4Y){return dt[f5Z]()[J6J]();}else if(self[Q4Z][q1n][P6J]===S4Y){var t4Y=P9Z;t4Y+=t7n;t4Y+=a1n;t4Y+=i3Z;var i4Y=R13.q7n;i4Y+=B6J;i4Y+=t7n;return dt[i4Y]()[t4Y]();}else{var H4Y=p7n;H4Y+=o6J;return dt[H4Y](self[Q4Z][q1n][p6J])[S6J]();}},"_dte":F9Z,"_ready":J2n,"_cssBackgroundOpacity":o9n,"_dom":{"wrapper":$(y4Y+i6J+t6J+j3Y)[B9n],"background":$(H6J)[B9n],"close":$(y6J)[B9n],"content":F9Z}});self=Editor[Q2Z][K0Z];self[f3Z]={"windowPadding":V7n,"heightCalc":F9Z,"attach":j8J,"windowScroll":B2n};}(window,document,jQuery,jQuery[z3Y][r0Z]));Editor[G9Z][z8J]=function(cfg,after){var V8J="tFie";var O8J='initField';var f8J="Error adding field '";var Q8J="ts with this name";var C8J=" already exis";var Z8J=". A field";var d8J="'";var T8J="r adding field. The field requires a `name` option";var I8J="rro";var Y8J="dataSourc";var F8J="orde";var g3Y=F8J;g3Y+=p7n;if($[M8J](cfg)){var F3Y=h1n;F3Y+=w8J;F3Y+=R13.q7n;F3Y+=P9Z;for(var i=B9n,iLen=cfg[F3Y];i<iLen;i++){var M3Y=a1n;M3Y+=A8J;this[M3Y](cfg[i]);}}else{var O3Y=l5n;O3Y+=F1n;O3Y+=t7n;O3Y+=s8J;var I3Y=n1n;I3Y+=o7n;I3Y+=m5n;I3Y+=t7n;var Y3Y=C5n;Y3Y+=Y8J;Y3Y+=t7n;var A3Y=l5n;A3Y+=I3Z;A3Y+=E0n;A3Y+=q1n;var name=cfg[Q8Z];if(name===undefined){var w3Y=e1n;w3Y+=I8J;w3Y+=T8J;throw w3Y;}if(this[q1n][A3Y][name]){var s3Y=d8J;s3Y+=Z8J;s3Y+=C8J;s3Y+=Q8J;throw f8J+name+s3Y;}this[Y3Y](O8J,cfg);var field=new Editor[g1n](cfg,this[J9Z][h8J],this);if(this[q1n][I3Y]){var T3Y=t7n;T3Y+=G1n;T3Y+=V8J;T3Y+=s8J;var editFields=this[q1n][T3Y];field[v8J]();$[D2n](editFields,function(idSrc,edit){var e8J="omD";var L8J="valFr";var R8J="tiSet";var g8J="ef";var f3Y=m5n;f3Y+=g8J;var Q3Y=i2n;Q3Y+=R8J;var d3Y=W8J;d3Y+=m8J;var val;if(edit[d3Y]){var C3Y=m5n;C3Y+=W0Z;C3Y+=a1n;var Z3Y=L8J;Z3Y+=e8J;Z3Y+=a1n;Z3Y+=m8J;val=field[Z3Y](edit[C3Y]);}field[Q3Y](idSrc,val!==undefined?val:field[f3Y]());});}this[q1n][O3Y][name]=field;if(after===undefined){this[q1n][E8J][l2n](name);}else if(after===F9Z){var h3Y=o7n;h3Y+=p7n;h3Y+=i3Z;this[q1n][h3Y][X8J](name);}else{var v3Y=q1n;v3Y+=E1Z;v3Y+=o2n;v3Y+=t7n;var V3Y=q3n;V3Y+=s3n;var idx=$[D8J](after,this[q1n][E8J]);this[q1n][V3Y][v3Y](idx+o9n,B9n,name);}}this[l8J](this[g3Y]());return this;};Editor[G9Z][x8J]=function(newAjax){var R3Y=a1n;R3Y+=n4n;if(newAjax){this[q1n][x8J]=newAjax;return this;}return this[q1n][R3Y];};Editor[G9Z][W3Y]=function(){var q8J="onBackground";var G8J="Opts";var e3Y=M5n;e3Y+=h1n;e3Y+=r1n;e3Y+=p7n;var L3Y=R13.r7n;L3Y+=R13.U7n;L3Y+=R13.q7n;L3Y+=c9Z;var m3Y=b8J;m3Y+=R13.q7n;m3Y+=G8J;var onBackground=this[q1n][m3Y][q8J];if(typeof onBackground===L3Y){onBackground(this);}else if(onBackground===e3Y){var E3Y=S9Z;E3Y+=a8J;this[E3Y]();}else if(onBackground===p5Z){this[V4Z]();}else if(onBackground===c8J){this[I5Z]();}return this;};Editor[G9Z][X3Y]=function(){this[n8J]();return this;};Editor[G9Z][r8J]=function(cells,fieldNames,show,opts){var k8J="boole";var u8J="rmOp";var U8J="divi";var G3Y=F1n;G3Y+=y7n;G3Y+=U8J;G3Y+=N8J;var b3Y=H3n;b3Y+=M5n;b3Y+=M5n;b3Y+=l3n;var x3Y=B1n;x3Y+=u8J;x3Y+=K8J;var l3Y=k8J;l3Y+=a1n;l3Y+=y7n;var D3Y=J8J;D3Y+=V5n;var that=this;if(this[D3Y](function(){that[r8J](cells,fieldNames,opts);})){return this;}if($[P8J](fieldNames)){opts=fieldNames;fieldNames=undefined;show=B2n;}else if(typeof fieldNames===l3Y){show=fieldNames;fieldNames=undefined;opts=undefined;}if($[P8J](show)){opts=show;show=B2n;}if(show===undefined){show=B2n;}opts=$[C8Z]({},this[q1n][x3Y][b3Y],opts);var editFields=this[B8J](G3Y,cells,fieldNames);this[o8J](cells,editFields,p8J,opts,function(){var G9J="heade";var L9J="bg";var W9J='attach';var R9J="concat";var V9J='resize.';var O9J="_preop";var f9J="bubbleNode";var Q9J="ses";var Z9J="\"><div/></div";var d9J="<div class";var T9J="<span></div>";var I9J="r\">";var Y9J="<div class=\"DTE_Processing_Indicato";var F9J="epend";var H8J="ubblePosition";var t8J="focu";var i8J="osto";var W0Y=M5n;W0Y+=S8J;W0Y+=M5n;W0Y+=l3n;var R0Y=l4n;R0Y+=i8J;R0Y+=a3n;var g0Y=C5n;g0Y+=t8J;g0Y+=q1n;var v0Y=M5n;v0Y+=H8J;var h0Y=R13.U7n;h0Y+=h1n;h0Y+=o2n;h0Y+=G0n;var f0Y=R13.U7n;f0Y+=h1n;f0Y+=o2n;f0Y+=G0n;var T0Y=a1n;T0Y+=m5n;T0Y+=m5n;var Y0Y=y8J;Y0Y+=o7n;Y0Y+=j9J;var A0Y=R13.q7n;A0Y+=B7n;A0Y+=h1n;A0Y+=t7n;var w0Y=z9J;w0Y+=q1n;w0Y+=a1n;w0Y+=i1Z;var M0Y=R4n;M0Y+=F9J;var F0Y=M9J;F0Y+=w9J;F0Y+=k4n;F0Y+=p7n;var z0Y=M3Z;z0Y+=J7Z;z0Y+=m5n;var i3Y=z4Z;i3Y+=m6Z;var S3Y=a5n;S3Y+=A9J;S3Y+=R13.q7n;S3Y+=s3n;var p3Y=G6Z;p3Y+=s9J;p3Y+=O6Z;var o3Y=Y9J;o3Y+=I9J;o3Y+=T9J;var B3Y=d9J;B3Y+=b6Z;var P3Y=e2n;P3Y+=O6Z;var J3Y=e2n;J3Y+=O6Z;var k3Y=h1n;k3Y+=k9Z;k3Y+=p7n;var K3Y=e2n;K3Y+=O6Z;var u3Y=V3n;u3Y+=D4Z;u3Y+=p7n;var N3Y=Z9J;N3Y+=O6Z;var U3Y=M5n;U3Y+=S8J;U3Y+=f9Z;var r3Y=C9J;r3Y+=Q9J;var n3Y=f9J;n3Y+=q1n;var a3Y=M5n;a3Y+=S8J;a3Y+=S9Z;a3Y+=t7n;var q3Y=O9J;q3Y+=J7Z;var namespace=that[h9J](opts);var ret=that[q3Y](a3Y);if(!ret){return that;}$(window)[g2n](V9J+namespace,function(){var g9J="osition";var v9J="bubbleP";var c3Y=v9J;c3Y+=g9J;that[c3Y]();});var nodes=[];that[q1n][n3Y]=nodes[R9J][b9Z](nodes,_pluck(editFields,W9J));var classes=that[r3Y][U3Y];var background=$(m9J+classes[L9J]+N3Y);var container=$(m9J+classes[u3Y]+K3Y+m9J+classes[k3Y]+J3Y+m9J+classes[f5Z]+P3Y+B3Y+classes[V4Z]+e9J+o3Y+c8Z+p3Y+m9J+classes[S3Y]+e9J+i3Y);if(show){var j0Y=M5n;j0Y+=o7n;j0Y+=A3Z;var y3Y=a1n;y3Y+=E9J;var H3Y=M5n;H3Y+=o7n;H3Y+=m5n;H3Y+=V5n;var t3Y=k7Z;t3Y+=B0Z;t3Y+=X9J;container[t3Y](H3Y);background[y3Y](j0Y);}var liner=container[O4Z]()[D9J](B9n);var table=liner[O4Z]();var close=table[O4Z]();liner[z0Y](that[I9Z][F0Y]);table[M0Y](that[I9Z][l9J]);if(opts[w0Y]){liner[x9J](that[I9Z][b9J]);}if(opts[A0Y]){var s0Y=G9J;s0Y+=p7n;liner[x9J](that[I9Z][s0Y]);}if(opts[Y0Y]){var I0Y=a1n;I0Y+=D2Z;I0Y+=F0n;table[I0Y](that[I9Z][q9J]);}var pair=$()[T0Y](container)[z8J](background);that[a9J](function(submitComplete){var d0Y=c9J;d0Y+=F1n;d0Y+=u3Z;d0Y+=d5n;that[d0Y](pair,{opacity:B9n},function(){var u9J="e.";var N9J="esiz";var U9J="amicInf";var r9J="yn";var n9J="_clearD";var Q0Y=n9J;Q0Y+=r9J;Q0Y+=U9J;Q0Y+=o7n;var C0Y=p7n;C0Y+=N9J;C0Y+=u9J;var Z0Y=o7n;Z0Y+=q6J;pair[B7Z]();$(window)[Z0Y](C0Y+namespace);that[Q0Y]();});});background[f0Y](function(){var O0Y=M5n;O0Y+=h1n;O0Y+=r1n;O0Y+=p7n;that[O0Y]();});close[h0Y](function(){var V0Y=K9J;V0Y+=V1n;that[V0Y]();});that[v0Y]();that[h3Z](pair,{opacity:o9n});that[g0Y](that[q1n][k9J],opts[R9Z]);that[R0Y](W0Y);});return this;};Editor[G9Z][m0Y]=function(){var Q7J='left';var C7J="eft";var Z7J="belo";var d7J="ttom";var Y7J="bottom";var s7J="right";var t9J='div.DTE_Bubble_Liner';var i9J='div.DTE_Bubble';var S9J="des";var p9J="bubbleNo";var P9J="lef";var J9J="ri";var w7n=15;var N0Y=R13.q7n;N0Y+=o7n;N0Y+=a5n;var U0Y=J9J;U0Y+=r0n;U0Y+=Z3Z;var r0Y=P9J;r0Y+=R13.q7n;var n0Y=K1Z;n0Y+=B9J;var c0Y=h1n;c0Y+=o9J;var a0Y=P9J;a0Y+=R13.q7n;var q0Y=K1Z;q0Y+=B9J;var G0Y=R13.q7n;G0Y+=o7n;G0Y+=a5n;var L0Y=p9J;L0Y+=S9J;var wrapper=$(i9J),liner=$(t9J),nodes=this[q1n][L0Y];var position={top:B9n,left:B9n,right:B9n,bottom:B9n};$[D2n](nodes,function(i,node){var F7J="righ";var z7J="setWidth";var y9J="Hei";var H9J="offse";var b0Y=H9J;b0Y+=R13.q7n;b0Y+=y9J;b0Y+=Q6J;var x0Y=M5n;x0Y+=o7n;x0Y+=j7J;x0Y+=T7Z;var l0Y=a2Z;l0Y+=z7J;var D0Y=F7J;D0Y+=R13.q7n;var X0Y=h1n;X0Y+=t7n;X0Y+=M7J;var E0Y=h1n;E0Y+=t7n;E0Y+=l5n;E0Y+=R13.q7n;var e0Y=R13.q7n;e0Y+=o7n;e0Y+=a5n;var pos=$(node)[z6J]();node=$(node)[w7J](B9n);position[t2Z]+=pos[e0Y];position[E0Y]+=pos[X0Y];position[D0Y]+=pos[A7J]+node[l0Y];position[x0Y]+=pos[t2Z]+node[b0Y];});position[G0Y]/=nodes[q0Y];position[a0Y]/=nodes[c0Y];position[s7J]/=nodes[n0Y];position[Y7J]/=nodes[k2n];var top=position[t2Z],left=(position[r0Y]+position[U0Y])/p9n,width=liner[I7J](),visLeft=left-width/p9n,visRight=visLeft+width,docWidth=$(window)[S2Z](),padding=w7n,classes=this[J9Z][r8J];wrapper[s9Z]({top:top,left:left});if(liner[k2n]&&liner[z6J]()[N0Y]<B9n){var J0Y=M5n;J0Y+=m0n;J0Y+=o6J;var k0Y=A3n;k0Y+=T7J;var K0Y=M5n;K0Y+=o7n;K0Y+=d7J;var u0Y=R13.q7n;u0Y+=o7n;u0Y+=a5n;wrapper[s9Z](u0Y,position[K0Y])[k0Y](J0Y);}else{var P0Y=Z7J;P0Y+=V3n;wrapper[g7Z](P0Y);}if(visRight+padding>docWidth){var B0Y=h1n;B0Y+=C7J;var diff=visRight-docWidth;liner[s9Z](B0Y,visLeft<padding?-(visLeft-padding):-(diff+padding));}else{liner[s9Z](Q7J,visLeft<padding?-(visLeft-padding):B9n);}return this;};Editor[G9Z][q9J]=function(buttons){var O7J="Arr";var S0Y=f7J;S0Y+=a5n;S0Y+=R13.q7n;S0Y+=V5n;var p0Y=m5n;p0Y+=o7n;p0Y+=n1n;var o0Y=U9Z;o0Y+=O7J;o0Y+=a1n;o0Y+=V5n;var that=this;if(buttons===h7J){buttons=[{text:this[Y8Z][this[q1n][P6J]][I5Z],action:function(){this[I5Z]();}}];}else if(!$[o0Y](buttons)){buttons=[buttons];}$(this[p0Y][q9J])[S0Y]();$[D2n](buttons,function(i,btn){var x7J='keypress';var D7J="className";var X7J='<button/>';var m7J="classNa";var g7J="tabI";var O2Y=m5n;O2Y+=T7Z;var f2Y=a1n;f2Y+=E9J;var C2Y=w0Z;C2Y+=g1Z;var T2Y=V7J;T2Y+=v7J;var I2Y=o7n;I2Y+=y7n;var Y2Y=g7J;Y2Y+=R7J;var s2Y=g7J;s2Y+=y7n;s2Y+=K4n;s2Y+=H7n;var A2Y=m8J;A2Y+=K4Z;A2Y+=R7J;var w2Y=a1n;w2Y+=R13.q7n;w2Y+=R13.q7n;w2Y+=p7n;var M2Y=R13.r7n;M2Y+=R13.U7n;M2Y+=W7J;M2Y+=y7n;var F2Y=Z3Z;F2Y+=h5Z;var z2Y=m7J;z2Y+=Z1n;var j2Y=L7J;j2Y+=y7n;var y0Y=C9J;y0Y+=q1n;y0Y+=t7n;y0Y+=q1n;var H0Y=l5n;H0Y+=y7n;var t0Y=R13.q7n;t0Y+=t7n;t0Y+=e7J;var i0Y=o1Z;i0Y+=E7J;if(typeof btn===i0Y){btn={text:btn,action:function(){this[I5Z]();}};}var text=btn[t0Y]||btn[G8Z];var action=btn[P6J]||btn[H0Y];$(X7J,{'class':that[y0Y][l9J][j2Y]+(btn[z2Y]?l8Z+btn[D7J]:C2n)})[F2Y](typeof text===M2Y?text(that):text||C2n)[w2Y](A2Y,btn[s2Y]!==undefined?btn[Y2Y]:B9n)[I2Y](T2Y,function(e){if(e[l7J]===F7n&&action){var d2Y=R13.U7n;d2Y+=a1n;d2Y+=h1n;d2Y+=h1n;action[d2Y](that);}})[g2n](x7J,function(e){var b7J="yCod";var Z2Y=G0n;Z2Y+=t7n;Z2Y+=b7J;Z2Y+=t7n;if(e[Z2Y]===F7n){e[G7J]();}})[g2n](C2Y,function(e){var a7J="Defa";var Q2Y=R4n;Q2Y+=q7J;Q2Y+=a7J;Q2Y+=c7J;e[Q2Y]();if(action){action[l9Z](that);}})[f2Y](that[O2Y][q9J]);});return this;};Editor[G9Z][n7J]=function(fieldName){var k7J="oy";var K7J="est";var u7J="Ar";var N7J="splic";var U7J="udeFie";var h2Y=z8Z;h2Y+=m0n;h2Y+=m5n;h2Y+=q1n;var that=this;var fields=this[q1n][h2Y];if(typeof fieldName===r7J){var L2Y=y6Z;L2Y+=s0n;L2Y+=U7J;L2Y+=s8J;var m2Y=F1n;m2Y+=y7n;m2Y+=V4n;m2Y+=V5n;var W2Y=N7J;W2Y+=t7n;var R2Y=i1n;R2Y+=K4n;R2Y+=p7n;var g2Y=o7n;g2Y+=p7n;g2Y+=m5n;g2Y+=s3n;var v2Y=y6Z;v2Y+=u7J;v2Y+=u1Z;v2Y+=V5n;var V2Y=m5n;V2Y+=K7J;V2Y+=p7n;V2Y+=k7J;that[h8J](fieldName)[V2Y]();delete fields[fieldName];var orderIdx=$[v2Y](fieldName,this[q1n][g2Y]);this[q1n][R2Y][W2Y](orderIdx,o9n);var includeIdx=$[m2Y](fieldName,this[q1n][L2Y]);if(includeIdx!==-o9n){var e2Y=J7J;e2Y+=P7J;e2Y+=t7n;this[q1n][k9J][e2Y](includeIdx,o9n);}}else{var E2Y=t7n;E2Y+=a1n;E2Y+=R13.U7n;E2Y+=P9Z;$[E2Y](this[B7J](fieldName),function(i,name){that[n7J](name);});}return this;};Editor[X2Y][V4Z]=function(){this[t4n](J2n);return this;};Editor[D2Y][o7J]=function(arg1,arg2,arg3,arg4){var z1J="mb";var j1J="nu";var y7J="_crudArg";var t7J="ionClass";var i7J="_act";var p7J="initC";var P2Y=p7J;P2Y+=p7n;P2Y+=t7n;P2Y+=Y6Z;var J2Y=S7J;J2Y+=Z5n;var K2Y=t7n;K2Y+=a1n;K2Y+=R13.U7n;K2Y+=P9Z;var u2Y=i7J;u2Y+=t7J;var N2Y=M5n;N2Y+=L2Z;N2Y+=g1Z;var U2Y=U7Z;U2Y+=S3n;var r2Y=B1n;r2Y+=p7n;r2Y+=n1n;var n2Y=m5n;n2Y+=o7n;n2Y+=n1n;var c2Y=H7J;c2Y+=c9Z;var a2Y=n1n;a2Y+=a1n;a2Y+=F1n;a2Y+=y7n;var q2Y=y7J;q2Y+=q1n;var x2Y=j1J;x2Y+=z1J;x2Y+=t7n;x2Y+=p7n;var l2Y=J8J;l2Y+=V5n;var that=this;var fields=this[q1n][F1J];var count=o9n;if(this[l2Y](function(){that[o7J](arg1,arg2,arg3,arg4);})){return this;}if(typeof arg1===x2Y){count=arg1;arg1=arg2;arg2=arg3;}this[q1n][M1J]={};for(var i=B9n;i<count;i++){var G2Y=l5n;G2Y+=F1n;G2Y+=w1J;var b2Y=A1J;b2Y+=B7n;b2Y+=s1J;b2Y+=w1J;this[q1n][b2Y][i]={fields:this[q1n][G2Y]};}var argOpts=this[q2Y](arg1,arg2,arg3,arg4);this[q1n][b5n]=a2Y;this[q1n][c2Y]=o7J;this[q1n][p6J]=F9Z;this[n2Y][r2Y][C2Z][U2Y]=N2Y;this[u2Y]();this[l8J](this[F1J]());$[K2Y](fields,function(name,field){var k2Y=K4n;k2Y+=l5n;field[v8J]();for(var i=B9n;i<count;i++){field[Y1J](i,field[g0n]());}field[H6Z](field[k2Y]());});this[J2Y](P2Y,F9Z,function(){var B2Y=o7n;B2Y+=I1J;B2Y+=q1n;that[T1J]();that[h9J](argOpts[B2Y]);argOpts[d1J]();});return this;};Editor[G9Z][o2Y]=function(parent,url,opts){var f1J="dependen";var I6c=Z1J;I6c+=K4n;var t2Y=G5n;t2Y+=R13.q7n;t2Y+=C1J;var i2Y=z8Z;i2Y+=F8Z;if($[M8J](parent)){var p2Y=l3n;p2Y+=Q1J;for(var i=B9n,ien=parent[p2Y];i<ien;i++){var S2Y=f1J;S2Y+=R13.q7n;this[S2Y](parent[i],url,opts);}return this;}var that=this;var field=this[i2Y](parent);var ajaxOpts={type:O1J,dataType:h1J};opts=$[t2Y]({event:V1J,data:F9Z,preUpdate:F9Z,postUpdate:F9Z},opts);var update=function(json){var G1J="postUpdate";var b1J='enable';var x1J='error';var l1J='message';var D1J='val';var X1J='label';var E1J="preUpdate";var e1J="preUp";var m1J="pdat";var W1J="hid";var R1J="sho";var Y6c=v1J;Y6c+=g1J;Y6c+=r2n;var s6c=G1n;s6c+=q1n;s6c+=B6J;s6c+=t7n;var A6c=R1J;A6c+=V3n;var w6c=W1J;w6c+=t7n;var M6c=t7n;M6c+=a1n;M6c+=L5n;var j6c=r1n;j6c+=m1J;j6c+=t7n;var y2Y=L1J;y2Y+=R13.U7n;y2Y+=P9Z;var H2Y=e1J;H2Y+=j1n;if(opts[H2Y]){opts[E1J](json);}$[y2Y]({labels:X1J,options:j6c,values:D1J,messages:l1J,errors:x1J},function(jsonProp,fieldFn){if(json[jsonProp]){var z6c=t7n;z6c+=a1n;z6c+=R13.U7n;z6c+=P9Z;$[z6c](json[jsonProp],function(field,val){var F6c=z8Z;F6c+=t7n;F6c+=h1n;F6c+=m5n;that[F6c](field)[fieldFn](val);});}});$[M6c]([w6c,A6c,b1J,s6c],function(i,key){if(json[key]){that[key](json[key]);}});if(opts[G1J]){opts[G1J](json);}field[Y6c](J2n);};$(field[I6c]())[g2n](opts[q7J],function(e){var u1J="PlainObjec";var c1J="itFields";var a1J="lues";var v6c=P4n;v6c+=A0n;var V6c=q1J;V6c+=a1J;var h6c=p7n;h6c+=o7n;h6c+=V3n;h6c+=q1n;var O6c=p7n;O6c+=o7n;O6c+=V3n;var f6c=L8Z;f6c+=s1J;f6c+=w1J;var Q6c=A1J;Q6c+=c1J;var C6c=p7n;C6c+=o7n;C6c+=V3n;C6c+=q1n;var Z6c=m8J;Z6c+=n1J;Z6c+=R13.q7n;var d6c=l5n;d6c+=F1n;d6c+=y7n;d6c+=m5n;var T6c=y7n;T6c+=c3n;T6c+=t7n;if($(field[T6c]())[d6c](e[Z6c])[k2n]===B9n){return;}field[j9Z](B2n);var data={};data[C6c]=that[q1n][Q6c]?_pluck(that[q1n][f6c],r1J):F9Z;data[O6c]=data[U1J]?data[h6c][B9n]:F9Z;data[V6c]=that[v6c]();if(opts[R8Z]){var ret=opts[R8Z](data);if(ret){opts[R8Z]=ret;}}if(typeof url===e9Z){var o=url(field[g9Z](),data,update);if(o){var R6c=q9Z;R6c+=y7n;R6c+=C5Z;var g6c=o7n;g6c+=M5n;g6c+=z3n;g6c+=u2n;if(typeof o===g6c&&typeof o[N1J]===R6c){o[N1J](function(resolved){if(resolved){update(resolved);}});}else{update(o);}}}else{var L6c=G5n;L6c+=d5n;L6c+=y7n;L6c+=m5n;var m6c=a1n;m6c+=n4n;var W6c=F1n;W6c+=q1n;W6c+=u1J;W6c+=R13.q7n;if($[W6c](url)){$[C8Z](ajaxOpts,url);}else{ajaxOpts[K1J]=url;}$[m6c]($[L6c](ajaxOpts,{url:url,data:data,success:update}));}});return this;};Editor[e6c][k1J]=function(){var i1J='.dte';var S1J="dest";var p1J="empl";var B1J="yed";var P1J="ar";var J1J="uniqu";var G6c=J1J;G6c+=t7n;var b6c=o7n;b6c+=l5n;b6c+=l5n;var X6c=R13.U7n;X6c+=l3n;X6c+=P1J;var E6c=m5n;E6c+=M0n;E6c+=w0n;E6c+=B1J;if(this[q1n][E6c]){this[V4Z]();}this[X6c]();if(this[q1n][o1J]){var l6c=R13.q7n;l6c+=p1J;l6c+=a1n;l6c+=d5n;var D6c=M5n;D6c+=c3n;D6c+=V5n;$(D6c)[h4Z](this[q1n][l6c]);}var controller=this[q1n][k0Z];if(controller[k1J]){var x6c=S1J;x6c+=p7n;x6c+=o7n;x6c+=V5n;controller[x6c](this);}$(document)[b6c](i1J+this[q1n][G6c]);this[I9Z]=F9Z;this[q1n]=F9Z;};Editor[G9Z][t1J]=function(name){var y1J="ldNames";var H1J="_fi";var a6c=H1J;a6c+=t7n;a6c+=y1J;var q6c=t7n;q6c+=j5J;var that=this;$[q6c](this[a6c](name),function(i,n){var c6c=z8Z;c6c+=m0n;c6c+=m5n;that[c6c](n)[t1J]();});return this;};Editor[n6c][r6c]=function(show){var N6c=s0n;N6c+=o7n;N6c+=q1n;N6c+=t7n;var U6c=x5Z;U6c+=t7n;U6c+=y7n;if(show===undefined){return this[q1n][z5J];}return this[show?U6c:N6c]();};Editor[G9Z][z5J]=function(){return $[F5J](this[q1n][F1J],function(field,name){return field[z5J]()?name:F9Z;});};Editor[u6c][K6c]=function(){var w5J="ntroller";var M5J="displayCo";var J6c=y7n;J6c+=c3n;J6c+=t7n;var k6c=M5J;k6c+=w5J;return this[q1n][k6c][J6c](this);};Editor[P6c][L8Z]=function(items,arg1,arg2,arg3,arg4){var I5J="Args";var Y5J="ud";var s5J="_ed";var S6c=n1n;S6c+=A5J;var p6c=s5J;p6c+=F1n;p6c+=R13.q7n;var o6c=v2n;o6c+=p7n;o6c+=Y5J;o6c+=I5J;var B6c=J8J;B6c+=V5n;var that=this;if(this[B6c](function(){that[L8Z](items,arg1,arg2,arg3,arg4);})){return this;}var argOpts=this[o6c](arg1,arg2,arg3,arg4);this[p6c](items,this[B8J](T5J,items),S6c,argOpts[O9Z],function(){var d5J="maybeOp";var i6c=d5J;i6c+=J7Z;that[T1J]();that[h9J](argOpts[O9Z]);argOpts[i6c]();});return this;};Editor[t6c][Z5J]=function(name){var C5J="_fieldNa";var H6c=C5J;H6c+=Z1n;H6c+=q1n;var that=this;$[D2n](this[H6c](name),function(i,n){var y6c=Q5J;y6c+=m5n;that[y6c](n)[Z5J]();});return this;};Editor[G9Z][E7Z]=function(name,msg){var h5J="globalError";var O5J="sage";var f5J="_mes";if(msg===undefined){var F8c=B1n;F8c+=p7n;F8c+=w9J;F8c+=Y3n;var z8c=m5n;z8c+=o7n;z8c+=n1n;var j8c=f5J;j8c+=O5J;this[j8c](this[z8c][F8c],name);this[q1n][h5J]=name;}else{var M8c=t7n;M8c+=h7Z;this[h8J](name)[M8c](msg);}return this;};Editor[w8c][h8J]=function(name){var V5J='Unknown field name - ';var fields=this[q1n][F1J];if(!fields[name]){throw V5J+name;}return fields[name];};Editor[G9Z][F1J]=function(){var s8c=h8J;s8c+=q1n;var A8c=n1n;A8c+=a1n;A8c+=a5n;return $[A8c](this[q1n][s8c],function(field,name){return name;});};Editor[G9Z][Y8c]=_api_file;Editor[G9Z][x2n]=_api_files;Editor[G9Z][w7J]=function(name){var v5J="sArray";var I8c=F1n;I8c+=v5J;var that=this;if(!name){name=this[F1J]();}if($[I8c](name)){var out={};$[D2n](name,function(i,n){var T8c=l5n;T8c+=F1n;T8c+=F8Z;out[n]=that[T8c](n)[w7J]();});return out;}return this[h8J](name)[w7J]();};Editor[d8c][t0Z]=function(names,animate){var R5J="ldName";var g5J="_fie";var C8c=g5J;C8c+=R5J;C8c+=q1n;var Z8c=t7n;Z8c+=a1n;Z8c+=R13.U7n;Z8c+=P9Z;var that=this;$[Z8c](this[C8c](names),function(i,n){var Q8c=l5n;Q8c+=p1n;Q8c+=m5n;that[Q8c](n)[t0Z](animate);});return this;};Editor[G9Z][f8c]=function(includeHash){var h8c=L8Z;h8c+=W5J;var O8c=n1n;O8c+=a1n;O8c+=a5n;return $[O8c](this[q1n][h8c],function(edit,idSrc){return includeHash===B2n?m5J+idSrc:idSrc;});};Editor[V8c][L5J]=function(inNames){var E5J="lError";var e5J="glo";var W8c=h1n;W8c+=t7n;W8c+=r2n;W8c+=U2n;var R8c=e5J;R8c+=N6J;R8c+=E5J;var g8c=l5n;g8c+=X5J;g8c+=p7n;g8c+=Y3n;var v8c=m5n;v8c+=o7n;v8c+=n1n;var formError=$(this[v8c][g8c]);if(this[q1n][R8c]){return B2n;}var names=this[B7J](inNames);for(var i=B9n,ien=names[W8c];i<ien;i++){if(this[h8J](names[i])[L5J]()){return B2n;}}return J2n;};Editor[m8c][L8c]=function(cell,fieldName,opts){var N5J='div.DTE_Field';var G5J="inline";var b5J="ject";var x5J="isPlainOb";var D5J="_ti";var q8c=D5J;q8c+=m5n;q8c+=V5n;var D8c=l5J;D8c+=m5n;D8c+=r1n;D8c+=A0n;var X8c=l5n;X8c+=x5n;var E8c=o5n;E8c+=C1J;var e8c=x5J;e8c+=b5J;var that=this;if($[e8c](fieldName)){opts=fieldName;fieldName=undefined;}opts=$[E8c]({},this[q1n][X8c][G5J],opts);var editFields=this[B8J](D8c,cell,fieldName);var node,field;var countOuter=B9n,countInner;var closed=J2n;var classes=this[J9Z][G5J];$[D2n](editFields,function(i,editField){var a5J='Cannot edit more than one row inline at a time';var b8c=m5n;b8c+=R1Z;b8c+=W5J;var x8c=t7n;x8c+=a1n;x8c+=L5n;var l8c=q5J;l8c+=a1n;l8c+=L5n;if(countOuter>B9n){throw a5J;}node=$(editField[l8c][B9n]);countInner=B9n;$[x8c](editField[b8c],function(j,f){var U5J="ld inline at a time";var r5J="ne fie";var n5J="n o";var c5J="Cannot edit more tha";if(countInner>B9n){var G8c=c5J;G8c+=n5J;G8c+=r5J;G8c+=U5J;throw G8c;}field=f;countInner++;});countOuter++;});if($(N5J,node)[k2n]){return this;}if(this[q8c](function(){var u5J="nlin";var a8c=F1n;a8c+=u5J;a8c+=t7n;that[a8c](cell,fieldName,opts);})){return this;}this[o8J](cell,editFields,K5J,opts,function(){var z4J='px">';var j4J="contents";var t5J="div cl";var i5J="style=\"width:";var S5J="\" ";var p5J="ng_Indicator\"><span/></div>";var o5J="<div class=\"DTE_Pro";var B5J="/div";var P5J="v clas";var Z9c=y6Z;Z9c+=h1n;Z9c+=k9Z;var d9c=k5J;d9c+=J5J;d9c+=q1n;var t8c=l5n;t8c+=X5J;t8c+=p7n;t8c+=Y3n;var i8c=h1Z;i8c+=n1n;var S8c=A2n;S8c+=E1Z;S8c+=X1Z;S8c+=t7n;var p8c=h1n;p8c+=F1n;p8c+=y7n;p8c+=s3n;var o8c=m5n;o8c+=f6Z;o8c+=h2Z;var B8c=Q6Z;B8c+=q6Z;var P8c=M5n;P8c+=r1n;P8c+=j7J;P8c+=X5n;var J8c=d6Z;J8c+=G1n;J8c+=P5J;J8c+=R6Z;var k8c=d6Z;k8c+=B5J;k8c+=O6Z;var K8c=o5J;K8c+=W1Z;K8c+=p5J;var u8c=S5J;u8c+=i5J;var N8c=h1n;N8c+=F1n;N8c+=v7Z;var U8c=d6Z;U8c+=t5J;U8c+=B6Z;var r8c=M3Z;r8c+=t7n;r8c+=y7n;r8c+=m5n;var n8c=m5n;n8c+=t7n;n8c+=H5J;n8c+=P9Z;var c8c=y6Z;c8c+=K3n;var namespace=that[h9J](opts);var ret=that[y5J](c8c);if(!ret){return that;}var children=node[j4J]()[n8c]();node[r8c]($(U8c+classes[D8Z]+q8Z+m9J+classes[N8c]+u8c+node[S2Z]()+z4J+K8c+k8c+J8c+classes[P8c]+u8Z+B8c));node[F4J](o8c+classes[p8c][S8c](/ /g,M4J))[h4Z](field[S6J]())[h4Z](that[i8c][t8c]);if(opts[q9J]){var y8c=h1Z;y8c+=n1n;var H8c=B5Z;H8c+=q1n;node[F4J](w4J+classes[H8c][b1Z](/ /g,M4J))[h4Z](that[y8c][q9J]);}that[a9J](function(submitComplete){var s4J="micInfo";var A4J="clearDy";var F9c=C5n;F9c+=A4J;F9c+=g8Z;F9c+=s4J;var j9c=R13.U7n;j9c+=P7J;j9c+=G0n;closed=B2n;$(document)[a2Z](j9c+namespace);if(!submitComplete){var z9c=R13.U7n;z9c+=g2n;z9c+=d5n;z9c+=Y4J;node[z9c]()[B7Z]();node[h4Z](children);}that[F9c]();});setTimeout(function(){if(closed){return;}$(document)[g2n](I4J+namespace,function(e){var O4J="B";var f4J="Sel";var Q4J="and";var C4J="peF";var Z4J="_ty";var d4J="arge";var T4J="ren";var I9c=Z5Z;I9c+=T4J;I9c+=s1n;var Y9c=R13.q7n;Y9c+=d4J;Y9c+=R13.q7n;var s9c=o7n;s9c+=V3n;s9c+=j9J;var A9c=Z4J;A9c+=C4J;A9c+=y7n;var w9c=Q4J;w9c+=f4J;w9c+=l5n;var M9c=z8J;M9c+=O4J;M9c+=X1Z;M9c+=G0n;var back=$[f2n][h4J]?M9c:w9c;if(!field[A9c](s9c,e[Y9c])&&$[D8J](node[B9n],$(e[l3Z])[I9c]()[back]())===-o9n){var T9c=S9Z;T9c+=r1n;T9c+=p7n;that[T9c]();}});},B9n);that[d9c]([field],opts[R9Z]);that[V4J](Z9c);});return this;};Editor[G9Z][C9c]=function(name,msg){var v4J="sag";if(msg===undefined){var f9c=m5n;f9c+=o7n;f9c+=n1n;var Q9c=C5n;Q9c+=z9J;Q9c+=v4J;Q9c+=t7n;this[Q9c](this[f9c][b9J],name);}else{this[h8J](name)[t8Z](msg);}return this;};Editor[O9c][b5n]=function(mode){var R4J='Not currently in an editing mode';var g4J="actio";if(!mode){var h9c=g4J;h9c+=y7n;return this[q1n][h9c];}if(!this[q1n][P6J]){throw R4J;}this[q1n][P6J]=mode;return this;};Editor[V9c][v9c]=function(){var W4J="ifier";var g9c=Z0n;g9c+=W4J;return this[q1n][g9c];};Editor[R9c][m4J]=function(fieldNames){var L4J="multiGe";var e9c=L4J;e9c+=R13.q7n;var L9c=l5n;L9c+=e4J;var W9c=U9Z;W9c+=U5n;W9c+=E4J;var that=this;if(fieldNames===undefined){fieldNames=this[F1J]();}if($[W9c](fieldNames)){var out={};$[D2n](fieldNames,function(i,name){var X4J="tiG";var m9c=i2n;m9c+=X4J;m9c+=t7n;m9c+=R13.q7n;out[name]=that[h8J](name)[m9c]();});return out;}return this[L9c](fieldNames)[e9c]();};Editor[E9c][Y1J]=function(fieldNames,val){var D4J="isPlai";var X9c=D4J;X9c+=y7n;X9c+=l4J;var that=this;if($[X9c](fieldNames)&&val===undefined){var D9c=L1J;D9c+=L5n;$[D9c](fieldNames,function(name,value){var l9c=z8Z;l9c+=t7n;l9c+=E0n;that[l9c](name)[Y1J](value);});}else{this[h8J](fieldNames)[Y1J](val);}return this;};Editor[G9Z][x9c]=function(name){var x4J="rd";var a9c=y7n;a9c+=o7n;a9c+=m5n;a9c+=t7n;var G9c=u3Z;G9c+=a5n;var that=this;if(!name){var b9c=o7n;b9c+=x4J;b9c+=t7n;b9c+=p7n;name=this[b9c]();}return $[M8J](name)?$[G9c](name,function(n){var b4J="ode";var q9c=y7n;q9c+=b4J;return that[h8J](n)[q9c]();}):this[h8J](name)[a9c]();};Editor[c9c][a2Z]=function(name,fn){var q4J="tName";var n9c=G4J;n9c+=q4J;$(this)[a2Z](this[n9c](name),fn);return this;};Editor[r9c][g2n]=function(name,fn){$(this)[g2n](this[a4J](name),fn);return this;};Editor[U9c][V1Z]=function(name,fn){$(this)[V1Z](this[a4J](name),fn);return this;};Editor[N9c][u9c]=function(){var r4J="yCon";var i9c=n1n;i9c+=a1n;i9c+=F1n;i9c+=y7n;var B9c=c4J;B9c+=y7n;var P9c=F7Z;P9c+=n4J;P9c+=r4J;P9c+=A4Z;var that=this;this[l8J]();this[a9J](function(submitComplete){var N4J="ler";var U4J="displayControl";var k9c=R13.U7n;k9c+=L2Z;k9c+=q1n;k9c+=t7n;var K9c=U4J;K9c+=N4J;that[q1n][K9c][k9c](that,function(){var u4J="earDynamic";var J9c=K9J;J9c+=u4J;J9c+=L5Z;that[J9c]();});});var ret=this[y5J](K4J);if(!ret){return this;}this[q1n][P9c][B9c](this,this[I9Z][D8Z],function(){var S9c=l5n;S9c+=e5Z;S9c+=r1n;S9c+=q1n;var p9c=n1n;p9c+=a1n;p9c+=a5n;var o9c=k5J;o9c+=J5J;o9c+=q1n;that[o9c]($[p9c](that[q1n][E8J],function(name){return that[q1n][F1J][name];}),that[q1n][k4J][S9c]);});this[V4J](i9c);return this;};Editor[t9c][H9c]=function(set){var i4J="ng.";var S4J="All fields, and no additional fields, must be provided for orderi";var p4J="sort";var P4J="isA";var J4J="so";var A7c=q3n;A7c+=s3n;var M7c=z3n;M7c+=o7n;M7c+=F1n;M7c+=y7n;var F7c=q1n;F7c+=h1n;F7c+=o2n;F7c+=t7n;var z7c=J4J;z7c+=p7n;z7c+=R13.q7n;var j7c=P4J;j7c+=p7n;j7c+=u1Z;j7c+=V5n;if(!set){var y9c=i1n;y9c+=i3Z;return this[q1n][y9c];}if(arguments[k2n]&&!$[j7c](set)){set=Array[G9Z][u5Z][l9Z](arguments);}if(this[q1n][E8J][u5Z]()[z7c]()[B4J](o4J)!==set[F7c]()[p4J]()[M7c](o4J)){var w7c=S4J;w7c+=i4J;throw w7c;}$[C8Z](this[q1n][A7c],set);this[l8J]();return this;};Editor[G9Z][t4J]=function(items,arg1,arg2,arg3,arg4){var j3J="udArgs";var H4J="itRem";var Q7c=m5n;Q7c+=v8Z;var C7c=y6Z;C7c+=H4J;C7c+=X1n;var Z7c=C5n;Z7c+=t7n;Z7c+=y4J;Z7c+=R13.q7n;var d7c=m5n;d7c+=R1Z;var T7c=L8Z;T7c+=g1n;T7c+=q1n;var I7c=H7J;I7c+=F1n;I7c+=o7n;I7c+=y7n;var Y7c=v2n;Y7c+=p7n;Y7c+=j3J;var s7c=l3n;s7c+=Q1J;var that=this;if(this[z3J](function(){that[t4J](items,arg1,arg2,arg3,arg4);})){return this;}if(items[s7c]===undefined){items=[items];}var argOpts=this[Y7c](arg1,arg2,arg3,arg4);var editFields=this[B8J](T5J,items);this[q1n][I7c]=t4J;this[q1n][p6J]=items;this[q1n][T7c]=editFields;this[I9Z][l9J][C2Z][d7c]=K7Z;this[F3J]();this[Z7c](C7c,[_pluck(editFields,M3J),_pluck(editFields,Q7c),items],function(){var A3J='initMultiRemove';that[w3J](A3J,[editFields,items],function(){var Y3J="_formOption";var s3J="itOpts";var h7c=l5n;h7c+=e5Z;h7c+=q7Z;var O7c=A1J;O7c+=s3J;var f7c=Y3J;f7c+=q1n;that[T1J]();that[f7c](argOpts[O9Z]);argOpts[d1J]();var opts=that[q1n][O7c];if(opts[h7c]!==F9Z){var V7c=M5n;V7c+=x7Z;V7c+=I3J;$(V7c,that[I9Z][q9J])[D9J](opts[R9Z])[R9Z]();}});});return this;};Editor[G9Z][v7c]=function(set,val){var d3J="Ob";var T3J="isPlain";var g7c=T3J;g7c+=d3J;g7c+=z3n;g7c+=u2n;var that=this;if(!$[g7c](set)){var o={};o[set]=val;set=o;}$[D2n](set,function(n,v){that[h8J](n)[H6Z](v);});return this;};Editor[G9Z][Z3J]=function(names,animate){var R7c=L1J;R7c+=R13.U7n;R7c+=P9Z;var that=this;$[R7c](this[B7J](names),function(i,n){var m7c=q1n;m7c+=P9Z;m7c+=o7n;m7c+=V3n;var W7c=z8Z;W7c+=F8Z;that[W7c](n)[m7c](animate);});return this;};Editor[G9Z][L7c]=function(successCallback,errorCallback,formatdata,hide){var C3J="_proces";var b7c=t7n;b7c+=a1n;b7c+=R13.U7n;b7c+=P9Z;var x7c=t7n;x7c+=a1n;x7c+=L5n;var l7c=t7n;l7c+=p7n;l7c+=k4n;l7c+=p7n;var e7c=C3J;e7c+=Q3J;e7c+=r0n;var that=this,fields=this[q1n][F1J],errorFields=[],errorReady=B9n,sent=J2n;if(this[q1n][j9Z]||!this[q1n][P6J]){return this;}this[e7c](B2n);var send=function(){var f3J="itSubmit";var X7c=y6Z;X7c+=f3J;var E7c=m7Z;E7c+=U2n;if(errorFields[E7c]!==errorReady||sent){return;}that[w3J](X7c,[that[q1n][P6J]],function(result){var D7c=m4n;D7c+=S8J;D7c+=E4n;if(result===J2n){that[O3J](J2n);return;}sent=B2n;that[D7c](successCallback,errorCallback,formatdata,hide);});};this[l7c]();$[x7c](fields,function(name,field){if(field[L5J]()){errorFields[l2n](name);}});$[b7c](errorFields,function(i,name){var G7c=t7n;G7c+=h7Z;fields[name][G7c](C2n,function(){errorReady++;send();});});send();return this;};Editor[G9Z][q7c]=function(set){var V3J="plate";var h3J="tem";if(set===undefined){var a7c=h3J;a7c+=V3J;return this[q1n][a7c];}this[q1n][o1J]=set===F9Z?F9Z:$(set);return this;};Editor[G9Z][c7c]=function(title){var g3J="lasses";var v3J="ader";var J7c=Z3Z;J7c+=n1n;J7c+=h1n;var N7c=j0Z;N7c+=v3J;var U7c=R13.U7n;U7c+=g3J;var r7c=m5n;r7c+=F1n;r7c+=P4n;r7c+=h2Z;var n7c=h1Z;n7c+=n1n;var header=$(this[n7c][J6J])[O4Z](r7c+this[U7c][N7c][q4Z]);if(title===undefined){var u7c=Z3Z;u7c+=n1n;u7c+=h1n;return header[u7c]();}if(typeof title===e9Z){var k7c=R13.q7n;k7c+=p7Z;k7c+=h1n;k7c+=t7n;var K7c=U5n;K7c+=a5n;K7c+=F1n;title=title(this,new DataTable[K7c](this[q1n][k7c]));}header[J7c](title);return this;};Editor[P7c][B7c]=function(field,value){var R3J="Pl";var p7c=r0n;p7c+=t7n;p7c+=R13.q7n;var o7c=U9Z;o7c+=R3J;o7c+=A5J;o7c+=l4J;if(value!==undefined||$[o7c](field)){return this[H6Z](field,value);}return this[p7c](field);};var apiRegister=DataTable[W3J][m3J];function __getInst(api){var L3J="oInit";var S7c=R13.U7n;S7c+=o7n;S7c+=y7n;S7c+=k5Z;var ctx=api[S7c][B9n];return ctx[L3J][e3J]||ctx[E3J];}function __setBasic(inst,opts,type,plural){var G3J='1';var b3J=/%d/;var l3J="mess";var X3J="_b";if(!opts){opts={};}if(opts[q9J]===undefined){var i7c=X3J;i7c+=D3J;i7c+=F1n;i7c+=R13.U7n;opts[q9J]=i7c;}if(opts[J8Z]===undefined){var t7c=R13.q7n;t7c+=F1n;t7c+=R13.q7n;t7c+=l3n;opts[J8Z]=inst[Y8Z][type][t7c];}if(opts[t8Z]===undefined){var H7c=p7n;H7c+=t7n;H7c+=n1n;H7c+=X1n;if(type===H7c){var z1c=l1Z;z1c+=k0n;var j1c=l3J;j1c+=a1n;j1c+=i1Z;var y7c=F1n;y7c+=M1n;y7c+=w1n;y7c+=y7n;var confirm=inst[y7c][type][x3J];opts[j1c]=plural!==o9n?confirm[C5n][z1c](b3J,plural):confirm[G3J];}else{opts[t8Z]=C2n;}}return opts;}apiRegister(F1c,function(){return __getInst(this);});apiRegister(M1c,function(opts){var inst=__getInst(this);inst[o7J](__setBasic(inst,opts,q3J));return this;});apiRegister(w1c,function(opts){var inst=__getInst(this);inst[L8Z](this[B9n][B9n],__setBasic(inst,opts,a3J));return this;});apiRegister(A1c,function(opts){var inst=__getInst(this);inst[L8Z](this[B9n],__setBasic(inst,opts,a3J));return this;});apiRegister(s1c,function(opts){var Y1c=p7n;Y1c+=c3J;var inst=__getInst(this);inst[Y1c](this[B9n][B9n],__setBasic(inst,opts,n3J,o9n));return this;});apiRegister(r3J,function(opts){var U3J="remov";var I1c=U3J;I1c+=t7n;var inst=__getInst(this);inst[t4J](this[B9n],__setBasic(inst,opts,I1c,this[B9n][k2n]));return this;});apiRegister(T1c,function(type,opts){var K3J="inli";var u3J="inlin";var N3J="isPlainObj";var Z1c=N3J;Z1c+=u2n;if(!type){var d1c=u3J;d1c+=t7n;type=d1c;}else if($[Z1c](type)){var C1c=K3J;C1c+=m1n;opts=type;type=C1c;}__getInst(this)[type](this[B9n][B9n],opts);return this;});apiRegister(Q1c,function(opts){var k3J="ubbl";var f1c=M5n;f1c+=k3J;f1c+=t7n;__getInst(this)[f1c](this[B9n],opts);return this;});apiRegister(O1c,_api_file);apiRegister(J3J,_api_files);$(document)[g2n](h1c,function(e,ctx,json){var B3J='dt';if(e[P3J]!==B3J){return;}if(json&&json[x2n]){var V1c=t7n;V1c+=j5J;$[V1c](json[x2n],function(name,files){var o3J="iles";var v1c=l5n;v1c+=o3J;Editor[v1c][name]=files;});}});Editor[g1c]=function(msg,tn){var t3J=" https://datatables.net/tn/";var i3J="refer to";var S3J="e ";var p3J=" For more information, pleas";var R1c=p3J;R1c+=S3J;R1c+=i3J;R1c+=t3J;throw tn?msg+R1c+tn:msg;};Editor[H3J]=function(data,props,fn){var W1c=N6Z;W1c+=m0n;var i,ien,dataPoint;props=$[C8Z]({label:W1c,value:y3J},props);if($[M8J](data)){for(i=B9n,ien=data[k2n];i<ien;i++){dataPoint=data[i];if($[P8J](dataPoint)){var e1c=P4n;e1c+=e1Z;var L1c=h1n;L1c+=p7Z;L1c+=t7n;L1c+=h1n;var m1c=P4n;m1c+=A0n;m1c+=r1n;m1c+=t7n;fn(dataPoint[props[m1c]]===undefined?dataPoint[props[L1c]]:dataPoint[props[e1c]],dataPoint[props[G8Z]],i,dataPoint[j0J]);}else{fn(dataPoint,dataPoint,i);}}}else{i=B9n;$[D2n](data,function(key,val){fn(val,key,i);i++;});}};Editor[z0J]=function(id){return id[b1Z](/\./g,o4J);};Editor[E1c]=function(editor,conf,files,progressCallback,completeCallback){var s2J="imitLe";var Z0J="onload";var d0J="<i>Uploading file</i>";var T0J="fileReadText";var I0J='A server error occurred while uploading the file';var Y0J="itLef";var s0J="_li";var A0J="L";var w0J="taUR";var M0J="dAsDa";var m5c=F0J;m5c+=M0J;m5c+=w0J;m5c+=A0J;var g5c=s0J;g5c+=n1n;g5c+=Y0J;g5c+=R13.q7n;var reader=new FileReader();var counter=B9n;var ids=[];var generalError=I0J;editor[E7Z](conf[Q8Z],C2n);progressCallback(conf,conf[T0J]||d0J);reader[Z0J]=function(e){var n0J="upload";var b0J="RL";var x0J="sDataU";var X0J="rin";var E0J='No Ajax option specified for upload plug-in';var L0J="ploa";var m0J="ajaxData";var W0J='upload';var R0J="upl";var g0J="uploadF";var V0J="bj";var h0J="isPlainO";var O0J="reU";var f0J="pload";var Q0J="preSubmit.DTE_U";var C0J="pos";var B1c=C0J;B1c+=R13.q7n;var P1c=Q0J;P1c+=f0J;var u1c=g8Z;u1c+=n1n;u1c+=t7n;var N1c=a5n;N1c+=O0J;N1c+=f0J;var n1c=m5n;n1c+=a1n;n1c+=R13.q7n;n1c+=a1n;var q1c=h0J;q1c+=V0J;q1c+=u2n;var G1c=v0J;G1c+=F3n;var b1c=k7Z;b1c+=a3n;b1c+=m5n;var x1c=g0J;x1c+=e4J;var l1c=R0J;l1c+=o7n;l1c+=a1n;l1c+=m5n;var D1c=X1Z;D1c+=R13.q7n;D1c+=F1n;D1c+=g2n;var X1c=a1n;X1c+=D2Z;X1c+=y7n;X1c+=m5n;var data=new FormData();var ajax;data[X1c](D1c,l1c);data[h4Z](x1c,conf[Q8Z]);data[b1c](W0J,files[counter]);if(conf[m0J]){conf[m0J](data);}if(conf[G1c]){ajax=conf[x8J];}else if($[q1c](editor[q1n][x8J])){var c1c=r1n;c1c+=L0J;c1c+=m5n;var a1c=r1n;a1c+=e0J;a1c+=A3n;ajax=editor[q1n][x8J][a1c]?editor[q1n][x8J][c1c]:editor[q1n][x8J];}else if(typeof editor[q1n][x8J]===r7J){ajax=editor[q1n][x8J];}if(!ajax){throw E0J;}if(typeof ajax===r7J){ajax={url:ajax};}if(typeof ajax[n1c]===e9Z){var U1c=t7n;U1c+=a1n;U1c+=L5n;var r1c=q1n;r1c+=R13.q7n;r1c+=X0J;r1c+=r0n;var d={};var ret=ajax[R8Z](d);if(ret!==undefined&&typeof ret!==r1c){d=ret;}$[U1c](d,function(key,value){data[h4Z](key,value);});}var preRet=editor[w3J](N1c,[conf[u1c],files[counter],data]);if(preRet===J2n){var K1c=h1n;K1c+=t7n;K1c+=D0J;K1c+=P9Z;if(counter<files[K1c]-o9n){var k1c=l0J;k1c+=U5n;k1c+=x0J;k1c+=b0J;counter++;reader[k1c](files[counter]);}else{var J1c=R13.U7n;J1c+=A0n;J1c+=h1n;completeCallback[J1c](editor,ids);}return;}var submit=J2n;editor[g2n](P1c,function(){submit=B2n;return J2n;});$[x8J]($[C8Z]({},ajax,{type:B1c,data:data,dataType:h1J,contentType:J2n,processData:J2n,xhr:function(){var J0J="onloadend";var c0J="ress";var a0J="etting";var q0J="axS";var S1c=G0J;S1c+=m5n;var p1c=H7n;p1c+=P9Z;p1c+=p7n;var o1c=v0J;o1c+=q0J;o1c+=a0J;o1c+=q1n;var xhr=$[o1c][p1c]();if(xhr[S1c]){var i1c=g2n;i1c+=R4n;i1c+=I2n;i1c+=c0J;xhr[n0J][i1c]=function(e){var k0J=':';var K0J="%";var u0J="loaded";var N0J="otal";var U0J="oFix";var r0J="lengthComputable";if(e[r0J]){var y1c=K1Z;y1c+=r0n;y1c+=R13.q7n;y1c+=P9Z;var H1c=R13.q7n;H1c+=U0J;H1c+=t7n;H1c+=m5n;var t1c=R13.q7n;t1c+=N0J;var percent=(e[u0J]/e[t1c]*W7n)[H1c](B9n)+K0J;progressCallback(conf,files[k2n]===o9n?percent:counter+k0J+files[y1c]+l8Z+percent);}};xhr[n0J][J0J]=function(e){var B0J='Processing';var P0J="ssingText";var j5c=v1J;j5c+=P0J;progressCallback(conf,conf[j5c]||B0J);};}return xhr;},success:function(json){var w2J="ataURL";var M2J="sD";var F2J="fil";var H0J="ubmit.DTE_Upload";var t0J="preS";var S0J="cc";var p0J="uploadXhrSu";var o0J="Errors";var Y5c=R0J;Y5c+=o7n;Y5c+=A3n;var w5c=z8Z;w5c+=m0n;w5c+=m5n;w5c+=o0J;var M5c=p0J;M5c+=S0J;M5c+=i0J;var F5c=t0J;F5c+=H0J;var z5c=o7n;z5c+=q6J;editor[z5c](F5c);editor[w3J](M5c,[conf[Q8Z],json]);if(json[w5c]&&json[y0J][k2n]){var A5c=l3n;A5c+=Q1J;var errors=json[y0J];for(var i=B9n,ien=errors[A5c];i<ien;i++){var s5c=y7n;s5c+=a1n;s5c+=Z1n;editor[E7Z](errors[i][s5c],errors[i][j2J]);}}else if(json[E7Z]){editor[E7Z](json[E7Z]);}else if(!json[Y5c]||!json[n0J][k3n]){var T5c=y7n;T5c+=z2J;T5c+=t7n;var I5c=s3n;I5c+=k4n;I5c+=p7n;editor[I5c](conf[T5c],generalError);}else{var f5c=F1n;f5c+=m5n;var Q5c=C1Z;Q5c+=q1n;Q5c+=P9Z;if(json[x2n]){var d5c=F2J;d5c+=t7n;d5c+=q1n;$[D2n](json[d5c],function(table,files){var C5c=W5n;C5c+=m5n;if(!Editor[x2n][table]){var Z5c=F2J;Z5c+=H5n;Editor[Z5c][table]={};}$[C5c](Editor[x2n][table],files);});}ids[Q5c](json[n0J][f5c]);if(counter<files[k2n]-o9n){var O5c=l0J;O5c+=U5n;O5c+=M2J;O5c+=w2J;counter++;reader[O5c](files[counter]);}else{var h5c=R13.U7n;h5c+=a1n;h5c+=h1n;h5c+=h1n;completeCallback[h5c](editor,ids);if(submit){editor[I5Z]();}}}progressCallback(conf);},error:function(xhr){var A2J="loadXhrE";var v5c=v7J;v5c+=A2J;v5c+=p7n;v5c+=Y3n;var V5c=G4J;V5c+=R13.q7n;editor[V5c](v5c,[conf[Q8Z],xhr]);editor[E7Z](conf[Q8Z],generalError);progressCallback(conf);}}));};files=$[F5J](files,function(val){return val;});if(conf[g5c]!==undefined){var W5c=h1n;W5c+=t7n;W5c+=D0J;W5c+=P9Z;var R5c=C5n;R5c+=h1n;R5c+=s2J;R5c+=M7J;files[Y2J](conf[R5c],files[W5c]);}reader[m5c](files[B9n]);};Editor[G9Z][I2J]=function(init){var G6y="nTable";var b6y='processing';var x6y='body_content';var D6y='foot';var X6y='form_content';var v6y="ools";var V6y="TableT";var h6y="NS";var O6y="TO";var f6y="BU";var C6y="creat";var d6y='<div data-dte-e="form_buttons" class="';var T6y='"><div class="';var I6y='<div data-dte-e="head" class="';var Y6y='<div data-dte-e="form_info" class="';var s6y='<div data-dte-e="form_error" class="';var A6y="indicator";var w6y="unique";var z6y="domTable";var j6y="efault";var y2J="etti";var H2J="bT";var i2J="rces";var S2J="taSou";var p2J="Ajax";var o2J="legacy";var B2J="sse";var J2J="g\" cla";var k2J="<div data-dte-e=\"processin";var K2J="rocessing";var u2J="><span/></div";var N2J="\" clas";var U2J="e-e=\"body";var r2J="<div data-dt";var n2J="-e=\"body_content\" class=\"";var c2J="foot\" class=\"";var a2J="<div data-dte-e=\"";var q2J="foot";var G2J="=\"form\" class=\"";var b2J="-e";var x2J="m data-dte";var l2J="<for";var D2J="nt\" class=\"";var X2J=" data-dte-e=\"form_conte";var e2J="m>";var m2J="\"/></div";var W2J="bleTo";var g2J="ormConte";var V2J="oce";var O2J=".dte";var f2J="t.d";var Q2J="ini";var C2J="t.dte";var Z2J="r.d";var d2J="niq";var T2J="itCompl";var u4c=y6Z;u4c+=T2J;u4c+=G3n;u4c+=t7n;var N4c=S7J;N4c+=y7n;N4c+=R13.q7n;var a4c=r1n;a4c+=d2J;a4c+=l5Z;var q4c=H7n;q4c+=P9Z;q4c+=Z2J;q4c+=C2J;var b4c=Q2J;b4c+=f2J;b4c+=R13.q7n;b4c+=O2J;var x4c=o7n;x4c+=y7n;var l4c=h2J;l4c+=h1n;l4c+=m5n;l4c+=q1n;var D4c=R4n;D4c+=V2J;D4c+=g1J;D4c+=r2n;var X4c=v2J;X4c+=V5n;var E4c=B1n;E4c+=o7n;E4c+=R13.q7n;E4c+=s3n;var e4c=l5n;e4c+=g2J;e4c+=y7n;e4c+=R13.q7n;var L4c=m5n;L4c+=o7n;L4c+=n1n;var v4c=q7J;v4c+=q1n;var I4c=R2J;I4c+=W2J;I4c+=o7n;I4c+=V0n;var Y4c=m2J;Y4c+=O6Z;var s4c=P3Z;s4c+=a5n;s4c+=B4Z;var A4c=l5n;A4c+=o7n;A4c+=L2J;var w4c=G6Z;w4c+=l5n;w4c+=i1n;w4c+=e2J;var M4c=e2n;M4c+=Z6Z;M4c+=O6Z;var F4c=l5n;F4c+=o7n;F4c+=p7n;F4c+=n1n;var z4c=E2J;z4c+=X2J;z4c+=D2J;var j4c=e2n;j4c+=O6Z;var y5c=R13.q7n;y5c+=a1n;y5c+=r0n;var H5c=l2J;H5c+=x2J;H5c+=b2J;H5c+=G2J;var t5c=e2n;t5c+=Z6Z;t5c+=O6Z;var i5c=s7Z;i5c+=S0Z;var S5c=B1n;S5c+=p4n;S5c+=t7n;S5c+=p7n;var p5c=q2J;p5c+=t7n;p5c+=p7n;var o5c=a2J;o5c+=c2J;var B5c=e2n;B5c+=Z6Z;B5c+=O6Z;var P5c=l6Z;P5c+=n2J;var J5c=H4Z;J5c+=k7Z;J5c+=B4Z;var k5c=M5n;k5c+=n4Z;var K5c=r2J;K5c+=U2J;K5c+=N2J;K5c+=R6Z;var u5c=e2n;u5c+=u2J;u5c+=O6Z;var N5c=a5n;N5c+=K2J;var U5c=k2J;U5c+=J2J;U5c+=P2J;var r5c=e2n;r5c+=O6Z;var n5c=Z0n;n5c+=L0n;var c5c=F1n;c5c+=M1n;c5c+=w1n;c5c+=y7n;var a5c=C9J;a5c+=q1n;a5c+=t7n;a5c+=q1n;var q5c=s0n;q5c+=a1n;q5c+=B2J;q5c+=q1n;var G5c=o2J;G5c+=p2J;var b5c=B1n;b5c+=T0n;b5c+=d0n;var x5c=R8Z;x5c+=M8Z;x5c+=a1n;x5c+=f9Z;var l5c=W8J;l5c+=S2J;l5c+=i2J;var D5c=I9Z;D5c+=R2J;D5c+=f9Z;var X5c=v0J;X5c+=t2J;var E5c=m5n;E5c+=H2J;E5c+=a1n;E5c+=f9Z;var e5c=q1n;e5c+=y2J;e5c+=y7n;e5c+=j8Z;var L5c=m5n;L5c+=j6y;L5c+=q1n;init=$[C8Z](B2n,{},Editor[L5c],init);this[q1n]=$[C8Z](B2n,{},Editor[T9Z][e5c],{table:init[z6y]||init[f5Z],dbTable:init[E5c]||F9Z,ajaxUrl:init[X5c],ajax:init[x8J],idSrc:init[F6y],dataSource:init[D5c]||init[f5Z]?Editor[l5c][x5c]:Editor[M6y][o7Z],formOptions:init[b5c],legacyAjax:init[G5c],template:init[o1J]?$(init[o1J])[B7Z]():F9Z});this[q5c]=$[C8Z](B2n,{},Editor[a5c]);this[c5c]=init[Y8Z];Editor[n5c][J5Z][w6y]++;var that=this;var classes=this[J9Z];this[I9Z]={"wrapper":$(m9J+classes[D8Z]+r5c+U5c+classes[N5c][A6y]+u5c+K5c+classes[k5c][J5c]+q8Z+P5c+classes[T2Z][q4Z]+B5c+c8Z+o5c+classes[p5c][D8Z]+q8Z+m9J+classes[S5c][i5c]+t5c+c8Z+c8Z)[B9n],"form":$(H5c+classes[l9J][y5c]+j4c+z4c+classes[F4c][q4Z]+M4c+w4c)[B9n],"formError":$(s6y+classes[A4c][E7Z]+u8Z)[B9n],"formInfo":$(Y6y+classes[l9J][K1n]+u8Z)[B9n],"header":$(I6y+classes[J6J][s4c]+T6y+classes[J6J][q4Z]+Y4c)[B9n],"buttons":$(d6y+classes[l9J][q9J]+u8Z)[B9n]};if($[f2n][r0Z][I4c]){var O4c=Z6y;O4c+=P4n;O4c+=t7n;var f4c=t7n;f4c+=m5n;f4c+=F1n;f4c+=R13.q7n;var Q4c=C6y;Q4c+=t7n;var C4c=Q6y;C4c+=P9Z;var Z4c=f6y;Z4c+=M8Z;Z4c+=O6y;Z4c+=h6y;var d4c=V6y;d4c+=v6y;var T4c=l5n;T4c+=y7n;var ttButtons=$[T4c][r0Z][d4c][Z4c];var i18n=this[Y8Z];$[C4c]([Q4c,f4c,O4c],function(i,val){var m6y='editor_';var W6y="nTex";var R6y="sButto";var V4c=M5n;V4c+=g6y;V4c+=y7n;var h4c=R6y;h4c+=W6y;h4c+=R13.q7n;ttButtons[m6y+val][h4c]=i18n[val][V4c];});}$[D2n](init[v4c],function(evt,fn){that[g2n](evt,function(){var E6y="shift";var e6y="ototy";var m4c=k7Z;m4c+=a5n;m4c+=L6y;var W4c=J0n;W4c+=h1n;W4c+=h1n;var R4c=D9Z;R4c+=F1n;R4c+=R13.U7n;R4c+=t7n;var g4c=R4n;g4c+=e6y;g4c+=a4n;var args=Array[g4c][R4c][W4c](arguments);args[E6y]();fn[m4c](that,args);});});var dom=this[L4c];var wrapper=dom[D8Z];dom[e4c]=_editor_el(X6y,dom[l9J])[B9n];dom[E4c]=_editor_el(D6y,wrapper)[B9n];dom[T2Z]=_editor_el(X4c,wrapper)[B9n];dom[l6y]=_editor_el(x6y,wrapper)[B9n];dom[D4c]=_editor_el(b6y,wrapper)[B9n];if(init[l4c]){this[z8J](init[F1J]);}$(document)[x4c](b4c+this[q1n][w6y],function(e,settings,json){var G4c=r0n;G4c+=t7n;G4c+=R13.q7n;if(that[q1n][f5Z]&&settings[G6y]===$(that[q1n][f5Z])[G4c](B9n)){settings[E3J]=that;}})[g2n](q4c+this[q1n][a4c],function(e,settings,json){var c6y="onsUpda";var q6y="tab";var r4c=r0n;r4c+=t7n;r4c+=R13.q7n;var n4c=m8J;n4c+=M5n;n4c+=l3n;var c4c=q6y;c4c+=h1n;c4c+=t7n;if(json&&that[q1n][c4c]&&settings[G6y]===$(that[q1n][n4c])[r4c](B9n)){var U4c=C5n;U4c+=a6y;U4c+=c6y;U4c+=d5n;that[U4c](json);}});try{this[q1n][k0Z]=Editor[Q2Z][init[Q2Z]][J0Z](this);}catch(e){var n6y='Cannot find display controller ';throw n6y+init[Q2Z];}this[N4c](u4c,[]);};Editor[G9Z][F3J]=function(){var U6y="removeClas";var r6y="joi";var o4c=t7n;o4c+=m5n;o4c+=F1n;o4c+=R13.q7n;var P4c=r6y;P4c+=y7n;var J4c=U6y;J4c+=q1n;var k4c=H7J;k4c+=F1n;k4c+=g2n;k4c+=q1n;var K4c=i9Z;K4c+=q1n;var classesActions=this[K4c][k4c];var action=this[q1n][P6J];var wrapper=$(this[I9Z][D8Z]);wrapper[J4c]([classesActions[o7J],classesActions[L8Z],classesActions[t4J]][P4c](l8Z));if(action===o7J){var B4c=R13.U7n;B4c+=p7n;B4c+=l1n;wrapper[O7Z](classesActions[B4c]);}else if(action===o4c){var S4c=t7n;S4c+=m5n;S4c+=B7n;var p4c=A3n;p4c+=T7J;wrapper[p4c](classesActions[S4c]);}else if(action===t4J){var t4c=p7n;t4c+=f7J;t4c+=X1n;var i4c=z8J;i4c+=N6y;i4c+=h1n;i4c+=y9Z;wrapper[i4c](classesActions[t4c]);}};Editor[H4c][y4c]=function(data,success,error,submitParams){var e8y='?';var m8y="indexO";var W8y="deleteBody";var R8y='DELETE';var v8y="complete";var V8y="ompl";var h8y="plet";var C8y="pli";var d8y=/_id_/;var I8y="ajaxUrl";var Y8y="Of";var s8y=',';var P6y="ajaxUr";var k6y="idS";var u6y="epl";var a3c=a1n;a3c+=z3n;a3c+=F3n;var D3c=W8J;D3c+=R13.q7n;D3c+=a1n;var L3c=m5n;L3c+=a1n;L3c+=R13.q7n;L3c+=a1n;var m3c=p7n;m3c+=u6y;m3c+=X1Z;m3c+=t7n;var W3c=a8J;W3c+=h1n;var T3c=l5n;T3c+=r1n;T3c+=K6y;T3c+=c9Z;var I3c=k6y;I3c+=J6y;var Y3c=t7n;Y3c+=G1n;Y3c+=R13.q7n;var s3c=P6y;s3c+=h1n;var j3c=z3n;j3c+=q1n;j3c+=o7n;j3c+=y7n;var that=this;var action=this[q1n][P6J];var thrown;var opts={type:O1J,dataType:j3c,data:F9Z,error:[function(xhr,text,err){thrown=err;}],success:[],complete:[function(xhr,text){var A8y="tus";var w8y="sta";var M8y="responseText";var F8y="parseJSON";var z8y="ON";var j8y="seJS";var y6y="respon";var H6y="SO";var t6y="eJ";var i6y="respons";var S6y='null';var p6y="onseTe";var o6y="resp";var B6y="lainObject";var m7n=204;var w3c=s1Z;w3c+=B6y;var z3c=o6y;z3c+=p6y;z3c+=e7J;var json=F9Z;if(xhr[j2J]===m7n||xhr[z3c]===S6y){json={};}else{try{var M3c=i6y;M3c+=t6y;M3c+=H6y;M3c+=B5n;var F3c=y6y;F3c+=j8y;F3c+=z8y;json=xhr[F3c]?xhr[M3c]:$[F8y](xhr[M8y]);}catch(e){}}if($[w3c](json)||$[M8J](json)){var A3c=w8y;A3c+=A8y;success(json,xhr[A3c]>=L7n,xhr);}else{error(xhr,text,thrown);}}]};var a;var ajaxSrc=this[q1n][x8J]||this[q1n][s3c];var id=action===Y3c||action===n3J?_pluck(this[q1n][M1J],I3c):F9Z;if($[M8J](id)){id=id[B4J](s8y);}if($[P8J](ajaxSrc)&&ajaxSrc[action]){ajaxSrc=ajaxSrc[action];}if(typeof ajaxSrc===T3c){var d3c=a1n;d3c+=z3n;d3c+=t2J;var uri=F9Z;var method=F9Z;if(this[q1n][d3c]){var C3c=F1n;C3c+=R7J;C3c+=Y8y;var Z3c=s6Z;Z3c+=Y6Z;var url=this[q1n][I8y];if(url[Z3c]){uri=url[action];}if(uri[C3c](l8Z)!==-o9n){a=uri[T8y](l8Z);method=a[B9n];uri=a[o9n];}uri=uri[b1Z](d8y,id);}ajaxSrc(method,uri,data,success,error);return;}else if(typeof ajaxSrc===r7J){if(ajaxSrc[Z8y](l8Z)!==-o9n){var f3c=a8J;f3c+=h1n;var Q3c=q1n;Q3c+=C8y;Q3c+=R13.q7n;a=ajaxSrc[Q3c](l8Z);opts[g4n]=a[B9n];opts[f3c]=a[o9n];}else{opts[K1J]=ajaxSrc;}}else{var R3c=G5n;R3c+=d5n;R3c+=F0n;var v3c=t7n;v3c+=Q8y;v3c+=o7n;v3c+=p7n;var O3c=R13.U7n;O3c+=o7n;O3c+=f8y;O3c+=O8y;var optsCopy=$[C8Z]({},ajaxSrc||{});if(optsCopy[O3c]){var V3c=R13.U7n;V3c+=T7Z;V3c+=h8y;V3c+=t7n;var h3c=R13.U7n;h3c+=V8y;h3c+=G3n;h3c+=t7n;opts[h3c][X8J](optsCopy[V3c]);delete optsCopy[v8y];}if(optsCopy[v3c]){var g3c=s3n;g3c+=p7n;g3c+=i1n;opts[E7Z][X8J](optsCopy[E7Z]);delete optsCopy[g3c];}opts=$[R3c]({},opts,optsCopy);}opts[W3c]=opts[K1J][m3c](d8y,id);if(opts[L3c]){var X3c=W8J;X3c+=m8J;var E3c=R13.r7n;E3c+=K2n;E3c+=g8y;E3c+=y7n;var e3c=W8J;e3c+=R13.q7n;e3c+=a1n;var isFn=typeof opts[e3c]===E3c;var newData=isFn?opts[X3c](data):opts[R8Z];data=isFn&&newData?newData:$[C8Z](B2n,data,newData);}opts[D3c]=data;if(opts[g4n]===R8y&&(opts[W8y]===undefined||opts[W8y]===B2n)){var q3c=m5n;q3c+=W0Z;q3c+=a1n;var G3c=m8y;G3c+=l5n;var b3c=r1n;b3c+=p7n;b3c+=h1n;var x3c=m5n;x3c+=v8Z;var l3c=L8y;l3c+=a1n;l3c+=n1n;var params=$[l3c](opts[x3c]);opts[K1J]+=opts[b3c][G3c](e8y)===-o9n?e8y+params:a1Z+params;delete opts[q3c];}$[a3c](opts);};Editor[c3c][h3Z]=function(target,style,time,callback){var x8y="nctio";var l8y="stop";var X8y="ima";var r3c=E8y;r3c+=X8y;r3c+=R13.q7n;r3c+=t7n;var n3c=l5n;n3c+=y7n;if($[n3c][r3c]){var U3c=Q0Z;U3c+=D8y;target[l8y]()[U3c](style,time,callback);}else{var u3c=l5n;u3c+=r1n;u3c+=x8y;u3c+=y7n;var N3c=R13.U7n;N3c+=q1n;N3c+=q1n;target[N3c](style);if(typeof time===u3c){time[l9Z](target);}else if(callback){var K3c=R13.U7n;K3c+=a1n;K3c+=h1n;K3c+=h1n;callback[K3c](target);}}};Editor[k3c][T1J]=function(){var b8y="wrapp";var S3c=M3Z;S3c+=C1J;var p3c=l5n;p3c+=o7n;p3c+=L2J;p3c+=L5Z;var o3c=l5n;o3c+=i1n;o3c+=w9J;o3c+=Y3n;var B3c=l5n;B3c+=o7n;B3c+=p4n;B3c+=s3n;var P3c=b8y;P3c+=t7n;P3c+=p7n;var J3c=m5n;J3c+=T7Z;var dom=this[J3c];$(dom[P3c])[x9J](dom[J6J]);$(dom[B3c])[h4Z](dom[o3c])[h4Z](dom[q9J]);$(dom[l6y])[h4Z](dom[p3c])[S3c](dom[l9J]);};Editor[G9Z][n8J]=function(){var a8y="onBl";var q8y="reBl";var z0c=R13.U7n;z0c+=h1n;z0c+=o7n;z0c+=Y0n;var y3c=q1n;y3c+=G8y;y3c+=F1n;y3c+=R13.q7n;var H3c=a5n;H3c+=q8y;H3c+=a8J;var t3c=G4J;t3c+=R13.q7n;var i3c=a8y;i3c+=a8J;var opts=this[q1n][k4J];var onBlur=opts[i3c];if(this[t3c](H3c)===J2n){return;}if(typeof onBlur===e9Z){onBlur(this);}else if(onBlur===y3c){var j0c=q1n;j0c+=r1n;j0c+=M5n;j0c+=E4n;this[j0c]();}else if(onBlur===z0c){var F0c=C5n;F0c+=G6J;F0c+=t7n;this[F0c]();}};Editor[M0c][c8y]=function(){var r8y="rapper";var n8y="ag";var T0c=Z1n;T0c+=u9Z;T0c+=n8y;T0c+=t7n;var Y0c=t7n;Y0c+=a1n;Y0c+=L5n;var s0c=t9Z;s0c+=H9Z;s0c+=D3J;s0c+=q1n;var A0c=V3n;A0c+=r8y;var w0c=s3n;w0c+=p7n;w0c+=o7n;w0c+=p7n;if(!this[q1n]){return;}var errorClass=this[J9Z][h8J][w0c];var fields=this[q1n][F1J];$(w4J+errorClass,this[I9Z][A0c])[s0c](errorClass);$[Y0c](fields,function(name,field){var I0c=n1n;I0c+=M6Z;field[E7Z](C2n)[I0c](C2n);});this[E7Z](C2n)[T0c](C2n);};Editor[G9Z][t4n]=function(submitComplete){var o8y="cb";var B8y="seI";var P8y="clo";var k8y="Cb";var K8y="closeCb";var U8y="eC";var O0c=R13.U7n;O0c+=A6J;var f0c=o7n;f0c+=l5n;f0c+=l5n;var Z0c=R4n;Z0c+=U8y;Z0c+=N8y;Z0c+=t7n;var d0c=u8y;d0c+=P4n;d0c+=S0Z;if(this[d0c](Z0c)===J2n){return;}if(this[q1n][K8y]){var C0c=s0n;C0c+=V1n;C0c+=k8y;this[q1n][C0c](submitComplete);this[q1n][K8y]=F9Z;}if(this[q1n][J8y]){var Q0c=P8y;Q0c+=B8y;Q0c+=o8y;this[q1n][J8y]();this[q1n][Q0c]=F9Z;}$(p9Z)[f0c](p8y);this[q1n][z5J]=J2n;this[w3J](O0c);};Editor[G9Z][h0c]=function(fn){var S8y="seCb";var V0c=s0n;V0c+=o7n;V0c+=S8y;this[q1n][V0c]=fn;};Editor[G9Z][i8y]=function(arg1,arg2,arg3,arg4){var j9y="tons";var y8y="but";var H8y="oolean";var t8y="Option";var m0c=n1n;m0c+=a1n;m0c+=F1n;m0c+=y7n;var W0c=B1n;W0c+=L2J;W0c+=t8y;W0c+=q1n;var R0c=t7n;R0c+=H7n;R0c+=d5n;R0c+=F0n;var v0c=M5n;v0c+=H8y;var that=this;var title;var buttons;var show;var opts;if($[P8J](arg1)){opts=arg1;}else if(typeof arg1===v0c){show=arg1;opts=arg2;}else{title=arg1;buttons=arg2;show=arg3;opts=arg4;}if(show===undefined){show=B2n;}if(title){that[J8Z](title);}if(buttons){var g0c=y8y;g0c+=j9y;that[g0c](buttons);}return{opts:$[R0c]({},this[q1n][W0c][m0c],opts),maybeOpen:function(){var z9y="open";if(show){that[z9y]();}}};};Editor[L0c][e0c]=function(name){var F9y="ift";var D0c=q1n;D0c+=P9Z;D0c+=F9y;var X0c=q1n;X0c+=h1n;X0c+=F1n;X0c+=k0n;var E0c=a5n;E0c+=p7n;E0c+=o7n;E0c+=y4n;var args=Array[E0c][X0c][l9Z](arguments);args[D0c]();var fn=this[q1n][M9y][name];if(fn){return fn[b9Z](this,args);}};Editor[G9Z][l8J]=function(includeFields){var v9y='displayOrder';var V9y="dTo";var I9y="deFi";var Y9y="nclu";var s9y="formContent";var A9y="ildr";var w9y="etac";var o0c=n1n;o0c+=a1n;o0c+=F1n;o0c+=y7n;var c0c=L1J;c0c+=L5n;var a0c=m5n;a0c+=w9y;a0c+=P9Z;var q0c=R13.U7n;q0c+=P9Z;q0c+=A9y;q0c+=J7Z;var b0c=n1n;b0c+=o7n;b0c+=m5n;b0c+=t7n;var x0c=l5n;x0c+=e4J;x0c+=q1n;var l0c=m5n;l0c+=T7Z;var that=this;var formContent=$(this[l0c][s9y]);var fields=this[q1n][x0c];var order=this[q1n][E8J];var template=this[q1n][o1J];var mode=this[q1n][b0c]||K4J;if(includeFields){this[q1n][k9J]=includeFields;}else{var G0c=F1n;G0c+=Y9y;G0c+=I9y;G0c+=w1J;includeFields=this[q1n][G0c];}formContent[q0c]()[a0c]();$[c0c](order,function(i,fieldOrName){var O9y='"]';var f9y='editor-field[name="';var Q9y="mplate=\"";var C9y="r-te";var Z9y="[data-edito";var d9y="ray";var T9y="nAr";var r0c=h4n;r0c+=T9y;r0c+=d9y;var n0c=W0n;n0c+=F1n;n0c+=t7n;n0c+=E0n;var name=fieldOrName instanceof Editor[n0c]?fieldOrName[Q8Z]():fieldOrName;if(that[r0c](name,includeFields)!==-o9n){if(template&&mode===K4J){var J0c=y7n;J0c+=o7n;J0c+=K4n;var k0c=k7Z;k0c+=a5n;k0c+=t7n;k0c+=F0n;var K0c=Z9y;K0c+=C9y;K0c+=Q9y;var u0c=l5n;u0c+=F1n;u0c+=y7n;u0c+=m5n;var N0c=a1n;N0c+=M7J;N0c+=t7n;N0c+=p7n;var U0c=e2n;U0c+=E2n;template[F4J](f9y+name+U0c)[N0c](fields[name][S6J]());template[u0c](K0c+name+O9y)[k0c](fields[name][J0c]());}else{var B0c=Z1J;B0c+=m5n;B0c+=t7n;var P0c=a1n;P0c+=B3Z;P0c+=C1J;formContent[P0c](fields[name][B0c]());}}});if(template&&mode===o0c){var p0c=h9y;p0c+=V9y;template[p0c](formContent);}this[w3J](v9y,[this[q1n][z5J],this[q1n][P6J],formContent]);};Editor[G9Z][o8J]=function(items,editFields,type,formOptions,setupDone){var a9y='initEdit';var q9y="nA";var G9y="toSt";var R9y="editF";var g9y="_actionC";var T2c=l3n;T2c+=y7n;T2c+=B9J;var y0c=t7n;y0c+=a1n;y0c+=R13.U7n;y0c+=P9Z;var H0c=g9y;H0c+=h1n;H0c+=a1n;H0c+=u9Z;var t0c=G1n;t0c+=q1n;t0c+=E1Z;t0c+=S3n;var i0c=R9y;i0c+=I3Z;i0c+=E0n;i0c+=q1n;var S0c=h2J;S0c+=h1n;S0c+=m5n;S0c+=q1n;var that=this;var fields=this[q1n][S0c];var usedFields=[];var includeInOrder;var editData={};this[q1n][i0c]=editFields;this[q1n][W9y]=editData;this[q1n][p6J]=items;this[q1n][P6J]=L8Z;this[I9Z][l9J][C2Z][t0c]=f2Z;this[q1n][b5n]=type;this[H0c]();$[y0c](fields,function(name,field){var b9y="ush";var x9y="multiIds";var m9y="multiR";var j2c=m9y;j2c+=t7n;j2c+=Y0n;j2c+=R13.q7n;field[j2c]();includeInOrder=J2n;editData[name]={};$[D2n](editFields,function(idSrc,edit){var D9y="ayFi";var X9y="iS";var E9y="displayFiel";if(edit[F1J][name]){var M2c=p7n;M2c+=o6J;var F2c=L9y;F2c+=c4J;var z2c=F1n;z2c+=q1n;z2c+=U5n;z2c+=E4J;var val=field[m8Z](edit[R8Z]);editData[name][idSrc]=val===F9Z?C2n:$[z2c](val)?val[u5Z]():val;if(!formOptions||formOptions[F2c]===M2c){var s2c=I4Z;s2c+=w0n;s2c+=e9y;var A2c=E9y;A2c+=e7Z;var w2c=N1n;w2c+=S2n;w2c+=X9y;w2c+=G3n;field[w2c](idSrc,val!==undefined?val:field[g0n]());if(!edit[A2c]||edit[s2c][name]){includeInOrder=B2n;}}else{var Y2c=U7Z;Y2c+=D9y;Y2c+=w1J;if(!edit[l9y]||edit[Y2c][name]){field[Y1J](idSrc,val!==undefined?val:field[g0n]());includeInOrder=B2n;}}}});if(field[x9y]()[k2n]!==B9n&&includeInOrder){var I2c=a5n;I2c+=b9y;usedFields[I2c](name);}});var currOrder=this[E8J]()[u5Z]();for(var i=currOrder[T2c]-o9n;i>=B9n;i--){var Z2c=G9y;Z2c+=E7J;var d2c=F1n;d2c+=q9y;d2c+=E4J;if($[d2c](currOrder[i][Z2c](),usedFields)===-o9n){currOrder[Y2J](i,o9n);}}this[l8J](currOrder);this[w3J](a9y,[_pluck(editFields,M3J)[B9n],_pluck(editFields,r1J)[B9n],items,type],function(){var n9y="ultiEdit";var c9y="initM";var C2c=c9y;C2c+=n9y;that[w3J](C2c,[editFields,items,type],function(){setupDone();});});};Editor[Q2c][w3J]=function(trigger,args,promiseComplete){var S9y="resu";var p9y="result";var o9y="bjec";var B9y='Cancelled';var J9y="Ev";var k9y="Event";var K9y="Handler";var u9y="igge";var U9y="sul";var f2c=F1n;f2c+=q1n;f2c+=U5n;f2c+=E4J;if(!args){args=[];}if($[f2c](trigger)){var O2c=K1Z;O2c+=r9y;O2c+=P9Z;for(var i=B9n,ien=trigger[O2c];i<ien;i++){this[w3J](trigger[i],args);}}else{var g2c=A2n;g2c+=U9y;g2c+=R13.q7n;var v2c=a5n;v2c+=A2n;var V2c=y6Z;V2c+=K4n;V2c+=N9y;V2c+=l5n;var h2c=O0n;h2c+=u9y;h2c+=p7n;h2c+=K9y;var e=$[k9y](trigger);$(this)[h2c](e,args);if(trigger[V2c](v2c)===B9n&&e[g2c]===J2n){var R2c=J9y;R2c+=S0Z;$(this)[P9y]($[R2c](trigger+B9y),args);}if(promiseComplete){var L2c=R13.q7n;L2c+=P9Z;L2c+=t7n;L2c+=y7n;var m2c=A2n;m2c+=q1n;m2c+=c7J;var W2c=o7n;W2c+=o9y;W2c+=R13.q7n;if(e[p9y]&&typeof e[p9y]===W2c&&e[m2c][L2c]){var e2c=S9y;e2c+=S2n;e[e2c][N1J](promiseComplete);}else{promiseComplete();}}return e[p9y];}};Editor[E2c][X2c]=function(input){var y9y="toLowerCase";var t9y="bstr";var i9y=/^on([A-Z])/;var x2c=z3n;x2c+=A9J;var name;var names=input[T8y](l8Z);for(var i=B9n,ien=names[k2n];i<ien;i++){var D2c=n1n;D2c+=a1n;D2c+=R13.q7n;D2c+=L5n;name=names[i];var onStyle=name[D2c](i9y);if(onStyle){var l2c=q1n;l2c+=r1n;l2c+=t9y;l2c+=H9y;name=onStyle[o9n][y9y]()+name[l2c](S9n);}names[i]=name;}return names[x2c](l8Z);};Editor[b2c][G2c]=function(node){var q2c=t7n;q2c+=X1Z;q2c+=P9Z;var foundField=F9Z;$[q2c](this[q1n][F1J],function(name,field){var j7y="nod";var c2c=l5n;c2c+=F1n;c2c+=y7n;c2c+=m5n;var a2c=j7y;a2c+=t7n;if($(field[a2c]())[c2c](node)[k2n]){foundField=field;}});return foundField;};Editor[n2c][B7J]=function(fieldNames){if(fieldNames===undefined){var r2c=z8Z;r2c+=m0n;r2c+=m5n;r2c+=q1n;return this[r2c]();}else if(!$[M8J](fieldNames)){return[fieldNames];}return fieldNames;};Editor[G9Z][z7y]=function(fieldsIn,focus){var s7y=/^jq:/;var A7y="v.DTE ";var w7y='jq:';var M7y='number';var F7y="etFocus";var u2c=q1n;u2c+=F7y;var that=this;var field;var fields=$[F5J](fieldsIn,function(fieldOrName){var U2c=l5n;U2c+=e4J;U2c+=q1n;return typeof fieldOrName===r7J?that[q1n][U2c][fieldOrName]:fieldOrName;});if(typeof focus===M7y){field=fields[focus];}else if(focus){if(focus[Z8y](w7y)===B9n){var N2c=m5n;N2c+=F1n;N2c+=A7y;field=$(N2c+focus[b1Z](s7y,C2n));}else{field=this[q1n][F1J][focus];}}this[q1n][u2c]=field;if(field){field[R9Z]();}};Editor[G9Z][h9J]=function(opts){var P7y='keyup';var u7y="_fieldFromNode";var c7y="itl";var a7y="onBackgroun";var q7y="rou";var G7y="blurOnBackg";var b7y="etu";var x7y="nR";var l7y="urn";var D7y="submitOnRet";var X7y="submitOnReturn";var E7y="onBlur";var e7y="nBlu";var L7y="submitO";var m7y="submitOnBlur";var W7y="closeOnComplete";var g7y="dteInline";var v7y="mple";var V7y="closeOnCo";var h7y="ckgrou";var O7y="nBa";var f7y="blurO";var Q7y="Count";var C7y="unct";var Z7y="lean";var d7y="boo";var T7y="down";var Y7y="eIc";var n6h=R13.U7n;n6h+=N8y;n6h+=Y7y;n6h+=M5n;var Q6h=o7n;Q6h+=y7n;var T6h=I7y;T6h+=V5n;T6h+=T7y;var I6h=o7n;I6h+=y7n;var s6h=d7y;s6h+=Z7y;var A6h=y8J;A6h+=X5n;var w6h=Z1n;w6h+=u3n;var z6h=l5n;z6h+=C7y;z6h+=F1n;z6h+=g2n;var j6h=R13.q7n;j6h+=B7n;j6h+=h1n;j6h+=t7n;var y2c=t7n;y2c+=G1n;y2c+=R13.q7n;y2c+=Q7y;var p2c=f7y;p2c+=O7y;p2c+=h7y;p2c+=F0n;var k2c=V7y;k2c+=v7y;k2c+=d5n;var K2c=h2Z;K2c+=g7y;var that=this;var inlineCount=__inlineCounter++;var namespace=K2c+inlineCount;if(opts[k2c]!==undefined){opts[R7y]=opts[W7y]?p5Z:K7Z;}if(opts[m7y]!==undefined){var J2c=L7y;J2c+=e7y;J2c+=p7n;opts[E7y]=opts[J2c]?c8J:p5Z;}if(opts[X7y]!==undefined){var o2c=y7n;o2c+=o7n;o2c+=y7n;o2c+=t7n;var B2c=D7y;B2c+=l7y;var P2c=o7n;P2c+=x7y;P2c+=b7y;P2c+=D5Z;opts[P2c]=opts[B2c]?c8J:o2c;}if(opts[p2c]!==undefined){var H2c=Z1J;H2c+=m1n;var t2c=M5n;t2c+=h1n;t2c+=r1n;t2c+=p7n;var i2c=G7y;i2c+=q7y;i2c+=F0n;var S2c=a7y;S2c+=m5n;opts[S2c]=opts[i2c]?t2c:H2c;}this[q1n][k4J]=opts;this[q1n][y2c]=inlineCount;if(typeof opts[J8Z]===r7J||typeof opts[j6h]===z6h){var M6h=R13.q7n;M6h+=c7y;M6h+=t7n;var F6h=R13.q7n;F6h+=B7n;F6h+=h1n;F6h+=t7n;this[F6h](opts[J8Z]);opts[M6h]=B2n;}if(typeof opts[t8Z]===r7J||typeof opts[w6h]===e9Z){this[t8Z](opts[t8Z]);opts[t8Z]=B2n;}if(typeof opts[A6h]!==s6h){var Y6h=B5Z;Y6h+=q1n;this[q9J](opts[Y6h]);opts[q9J]=B2n;}$(document)[I6h](T6h+namespace,function(e){var J7y="De";var k7y="vent";var N7y="rnSubmit";var U7y="canRe";var r7y="activeElement";var n7y="keyCod";var d6h=n7y;d6h+=t7n;if(e[d6h]===F7n&&that[q1n][z5J]){var el=$(document[r7y]);if(el){var Z6h=U7y;Z6h+=X5Z;Z6h+=N7y;var field=that[u7y](el);if(field[Z6h](el)){var C6h=K7y;C6h+=k7y;C6h+=J7y;C6h+=A8Z;e[C6h]();}}}});$(document)[Q6h](P7y+namespace,function(e){var h1y="blur";var O1y="nE";var f1y="onEsc";var Q1y="entDefault";var Z1y="functio";var d1y="Es";var T1y="onRe";var I1y="onReturn";var Y1y="ubmi";var s1y="etur";var A1y="onR";var M1y="urnSu";var F1y="Ret";var j1y="func";var y7y="ubmit";var H7y="canReturnS";var t7y="ment";var i7y="ctiveEle";var p7y="m_Buttons";var o7y="E_For";var B7y=".DT";var O7n=39;var f7n=37;var b6h=B7y;b6h+=o7y;b6h+=p7y;var x6h=S7y;x6h+=q1n;var f6h=a1n;f6h+=i7y;f6h+=t7y;var el=$(document[f6h]);if(e[l7J]===F7n&&that[q1n][z5J]){var V6h=H7y;V6h+=y7y;var h6h=j1y;h6h+=R13.q7n;h6h+=c9Z;var O6h=z1y;O6h+=F1y;O6h+=M1y;O6h+=I0n;var field=that[u7y](el);if(field&&typeof field[O6h]===h6h&&field[V6h](el)){var W6h=q9Z;W6h+=K6y;W6h+=g8y;W6h+=y7n;var g6h=w1y;g6h+=E4n;var v6h=A1y;v6h+=s1y;v6h+=y7n;if(opts[v6h]===g6h){var R6h=q1n;R6h+=Y1y;R6h+=R13.q7n;e[G7J]();that[R6h]();}else if(typeof opts[I1y]===W6h){var m6h=T1y;m6h+=X5Z;m6h+=D5Z;e[G7J]();opts[m6h](that,e);}}}else if(e[l7J]===T7n){var l6h=g2n;l6h+=d1y;l6h+=R13.U7n;var D6h=M5n;D6h+=h1n;D6h+=a8J;var X6h=g2n;X6h+=e1n;X6h+=q1n;X6h+=R13.U7n;var e6h=Z1y;e6h+=y7n;var L6h=C1y;L6h+=Q1y;e[L6h]();if(typeof opts[f1y]===e6h){var E6h=o7n;E6h+=O1y;E6h+=L9y;opts[E6h](that,e);}else if(opts[X6h]===D6h){that[h1y]();}else if(opts[f1y]===p5Z){that[V4Z]();}else if(opts[l6h]===c8J){that[I5Z]();}}else if(el[x6h](b6h)[k2n]){if(e[l7J]===f7n){var q6h=y8J;q6h+=g2n;var G6h=R4n;G6h+=t7n;G6h+=P4n;el[G6h](q6h)[R9Z]();}else if(e[l7J]===O7n){var c6h=L7J;c6h+=y7n;var a6h=y7n;a6h+=o5n;el[a6h](c6h)[R9Z]();}}});this[q1n][n6h]=function(){var V1y="ey";var U6h=G0n;U6h+=V1y;U6h+=r1n;U6h+=a5n;var r6h=f0Z;r6h+=l5n;$(document)[r6h](v1y+namespace);$(document)[a2Z](U6h+namespace);};return namespace;};Editor[N6h][u6h]=function(direction,action,data){var R1y="acyAj";var g1y="eg";var K6h=h1n;K6h+=g1y;K6h+=R1y;K6h+=F3n;if(!this[q1n][K6h]||!data){return;}if(direction===W1y){var k6h=t7n;k6h+=m5n;k6h+=F1n;k6h+=R13.q7n;if(action===q3J||action===k6h){var P6h=m5n;P6h+=v8Z;var J6h=m5n;J6h+=a1n;J6h+=R13.q7n;J6h+=a1n;var id;$[D2n](data[J6h],function(rowId,values){var m1y='Editor: Multi-row editing is not supported by the legacy Ajax data format';if(id!==undefined){throw m1y;}id=rowId;});data[P6h]=data[R8Z][id];if(action===a3J){var B6h=F1n;B6h+=m5n;data[B6h]=id;}}else{var S6h=m5n;S6h+=a1n;S6h+=R13.q7n;S6h+=a1n;var p6h=n1n;p6h+=a1n;p6h+=a5n;var o6h=F1n;o6h+=m5n;data[o6h]=$[p6h](data[R8Z],function(values,id){return id;});delete data[S6h];}}else{var y6h=m5n;y6h+=W0Z;y6h+=a1n;var t6h=p7n;t6h+=o7n;t6h+=V3n;var i6h=W8J;i6h+=m8J;if(!data[i6h]&&data[t6h]){var H6h=W8J;H6h+=R13.q7n;H6h+=a1n;data[H6h]=[data[j8J]];}else if(!data[y6h]){data[R8Z]=[];}}};Editor[G9Z][L1y]=function(json){var j8h=x5Z;j8h+=R13.q7n;j8h+=d0n;var that=this;if(json[j8h]){var F8h=l5n;F8h+=F1n;F8h+=t7n;F8h+=s8J;var z8h=L1J;z8h+=R13.U7n;z8h+=P9Z;$[z8h](this[q1n][F8h],function(name,field){var e1y="update";var M8h=r5Z;M8h+=d0n;if(json[M8h][name]!==undefined){var fieldInst=that[h8J](name);if(fieldInst&&fieldInst[e1y]){fieldInst[e1y](json[E1y][name]);}}});}};Editor[G9Z][X1y]=function(el,msg){var G1y="I";var b1y="fad";var x1y="adeO";var D1y="nim";var A8h=R13.r7n;A8h+=R13.U7n;A8h+=R13.q7n;A8h+=c9Z;var w8h=a1n;w8h+=D1y;w8h+=Y6Z;var canAnimate=$[f2n][w8h]?B2n:J2n;if(typeof msg===A8h){var s8h=U5n;s8h+=l1y;msg=msg(this,new DataTable[s8h](this[q1n][f5Z]));}el=$(el);if(canAnimate){var Y8h=q1n;Y8h+=R13.q7n;Y8h+=o7n;Y8h+=a5n;el[Y8h]();}if(!msg){if(this[q1n][z5J]&&canAnimate){var I8h=l5n;I8h+=x1y;I8h+=r1n;I8h+=R13.q7n;el[I8h](function(){el[o7Z](C2n);});}else{var d8h=y7n;d8h+=o7n;d8h+=m1n;var T8h=P9Z;T8h+=R13.q7n;T8h+=n1n;T8h+=h1n;el[T8h](C2n)[s9Z](Y9Z,d8h);}}else{if(this[q1n][z5J]&&canAnimate){var Z8h=b1y;Z8h+=t7n;Z8h+=G1y;Z8h+=y7n;el[o7Z](msg)[Z8h]();}else{var f8h=M5n;f8h+=L2Z;f8h+=g1Z;var Q8h=R13.U7n;Q8h+=q1n;Q8h+=q1n;var C8h=P9Z;C8h+=R13.q7n;C8h+=n1n;C8h+=h1n;el[C8h](msg)[Q8h](Y9Z,f8h);}}};Editor[O8h][h8h]=function(){var r1y="multiInfoShown";var n1y="ita";var c1y="multiEd";var a1y="tiVal";var q1y="isMul";var fields=this[q1n][F1J];var include=this[q1n][k9J];var show=B2n;var state;if(!include){return;}for(var i=B9n,ien=include[k2n];i<ien;i++){var g8h=q1y;g8h+=a1y;g8h+=l5Z;var v8h=t7Z;v8h+=h1n;v8h+=M1Z;var V8h=c1y;V8h+=n1y;V8h+=M5n;V8h+=l3n;var field=fields[include[i]];var multiEditable=field[V8h]();if(field[v8h]()&&multiEditable&&show){state=B2n;show=J2n;}else if(field[g8h]()&&!multiEditable){state=B2n;}else{state=J2n;}fields[include[i]][r1y](state);}};Editor[G9Z][V4J]=function(type){var k1y='submit.editor-internal';var K1y="eF";var u1y="captur";var U1y="cti";var c8h=a1n;c8h+=U1y;c8h+=g2n;var a8h=o7n;a8h+=a5n;a8h+=t7n;a8h+=y7n;var q8h=N1y;q8h+=J7Z;q8h+=R13.q7n;var L8h=o7n;L8h+=y7n;var m8h=o7n;m8h+=l5n;m8h+=l5n;var W8h=l5n;W8h+=o7n;W8h+=L2J;var R8h=u1y;R8h+=K1y;R8h+=a7Z;var that=this;var focusCapture=this[q1n][k0Z][R8h];if(focusCapture===undefined){focusCapture=B2n;}$(this[I9Z][W8h])[m8h](k1y)[L8h](k1y,function(e){e[G7J]();});if(focusCapture&&(type===K4J||type===p8J)){$(p9Z)[g2n](p8y,function(){var H1y="setFocus";var t1y="cus";var i1y="setFo";var p1y="ement";var o1y="veEl";var B1y="acti";var P1y="iveElement";var J1y="TED";var x8h=h1n;x8h+=t7n;x8h+=D0J;x8h+=P9Z;var l8h=h2Z;l8h+=R1n;l8h+=J1y;var D8h=B9Z;D8h+=R13.q7n;D8h+=q1n;var X8h=a1n;X8h+=K2n;X8h+=P1y;var E8h=h2Z;E8h+=R1n;E8h+=M8Z;E8h+=e1n;var e8h=B1y;e8h+=o1y;e8h+=p1y;if($(document[e8h])[S1y](E8h)[k2n]===B9n&&$(document[X8h])[D8h](l8h)[x8h]===B9n){var b8h=i1y;b8h+=t1y;if(that[q1n][b8h]){var G8h=l5n;G8h+=o7n;G8h+=J5J;G8h+=q1n;that[q1n][H1y][G8h]();}}});}this[n5Z]();this[q8h](a8h,[type,this[q1n][c8h]]);return B2n;};Editor[G9Z][n8h]=function(type){var s5y="eIcb";var w5y="rDynamicInf";var M5y="lea";var F5y="lOpen";var z5y="anc";var y1y="even";var U8h=H7J;U8h+=F1n;U8h+=o7n;U8h+=y7n;var r8h=C5n;r8h+=y1y;r8h+=R13.q7n;if(this[r8h](j5y,[type,this[q1n][U8h]])===J2n){var k8h=X1Z;k8h+=D3n;k8h+=g2n;var K8h=R13.U7n;K8h+=z5y;K8h+=t7n;K8h+=F5y;var u8h=u8y;u8h+=P4n;u8h+=J7Z;u8h+=R13.q7n;var N8h=v2n;N8h+=M5y;N8h+=w5y;N8h+=o7n;this[N8h]();this[u8h](K8h,[type,this[q1n][k8h]]);if((this[q1n][b5n]===K5J||this[q1n][b5n]===p8J)&&this[q1n][J8y]){var J8h=s0n;J8h+=A5y;J8h+=s5y;this[q1n][J8h]();}this[q1n][J8y]=F9Z;return J2n;}this[q1n][z5J]=type;return B2n;};Editor[G9Z][O3J]=function(processing){var d5y="active";var T5y=".DTE";var I5y="toggleC";var Y5y="proc";var p8h=Y5y;p8h+=i0J;p8h+=H9y;var o8h=N1y;o8h+=t7n;o8h+=Z5n;var B8h=I5y;B8h+=w0n;B8h+=u9Z;var P8h=m5n;P8h+=F1n;P8h+=P4n;P8h+=T5y;var procClass=this[J9Z][j9Z][d5y];$([P8h,this[I9Z][D8Z]])[B8h](procClass,processing);this[q1n][j9Z]=processing;this[o8h](p8h,[processing]);};Editor[S8h][i8h]=function(successCallback,errorCallback,formatdata,hide){var r5y="_process";var n5y="_legacyAjax";var c5y="cal";var a5y="Complete";var G5y="itC";var b5y='changed';var x5y='all';var g5y="allIfCha";var v5y="dbTable";var V5y="editCou";var h5y="difie";var O5y="mov";var f5y="Submit";var Q5y="Url";var C5y="ja";var e9h=Z5y;e9h+=C5y;e9h+=H7n;var L9h=a1n;L9h+=C5y;L9h+=H7n;L9h+=Q5y;var m9h=a1n;m9h+=n4n;var R9h=R4n;R9h+=t7n;R9h+=f5y;var g9h=G5n;g9h+=d5n;g9h+=y7n;g9h+=m5n;var V9h=A2n;V9h+=O5y;V9h+=t7n;var z9h=t7n;z9h+=G1n;z9h+=R13.q7n;var j9h=X1Z;j9h+=W7J;j9h+=y7n;var y8h=C0n;y8h+=h5y;y8h+=p7n;var H8h=V5y;H8h+=y7n;H8h+=R13.q7n;var t8h=t7n;t8h+=H7n;t8h+=R13.q7n;var that=this;var i,iLen,eventRet,errorNodes;var changed=J2n,allData={},changedData={};var setBuilder=DataTable[t8h][W8Z][X8Z];var dataSource=this[q1n][M9y];var fields=this[q1n][F1J];var editCount=this[q1n][H8h];var modifier=this[q1n][y8h];var editFields=this[q1n][M1J];var editData=this[q1n][W9y];var opts=this[q1n][k4J];var changedSubmit=opts[I5Z];var submitParamsLocal;var action=this[q1n][j9h];var submitParams={"action":action,"data":{}};if(this[q1n][v5y]){submitParams[f5Z]=this[q1n][v5y];}if(action===o7J||action===z9h){var Z9h=g5y;Z9h+=e5n;var F9h=L1J;F9h+=R13.U7n;F9h+=P9Z;$[F9h](editFields,function(idSrc,edit){var M9h=t7n;M9h+=a1n;M9h+=R13.U7n;M9h+=P9Z;var allRowData={};var changedRowData={};$[M9h](fields,function(name,field){var D5y='-many-count';var X5y=/\[.*$/;var E5y="nde";var L5y="place";var m5y="pare";var W5y="com";var R5y="submittable";if(edit[F1J][name]&&field[R5y]()){var T9h=W5y;T9h+=m5y;var I9h=t7n;I9h+=m5n;I9h+=F1n;I9h+=R13.q7n;var s9h=A2n;s9h+=L5y;var A9h=e5y;A9h+=E2n;var w9h=F1n;w9h+=E5y;w9h+=N9y;w9h+=l5n;var multiGet=field[m4J]();var builder=setBuilder(name);if(multiGet[idSrc]===undefined){var originalVal=field[m8Z](edit[R8Z]);builder(allRowData,originalVal);return;}var value=multiGet[idSrc];var manyBuilder=$[M8J](value)&&name[w9h](A9h)!==-o9n?setBuilder(name[s9h](X5y,C2n)+D5y):F9Z;builder(allRowData,value);if(manyBuilder){var Y9h=h1n;Y9h+=J7Z;Y9h+=r9y;Y9h+=P9Z;manyBuilder(allRowData,value[Y9h]);}if(action===I9h&&(!editData[name]||!field[T9h](value,editData[name][idSrc]))){builder(changedRowData,value);changed=B2n;if(manyBuilder){var d9h=h1n;d9h+=w8J;d9h+=R13.q7n;d9h+=P9Z;manyBuilder(changedRowData,value[d9h]);}}}});if(!$[l5y](allRowData)){allData[idSrc]=allRowData;}if(!$[l5y](changedRowData)){changedData[idSrc]=changedRowData;}});if(action===q3J||changedSubmit===x5y||changedSubmit===Z9h&&changed){submitParams[R8Z]=allData;}else if(changedSubmit===b5y&&changed){submitParams[R8Z]=changedData;}else{var h9h=w1y;h9h+=n1n;h9h+=G5y;h9h+=q5y;var O9h=S7J;O9h+=y7n;O9h+=R13.q7n;var C9h=X1Z;C9h+=R13.N7n;this[q1n][C9h]=F9Z;if(opts[R7y]===p5Z&&(hide===undefined||hide)){this[t4n](J2n);}else if(typeof opts[R7y]===e9Z){var Q9h=g2n;Q9h+=a5y;opts[Q9h](this);}if(successCallback){var f9h=c5y;f9h+=h1n;successCallback[f9h](this);}this[O3J](J2n);this[O9h](h9h);return;}}else if(action===V9h){var v9h=L1J;v9h+=R13.U7n;v9h+=P9Z;$[v9h](editFields,function(idSrc,edit){submitParams[R8Z][idSrc]=edit[R8Z];});}this[n5y](W1y,action,submitParams);submitParamsLocal=$[g9h](B2n,{},submitParams);if(formatdata){formatdata(submitParams);}if(this[w3J](R9h,[submitParams,action])===J2n){var W9h=r5y;W9h+=H9y;this[W9h](J2n);return;}var submitWire=this[q1n][m9h]||this[q1n][L9h]?this[e9h]:this[U5y];submitWire[l9Z](this,submitParams,function(json,notGood,xhr){var u5y="itSucces";var N5y="_subm";var E9h=N5y;E9h+=u5y;E9h+=q1n;that[E9h](json,notGood,submitParams,submitParamsLocal,that[q1n][P6J],editCount,hide,successCallback,errorCallback,xhr);},function(xhr,err,thrown){var K5y="ubmitEr";var X9h=m4n;X9h+=K5y;X9h+=p7n;X9h+=i1n;that[X9h](xhr,err,thrown,errorCallback,submitParams,that[q1n][P6J]);},submitParams);};Editor[G9Z][U5y]=function(data,success,error,submitParams){var t5y="taSource";var i5y="modi";var S5y="tDataFn";var p5y="nGetObjec";var o5y="_f";var B5y="oA";var P5y="ObjectDataFn";var J5y="_fnS";var a9h=p7n;a9h+=f7J;a9h+=k5y;a9h+=t7n;var q9h=k3n;q9h+=i7n;q9h+=J6y;var G9h=J5y;G9h+=G3n;G9h+=P5y;var b9h=B5y;b9h+=l1y;var x9h=o5y;x9h+=p5y;x9h+=S5y;var l9h=G5n;l9h+=R13.q7n;var D9h=X1Z;D9h+=D3n;D9h+=o7n;D9h+=y7n;var that=this;var action=data[D9h];var out={data:[]};var idGet=DataTable[l9h][W8Z][x9h](this[q1n][F6y]);var idSet=DataTable[o5n][b9h][G9h](this[q1n][q9h]);if(action!==a9h){var u9h=h8Z;u9h+=a1n;var N9h=i5y;N9h+=z8Z;N9h+=s3n;var U9h=l5J;U9h+=N8J;var r9h=b3Z;r9h+=a1n;r9h+=t5y;var n9h=C0n;n9h+=m5n;n9h+=F1n;n9h+=H5y;var c9h=n1n;c9h+=a1n;c9h+=F1n;c9h+=y7n;var originalData=this[q1n][b5n]===c9h?this[B8J](T5J,this[n9h]()):this[r9h](U9h,this[N9h]());$[D2n](data[u9h],function(key,vals){var z4y="TableExt";var j4y="_fnExten";var p9h=a5n;p9h+=r1n;p9h+=q1n;p9h+=P9Z;var o9h=R13.U7n;o9h+=p7n;o9h+=y5y;o9h+=t7n;var P9h=t7n;P9h+=m5n;P9h+=F1n;P9h+=R13.q7n;var J9h=j4y;J9h+=m5n;var k9h=o7n;k9h+=U5n;k9h+=a5n;k9h+=F1n;var K9h=R8Z;K9h+=z4y;var toSave;var extender=$[f2n][K9h][k9h][J9h];if(action===P9h){var B9h=m5n;B9h+=a1n;B9h+=R13.q7n;B9h+=a1n;var rowData=originalData[key][B9h];toSave=extender({},rowData,B2n);toSave=extender(toSave,vals,B2n);}else{toSave=extender({},vals,B2n);}var overrideId=idGet(toSave);if(action===o9h&&overrideId===undefined){idSet(toSave,+new Date()+C2n+key);}else{idSet(toSave,overrideId);}out[R8Z][p9h](toSave);});}success(out);};Editor[G9Z][S9h]=function(json,notGood,submitParams,submitParamsLocal,action,editCount,hide,successCallback,errorCallback,xhr){var o4y='submitComplete';var B4y='submitSuccess';var J4y="nComplete";var k4y="ids";var K4y='preRemove';var u4y="Remov";var N4y="ataSou";var U4y="mmit";var r4y="reEdit";var n4y="Edit";var c4y="po";var a4y="Crea";var q4y="rce";var G4y="Sou";var b4y="_dat";var x4y="postCre";var l4y='id';var E4y="comm";var e4y="itCount";var L4y='<br>';var Q4y="fieldEr";var C4y="ccessfu";var Z4y="submitUnsu";var d4y='receive';var I4y="itO";var Y4y="cyAjax";var s4y="ega";var A4y="_l";var w4y="Su";var M4y="post";var F4y="erro";var M7h=F4y;M7h+=p7n;var z7h=s3n;z7h+=p7n;z7h+=i1n;var j7h=M4y;j7h+=w4y;j7h+=I0n;var y9h=A4y;y9h+=s4y;y9h+=Y4y;var H9h=Z0n;H9h+=F1n;H9h+=H5y;var t9h=t7n;t9h+=m5n;t9h+=I4y;t9h+=T4y;var i9h=z8Z;i9h+=t7n;i9h+=E0n;i9h+=q1n;var that=this;var setData;var fields=this[q1n][i9h];var opts=this[q1n][t9h];var modifier=this[q1n][H9h];this[y9h](d4y,action,json);this[w3J](j7h,[json,submitParams,action,xhr]);if(!json[z7h]){var F7h=F4y;F7h+=p7n;json[F7h]=R13.K7n;}if(!json[y0J]){json[y0J]=[];}if(notGood||json[M7h]||json[y0J][k2n]){var R7h=Z4y;R7h+=C4y;R7h+=h1n;var Y7h=Q4y;Y7h+=k4n;Y7h+=p7n;Y7h+=q1n;var s7h=t7n;s7h+=a1n;s7h+=R13.U7n;s7h+=P9Z;var globalError=[];if(json[E7Z]){var A7h=t7n;A7h+=Q8y;A7h+=i1n;var w7h=a5n;w7h+=q7Z;w7h+=P9Z;globalError[w7h](json[A7h]);}$[s7h](json[Y7h],function(i,err){var m4y=":";var W4y="atus";var R4y="nFieldErr";var g4y="onFieldError";var v4y="yContent";var V4y="osi";var h4y="FieldErr";var O4y="unction";var f4y="Er";var I7h=y7n;I7h+=a1n;I7h+=n1n;I7h+=t7n;var field=fields[err[I7h]];if(field[z5J]()){var T7h=f4y;T7h+=p7n;T7h+=o7n;T7h+=p7n;field[E7Z](err[j2J]||T7h);if(i===B9n){var O7h=l5n;O7h+=O4y;var d7h=g2n;d7h+=h4y;d7h+=o7n;d7h+=p7n;if(opts[d7h]===S5Z){var f7h=R13.q7n;f7h+=o7n;f7h+=a5n;var Q7h=a5n;Q7h+=V4y;Q7h+=R13.N7n;var C7h=v2J;C7h+=v4y;var Z7h=c9J;Z7h+=F1n;Z7h+=D8y;that[Z7h]($(that[I9Z][C7h],that[q1n][D8Z]),{scrollTop:$(field[S6J]())[Q7h]()[f7h]},e7n);field[R9Z]();}else if(typeof opts[g4y]===O7h){var h7h=o7n;h7h+=R4y;h7h+=i1n;opts[h7h](that,err);}}}else{var g7h=e1n;g7h+=Q8y;g7h+=o7n;g7h+=p7n;var v7h=o1Z;v7h+=W4y;var V7h=m4y;V7h+=w2n;globalError[l2n](field[Q8Z]()+V7h+(err[v7h]||g7h));}});this[E7Z](globalError[B4J](L4y));this[w3J](R7h,[json]);if(errorCallback){errorCallback[l9Z](that,json);}}else{var i7h=u8y;i7h+=y4J;i7h+=R13.q7n;var B7h=t7n;B7h+=m5n;B7h+=e4y;var U7h=t9Z;U7h+=X1n;var W7h=m5n;W7h+=a1n;W7h+=R13.q7n;W7h+=a1n;var store={};if(json[W7h]&&(action===o7J||action===L8Z)){var r7h=E4y;r7h+=B7n;var L7h=h8Z;L7h+=a1n;var m7h=a5n;m7h+=p7n;m7h+=t7n;m7h+=a5n;this[B8J](m7h,action,modifier,submitParamsLocal,json,store);for(var i=B9n;i<json[L7h][k2n];i++){var X7h=X4y;X7h+=t7n;X7h+=W0Z;X7h+=t7n;var E7h=H6Z;E7h+=D4y;var e7h=m5n;e7h+=a1n;e7h+=R13.q7n;e7h+=a1n;setData=json[e7h][i];var id=this[B8J](l4y,setData);this[w3J](E7h,[json,setData,action]);if(action===X7h){var G7h=x4y;G7h+=a1n;G7h+=d5n;var b7h=R13.U7n;b7h+=p7n;b7h+=y5y;b7h+=t7n;var x7h=b4y;x7h+=a1n;x7h+=G4y;x7h+=q4y;var l7h=a5n;l7h+=A2n;l7h+=a4y;l7h+=d5n;var D7h=C5n;D7h+=q7J;this[D7h](l7h,[json,setData,id]);this[x7h](b7h,fields,setData,store);this[w3J]([q3J,G7h],[json,setData,id]);}else if(action===L8Z){var n7h=c4y;n7h+=o1Z;n7h+=n4y;var c7h=S7J;c7h+=y7n;c7h+=R13.q7n;var a7h=b8J;a7h+=R13.q7n;var q7h=a5n;q7h+=r4y;this[w3J](q7h,[json,setData,id]);this[B8J](a7h,modifier,fields,setData,store);this[c7h]([a3J,n7h],[json,setData,id]);}}this[B8J](r7h,action,modifier,json[R8Z],store);}else if(action===U7h){var P7h=m5n;P7h+=W0Z;P7h+=a1n;var J7h=R13.U7n;J7h+=o7n;J7h+=U4y;var k7h=C5n;k7h+=m5n;k7h+=N4y;k7h+=q4y;var K7h=F1n;K7h+=e7Z;var u7h=M4y;u7h+=u4y;u7h+=t7n;var N7h=a5n;N7h+=p7n;N7h+=t7n;N7h+=a5n;this[B8J](N7h,action,modifier,submitParamsLocal,json,store);this[w3J](K4y,[json,this[k4y]()]);this[B8J](n3J,modifier,fields,store);this[w3J]([n3J,u7h],[json,this[K7h]()]);this[k7h](J7h,action,modifier,json[P7h],store);}if(editCount===this[q1n][B7h]){var p7h=R13.U7n;p7h+=L2Z;p7h+=Y0n;var o7h=o7n;o7h+=J4y;this[q1n][P6J]=F9Z;if(opts[o7h]===p7h&&(hide===undefined||hide)){this[t4n](json[R8Z]?B2n:J2n);}else if(typeof opts[R7y]===e9Z){opts[R7y](this);}}if(successCallback){var S7h=R13.U7n;S7h+=a1n;S7h+=P4y;successCallback[S7h](that,json);}this[i7h](B4y,[json,setData,action]);}this[O3J](J2n);this[w3J](o4y,[json,setData,action]);};Editor[G9Z][p4y]=function(xhr,err,thrown,errorCallback,submitParams,action){var z3y="Subm";var j3y="syste";var y4y="_proce";var H4y="Err";var i4y="omp";var M1h=S4y;M1h+=i4y;M1h+=O8y;var F1h=t4y;F1h+=R13.q7n;F1h+=H4y;F1h+=i1n;var j1h=y4y;j1h+=q1n;j1h+=Q3J;j1h+=r0n;var y7h=j3y;y7h+=n1n;var H7h=s3n;H7h+=p7n;H7h+=o7n;H7h+=p7n;var t7h=a5n;t7h+=N7Z;t7h+=z3y;t7h+=B7n;this[w3J](t7h,[F9Z,submitParams,action,xhr]);this[E7Z](this[Y8Z][H7h][y7h]);this[j1h](J2n);if(errorCallback){var z1h=R13.U7n;z1h+=a1n;z1h+=P4y;errorCallback[z1h](this,xhr,err,thrown);}this[w3J]([F1h,M1h],[xhr,err,thrown,submitParams]);};Editor[w1h][z3J]=function(fn){var C3y="lur";var d3y="omple";var T3y="ngs";var I3y="oFeatur";var Y3y="erSid";var s3y="rv";var A3y="bSe";var w3y="Table";var M3y="ocessing";var F3y="nl";var O1h=M5n;O1h+=S8J;O1h+=M5n;O1h+=l3n;var f1h=G1n;f1h+=J2Z;var Q1h=F1n;Q1h+=F3y;Q1h+=y6Z;Q1h+=t7n;var d1h=R4n;d1h+=M3y;var s1h=W8J;s1h+=m8J;s1h+=w3y;var A1h=R13.q7n;A1h+=p7Z;A1h+=h1n;A1h+=t7n;var that=this;var dt=this[q1n][A1h]?new $[f2n][s1h][W3J](this[q1n][f5Z]):F9Z;var ssp=J2n;if(dt){var T1h=A3y;T1h+=s3y;T1h+=Y3y;T1h+=t7n;var I1h=I3y;I1h+=H5n;var Y1h=H6Z;Y1h+=D3n;Y1h+=T3y;ssp=dt[Y1h]()[B9n][I1h][T1h];}if(this[q1n][d1h]){var Z1h=S4y;Z1h+=d3y;Z1h+=R13.q7n;Z1h+=t7n;this[V1Z](Z1h,function(){var Z3y='draw';if(ssp){var C1h=o7n;C1h+=m1n;dt[C1h](Z3y,fn);}else{setTimeout(function(){fn();},y9n);}});return B2n;}else if(this[Q2Z]()===Q1h||this[f1h]()===O1h){var R1h=M5n;R1h+=C3y;var V1h=R13.U7n;V1h+=h1n;V1h+=o7n;V1h+=Y0n;var h1h=o7n;h1h+=m1n;this[h1h](V1h,function(){var f3y="bmitC";if(!that[q1n][j9Z]){setTimeout(function(){if(that[q1n]){fn();}},y9n);}else{var v1h=Q3y;v1h+=f3y;v1h+=q5y;that[V1Z](v1h,function(e,json){if(ssp&&json){var g1h=m5n;g1h+=p7n;g1h+=a1n;g1h+=V3n;dt[V1Z](g1h,fn);}else{setTimeout(function(){if(that[q1n]){fn();}},y9n);}});}})[R1h]();return B2n;}return J2n;};Editor[W1h][m1h]=function(name,arr){var L1h=h1n;L1h+=o9J;for(var i=B9n,ien=arr[L1h];i<ien;i++){if(name==arr[i]){return i;}}return-o9n;};Editor[O3y]={"table":F9Z,"ajaxUrl":F9Z,"fields":[],"display":h3y,"ajax":F9Z,"idSrc":e1h,"events":{},"i18n":{"create":{"button":V3y,"title":v3y,"submit":g3y},"edit":{"button":E1h,"title":X1h,"submit":R3y},"remove":{"button":D1h,"title":l1h,"submit":W3y,"confirm":{"_":x1h,"1":b1h}},"error":{"system":G1h},multi:{title:q1h,info:m3y,restore:a1h,noMulti:c1h},"datetime":{previous:L3y,next:n1h,months:[e3y,r1h,E3y,U1h,X3y,N1h,D3y,u1h,l3y,x3y,K1h,b3y],weekdays:[G3y,k1h,q3y,a3y,c3y,n3y,J1h],amPm:[r3y,P1h],unknown:o4J}},formOptions:{bubble:$[B1h]({},Editor[o1h][p1h],{title:J2n,message:J2n,buttons:h7J,submit:S1h}),inline:$[C8Z]({},Editor[T9Z][i1h],{buttons:J2n,submit:t1h}),main:$[H1h]({},Editor[T9Z][U3y])},legacyAjax:J2n};(function(){var O2y="dataSrc";var T2y="[da";var B0y="cancelled";var v0y="cells";var N3y="dataTabl";var b5h=N3y;b5h+=t7n;var __dataSources=Editor[M6y]={};var __dtIsSsp=function(dt,editor){var J3y="bServerSide";var k3y="oFeatures";var K3y="tOpts";var u3y="wType";var j5h=m5n;j5h+=u1Z;j5h+=u3y;var y1h=t7n;y1h+=G1n;y1h+=K3y;return dt[J5Z]()[B9n][k3y][J3y]&&editor[q1n][y1h][j5h]!==K7Z;};var __dtApi=function(table){var z5h=D4y;z5h+=R2J;z5h+=S9Z;z5h+=t7n;return $(table)[z5h]();};var __dtHighlight=function(node){node=$(node);setTimeout(function(){var B3y="hl";var P3y="ig";var F5h=P9Z;F5h+=P3y;F5h+=B3y;F5h+=z0Z;node[O7Z](F5h);setTimeout(function(){var S3y="ighlight";var p3y="noH";var o3y="high";var E7n=550;var w5h=o3y;w5h+=h1n;w5h+=F1n;w5h+=Q6J;var M5h=p3y;M5h+=S3y;node[O7Z](M5h)[g7Z](w5h);setTimeout(function(){var i3y="noHighli";var A5h=i3y;A5h+=Q6J;node[g7Z](A5h);},E7n);},e7n);},A7n);};var __dtRowSelector=function(out,dt,identifier,fields,idFn){var Y5h=F1n;Y5h+=F0n;Y5h+=t3y;Y5h+=q1n;var s5h=p7n;s5h+=o6J;s5h+=q1n;dt[s5h](identifier)[Y5h]()[D2n](function(idx){var j0y="ow identifier";var y3y=" r";var H3y="Unable to find";var M7n=14;var Z5h=p7n;Z5h+=o6J;var d5h=y7n;d5h+=o7n;d5h+=m5n;d5h+=t7n;var row=dt[j8J](idx);var data=row[R8Z]();var idSrc=idFn(data);if(idSrc===undefined){var T5h=H3y;T5h+=y3y;T5h+=j0y;var I5h=t7n;I5h+=p7n;I5h+=p7n;I5h+=i1n;Editor[I5h](T5h,M7n);}out[idSrc]={idSrc:idSrc,data:data,node:row[d5h](),fields:fields,type:Z5h};});};var __dtFieldsFromIdx=function(dt,fields,idx){var T0y="ld from source. Please specify the field name.";var I0y="ly determine fie";var Y0y="to automatical";var s0y="Unable ";var A0y="mData";var w0y="editField";var F0y="aoColu";var z0y="tField";var Q5h=A1J;Q5h+=F1n;Q5h+=z0y;var C5h=F0y;C5h+=M0y;var field;var col=dt[J5Z]()[B9n][C5h][idx];var dataSrc=col[w0y]!==undefined?col[Q5h]:col[A0y];var resolvedFields={};var run=function(field,dataSrc){if(field[Q8Z]()===dataSrc){resolvedFields[field[Q8Z]()]=field;}};$[D2n](fields,function(name,fieldInst){if($[M8J](dataSrc)){for(var i=B9n;i<dataSrc[k2n];i++){run(fieldInst,dataSrc[i]);}}else{run(fieldInst,dataSrc);}});if($[l5y](resolvedFields)){var f5h=s0y;f5h+=Y0y;f5h+=I0y;f5h+=T0y;Editor[E7Z](f5h,j7n);}return resolvedFields;};var __dtCellSelector=function(out,dt,identifier,allFields,idFn,forceFields){var h5h=t7n;h5h+=j5J;var O5h=k0n;O5h+=P4y;O5h+=q1n;dt[O5h](identifier)[d0y]()[h5h](function(idx){var h0y="layFields";var f0y="column";var Q0y="ell";var C0y="ttach";var Z0y="splayFiel";var X5h=m5n;X5h+=F1n;X5h+=Z0y;X5h+=e7Z;var E5h=t7n;E5h+=H7n;E5h+=q5n;var e5h=t5Z;e5h+=e9y;var L5h=a1n;L5h+=C0y;var R5h=R13.b7n;R5h+=t7n;R5h+=R13.U7n;R5h+=R13.q7n;var g5h=m5n;g5h+=a1n;g5h+=R13.q7n;g5h+=a1n;var v5h=p7n;v5h+=o6J;var V5h=R13.U7n;V5h+=Q0y;var cell=dt[V5h](idx);var row=dt[j8J](idx[v5h]);var data=row[g5h]();var idSrc=idFn(data);var fields=forceFields||__dtFieldsFromIdx(dt,allFields,idx[f0y]);var isNode=typeof identifier===R5h&&identifier[O0y]||identifier instanceof $;var prevDisplayFields,prevAttach;if(out[idSrc]){var m5h=I4Z;m5h+=h0y;var W5h=W0Z;W5h+=H5J;W5h+=P9Z;prevAttach=out[idSrc][W5h];prevDisplayFields=out[idSrc][m5h];}__dtRowSelector(out,dt,idx[j8J],allFields,idFn);out[idSrc][k6J]=prevAttach||[];out[idSrc][L5h][l2n](isNode?$(identifier)[w7J](B9n):cell[S6J]());out[idSrc][e5h]=prevDisplayFields||{};$[E5h](out[idSrc][X5h],fields);});};var __dtColumnSelector=function(out,dt,identifier,fields,idFn){var V0y="xes";var l5h=t7n;l5h+=j5J;var D5h=y6Z;D5h+=m5n;D5h+=t7n;D5h+=V0y;dt[v0y](F9Z,identifier)[D5h]()[l5h](function(idx){__dtCellSelector(out,dt,idx,fields,idFn);});};var __dtjqId=function(id){var R0y="$";var g0y="\\";var x5h=g0y;x5h+=R0y;x5h+=M1n;return typeof id===r7J?m5J+id[b1Z](/(:|\.|\[|\]|,)/g,x5h):m5J+id;};__dataSources[b5h]={id:function(data){var L0y="DataFn";var m0y="tObje";var W0y="_fnGe";var a5h=W0y;a5h+=m0y;a5h+=K2n;a5h+=L0y;var q5h=o7n;q5h+=U5n;q5h+=a5n;q5h+=F1n;var G5h=t7n;G5h+=H7n;G5h+=R13.q7n;var idFn=DataTable[G5h][q5h][a5h](this[q1n][F6y]);return idFn(data);},individual:function(identifier,fieldNames){var D0y="ataFn";var X0y="etObjectD";var E0y="_fnG";var e0y="dSr";var r5h=l5n;r5h+=F1n;r5h+=t7n;r5h+=s8J;var n5h=F1n;n5h+=e0y;n5h+=R13.U7n;var c5h=E0y;c5h+=X0y;c5h+=D0y;var idFn=DataTable[o5n][W8Z][c5h](this[q1n][n5h]);var dt=__dtApi(this[q1n][f5Z]);var fields=this[q1n][r5h];var out={};var forceFields;var responsiveNode;if(fieldNames){var N5h=t7n;N5h+=X1Z;N5h+=P9Z;var U5h=l0y;U5h+=S3n;if(!$[U5h](fieldNames)){fieldNames=[fieldNames];}forceFields={};$[N5h](fieldNames,function(i,name){forceFields[name]=fields[name];});}__dtCellSelector(out,dt,identifier,fields,idFn,forceFields);return out;},fields:function(identifier){var n0y="colu";var c0y="um";var a0y="col";var q0y="ws";var G0y="mn";var b0y="lu";var x0y="cell";var k5h=x0y;k5h+=q1n;var K5h=R6J;K5h+=b0y;K5h+=G0y;K5h+=q1n;var u5h=k4n;u5h+=q0y;var idFn=DataTable[o5n][W8Z][e8Z](this[q1n][F6y]);var dt=__dtApi(this[q1n][f5Z]);var fields=this[q1n][F1J];var out={};if($[P8J](identifier)&&(identifier[u5h]!==undefined||identifier[K5h]!==undefined||identifier[k5h]!==undefined)){var B5h=a0y;B5h+=c0y;B5h+=j9J;var J5h=j8J;J5h+=q1n;if(identifier[J5h]!==undefined){var P5h=k4n;P5h+=q0y;__dtRowSelector(out,dt,identifier[P5h],fields,idFn);}if(identifier[B5h]!==undefined){var o5h=n0y;o5h+=M0y;__dtColumnSelector(out,dt,identifier[o5h],fields,idFn);}if(identifier[v0y]!==undefined){__dtCellSelector(out,dt,identifier[v0y],fields,idFn);}}else{__dtRowSelector(out,dt,identifier,fields,idFn);}return out;},create:function(fields,data){var dt=__dtApi(this[q1n][f5Z]);if(!__dtIsSsp(dt,this)){var i5h=y7n;i5h+=o7n;i5h+=K4n;var S5h=a1n;S5h+=m5n;S5h+=m5n;var p5h=k4n;p5h+=V3n;var row=dt[p5h][S5h](data);__dtHighlight(row[i5h]());}},edit:function(identifier,fields,data,store){var P0y="dataTableExt";var J0y="Ext";var k0y="_fn";var K0y="Ids";var u0y="owId";var N0y="li";var U0y="drawType";var r0y="itOp";var t5h=A1J;t5h+=r0y;t5h+=R13.q7n;t5h+=q1n;var that=this;var dt=__dtApi(this[q1n][f5Z]);if(!__dtIsSsp(dt,this)||this[q1n][t5h][U0y]===K7Z){var d4h=y7n;d4h+=c3n;d4h+=t7n;var w4h=a1n;w4h+=y7n;w4h+=V5n;var j4h=a1n;j4h+=y7n;j4h+=V5n;var H5h=F1n;H5h+=m5n;var rowId=__dataSources[r0Z][H5h][l9Z](this,data);var row;try{var y5h=p7n;y5h+=o7n;y5h+=V3n;row=dt[y5h](__dtjqId(rowId));}catch(e){row=dt;}if(!row[j4h]()){var z4h=p7n;z4h+=o7n;z4h+=V3n;row=dt[z4h](function(rowIdx,rowData,rowNode){var M4h=F1n;M4h+=m5n;var F4h=R8Z;F4h+=R2J;F4h+=f9Z;return rowId==__dataSources[F4h][M4h][l9Z](that,rowData);});}if(row[w4h]()){var T4h=J7J;T4h+=N0y;T4h+=k0n;var I4h=p7n;I4h+=u0y;I4h+=q1n;var Y4h=j8J;Y4h+=K0y;var s4h=y6Z;s4h+=U5n;s4h+=Z1Z;s4h+=V5n;var A4h=k0y;A4h+=J0y;A4h+=t7n;A4h+=F0n;var extender=$[f2n][P0y][W8Z][A4h];var toSave=extender({},row[R8Z](),B2n);toSave=extender(toSave,data,B2n);row[R8Z](toSave);var idx=$[s4h](rowId,store[Y4h]);store[I4h][T4h](idx,o9n);}else{row=dt[j8J][z8J](data);}__dtHighlight(row[d4h]());}},remove:function(identifier,fields,store){var p0y="ver";var Z4h=l3n;Z4h+=y7n;Z4h+=r9y;Z4h+=P9Z;var that=this;var dt=__dtApi(this[q1n][f5Z]);var cancelled=store[B0y];if(cancelled[Z4h]===B9n){var Q4h=p7n;Q4h+=f7J;Q4h+=k5y;Q4h+=t7n;var C4h=p7n;C4h+=o6J;C4h+=q1n;dt[C4h](identifier)[Q4h]();}else{var R4h=Z6y;R4h+=o0y;var f4h=t7n;f4h+=p0y;f4h+=V5n;var indexes=[];dt[U1J](identifier)[f4h](function(){var V4h=m5n;V4h+=v8Z;var h4h=R13.U7n;h4h+=a1n;h4h+=P4y;var O4h=F1n;O4h+=m5n;var id=__dataSources[r0Z][O4h][h4h](that,this[V4h]());if($[D8J](id,cancelled)===-o9n){var g4h=F1n;g4h+=R7J;var v4h=C1Z;v4h+=Q1Z;indexes[v4h](this[g4h]());}});dt[U1J](indexes)[R4h]();}},prep:function(action,identifier,submit,json,store){var y0y="lle";var i0y="lled";var S0y="owI";var x4h=p7n;x4h+=f7J;x4h+=o7n;x4h+=o0y;var W4h=t7n;W4h+=m5n;W4h+=F1n;W4h+=R13.q7n;if(action===W4h){var E4h=W8J;E4h+=m8J;var e4h=n1n;e4h+=a1n;e4h+=a5n;var L4h=p7n;L4h+=S0y;L4h+=e7Z;var m4h=z1y;m4h+=R13.U7n;m4h+=t7n;m4h+=i0y;var cancelled=json[m4h]||[];store[L4h]=$[e4h](submit[E4h],function(val,key){var t0y="Empty";var l4h=d1Z;l4h+=E4J;var D4h=m5n;D4h+=W0Z;D4h+=a1n;var X4h=U9Z;X4h+=t0y;X4h+=l4J;return!$[X4h](submit[D4h][key])&&$[l4h](key,cancelled)===-o9n?key:undefined;});}else if(action===x4h){var b4h=R13.U7n;b4h+=H0y;b4h+=y0y;b4h+=m5n;store[b4h]=json[B0y]||[];}},commit:function(action,identifier,data,store){var w2y="any";var F2y="wIds";var z2y="tabl";var j2y="awType";var u4h=m5n;u4h+=p7n;u4h+=j2y;var a4h=k4n;a4h+=V3n;a4h+=K6Z;a4h+=q1n;var q4h=t7n;q4h+=G1n;q4h+=R13.q7n;var G4h=z2y;G4h+=t7n;var that=this;var dt=__dtApi(this[q1n][G4h]);if(action===q4h&&store[a4h][k2n]){var c4h=k4n;c4h+=F2y;var ids=store[c4h];var row;var compare=function(id){return function(rowIdx,rowData,rowNode){var M2y="Tabl";var r4h=F1n;r4h+=m5n;var n4h=R8Z;n4h+=M2y;n4h+=t7n;return id==__dataSources[n4h][r4h][l9Z](that,rowData);};};for(var i=B9n,ien=ids[k2n];i<ien;i++){var U4h=a1n;U4h+=y7n;U4h+=V5n;try{row=dt[j8J](__dtjqId(ids[i]));}catch(e){row=dt;}if(!row[U4h]()){row=dt[j8J](compare(ids[i]));}if(row[w2y]()){var N4h=Z6y;N4h+=P4n;N4h+=t7n;row[N4h]();}}}var drawType=this[q1n][k4J][u4h];if(drawType!==K7Z){var K4h=m5n;K4h+=p7n;K4h+=a1n;K4h+=V3n;dt[K4h](drawType);}}};function __html_id(identifier){var I2y='Could not find an element with `data-editor-id` or `id` of: ';var Y2y="r-id";var s2y="data-edito";var A2y="less";var k4h=G0n;k4h+=t7n;k4h+=V5n;k4h+=A2y;var context=document;if(identifier!==k4h){var B4h=l3n;B4h+=r2n;B4h+=U2n;var P4h=e2n;P4h+=E2n;var J4h=e5y;J4h+=s2y;J4h+=Y2y;J4h+=b6Z;context=$(J4h+identifier+P4h);if(context[k2n]===B9n){context=typeof identifier===r7J?$(__dtjqId(identifier)):$(identifier);}if(context[B4h]===B9n){throw I2y+identifier;}}return context;}function __html_el(identifier,name){var Z2y="d=\"";var d2y="ta-editor-fie";var p4h=e2n;p4h+=E2n;var o4h=T2y;o4h+=d2y;o4h+=h1n;o4h+=Z2y;var context=__html_id(identifier);return $(o4h+name+p4h,context);}function __html_els(identifier,names){var out=$();for(var i=B9n,ien=names[k2n];i<ien;i++){var S4h=A3n;S4h+=m5n;out=out[S4h](__html_el(identifier,names[i]));}return out;}function __html_get(identifier,dataSrc){var f2y="lte";var Q2y="ta-editor-val";var C2y="a-editor-value";var y4h=P9Z;y4h+=g5Z;var H4h=h8Z;H4h+=C2y;var t4h=T2y;t4h+=Q2y;t4h+=l5Z;t4h+=E2n;var i4h=l5n;i4h+=F1n;i4h+=f2y;i4h+=p7n;var el=__html_el(identifier,dataSrc);return el[i4h](t4h)[k2n]?el[j0J](H4h):el[y4h]();}function __html_set(identifier,fields,data){$[D2n](fields,function(name,field){var V2y='data-editor-value';var h2y='[data-editor-value]';var val=field[m8Z](data);if(val!==undefined){var j3h=l5n;j3h+=K5n;j3h+=d5n;j3h+=p7n;var el=__html_el(identifier,field[O2y]());if(el[j3h](h2y)[k2n]){el[j0J](V2y,val);}else{var M3h=P9Z;M3h+=R13.q7n;M3h+=n1n;M3h+=h1n;var z3h=t7n;z3h+=a1n;z3h+=L5n;el[z3h](function(){var W2y="removeChild";var R2y="tChild";var g2y="irs";var v2y="childNodes";while(this[v2y][k2n]){var F3h=l5n;F3h+=g2y;F3h+=R2y;this[W2y](this[F3h]);}})[M3h](val);}}});}__dataSources[o7Z]={id:function(data){var A3h=o7n;A3h+=U5n;A3h+=a5n;A3h+=F1n;var w3h=G5n;w3h+=R13.q7n;var idFn=DataTable[w3h][A3h][e8Z](this[q1n][F6y]);return idFn(data);},initField:function(cfg){var X2y="label=\"";var E2y="editor-";var e2y="[data-";var L2y="ame";var m2y="labe";var Z3h=h1n;Z3h+=t7n;Z3h+=Q1J;var d3h=m2y;d3h+=h1n;var T3h=e2n;T3h+=E2n;var I3h=y7n;I3h+=L2y;var Y3h=m5n;Y3h+=a1n;Y3h+=R13.q7n;Y3h+=a1n;var s3h=e2y;s3h+=E2y;s3h+=X2y;var label=$(s3h+(cfg[Y3h]||cfg[I3h])+T3h);if(!cfg[d3h]&&label[Z3h]){cfg[G8Z]=label[o7Z]();}},individual:function(identifier,fieldNames){var r2y="data source";var n2y="name from ";var c2y=" determine field ";var a2y="Cannot automatically";var q2y='keyless';var G2y='editor-id';var b2y='[data-editor-id]';var x2y='andSelf';var l2y='addBack';var D2y="ata-editor-field";var m3h=Q6y;m3h+=P9Z;var W3h=z8Z;W3h+=m0n;W3h+=m5n;W3h+=q1n;var R3h=R13.U7n;R3h+=a1n;R3h+=h1n;R3h+=h1n;var g3h=l5n;g3h+=e4J;g3h+=q1n;var V3h=K1Z;V3h+=r0n;V3h+=U2n;var attachEl;if(identifier instanceof $||identifier[O0y]){var h3h=m5n;h3h+=a1n;h3h+=m8J;var O3h=L8y;O3h+=t7n;O3h+=Y4J;var f3h=l5n;f3h+=y7n;attachEl=identifier;if(!fieldNames){var Q3h=m5n;Q3h+=D2y;var C3h=q5J;C3h+=p7n;fieldNames=[$(identifier)[C3h](Q3h)];}var back=$[f3h][h4J]?l2y:x2y;identifier=$(identifier)[O3h](b2y)[back]()[h3h](G2y);}if(!identifier){identifier=q2y;}if(fieldNames&&!$[M8J](fieldNames)){fieldNames=[fieldNames];}if(!fieldNames||fieldNames[V3h]===B9n){var v3h=a2y;v3h+=c2y;v3h+=n2y;v3h+=r2y;throw v3h;}var out=__dataSources[o7Z][g3h][R3h](this,identifier);var fields=this[q1n][W3h];var forceFields={};$[D2n](fieldNames,function(i,name){forceFields[name]=fields[name];});$[m3h](out,function(id,set){var N2y='cell';var U2y="toArra";var E3h=U2y;E3h+=V5n;var e3h=a1n;e3h+=R13.q7n;e3h+=R13.q7n;e3h+=j5J;var L3h=R13.q7n;L3h+=r4n;L3h+=t7n;set[L3h]=N2y;set[e3h]=attachEl?$(attachEl):__html_els(identifier,fieldNames)[E3h]();set[F1J]=fields;set[l9y]=forceFields;});return out;},fields:function(identifier){var K2y='row';var u2y="keyl";var b3h=z8Z;b3h+=m0n;b3h+=e7Z;var X3h=P9Z;X3h+=g5Z;var out={};var self=__dataSources[X3h];if($[M8J](identifier)){var D3h=h1n;D3h+=t7n;D3h+=D0J;D3h+=P9Z;for(var i=B9n,ien=identifier[D3h];i<ien;i++){var x3h=R13.U7n;x3h+=A0n;x3h+=h1n;var l3h=Q5J;l3h+=m5n;l3h+=q1n;var res=self[l3h][x3h](this,identifier[i]);out[identifier[i]]=res[identifier[i]];}return out;}var data={};var fields=this[q1n][b3h];if(!identifier){var G3h=u2y;G3h+=i0J;identifier=G3h;}$[D2n](fields,function(name,field){var q3h=g9Z;q3h+=X9J;q3h+=R1n;q3h+=v8Z;var val=__html_get(identifier,field[O2y]());field[q3h](data,val===F9Z?undefined:val);});out[identifier]={idSrc:identifier,data:data,node:document,fields:fields,type:K2y};return out;},create:function(fields,data){if(data){var a3h=R13.U7n;a3h+=a1n;a3h+=h1n;a3h+=h1n;var id=__dataSources[o7Z][k3n][a3h](this,data);try{if(__html_id(id)[k2n]){__html_set(id,fields,data);}}catch(e){}}},edit:function(identifier,fields,data){var k2y="yless";var n3h=I7y;n3h+=k2y;var c3h=P9Z;c3h+=R13.q7n;c3h+=h5Z;var id=__dataSources[c3h][k3n][l9Z](this,data)||n3h;__html_set(id,fields,data);},remove:function(identifier,fields){var r3h=A2n;r3h+=n1n;r3h+=o7n;r3h+=o0y;__html_id(identifier)[r3h]();}};}());Editor[J9Z]={"wrapper":J2y,"processing":{"indicator":U3h,"active":j9Z},"header":{"wrapper":P2y,"content":B2y},"body":{"wrapper":N3h,"content":u3h},"footer":{"wrapper":K3h,"content":k3h},"form":{"wrapper":o2y,"content":J3h,"tag":R13.K7n,"info":P3h,"error":p2y,"buttons":S2y,"button":B3h},"field":{"wrapper":i2y,"typePrefix":o3h,"namePrefix":p3h,"label":t2y,"input":H2y,"inputControl":S3h,"error":i3h,"msg-label":t3h,"msg-error":y2y,"msg-message":j6u,"msg-info":H3h,"multiValue":z6u,"multiInfo":y3h,"multiRestore":F6u,"multiNoEdit":j0h,"disabled":z0h,"processing":F0h},"actions":{"create":M0h,"edit":M6u,"remove":w0h},"inline":{"wrapper":A0h,"liner":s0h,"buttons":w6u},"bubble":{"wrapper":A6u,"liner":s6u,"table":Y6u,"close":Y0h,"pointer":I0h,"bg":I6u}};(function(){var e8u="removeSingle";var L8u='selectedSingle';var m8u="editSingle";var Z8u="formTitle";var j8u="18";var H6u='buttons-create';var S6u="subm";var c6u="editor_remove";var a6u="formButtons";var G6u="i18";var l6u="editor_edit";var E6u="BUTTONS";var e6u="TableTools";var L6u="r_create";var m6u="edito";var W6u="ten";var R6u="_single";var v6u="Too";var V6u="Tab";var h6u="ons-edit";var O6u="utt";var Q6u="emo";var C6u="buttons-r";var Z6u="oveSing";var d6u="ctedSingl";var N2h=T6u;N2h+=d6u;N2h+=t7n;var U2h=G5n;U2h+=d5n;U2h+=F0n;var r2h=t9Z;r2h+=Z6u;r2h+=l3n;var n2h=t7n;n2h+=m5n;n2h+=F1n;n2h+=R13.q7n;var c2h=t7n;c2h+=s8Z;c2h+=m5n;var g2h=C6u;g2h+=Q6u;g2h+=P4n;g2h+=t7n;var f2h=k4n;f2h+=V3n;f2h+=q1n;var Q2h=q1n;Q2h+=t7n;Q2h+=f6u;Q2h+=A1J;var j2h=M5n;j2h+=O6u;j2h+=h6u;var U0h=t7n;U0h+=H7n;U0h+=R13.q7n;var T0h=V6u;T0h+=l3n;T0h+=v6u;T0h+=V0n;if(DataTable[T0h]){var W0h=G5n;W0h+=d5n;W0h+=F0n;var O0h=g6u;O0h+=u2n;O0h+=R6u;var Z0h=t7n;Z0h+=H7n;Z0h+=W6u;Z0h+=m5n;var d0h=m6u;d0h+=L6u;var ttButtons=DataTable[e6u][E6u];var ttButtonBase={sButtonText:F9Z,editor:F9Z,formTitle:F9Z};ttButtons[d0h]=$[Z0h](B2n,ttButtons[k5Z],ttButtonBase,{formButtons:[{label:F9Z,fn:function(e){this[I5Z]();}}],fnClick:function(button,config){var D6u="tor";var X6u="mB";var Q0h=M9J;Q0h+=X6u;Q0h+=g6y;Q0h+=j9J;var C0h=b8J;C0h+=D6u;var editor=config[C0h];var i18nCreate=editor[Y8Z][o7J];var buttons=config[Q0h];if(!buttons[B9n][G8Z]){var f0h=t4y;f0h+=R13.q7n;buttons[B9n][G8Z]=i18nCreate[f0h];}editor[o7J]({title:i18nCreate[J8Z],buttons:buttons});}});ttButtons[l6u]=$[C8Z](B2n,ttButtons[O0h],ttButtonBase,{formButtons:[{label:F9Z,fn:function(e){this[I5Z]();}}],fnClick:function(button,config){var q6u="fnGetSelectedIndexes";var x6u="titl";var R0h=x6u;R0h+=t7n;var g0h=N6Z;g0h+=t7n;g0h+=h1n;var v0h=t7n;v0h+=b6u;var V0h=G6u;V0h+=y7n;var h0h=L8Z;h0h+=i1n;var selected=this[q6u]();if(selected[k2n]!==o9n){return;}var editor=config[h0h];var i18nEdit=editor[V0h][v0h];var buttons=config[a6u];if(!buttons[B9n][g0h]){buttons[B9n][G8Z]=i18nEdit[I5Z];}editor[L8Z](selected[B9n],{title:i18nEdit[R0h],buttons:buttons});}});ttButtons[c6u]=$[W0h](B2n,ttButtons[n6u],ttButtonBase,{question:F9Z,formButtons:[{label:F9Z,fn:function(e){var that=this;this[I5Z](function(json){var K6u="fnSelectNone";var u6u="ataTa";var N6u="ols";var U6u="tInstance";var r6u="fnGe";var E0h=r6u;E0h+=U6u;var e0h=V6u;e0h+=l3n;e0h+=X9J;e0h+=N6u;var L0h=m5n;L0h+=u6u;L0h+=S9Z;L0h+=t7n;var m0h=l5n;m0h+=y7n;var tt=$[m0h][L0h][e0h][E0h]($(that[q1n][f5Z])[L2n]()[f5Z]()[S6J]());tt[K6u]();});}}],fnClick:function(button,config){var i6u="be";var p6u="SelectedIndexes";var o6u="fnGet";var B6u="tring";var P6u="firm";var J6u="ir";var k6u="nfirm";var r0h=K1Z;r0h+=B9J;var a0h=R6J;a0h+=k6u;var q0h=f3Z;q0h+=J6u;q0h+=n1n;var G0h=Q7Z;G0h+=P6u;var b0h=Q7Z;b0h+=l5n;b0h+=J6u;b0h+=n1n;var x0h=q1n;x0h+=B6u;var l0h=p7n;l0h+=c3J;var D0h=F1n;D0h+=M1n;D0h+=w1n;D0h+=y7n;var X0h=o6u;X0h+=p6u;var rows=this[X0h]();if(rows[k2n]===B9n){return;}var editor=config[e3J];var i18nRemove=editor[D0h][l0h];var buttons=config[a6u];var question=typeof i18nRemove[x3J]===x0h?i18nRemove[b0h]:i18nRemove[G0h][rows[k2n]]?i18nRemove[q0h][rows[k2n]]:i18nRemove[a0h][C5n];if(!buttons[B9n][G8Z]){var n0h=S6u;n0h+=F1n;n0h+=R13.q7n;var c0h=h1n;c0h+=a1n;c0h+=i6u;c0h+=h1n;buttons[B9n][c0h]=i18nRemove[n0h];}editor[t4J](rows,{message:question[b1Z](/%d/g,rows[r0h]),title:i18nRemove[J8Z],buttons:buttons});}});}var _buttons=DataTable[U0h][q9J];$[C8Z](_buttons,{create:{text:function(dt,node,config){var t6u="buttons.";var k0h=M5n;k0h+=r1n;k0h+=j7J;k0h+=g2n;var K0h=R13.U7n;K0h+=F0J;K0h+=d5n;var u0h=A1J;u0h+=B7n;u0h+=i1n;var N0h=t6u;N0h+=R13.U7n;N0h+=K6J;return dt[Y8Z](N0h,config[u0h][Y8Z][K0h][k0h]);},className:H6u,editor:F9Z,formButtons:{text:function(editor){var y6u="mi";var J0h=Q3y;J0h+=M5n;J0h+=y6u;J0h+=R13.q7n;return editor[Y8Z][o7J][J0h];},action:function(e){var P0h=S6u;P0h+=B7n;this[P0h]();}},formMessage:F9Z,formTitle:F9Z,action:function(e,dt,node,config){var w8u="Butto";var M8u="formMe";var z8u="mT";var t0h=R13.q7n;t0h+=B7n;t0h+=l3n;var i0h=F1n;i0h+=j8u;i0h+=y7n;var S0h=B1n;S0h+=p7n;S0h+=z8u;S0h+=F8u;var p0h=M8u;p0h+=u3n;var o0h=X4y;o0h+=t7n;o0h+=a1n;o0h+=d5n;var B0h=M9J;B0h+=n1n;B0h+=w8u;B0h+=j9J;var that=this;var editor=config[e3J];var buttons=config[B0h];this[j9Z](B2n);editor[V1Z](j5y,function(){that[j9Z](J2n);})[o0h]({buttons:config[a6u],message:config[p0h],title:config[S0h]||editor[i0h][o7J][t0h]});}},edit:{extend:A8u,text:function(dt,node,config){var s8u='buttons.edit';var y0h=t7n;y0h+=m5n;y0h+=F1n;y0h+=R13.q7n;var H0h=t7n;H0h+=b6u;H0h+=o7n;H0h+=p7n;return dt[Y8Z](s8u,config[H0h][Y8Z][y0h][B5Z]);},className:j2h,editor:F9Z,formButtons:{text:function(editor){var M2h=t4y;M2h+=R13.q7n;var F2h=b8J;F2h+=R13.q7n;var z2h=G6u;z2h+=y7n;return editor[z2h][F2h][M2h];},action:function(e){var w2h=q1n;w2h+=r1n;w2h+=M5n;w2h+=E4n;this[w2h]();}},formMessage:F9Z,formTitle:F9Z,action:function(e,dt,node,config){var d8u="Ope";var T8u="age";var I8u="ormMess";var Y8u="tle";var C2h=D3n;C2h+=Y8u;var Z2h=F1n;Z2h+=j8u;Z2h+=y7n;var d2h=l5n;d2h+=I8u;d2h+=T8u;var T2h=K7y;T2h+=d8u;T2h+=y7n;var I2h=g2n;I2h+=t7n;var Y2h=Q3n;Y2h+=V0n;var s2h=R6J;s2h+=h1n;s2h+=r1n;s2h+=M0y;var A2h=F1n;A2h+=F0n;A2h+=t3y;A2h+=q1n;var that=this;var editor=config[e3J];var rows=dt[U1J]({selected:B2n})[A2h]();var columns=dt[s2h]({selected:B2n})[d0y]();var cells=dt[Y2h]({selected:B2n})[d0y]();var items=columns[k2n]||cells[k2n]?{rows:rows,columns:columns,cells:cells}:rows;this[j9Z](B2n);editor[I2h](T2h,function(){that[j9Z](J2n);})[L8Z](items,{message:config[d2h],buttons:config[a6u],title:config[Z8u]||editor[Z2h][L8Z][C2h]});}},remove:{extend:Q2h,limitTo:[f2h],text:function(dt,node,config){var f8u='buttons.remove';var Q8u="18n";var C8u="8n";var v2h=p7n;v2h+=f7J;v2h+=o7n;v2h+=o0y;var V2h=F1n;V2h+=M1n;V2h+=C8u;var h2h=m6u;h2h+=p7n;var O2h=F1n;O2h+=Q8u;return dt[O2h](f8u,config[h2h][V2h][v2h][B5Z]);},className:g2h,editor:F9Z,formButtons:{text:function(editor){var W2h=q1n;W2h+=G8y;W2h+=F1n;W2h+=R13.q7n;var R2h=A2n;R2h+=C0n;R2h+=o0y;return editor[Y8Z][R2h][W2h];},action:function(e){this[I5Z]();}},formMessage:function(editor,dt){var v8u="irm";var V8u="onfirm";var x2h=D1Z;x2h+=O8u;var l2h=R6J;l2h+=i5Z;l2h+=F1n;l2h+=L2J;var D2h=R13.U7n;D2h+=g2n;D2h+=h8u;D2h+=n1n;var X2h=R13.U7n;X2h+=V8u;var E2h=Q7Z;E2h+=l5n;E2h+=v8u;var e2h=q1n;e2h+=O0n;e2h+=y6Z;e2h+=r0n;var L2h=A2n;L2h+=C0n;L2h+=P4n;L2h+=t7n;var m2h=p7n;m2h+=o7n;m2h+=V3n;m2h+=q1n;var rows=dt[m2h]({selected:B2n})[d0y]();var i18n=editor[Y8Z][L2h];var question=typeof i18n[x3J]===e2h?i18n[E2h]:i18n[X2h][rows[k2n]]?i18n[D2h][rows[k2n]]:i18n[l2h][C5n];return question[x2h](/%d/g,rows[k2n]);},formTitle:F9Z,action:function(e,dt,node,config){var W8u="formMessage";var R8u="eO";var g8u="exes";var a2h=p7n;a2h+=c3J;var q2h=F1n;q2h+=y7n;q2h+=m5n;q2h+=g8u;var G2h=t9Z;G2h+=X1n;var b2h=R4n;b2h+=R8u;b2h+=a5n;b2h+=J7Z;var that=this;var editor=config[e3J];this[j9Z](B2n);editor[V1Z](b2h,function(){that[j9Z](J2n);})[G2h](dt[U1J]({selected:B2n})[q2h](),{buttons:config[a6u],message:config[W8u],title:config[Z8u]||editor[Y8Z][a2h][J8Z]});}}});_buttons[m8u]=$[c2h]({},_buttons[n2h]);_buttons[m8u][C8Z]=L8u;_buttons[r2h]=$[U2h]({},_buttons[t4J]);_buttons[e8u][C8Z]=N2h;}());Editor[E8u]={};Editor[X8u]=function(input,opts){var j7u="calendar";var y9u=/[haA]/;var H9u=/[Hhm]|LT|LTS/;var t9u="match";var i9u=/[YMD]|L(?!T)|l/;var S9u='-date';var p9u='-error"/>';var o9u='<span/>';var B9u='-label">';var P9u='-iconRight">';var J9u='-title">';var u9u='<select class="';var N9u='<button>';var X9u="Editor datetime: Without momentjs only the format 'YYYY-MM-DD' can be used";var m9u="efix";var W9u="classPr";var R9u="-MM-DD";var v9u="YYY";var V9u=" class=\"";var h9u="e\">";var O9u="\">";var f9u="onLeft";var Q9u="-ic";var C9u="</button";var Z9u="ass=";var d9u="<div cl";var T9u="/button>";var I9u="span";var Y9u="-month";var A9u="<div class=";var w9u="abel\">";var M9u="lass=\"";var F9u="<select c";var j9u="-year";var H8u="ndar\"/>";var o8u="tes";var B8u="minu";var J8u="pm";var k8u="tl";var K8u="-ti";var u8u="alenda";var N8u="-c";var U8u="-tim";var r8u="-erro";var n8u="ateime-";var c8u="-d";var q8u="ormat";var x8u="exO";var l8u="tructor";var D8u="_cons";var F8x=D8u;F8x+=l8u;var z8x=h1Z;z8x+=n1n;var j8x=a1n;j8x+=B3Z;j8x+=C1J;var y6x=k7Z;y6x+=B0Z;var H6x=t7n;H6x+=Q8y;H6x+=o7n;H6x+=p7n;var t6x=h9y;t6x+=m5n;var i6x=m5n;i6x+=o7n;i6x+=n1n;var S6x=y6Z;S6x+=m5n;S6x+=x8u;S6x+=l5n;var p6x=l5n;p6x+=i1n;p6x+=b8u;var o6x=B1n;o6x+=L2J;o6x+=a1n;o6x+=R13.q7n;var B6x=u3Z;B6x+=G8u;B6x+=P9Z;var P6x=l5n;P6x+=q8u;var J6x=a8u;J6x+=o1Z;J6x+=H0y;var k6x=L8Z;k6x+=i1n;k6x+=c8u;k6x+=n8u;var K6x=r8u;K6x+=p7n;var u6x=U8u;u6x+=t7n;var N6x=l5n;N6x+=F1n;N6x+=y7n;N6x+=m5n;var U6x=N8u;U6x+=u8u;U6x+=p7n;var r6x=l5n;r6x+=F1n;r6x+=y7n;r6x+=m5n;var n6x=K8u;n6x+=k8u;n6x+=t7n;var c6x=l5n;c6x+=x6J;var a6x=z2J;a6x+=J8u;var q6x=P8u;q6x+=F0n;q6x+=q1n;var G6x=B8u;G6x+=o8u;var b6x=P9Z;b6x+=o7n;b6x+=p8u;var x6x=S8u;x6x+=i8u;x6x+=e2n;x6x+=O6Z;var l6x=F4Z;l6x+=q1n;l6x+=R6Z;var D6x=d6Z;D6x+=e6Z;D6x+=f6Z;D6x+=O6Z;var X6x=S8u;X6x+=t8u;X6x+=H8u;var E6x=U0Z;E6x+=y8u;E6x+=o6Z;E6x+=P2J;var e6x=d6Z;e6x+=Z6Z;e6x+=m5n;e6x+=q6Z;var L6x=G6Z;L6x+=s9J;L6x+=O6Z;var m6x=j9u;m6x+=e2n;m6x+=z9u;var W6x=F9u;W6x+=M9u;var R6x=S8u;R6x+=h1n;R6x+=w9u;var g6x=A9u;g6x+=e2n;var v6x=s9u;v6x+=O6Z;var V6x=Y9u;V6x+=e2n;V6x+=z9u;var h6x=d6Z;h6x+=I9u;h6x+=z9u;var O6x=Q6Z;O6x+=f6Z;O6x+=O6Z;var f6x=d6Z;f6x+=T9u;var Q6x=y7n;Q6x+=t7n;Q6x+=e7J;var C6x=d9u;C6x+=Z9u;C6x+=e2n;var Z6x=C9u;Z6x+=O6Z;var d6x=C1y;d6x+=F1n;d6x+=o7n;d6x+=q7Z;var T6x=Q9u;T6x+=f9u;T6x+=O9u;var I6x=i6Z;I6x+=R6Z;var Y6x=c8u;Y6x+=W0Z;Y6x+=h9u;var s6x=E2J;s6x+=V9u;var A6x=E2J;A6x+=V9u;var P2h=v9u;P2h+=g9u;P2h+=R9u;var J2h=F1n;J2h+=M1n;J2h+=w1n;J2h+=y7n;var k2h=W9u;k2h+=m9u;var K2h=g0n;K2h+=a1n;K2h+=L9u;K2h+=s1n;var u2h=W5n;u2h+=m5n;this[R13.U7n]=$[u2h](B2n,{},Editor[X8u][K2h],opts);var classPrefix=this[R13.U7n][k2h];var i18n=this[R13.U7n][J2h];if(!window[e9u]&&this[R13.U7n][E9u]!==P2h){throw X9u;}var timeBlock=function(type){var U9u='-iconUp">';var r9u='-timeblock">';var n9u="reviou";var c9u="</butto";var q9u="-l";var G9u="an/";var b9u="<sp";var x9u="conDow";var M6x=G6Z;M6x+=G1n;M6x+=m6Z;var F6x=Q6Z;F6x+=q6Z;var z6x=G6Z;z6x+=L7J;z6x+=D9u;var j6x=m1n;j6x+=H7n;j6x+=R13.q7n;var y2h=l9u;y2h+=x9u;y2h+=y7n;y2h+=O9u;var H2h=e2n;H2h+=Z6Z;H2h+=O6Z;var t2h=b9u;t2h+=G9u;t2h+=O6Z;var i2h=q9u;i2h+=w9u;var S2h=a9u;S2h+=n6Z;var p2h=c9u;p2h+=y7n;p2h+=O6Z;var o2h=a5n;o2h+=n9u;o2h+=q1n;var B2h=a9u;B2h+=s0n;B2h+=Z9u;B2h+=e2n;return m9J+classPrefix+r9u+B2h+classPrefix+U9u+N9u+i18n[o2h]+p2h+c8Z+S2h+classPrefix+i2h+t2h+u9u+classPrefix+o4J+type+H2h+c8Z+m9J+classPrefix+y2h+N9u+i18n[j6x]+z6x+F6x+M6x;};var gap=function(){var K9u="<span>:</sp";var w6x=K9u;w6x+=k9u;return w6x;};var structure=$(A6x+classPrefix+q8Z+s6x+classPrefix+Y6x+m9J+classPrefix+J9u+I6x+classPrefix+T6x+N9u+i18n[d6x]+Z6x+c8Z+C6x+classPrefix+P9u+N9u+i18n[Q6x]+f6x+O6x+m9J+classPrefix+B9u+h6x+u9u+classPrefix+V6x+v6x+g6x+classPrefix+R6x+o9u+W6x+classPrefix+m6x+L6x+e6x+E6x+classPrefix+X6x+D6x+l6x+classPrefix+x6x+timeBlock(b6x)+gap()+timeBlock(G6x)+gap()+timeBlock(q6x)+timeBlock(a6x)+c8Z+m9J+classPrefix+p9u+c8Z);this[I9Z]={container:structure,date:structure[c6x](M4J+classPrefix+S9u),title:structure[F4J](M4J+classPrefix+n6x),calendar:structure[r6x](M4J+classPrefix+U6x),time:structure[N6x](M4J+classPrefix+u6x),error:structure[F4J](M4J+classPrefix+K6x),input:$(input)};this[q1n]={d:F9Z,display:F9Z,namespace:k6x+Editor[X8u][J6x]++,parts:{date:this[R13.U7n][P6x][B6x](i9u)!==F9Z,time:this[R13.U7n][o6x][t9u](H9u)!==F9Z,seconds:this[R13.U7n][p6x][S6x](Q2n)!==-o9n,hours12:this[R13.U7n][E9u][t9u](y9u)!==F9Z}};this[i6x][j7Z][h4Z](this[I9Z][j1n])[t6x](this[I9Z][i8u])[h4Z](this[I9Z][H6x]);this[I9Z][j1n][y6x](this[I9Z][J8Z])[j8x](this[z8x][j7u]);this[F8x]();};$[C8Z](Editor[M8x][G9Z],{destroy:function(){var F7u="tetime";var z7u=".editor-da";var T8x=z7u;T8x+=F7u;var I8x=F1n;I8x+=M7u;I8x+=R13.q7n;var Y8x=m5n;Y8x+=T7Z;var s8x=f0Z;s8x+=l5n;var A8x=m5n;A8x+=o7n;A8x+=n1n;var w8x=C5n;w8x+=P9Z;w8x+=k3n;w8x+=t7n;this[w8x]();this[A8x][j7Z][s8x]()[w7u]();this[Y8x][I8x][a2Z](T8x);},errorMsg:function(msg){var A7u="empt";var error=this[I9Z][E7Z];if(msg){var d8x=P9Z;d8x+=R13.q7n;d8x+=n1n;d8x+=h1n;error[d8x](msg);}else{var Z8x=A7u;Z8x+=V5n;error[Z8x]();}},hide:function(){this[s7u]();},max:function(date){var Y7u="_setCalande";var Q8x=Y7u;Q8x+=p7n;var C8x=u3Z;C8x+=H7n;C8x+=Y1n;C8x+=t7n;this[R13.U7n][C8x]=date;this[I7u]();this[Q8x]();},min:function(date){this[R13.U7n][T7u]=date;this[I7u]();this[d7u]();},owns:function(node){var C7u="filter";var Z7u="rent";var f8x=a5n;f8x+=a1n;f8x+=Z7u;f8x+=q1n;return $(node)[f8x]()[C7u](this[I9Z][j7Z])[k2n]>B9n;},val:function(set,write){var x7u="toString";var X7u=/(\d{4})\-(\d{2})\-(\d{2})/;var e7u="isValid";var L7u="men";var m7u="mentLoca";var W7u="entStrict";var g7u="eToUt";var v7u="_da";var V7u="stri";var h7u="_set";var O7u="_setCa";var f7u="ime";var Q7u="setT";var D8x=C5n;D8x+=Q7u;D8x+=f7u;var X8x=O7u;X8x+=h1n;X8x+=E8y;X8x+=i3Z;var E8x=h7u;E8x+=d1n;E8x+=R13.q7n;E8x+=l3n;var e8x=m5n;e8x+=U9Z;e8x+=E1Z;e8x+=S3n;var h8x=V7u;h8x+=r2n;if(set===undefined){return this[q1n][m5n];}if(set instanceof Date){var O8x=v7u;O8x+=R13.q7n;O8x+=g7u;O8x+=R13.U7n;this[q1n][m5n]=this[O8x](set);}else if(set===F9Z||set===C2n){this[q1n][m5n]=F9Z;}else if(typeof set===h8x){if(window[e9u]){var W8x=J4n;W8x+=R7u;W8x+=R13.q7n;W8x+=t7n;var R8x=n1n;R8x+=T7Z;R8x+=W7u;var g8x=C0n;g8x+=m7u;g8x+=l3n;var v8x=r1n;v8x+=R13.q7n;v8x+=R13.U7n;var V8x=n1n;V8x+=o7n;V8x+=L7u;V8x+=R13.q7n;var m=window[V8x][v8x](set,this[R13.U7n][E9u],this[R13.U7n][g8x],this[R13.U7n][R8x]);this[q1n][m5n]=m[e7u]()?m[W8x]():F9Z;}else{var L8x=E7u;L8x+=M8Z;L8x+=N6y;var m8x=u3Z;m8x+=G8u;m8x+=P9Z;var match=set[m8x](X7u);this[q1n][m5n]=match?new Date(Date[L8x](match[o9n],match[p9n]-o9n,match[S9n])):F9Z;}}if(write||write===undefined){if(this[q1n][m5n]){this[D7u]();}else{this[I9Z][r8Z][g9Z](set);}}if(!this[q1n][m5n]){this[q1n][m5n]=this[l7u](new Date());}this[q1n][Q2Z]=new Date(this[q1n][m5n][x7u]());this[q1n][e8x][b7u](o9n);this[E8x]();this[X8x]();this[D8x]();},_constructor:function(){var J1u="_writeOu";var q1u="_setTitle";var L1u="onth";var W1u="has";var g1u="Class";var d1u='keyup.editor-datetime';var A1u='off';var w1u='autocomplete';var j1u="_optionsTime";var H7u="last";var t7u="timeblock";var i7u="tetime-";var S7u="div.editor-da";var o7u='div.editor-datetime-timeblock';var B7u="childre";var P7u="q";var J7u="parts";var K7u="Cha";var u7u="sec";var N7u="inut";var U7u="Increment";var r7u="minutes";var n7u="nds";var c7u="ndsIncrement";var a7u="mPm";var q7u="-datetime";var G7u="focus.editor-datetime click.editor";var P9x=o7n;P9x+=y7n;var v9x=q1n;v9x+=m0n;v9x+=R13.G7n;v9x+=R13.q7n;var s9x=G7u;s9x+=q7u;var A9x=a1n;A9x+=R13.q7n;A9x+=R13.q7n;A9x+=p7n;var w9x=y6Z;w9x+=C1Z;w9x+=R13.q7n;var M9x=a1n;M9x+=a7u;var F9x=a1n;F9x+=n1n;var z9x=P8u;z9x+=c7u;var j9x=P8u;j9x+=n7u;var y8x=r7u;y8x+=U7u;var H8x=n1n;H8x+=N7u;H8x+=H5n;var t8x=K5Z;t8x+=r1n;t8x+=p7n;t8x+=q1n;var i8x=C5n;i8x+=a6y;i8x+=X5n;i8x+=I1n;var B8x=K5Z;B8x+=p8u;B8x+=M1n;B8x+=t0n;var U8x=u7u;U8x+=g2n;U8x+=e7Z;var a8x=D3n;a8x+=n1n;a8x+=t7n;var b8x=g2n;b8x+=K7u;b8x+=r2n;b8x+=t7n;var x8x=R6J;x8x+=Z5n;x8x+=a1n;x8x+=j5Z;var l8x=m5n;l8x+=o7n;l8x+=n1n;var that=this;var classPrefix=this[R13.U7n][k7u];var container=this[l8x][x8x];var i18n=this[R13.U7n][Y8Z];var onChange=this[R13.U7n][b8x];if(!this[q1n][J7u][j1n]){var q8x=y7n;q8x+=V1Z;var G8x=h1Z;G8x+=n1n;this[G8x][j1n][s9Z](Y9Z,q8x);}if(!this[q1n][J7u][a8x]){var r8x=Z1J;r8x+=m1n;var n8x=U7Z;n8x+=S3n;var c8x=D3n;c8x+=Z1n;this[I9Z][c8x][s9Z](n8x,r8x);}if(!this[q1n][J7u][U8x]){var P8x=t9Z;P8x+=k5y;P8x+=t7n;var J8x=t7n;J8x+=P7u;var k8x=B7u;k8x+=y7n;var K8x=m5n;K8x+=o7n;K8x+=n1n;var u8x=t7n;u8x+=P7u;var N8x=m5n;N8x+=o7n;N8x+=n1n;this[N8x][i8u][O4Z](o7u)[u8x](p9n)[t4J]();this[K8x][i8u][k8x](p7u)[J8x](o9n)[P8x]();}if(!this[q1n][J7u][B8x]){var S8x=A2n;S8x+=C0n;S8x+=o0y;var p8x=S7u;p8x+=i7u;p8x+=t7u;var o8x=m5n;o8x+=o7n;o8x+=n1n;this[o8x][i8u][O4Z](p8x)[H7u]()[S8x]();}this[I7u]();this[i8x](t8x,this[q1n][J7u][y7u]?z7n:Y7n,o9n);this[j1u](H8x,g7n,this[R13.U7n][y8x]);this[j1u](j9x,g7n,this[R13.U7n][z9x]);this[z1u](F1u,[F9x,M1u],i18n[M9x]);this[I9Z][w9x][A9x](w1u,A1u)[g2n](s9x,function(){var T1u="tai";var I1u="isible";var Y1u=":v";var s1u=":disable";var Q9x=P4n;Q9x+=a1n;Q9x+=h1n;var C9x=l7Z;C9x+=x7Z;var Z9x=s1u;Z9x+=m5n;var d9x=m5n;d9x+=o7n;d9x+=n1n;var T9x=Y1u;T9x+=I1u;var I9x=Q7Z;I9x+=T1u;I9x+=m1n;I9x+=p7n;var Y9x=m5n;Y9x+=o7n;Y9x+=n1n;if(that[Y9x][I9x][U9Z](T9x)||that[d9x][r8Z][U9Z](Z9x)){return;}that[g9Z](that[I9Z][C9x][Q9x](),J2n);that[v4Z]();})[g2n](d1u,function(){var C1u="ible";var Z1u=":vis";var h9x=Z1u;h9x+=C1u;var O9x=F1n;O9x+=q1n;var f9x=K9Z;f9x+=j5Z;if(that[I9Z][f9x][O9x](h9x)){var V9x=F1n;V9x+=Q1u;that[g9Z](that[I9Z][V9x][g9Z](),J2n);}});this[I9Z][j7Z][g2n](V1J,v9x,function(){var o1u="setSeconds";var B1u="_setTi";var P1u="tput";var K1u="_setTime";var u1u="tUTCHour";var U1u='-ampm';var r1u='-hours';var n1u="rt";var c1u="iteOutput";var a1u="_w";var G1u="setUTCFullYea";var b1u='-year';var x1u="_correctMonth";var D1u="_setT";var X1u="nder";var E1u="_setCala";var e1u="asClas";var m1u="-m";var R1u="hours";var v1u="-a";var V1u="sClass";var h1u="nut";var O1u="-mi";var f1u="seconds";var K9x=S8u;K9x+=f1u;var N9x=O1u;N9x+=h1u;N9x+=t7n;N9x+=q1n;var U9x=P9Z;U9x+=a1n;U9x+=V1u;var l9x=v1u;l9x+=n1n;l9x+=a5n;l9x+=n1n;var D9x=P9Z;D9x+=a1n;D9x+=q1n;D9x+=g1u;var X9x=S8u;X9x+=R1u;var e9x=W1u;e9x+=N6y;e9x+=w7Z;var R9x=m1u;R9x+=L1u;var g9x=P9Z;g9x+=e1u;g9x+=q1n;var select=$(this);var val=select[g9Z]();if(select[g9x](classPrefix+R9x)){var L9x=E1u;L9x+=X1u;var m9x=D1u;m9x+=F8u;var W9x=F7Z;W9x+=l1u;that[x1u](that[q1n][W9x],val);that[m9x]();that[L9x]();}else if(select[e9x](classPrefix+b1u)){var E9x=G1u;E9x+=p7n;that[q1n][Q2Z][E9x](val);that[q1u]();that[d7u]();}else if(select[V9Z](classPrefix+X9x)||select[D9x](classPrefix+l9x)){var r9x=a1u;r9x+=p7n;r9x+=c1u;var x9x=a5n;x9x+=a1n;x9x+=n1u;x9x+=q1n;if(that[q1n][x9x][y7u]){var c9x=a5n;c9x+=n1n;var a9x=P4n;a9x+=a1n;a9x+=h1n;var q9x=O1Z;q9x+=p7n;var G9x=m5n;G9x+=o7n;G9x+=n1n;var b9x=l5n;b9x+=F1n;b9x+=y7n;b9x+=m5n;var hours=$(that[I9Z][j7Z])[b9x](M4J+classPrefix+r1u)[g9Z]()*o9n;var pm=$(that[G9x][q9x])[F4J](M4J+classPrefix+U1u)[a9x]()===c9x;that[q1n][m5n][N1u](hours===z7n&&!pm?B9n:pm&&hours!==z7n?hours+z7n:hours);}else{var n9x=q1n;n9x+=t7n;n9x+=u1u;n9x+=q1n;that[q1n][m5n][n9x](val);}that[K1u]();that[r9x](B2n);onChange();}else if(select[U9x](classPrefix+N9x)){var u9x=D1u;u9x+=F1n;u9x+=Z1n;that[q1n][m5n][k1u](val);that[u9x]();that[D7u](B2n);onChange();}else if(select[V9Z](classPrefix+K9x)){var J9x=J1u;J9x+=P1u;var k9x=B1u;k9x+=Z1n;that[q1n][m5n][o1u](val);that[k9x]();that[J9x](B2n);onChange();}that[I9Z][r8Z][R9Z]();that[p1u]();})[P9x](I4J,function(e){var x5u='year';var l5u="setUTCFullYear";var D5u="tUTC";var X5u="mon";var E5u="part";var L5u='-iconDown';var m5u="selectedIndex";var W5u="dex";var R5u="ctedIn";var g5u="hange";var v5u='-iconUp';var V5u="_correctMon";var h5u="TCMonth";var f5u="_setTit";var Q5u='-iconRight';var I5u="lander";var Y5u="Ca";var s5u="_se";var F5u="asCla";var z5u="sCl";var y1u="tar";var H1u="rCas";var t1u="toLowe";var i1u="ropagation";var S1u="stopP";var i9x=M5n;i9x+=x7Z;i9x+=R13.q7n;i9x+=g2n;var S9x=S1u;S9x+=i1u;var p9x=g6u;p9x+=t7n;p9x+=R13.U7n;p9x+=R13.q7n;var o9x=t1u;o9x+=H1u;o9x+=t7n;var B9x=y1u;B9x+=w7J;var nodeName=e[B9x][O0y][o9x]();if(nodeName===p9x){return;}e[S9x]();if(nodeName===i9x){var T7x=j5u;T7x+=z5u;T7x+=a1n;T7x+=u9Z;var H9x=P9Z;H9x+=F5u;H9x+=u9Z;var t9x=W1u;t9x+=g1u;var button=$(e[l3Z]);var parent=button[S7y]();var select;if(parent[t9x](M5u)){return;}if(parent[H9x](classPrefix+w5u)){var F7x=y6Z;F7x+=A5u;var z7x=s5u;z7x+=R13.q7n;z7x+=Y5u;z7x+=I5u;var j7x=m5n;j7x+=T5u;j7x+=V5n;var y9x=d5u;y9x+=N6y;y9x+=Z5u;y9x+=U2n;that[q1n][Q2Z][y9x](that[q1n][j7x][C5u]()-o9n);that[q1u]();that[z7x]();that[I9Z][F7x][R9Z]();}else if(parent[V9Z](classPrefix+Q5u)){var I7x=l7Z;I7x+=x7Z;var Y7x=m5n;Y7x+=T7Z;var s7x=f5u;s7x+=l3n;var A7x=O5u;A7x+=h5u;var w7x=m5n;w7x+=R1Z;var M7x=V5u;M7x+=U2n;that[M7x](that[q1n][w7x],that[q1n][Q2Z][A7x]()+o9n);that[s7x]();that[d7u]();that[Y7x][I7x][R9Z]();}else if(parent[T7x](classPrefix+v5u)){var Q7x=R13.U7n;Q7x+=g5u;var C7x=g6u;C7x+=t7n;C7x+=R5u;C7x+=W5u;var Z7x=h1n;Z7x+=t7n;Z7x+=r2n;Z7x+=U2n;var d7x=q1n;d7x+=t7n;d7x+=l3n;d7x+=K2n;select=parent[S7y]()[F4J](d7x)[B9n];select[m5u]=select[m5u]!==select[E1y][Z7x]-o9n?select[C7x]+o9n:B9n;$(select)[Q7x]();}else if(parent[V9Z](classPrefix+L5u)){var h7x=Y0n;h7x+=f6u;var O7x=l5n;O7x+=y6Z;O7x+=m5n;var f7x=L8y;f7x+=S0Z;select=parent[f7x]()[O7x](h7x)[B9n];select[m5u]=select[m5u]===B9n?select[E1y][k2n]-o9n:select[m5u]-o9n;$(select)[e5u]();}else{var R7x=E5u;R7x+=q1n;var g7x=J1u;g7x+=R13.q7n;g7x+=A5u;var v7x=X5u;v7x+=R13.q7n;v7x+=P9Z;var V7x=Y0n;V7x+=D5u;V7x+=c5n;V7x+=L1u;if(!that[q1n][m5n]){that[q1n][m5n]=that[l7u](new Date());}that[q1n][m5n][b7u](o9n);that[q1n][m5n][l5u](button[R8Z](x5u));that[q1n][m5n][V7x](button[R8Z](v7x));that[q1n][m5n][b7u](button[R8Z](b5u));that[g7x](B2n);if(!that[q1n][R7x][i8u]){setTimeout(function(){that[s7u]();},y9n);}else{that[d7u]();}onChange();}}else{var m7x=l5n;m7x+=o7n;m7x+=R13.U7n;m7x+=q7Z;var W7x=m5n;W7x+=o7n;W7x+=n1n;that[W7x][r8Z][m7x]();}});},_compareDates:function(a,b){var G5u="_dateToUtcString";return this[G5u](a)===this[G5u](b);},_correctMonth:function(date,month){var U5u="setUTCMonth";var r5u="getUTCDate";var c5u="InMon";var a5u="_day";var q5u="setUTCMo";var e7x=q5u;e7x+=Z5n;e7x+=P9Z;var L7x=a5u;L7x+=q1n;L7x+=c5u;L7x+=U2n;var days=this[L7x](date[n5u](),month);var correctDays=date[r5u]()>days;date[e7x](month);if(correctDays){var E7x=d5u;E7x+=N6y;E7x+=R7u;E7x+=d5n;date[E7x](days);date[U5u](month);}},_daysInMonth:function(year,month){var Q7n=31;var C7n=30;var Z7n=29;var d7n=28;var isLeap=year%i9n===B9n&&(year%W7n!==B9n||year%L7n===B9n);var months=[Q7n,isLeap?Z7n:d7n,Q7n,C7n,Q7n,C7n,Q7n,Q7n,C7n,Q7n,C7n,Q7n];return months[month];},_dateToUtc:function(s){var B5u="getMinutes";var P5u="getHours";var J5u="getDate";var u5u="tFull";var N5u="conds";var x7x=w7J;x7x+=i7n;x7x+=t7n;x7x+=N5u;var l7x=w7J;l7x+=Z5u;l7x+=U2n;var D7x=i1Z;D7x+=u5u;D7x+=g9u;D7x+=K5u;var X7x=k5u;X7x+=N6y;return new Date(Date[X7x](s[D7x](),s[l7x](),s[J5u](),s[P5u](),s[B5u](),s[x7x]()));},_dateToUtcString:function(d){var o5u="getUTCDa";var b7x=o5u;b7x+=d5n;return d[n5u]()+o4J+this[p5u](d[C5u]()+o9n)+o4J+this[p5u](d[b7x]());},_hide:function(){var i5u="keydown";var S5u="ck.";var U7x=R13.U7n;U7x+=h1n;U7x+=F1n;U7x+=S5u;var r7x=f0Z;r7x+=l5n;var n7x=o7n;n7x+=q6J;var c7x=i5u;c7x+=h2Z;var a7x=o7n;a7x+=l5n;a7x+=l5n;var q7x=R13.U7n;q7x+=y1Z;q7x+=F1n;q7x+=v7Z;var G7x=m5n;G7x+=o7n;G7x+=n1n;var namespace=this[q1n][P3J];this[G7x][q7x][B7Z]();$(window)[a7x](M4J+namespace);$(document)[a2Z](c7x+namespace);$(M0Z)[n7x](t5u+namespace);$(p9Z)[r7x](U7x+namespace);},_hours24To12:function(val){return val===B9n?z7n:val>z7n?val-z7n:val;},_htmlDay:function(day){var O4u='" data-day="';var f4u="month";var Q4u='" data-month="';var C4u='-day" type="button" ';var Z4u='<button class="';var d4u='" class="';var T4u="day";var I4u='<td data-day="';var Y4u="td>";var s4u="<td class=\"empty\"></";var A4u="sPrefix";var w4u="tod";var F4u="-b";var z4u="year=\"";var j4u="a-";var y5u="ye";var H5u="button>";var y7x=G6Z;y7x+=H5u;var H7x=m5n;H7x+=a1n;H7x+=V5n;var t7x=e2n;t7x+=O6Z;var i7x=y5u;i7x+=a1n;i7x+=p7n;var S7x=h8Z;S7x+=j4u;S7x+=z4u;var p7x=F4u;p7x+=x7Z;p7x+=I3J;p7x+=w2n;var o7x=z3n;o7x+=o7n;o7x+=y6Z;var B7x=q1n;B7x+=t7n;B7x+=l3n;B7x+=M4u;var k7x=w4u;k7x+=a1n;k7x+=V5n;var K7x=m5n;K7x+=Q9Z;K7x+=f9Z;K7x+=m5n;var u7x=R13.U7n;u7x+=g6Z;u7x+=A4u;if(day[w7u]){var N7x=s4u;N7x+=Y4u;return N7x;}var classes=[b5u];var classPrefix=this[R13.U7n][u7x];if(day[K7x]){classes[l2n](M5u);}if(day[k7x]){var P7x=R13.q7n;P7x+=c3n;P7x+=S3n;var J7x=a5n;J7x+=q7Z;J7x+=P9Z;classes[J7x](P7x);}if(day[B7x]){classes[l2n](A8u);}return I4u+day[T4u]+d4u+classes[o7x](l8Z)+q8Z+Z4u+classPrefix+p7x+classPrefix+C4u+S7x+day[i7x]+Q4u+day[f4u]+O4u+day[T4u]+t7x+day[H7x]+y7x+h4u;},_htmlMonth:function(year,month){var F3u='</table>';var z3u='</tbody>';var j3u="_htmlMonthHead";var y4u='<thead>';var H4u="tit";var t4u="onRight";var i4u="UTC";var S4u=" weekNumb";var p4u='</tr>';var o4u="_htmlWeekOfYea";var B4u="showWeekNumber";var P4u="_htmlDay";var J4u="getUTCDay";var k4u="disableDays";var K4u="_compareDates";var u4u="TC";var N4u="Dates";var U4u="mpare";var n4u="inArra";var c4u="etSeconds";var a4u="setSecon";var q4u="maxDate";var G4u="stDa";var b4u="_daysInMonth";var x4u="tUTCDay";var l4u="stD";var D4u="fix";var X4u="P";var E4u="class";var e4u="-tab";var L4u="class=";var m4u="le ";var W4u="<tab";var R4u="d>";var g4u="</t";var v4u="ody>";var v7n=59;var s7n=23;var b1x=V4u;b1x+=M5n;b1x+=v4u;var x1x=g4u;x1x+=j0Z;x1x+=a1n;x1x+=R4u;var l1x=W4u;l1x+=m4u;l1x+=L4u;l1x+=e2n;var O1x=e4u;O1x+=l3n;var f1x=E4u;f1x+=X4u;f1x+=A2n;f1x+=D4u;var F1x=h8u;F1x+=l4u;F1x+=S3n;var z1x=i1Z;z1x+=x4u;var j1x=E7u;j1x+=M8Z;j1x+=N6y;var now=this[l7u](new Date()),days=this[b4u](year,month),before=new Date(Date[j1x](year,month,o9n))[z1x](),data=[],row=[];if(this[R13.U7n][F1x]>B9n){var M1x=h8u;M1x+=G4u;M1x+=V5n;before-=this[R13.U7n][M1x];if(before<B9n){before+=t9n;}}var cells=days+before,after=cells;while(after>t9n){after-=t9n;}cells+=t9n-after;var minDate=this[R13.U7n][T7u];var maxDate=this[R13.U7n][q4u];if(minDate){var w1x=a4u;w1x+=e7Z;minDate[N1u](B9n);minDate[k1u](B9n);minDate[w1x](B9n);}if(maxDate){var A1x=q1n;A1x+=c4u;maxDate[N1u](s7n);maxDate[k1u](v7n);maxDate[A1x](v7n);}for(var i=B9n,r=B9n;i<cells;i++){var T1x=C1Z;T1x+=q1n;T1x+=P9Z;var I1x=n4u;I1x+=V5n;var Y1x=r4u;Y1x+=U4u;Y1x+=N4u;var s1x=E7u;s1x+=u4u;var day=new Date(Date[s1x](year,month,o9n+(i-before))),selected=this[q1n][m5n]?this[K4u](day,this[q1n][m5n]):J2n,today=this[Y1x](day,now),empty=i<before||i>=days+before,disabled=minDate&&day<minDate||maxDate&&day>maxDate;var disableDays=this[R13.U7n][k4u];if($[M8J](disableDays)&&$[I1x](day[J4u](),disableDays)!==-o9n){disabled=B2n;}else if(typeof disableDays===e9Z&&disableDays(day)===B2n){disabled=B2n;}var dayConfig={day:o9n+(i-before),month:month,year:year,selected:selected,today:today,disabled:disabled,empty:empty};row[T1x](this[P4u](dayConfig));if(++r===t9n){var Q1x=z3n;Q1x+=o7n;Q1x+=F1n;Q1x+=y7n;var C1x=V4u;C1x+=p7n;C1x+=O6Z;if(this[R13.U7n][B4u]){var Z1x=o4u;Z1x+=p7n;var d1x=U5Z;d1x+=Q1Z;d1x+=F1n;d1x+=M7J;row[d1x](this[Z1x](i-before,month,year));}data[l2n](C1x+row[Q1x](C2n)+p4u);row=[];r=B9n;}}var classPrefix=this[R13.U7n][f1x];var className=classPrefix+O1x;if(this[R13.U7n][B4u]){var h1x=S4u;h1x+=s3n;className+=h1x;}if(minDate){var R1x=y7n;R1x+=g2n;R1x+=t7n;var g1x=R13.U7n;g1x+=q1n;g1x+=q1n;var v1x=R13.q7n;v1x+=F8u;var V1x=m5n;V1x+=T7Z;var underMin=minDate>new Date(Date[i4u](year,month,o9n,B9n,B9n,B9n));this[V1x][v1x][F4J](w4J+classPrefix+w5u)[g1x](Y9Z,underMin?R1x:f2Z);}if(maxDate){var D1x=Z1J;D1x+=y7n;D1x+=t7n;var X1x=R13.U7n;X1x+=q1n;X1x+=q1n;var E1x=l9u;E1x+=R13.U7n;E1x+=t4u;var e1x=l5n;e1x+=F1n;e1x+=y7n;e1x+=m5n;var L1x=H4u;L1x+=l3n;var m1x=h1Z;m1x+=n1n;var W1x=k5u;W1x+=N6y;var overMax=maxDate<new Date(Date[W1x](year,month+o9n,o9n,B9n,B9n,B9n));this[m1x][L1x][e1x](w4J+classPrefix+E1x)[X1x](Y9Z,overMax?D1x:f2Z);}return l1x+className+q8Z+y4u+this[j3u]()+x1x+b1x+data[B4J](C2n)+z3u+F3u;},_htmlMonthHead:function(){var T3u='</th>';var I3u="th>";var Y3u="h></";var w3u="firs";var M3u="howWeekNumber";var q1x=q1n;q1x+=M3u;var G1x=w3u;G1x+=A3u;G1x+=S3n;var a=[];var firstDay=this[R13.U7n][G1x];var i18n=this[R13.U7n][Y8Z];var dayName=function(day){var s3u="weekdays";day+=firstDay;while(day>=t9n){day-=t9n;}return i18n[s3u][day];};if(this[R13.U7n][q1x]){var c1x=V4u;c1x+=Y3u;c1x+=I3u;var a1x=a5n;a1x+=r1n;a1x+=q1n;a1x+=P9Z;a[a1x](c1x);}for(var i=B9n;i<t9n;i++){var r1x=d6Z;r1x+=U2n;r1x+=O6Z;var n1x=a5n;n1x+=r1n;n1x+=q1n;n1x+=P9Z;a[n1x](r1x+dayName(i)+T3u);}return a[B4J](C2n);},_htmlWeekOfYear:function(d,m,y){var Q3u='-week">';var C3u='<td class="';var Z3u="ceil";var d3u="getDay";var l7n=86400000;var N1x=w7J;N1x+=T1n;var U1x=Y0n;U1x+=A3u;U1x+=Y6Z;var date=new Date(y,m,d,B9n,B9n,B9n,B9n);date[U1x](date[N1x]()+i9n-(date[d3u]()||t9n));var oneJan=new Date(y,B9n,o9n);var weekNum=Math[Z3u](((date-oneJan)/l7n+o9n)/t9n);return C3u+this[R13.U7n][k7u]+Q3u+weekNum+h4u;},_options:function(selector,values,labels){var R3u="ue=\"";var g3u="tion val";var V3u="ion>";var h3u="ct.";var O3u="Prefix";var J1x=t7n;J1x+=n1n;J1x+=f3u;var k1x=o6Z;k1x+=u9Z;k1x+=O3u;var K1x=Y0n;K1x+=l3n;K1x+=h3u;var u1x=l5n;u1x+=y6Z;u1x+=m5n;if(!labels){labels=values;}var select=this[I9Z][j7Z][u1x](K1x+this[R13.U7n][k1x]+o4J+selector);select[J1x]();for(var i=B9n,ien=values[k2n];i<ien;i++){var o1x=d6Z;o1x+=Z6Z;o1x+=r5Z;o1x+=V3u;var B1x=e2n;B1x+=O6Z;var P1x=v3u;P1x+=g3u;P1x+=R3u;select[h4Z](P1x+values[i]+B1x+labels[i]+o1x);}},_optionSet:function(selector,val){var X3u="unknown";var E3u='option:selected';var m3u="efi";var W3u="assPr";var H1x=F1n;H1x+=M1n;H1x+=w1n;H1x+=y7n;var t1x=R13.q7n;t1x+=t7n;t1x+=H7n;t1x+=R13.q7n;var i1x=s0n;i1x+=W3u;i1x+=m3u;i1x+=H7n;var S1x=s7Z;S1x+=L3u;S1x+=v7Z;var p1x=m5n;p1x+=T7Z;var select=this[p1x][S1x][F4J](e3u+this[R13.U7n][i1x]+o4J+selector);var span=select[S7y]()[O4Z](p7u);select[g9Z](val);var selected=select[F4J](E3u);span[o7Z](selected[k2n]!==B9n?selected[t1x]():this[R13.U7n][H1x][X3u]);},_optionsTime:function(select,count,inc){var q3u="lue=\"";var G3u="tion ";var b3u="/opti";var x3u="hoursAvailable";var l3u="refix";var D3u="classP";var j5x=C5n;j5x+=a5n;j5x+=a1n;j5x+=m5n;var y1x=D3u;y1x+=l3u;var classPrefix=this[R13.U7n][y1x];var sel=this[I9Z][j7Z][F4J](e3u+classPrefix+o4J+select);var start=B9n,end=count;var allowed;var render=count===z7n?function(i){return i;}:this[j5x];if(count===z7n){start=o9n;end=F7n;}if(count===z7n||count===Y7n){allowed=this[R13.U7n][x3u];}for(var i=start;i<end;i+=inc){if(!allowed||$[D8J](i,allowed)!==-o9n){var M5x=d6Z;M5x+=b3u;M5x+=o7n;M5x+=D9u;var F5x=e2n;F5x+=O6Z;var z5x=v3u;z5x+=G3u;z5x+=q1J;z5x+=q3u;sel[h4Z](z5x+i+F5x+render(i)+M5x);}}},_optionsTitle:function(year,month){var P3u="_range";var J3u="yearRange";var k3u="getFullYear";var u3u="rRan";var N3u="yea";var U3u="getFullY";var n3u="nge";var c3u="_ra";var a3u="onths";var Z5x=V5n;Z5x+=t7n;Z5x+=a1n;Z5x+=p7n;var d5x=n1n;d5x+=a3u;var T5x=c3u;T5x+=n3u;var I5x=n1n;I5x+=g2n;I5x+=R13.q7n;I5x+=P9Z;var Y5x=r3u;Y5x+=g8y;Y5x+=j9J;var s5x=U3u;s5x+=K5u;var A5x=N3u;A5x+=u3u;A5x+=i1Z;var w5x=K3u;w5x+=T1n;var classPrefix=this[R13.U7n][k7u];var i18n=this[R13.U7n][Y8Z];var min=this[R13.U7n][T7u];var max=this[R13.U7n][w5x];var minYear=min?min[k3u]():F9Z;var maxYear=max?max[k3u]():F9Z;var i=minYear!==F9Z?minYear:new Date()[k3u]()-this[R13.U7n][A5x];var j=maxYear!==F9Z?maxYear:new Date()[s5x]()+this[R13.U7n][J3u];this[Y5x](I5x,this[T5x](B9n,j7n),i18n[d5x]);this[z1u](Z5x,this[P3u](i,j));},_pad:function(i){var B3u='0';return i<y9n?B3u+i:i;},_position:function(){var t3u='top';var S3u="terHe";var p3u="Top";var o3u="croll";var v5x=R13.q7n;v5x+=o7n;v5x+=a5n;var V5x=q1n;V5x+=o3u;V5x+=p3u;var h5x=w3Z;h5x+=A3Z;var O5x=R13.U7n;O5x+=q1n;O5x+=q1n;var f5x=o3Z;f5x+=S3u;f5x+=z0Z;var Q5x=m5n;Q5x+=T7Z;var C5x=i3u;C5x+=t7n;C5x+=p7n;var offset=this[I9Z][r8Z][z6J]();var container=this[I9Z][C5x];var inputHeight=this[Q5x][r8Z][f5x]();container[O5x]({top:offset[t2Z]+inputHeight,left:offset[A7J]})[E0Z](h5x);var calHeight=container[X6J]();var calWidth=container[I7J]();var scrollTop=$(window)[V5x]();if(offset[v5x]+inputHeight+calHeight-scrollTop>$(window)[e6J]()){var g5x=R13.U7n;g5x+=q1n;g5x+=q1n;var newTop=offset[t2Z]-calHeight;container[g5x](t3u,newTop<B9n?B9n:newTop);}if(calWidth+offset[A7J]>$(window)[S2Z]()){var W5x=h1n;W5x+=t7n;W5x+=l5n;W5x+=R13.q7n;var R5x=R13.U7n;R5x+=q1n;R5x+=q1n;var newLeft=$(window)[S2Z]()-calWidth;container[R5x](W5x,newLeft<B9n?B9n:newLeft);}},_range:function(start,end){var a=[];for(var i=start;i<=end;i++){a[l2n](i);}return a;},_setCalander:function(){var M0u="dar";var F0u="emp";var j0u="mlMo";var y3u="FullYear";var H3u="getUTC";var m5x=m5n;m5x+=T5u;m5x+=V5n;if(this[q1n][m5x]){var x5x=G1n;x5x+=J2Z;var l5x=H3u;l5x+=y3u;var D5x=C5n;D5x+=Z3Z;D5x+=j0u;D5x+=z0u;var X5x=Z4Z;X5x+=F0n;var E5x=F0u;E5x+=M3n;var e5x=t8u;e5x+=y7n;e5x+=M0u;var L5x=m5n;L5x+=o7n;L5x+=n1n;this[L5x][e5x][E5x]()[X5x](this[D5x](this[q1n][Q2Z][l5x](),this[q1n][x5x][C5u]()));}},_setTitle:function(){var Y0u='month';var A0u="etUTCMonth";var w0u="TCFullY";var a5x=O5u;a5x+=w0u;a5x+=K5u;var q5x=G1n;q5x+=J7J;q5x+=S1Z;var G5x=V5n;G5x+=t7n;G5x+=a1n;G5x+=p7n;var b5x=r0n;b5x+=A0u;this[s0u](Y0u,this[q1n][Q2Z][b5x]());this[s0u](G5x,this[q1n][q5x][a5x]());},_setTime:function(){var R0u="getSeconds";var g0u="getUTCMinutes";var v0u='minutes';var V0u='hours';var h0u="nS";var O0u="24To12";var f0u="ours";var C0u="getUTCHours";var Z0u="rts";var d0u="ionSe";var T0u="_o";var I0u="onds";var K5x=q1n;K5x+=R13.G7n;K5x+=I0u;var u5x=T0u;u5x+=I1J;u5x+=d0u;u5x+=R13.q7n;var c5x=Z5Z;c5x+=Z0u;var d=this[q1n][m5n];var hours=d?d[C0u]():B9n;if(this[q1n][c5x][y7u]){var U5x=a1n;U5x+=n1n;var r5x=Q0u;r5x+=f0u;r5x+=O0u;var n5x=r3u;n5x+=g8y;n5x+=h0u;n5x+=G3n;this[n5x](V0u,this[r5x](hours));this[s0u](F1u,hours<z7n?U5x:M1u);}else{var N5x=P9Z;N5x+=f0u;this[s0u](N5x,hours);}this[s0u](v0u,d?d[g0u]():B9n);this[u5x](K5x,d?d[R0u]():B9n);},_show:function(){var e0u=' resize.';var L0u="TE_Body_Con";var m0u="n.";var W0u="dow";var o5x=V7J;o5x+=W0u;o5x+=m0u;var P5x=o7n;P5x+=y7n;var J5x=v3Z;J5x+=L0u;J5x+=C4Z;var k5x=o7n;k5x+=y7n;var that=this;var namespace=this[q1n][P3J];this[p1u]();$(window)[k5x](t5u+namespace+e0u+namespace,function(){that[p1u]();});$(J5x)[P5x](t5u+namespace,function(){var E0u="posit";var B5x=C5n;B5x+=E0u;B5x+=c9Z;that[B5x]();});$(document)[g2n](o5x+namespace,function(e){var H9n=9;var p5x=V7J;p5x+=N6y;p5x+=o7n;p5x+=K4n;if(e[l7J]===H9n||e[p5x]===T7n||e[l7J]===F7n){that[s7u]();}});setTimeout(function(){var X0u='click.';var S5x=o7n;S5x+=y7n;$(p9Z)[S5x](X0u+namespace,function(e){var x0u="ide";var l0u="ilter";var D0u="engt";var j4x=h1n;j4x+=D0u;j4x+=P9Z;var y5x=i3u;y5x+=s3n;var H5x=m5n;H5x+=o7n;H5x+=n1n;var t5x=l5n;t5x+=l0u;var i5x=m8J;i5x+=n1J;i5x+=R13.q7n;var parents=$(e[i5x])[S1y]();if(!parents[t5x](that[H5x][y5x])[j4x]&&e[l3Z]!==that[I9Z][r8Z][B9n]){var z4x=Q0u;z4x+=x0u;that[z4x]();}});},y9n);},_writeOutput:function(focus){var u0u="ocale";var N0u="mentL";var U0u="Strict";var r0u="momen";var n0u="rma";var c0u="lYear";var a0u="getUTCF";var q0u="getUTCM";var G0u="getUTCD";var C4x=P4n;C4x+=a1n;C4x+=h1n;var Z4x=b0u;Z4x+=R13.q7n;var d4x=h1Z;d4x+=n1n;var T4x=G0u;T4x+=Y6Z;var I4x=q0u;I4x+=o7n;I4x+=z0u;var Y4x=C5n;Y4x+=a5n;Y4x+=A3n;var s4x=a0u;s4x+=L9u;s4x+=c0u;var A4x=B1n;A4x+=n0u;A4x+=R13.q7n;var w4x=r0u;w4x+=R13.q7n;w4x+=U0u;var M4x=n1n;M4x+=o7n;M4x+=N0u;M4x+=u0u;var F4x=r1n;F4x+=R13.q7n;F4x+=R13.U7n;var date=this[q1n][m5n];var out=window[e9u]?window[e9u][F4x](date,undefined,this[R13.U7n][M4x],this[R13.U7n][w4x])[E9u](this[R13.U7n][A4x]):date[s4x]()+o4J+this[Y4x](date[I4x]()+o9n)+o4J+this[p5u](date[T4x]());this[d4x][Z4x][C4x](out);if(focus){this[I9Z][r8Z][R9Z]();}}});Editor[X8u][K0u]=B9n;Editor[Q4x][O3y]={classPrefix:k0u,disableDays:F9Z,firstDay:o9n,format:J0u,hoursAvailable:F9Z,i18n:Editor[f4x][O4x][h4x],maxDate:F9Z,minDate:F9Z,minutesIncrement:o9n,momentStrict:B2n,momentLocale:V4x,onChange:function(){},secondsIncrement:o9n,showWeekNumber:J2n,yearRange:y9n};(function(){var g73="uploadMany";var A73="_va";var w73="_val";var y93="_closeFn";var t93="_picker";var o93="datetime";var K93="icker";var n93="datep";var v93="cker";var T93="_preChecked";var o83="radio";var B83="Options";var P83="_ad";var k83="checked";var W83='_';var R83='<input id="';var O83="rs";var Q83="checkbox";var w83="separator";var M83="multiple";var p63='change.dte';var K63="_editor_val";var q63="isabled";var E63="textarea";var g63='text';var h63="_v";var O63="prop";var f63="disab";var Z63="oa";var H2u="_enabled";var O2u="n cl";var w2u="_i";var H0u="_input";var S0u="ldT";var p0u="sword";var o0u="pas";var B0u="exte";var P0u="xtend";var n8n=t7n;n8n+=P0u;var C8n=W5n;C8n+=m5n;var Z8n=r1n;Z8n+=e0J;Z8n+=a1n;Z8n+=m5n;var k6n=t7n;k6n+=H7n;k6n+=R13.q7n;k6n+=C1J;var X2x=B0u;X2x+=y7n;X2x+=m5n;var K0x=o5n;K0x+=C1J;var s0x=T6u;s0x+=R13.U7n;s0x+=R13.q7n;var t3x=t7n;t3x+=e7J;t3x+=J7Z;t3x+=m5n;var i3x=o0u;i3x+=p0u;var J3x=o5n;J3x+=C1J;var N3x=l0J;N3x+=g2n;N3x+=L6y;var c3x=P9Z;c3x+=k3n;c3x+=m5n;c3x+=J7Z;var l3x=B0u;l3x+=F0n;var v4x=h2J;v4x+=S0u;v4x+=w8Z;var fieldTypes=Editor[v4x];function _buttonText(conf,text){var y0u='div.upload button';var t0u="Choose file...";var i0u="uploadText";var g4x=u2Z;g4x+=m5n;if(text===F9Z||text===undefined){text=conf[i0u]||t0u;}conf[H0u][g4x](y0u)[o7Z](text);}function _commonUpload(editor,conf,dropCallback,multiple){var I63="dCla";var Y63="noDro";var s63="v.render";var z63='open';var t2u='over';var o2u="les";var P2u='drop';var J2u='div.drop';var k2u="dragDropText";var K2u="div.drop s";var u2u="rag and drop a file here to upload";var N2u="agexit";var U2u="e dr";var r2u="gleav";var n2u="dra";var c2u="agove";var a2u="dr";var q2u="dragDrop";var G2u="FileReader";var b2u='<div class="row second">';var x2u='<div class="cell clearValue">';var l2u='multiple';var D2u='<div class="editor_upload">';var X2u="ss=\"eu_table\">";var E2u="w\">";var e2u="=\"ro";var L2u="iv class";var m2u="<d";var W2u="de\">";var R2u="<div class=\"cell upload limitHi";var g2u="<button cla";var v2u="e=\"file\" ";var V2u="put t";var h2u="<in";var f2u="tto";var Q2u="<bu";var C2u="ide\">";var Z2u="class=\"cell limitH";var d2u="<span/></div>";var T2u="<div class=\"drop\">";var I2u="ll\">";var Y2u="<div class=\"c";var s2u="s=\"rendered\"/>";var A2u="/di";var M2u="able";var F2u="clearValue button";var z2u="ype=file]";var j2u="input[t";var W3x=D5n;W3x+=y7n;W3x+=r0n;W3x+=t7n;var R3x=j2u;R3x+=z2u;var O3x=s0n;O3x+=o2n;O3x+=G0n;var f3x=o7n;f3x+=y7n;var Q3x=f6J;Q3x+=F2u;var C3x=u2Z;C3x+=m5n;var J4x=C5n;J4x+=J7Z;J4x+=M2u;J4x+=m5n;var k4x=w2u;k4x+=M7u;k4x+=R13.q7n;var K4x=G6Z;K4x+=C6Z;var u4x=d6Z;u4x+=A2u;u4x+=P4n;u4x+=O6Z;var N4x=G6Z;N4x+=s9J;N4x+=O6Z;var U4x=i6Z;U4x+=s2u;var r4x=Y2u;r4x+=t7n;r4x+=I2u;var n4x=s9u;n4x+=O6Z;var c4x=T2u;c4x+=d2u;var a4x=a9u;a4x+=Z2u;a4x+=C2u;var q4x=Q6Z;q4x+=f6Z;q4x+=O6Z;var G4x=Q2u;G4x+=f2u;G4x+=O2u;G4x+=B6Z;var b4x=Q6Z;b4x+=q6Z;var x4x=Z6Z;x4x+=O6Z;var l4x=h2u;l4x+=V2u;l4x+=r4n;l4x+=v2u;var D4x=e2n;D4x+=w2n;D4x+=Z6Z;D4x+=O6Z;var X4x=g2u;X4x+=P2J;var E4x=R2u;E4x+=W2u;var e4x=m2u;e4x+=L2u;e4x+=e2u;e4x+=E2u;var L4x=F4Z;L4x+=X2u;var m4x=y8J;m4x+=g2n;var W4x=l5n;W4x+=i1n;W4x+=n1n;var R4x=i9Z;R4x+=q1n;var btnClass=editor[R4x][W4x][m4x];var container=$(D2u+L4x+e4x+E4x+X4x+btnClass+D4x+l4x+(multiple?l2u:C2n)+x4x+b4x+x2u+G4x+btnClass+e9J+q4x+c8Z+b2u+a4x+c4x+n4x+r4x+U4x+N4x+u4x+K4x+c8Z);conf[k4x]=container;conf[J4x]=B2n;_buttonText(conf);if(window[G2u]&&conf[q2u]!==J2n){var s3x=o7n;s3x+=y7n;var M3x=a2u;M3x+=c2u;M3x+=p7n;var F3x=o7n;F3x+=y7n;var y4x=n2u;y4x+=r2u;y4x+=U2u;y4x+=N2u;var p4x=z8Z;p4x+=y7n;p4x+=m5n;var o4x=R1n;o4x+=u2u;var B4x=K2u;B4x+=Z5Z;B4x+=y7n;var P4x=l5n;P4x+=x6J;container[P4x](B4x)[k5Z](conf[k2u]||o4x);var dragDrop=container[p4x](J2u);dragDrop[g2n](P2u,function(e){var i2u="dataTransfer";var S2u="nalEvent";var p2u="rigi";var B2u="nable";var S4x=u8y;S4x+=B2u;S4x+=m5n;if(conf[S4x]){var H4x=l5n;H4x+=F1n;H4x+=o2u;var t4x=o7n;t4x+=p2u;t4x+=S2u;var i4x=G0J;i4x+=m5n;Editor[i4x](editor,conf,e[t4x][i2u][H4x],_buttonText,dropCallback);dragDrop[g7Z](t2u);}return J2n;})[g2n](y4x,function(e){var y2u="moveCl";if(conf[H2u]){var z3x=o7n;z3x+=P4n;z3x+=t7n;z3x+=p7n;var j3x=p7n;j3x+=t7n;j3x+=y2u;j3x+=y9Z;dragDrop[j3x](z3x);}return J2n;})[F3x](M3x,function(e){var j63="Cla";if(conf[H2u]){var w3x=a1n;w3x+=A8J;w3x+=j63;w3x+=u9Z;dragDrop[w3x](t2u);}return J2n;});editor[g2n](z63,function(){var M63="E_Upload";var F63="dragover.DTE_Upload drop.DT";var A3x=F63;A3x+=M63;$(p9Z)[g2n](A3x,function(e){return J2n;});})[s3x](p5Z,function(){var A63="Upload drop.DTE_Upload";var w63="dragover.DT";var Y3x=w63;Y3x+=w5n;Y3x+=A63;$(p9Z)[a2Z](Y3x);});}else{var Z3x=G1n;Z3x+=s63;Z3x+=A1J;var d3x=l5n;d3x+=F1n;d3x+=y7n;d3x+=m5n;var T3x=Y63;T3x+=a5n;var I3x=A3n;I3x+=I63;I3x+=u9Z;container[I3x](T3x);container[h4Z](container[d3x](Z3x));}container[C3x](Q3x)[f3x](O3x,function(){var d63="dTyp";var T63="load";var g3x=R13.U7n;g3x+=a1n;g3x+=h1n;g3x+=h1n;var v3x=q1n;v3x+=t7n;v3x+=R13.q7n;var V3x=v7J;V3x+=T63;var h3x=Q5J;h3x+=d63;h3x+=H5n;Editor[h3x][V3x][v3x][g3x](editor,conf,C2n);});container[F4J](R3x)[g2n](W3x,function(){var L3x=z8Z;L3x+=o2u;var m3x=v7J;m3x+=h1n;m3x+=Z63;m3x+=m5n;Editor[m3x](editor,conf,this[L3x],_buttonText,function(ids){var C63='input[type=file]';var E3x=P4n;E3x+=a1n;E3x+=h1n;var e3x=l5n;e3x+=F1n;e3x+=y7n;e3x+=m5n;dropCallback[l9Z](editor,ids);container[e3x](C63)[E3x](C2n);});});return container;}function _triggerChange(input){setTimeout(function(){var Q63="rigg";var D3x=R13.U7n;D3x+=j5u;D3x+=y7n;D3x+=i1Z;var X3x=R13.q7n;X3x+=Q63;X3x+=s3n;input[X3x](D3x,{editor:B2n,editorSet:B2n});},B9n);}var baseFieldType=$[l3x](B2n,{},Editor[T9Z][P5Z],{get:function(conf){var x3x=q1J;x3x+=h1n;return conf[H0u][x3x]();},set:function(conf,val){var b3x=q1J;b3x+=h1n;conf[H0u][b3x](val);_triggerChange(conf[H0u]);},enable:function(conf){var q3x=f63;q3x+=l3n;q3x+=m5n;var G3x=a8u;G3x+=A5u;conf[G3x][O63](q3x,J2n);},disable:function(conf){var a3x=f63;a3x+=l3n;a3x+=m5n;conf[H0u][O63](a3x,B2n);},canReturnSubmit:function(conf,node){return B2n;}});fieldTypes[c3x]={create:function(conf){var V63="value";var n3x=h63;n3x+=a1n;n3x+=h1n;conf[n3x]=conf[V63];return F9Z;},get:function(conf){var r3x=h63;r3x+=a1n;r3x+=h1n;return conf[r3x];},set:function(conf,val){var U3x=C5n;U3x+=P4n;U3x+=A0n;conf[U3x]=val;}};fieldTypes[N3x]=$[C8Z](B2n,{},baseFieldType,{create:function(conf){var v63='<input/>';var k3x=a1n;k3x+=R13.q7n;k3x+=R13.q7n;k3x+=p7n;var K3x=F1n;K3x+=m5n;var u3x=C5n;u3x+=y6Z;u3x+=a5n;u3x+=x7Z;conf[u3x]=$(v63)[j0J]($[C8Z]({id:Editor[z0J](conf[K3x]),type:g63,readonly:v9Z},conf[k3x]||{}));return conf[H0u][B9n];}});fieldTypes[k5Z]=$[J3x](B2n,{},baseFieldType,{create:function(conf){var R63="put/";var S3x=F1n;S3x+=m5n;var p3x=t7n;p3x+=e7J;p3x+=t7n;p3x+=F0n;var o3x=W0Z;o3x+=O0n;var B3x=d6Z;B3x+=y6Z;B3x+=R63;B3x+=O6Z;var P3x=w2u;P3x+=y7n;P3x+=a5n;P3x+=x7Z;conf[P3x]=$(B3x)[o3x]($[p3x]({id:Editor[z0J](conf[S3x]),type:g63},conf[j0J]||{}));return conf[H0u][B9n];}});fieldTypes[i3x]=$[t3x](B2n,{},baseFieldType,{create:function(conf){var e63='password';var L63="nput/>";var m63="<i";var W63="ttr";var j0x=C5n;j0x+=y6Z;j0x+=A5u;var y3x=a1n;y3x+=W63;var H3x=m63;H3x+=L63;conf[H0u]=$(H3x)[y3x]($[C8Z]({id:Editor[z0J](conf[k3n]),type:e63},conf[j0J]||{}));return conf[j0x][B9n];}});fieldTypes[E63]=$[C8Z](B2n,{},baseFieldType,{create:function(conf){var X63="<textarea";var A0x=C5n;A0x+=F1n;A0x+=y7n;A0x+=A5u;var w0x=q5J;w0x+=p7n;var M0x=F1n;M0x+=m5n;var F0x=W5n;F0x+=m5n;var z0x=X63;z0x+=Z6Z;z0x+=O6Z;conf[H0u]=$(z0x)[j0J]($[F0x]({id:Editor[z0J](conf[M0x])},conf[w0x]||{}));return conf[A0x][B9n];},canReturnSubmit:function(conf,node){return J2n;}});fieldTypes[s0x]=$[C8Z](B2n,{},baseFieldType,{_addOptions:function(conf,opts,append){var u63="optionsPair";var N63="hidden";var U63="placeholderDisabled";var r63="lderValue";var n63="aceh";var c63="ceholderValu";var a63="hol";var G63="holderD";var b63="or_v";var x63="holder";var l63="plac";var D63="pti";var Y0x=o7n;Y0x+=D63;Y0x+=g2n;Y0x+=q1n;var elOpts=conf[H0u][B9n][Y0x];var countOffset=B9n;if(!append){var I0x=l63;I0x+=t7n;I0x+=x63;elOpts[k2n]=B9n;if(conf[I0x]!==undefined){var Q0x=u8y;Q0x+=b6u;Q0x+=b63;Q0x+=A0n;var C0x=n4J;C0x+=k0n;C0x+=G63;C0x+=q63;var Z0x=E1Z;Z0x+=O8u;Z0x+=a63;Z0x+=i3Z;var d0x=n4J;d0x+=c63;d0x+=t7n;var T0x=E1Z;T0x+=n63;T0x+=o7n;T0x+=r63;var placeholderValue=conf[T0x]!==undefined?conf[d0x]:C2n;countOffset+=o9n;elOpts[B9n]=new Option(conf[Z0x],placeholderValue);var disabled=conf[C0x]!==undefined?conf[U63]:B2n;elOpts[B9n][N63]=disabled;elOpts[B9n][z7Z]=disabled;elOpts[B9n][Q0x]=placeholderValue;}}else{countOffset=elOpts[k2n];}if(opts){Editor[H3J](opts,conf[u63],function(val,label,i,attr){var option=new Option(label,val);option[K63]=val;if(attr){var f0x=a1n;f0x+=R13.q7n;f0x+=R13.q7n;f0x+=p7n;$(option)[f0x](attr);}elOpts[i+countOffset]=option;});}},create:function(conf){var H63="ipOpts";var t63="_addOptions";var o63='<select/>';var B63="feId";var P63="sa";var J63="tip";var k63="ele";var W0x=x5Z;W0x+=D3n;W0x+=X5n;var R0x=q1n;R0x+=k63;R0x+=R13.U7n;R0x+=R13.q7n;var v0x=n1n;v0x+=L9u;v0x+=J63;v0x+=l3n;var V0x=F1n;V0x+=m5n;var h0x=P63;h0x+=B63;var O0x=a1n;O0x+=R13.q7n;O0x+=R13.q7n;O0x+=p7n;conf[H0u]=$(o63)[O0x]($[C8Z]({id:Editor[h0x](conf[V0x]),multiple:conf[v0x]===B2n},conf[j0J]||{}))[g2n](p63,function(e,d){var i63="Set";var S63="_last";if(!d||!d[e3J]){var g0x=S63;g0x+=i63;conf[g0x]=fieldTypes[n6u][w7J](conf);}});fieldTypes[R0x][t63](conf,conf[W0x]||conf[H63]);return conf[H0u][B9n];},update:function(conf,options,append){var j83="_lastSet";var y63="addOp";var m0x=C5n;m0x+=y63;m0x+=K8J;fieldTypes[n6u][m0x](conf,options,append);var lastSet=conf[j83];if(lastSet!==undefined){var L0x=Y0n;L0x+=R13.q7n;fieldTypes[n6u][L0x](conf,lastSet,B2n);}_triggerChange(conf[H0u]);},get:function(conf){var F83="toArray";var z83="on:se";var E0x=a6y;E0x+=z83;E0x+=l3n;E0x+=M4u;var e0x=a8u;e0x+=A5u;var val=conf[e0x][F4J](E0x)[F5J](function(){return this[K63];})[F83]();if(conf[M83]){var X0x=z3n;X0x+=o7n;X0x+=F1n;X0x+=y7n;return conf[w83]?val[X0x](conf[w83]):val;}return val[k2n]?val[B9n]:F9Z;},set:function(conf,val,localUpdate){var d83='option';var T83="tSet";var I83="_las";var Y83="isArra";var s83="lder";var A83="ple";var N0x=I8Z;N0x+=A83;var U0x=E1Z;U0x+=O8u;U0x+=K5Z;U0x+=s83;var n0x=L1J;n0x+=R13.U7n;n0x+=P9Z;var c0x=C5n;c0x+=b0u;c0x+=R13.q7n;var a0x=z8Z;a0x+=y7n;a0x+=m5n;var q0x=C5n;q0x+=y6Z;q0x+=A5u;var G0x=l3n;G0x+=Q1J;var b0x=Y83;b0x+=V5n;var l0x=l0y;l0x+=S3n;if(!localUpdate){var D0x=I83;D0x+=T83;conf[D0x]=val;}if(conf[M83]&&conf[w83]&&!$[l0x](val)){var x0x=o1Z;x0x+=p7n;x0x+=F1n;x0x+=r2n;val=typeof val===x0x?val[T8y](conf[w83]):[];}else if(!$[b0x](val)){val=[val];}var i,len=val[G0x],found,allFound=J2n;var options=conf[q0x][a0x](d83);conf[c0x][F4J](d83)[n0x](function(){var C83="ted";var Z83="lec";var r0x=Y0n;r0x+=Z83;r0x+=C83;found=J2n;for(i=B9n;i<len;i++){if(this[K63]==val[i]){found=B2n;allFound=B2n;break;}}this[r0x]=found;});if(conf[U0x]&&!allFound&&!conf[N0x]&&options[k2n]){var u0x=n6u;u0x+=t7n;u0x+=m5n;options[B9n][u0x]=B2n;}if(!localUpdate){_triggerChange(conf[H0u]);}return allFound;},destroy:function(conf){conf[H0u][a2Z](p63);}});fieldTypes[Q83]=$[K0x](B2n,{},baseFieldType,{_addOptions:function(conf,opts,append){var f83="optionsP";var k0x=C5n;k0x+=b0u;k0x+=R13.q7n;var val,label;var jqInput=conf[k0x];var offset=B9n;if(!append){var J0x=t7n;J0x+=f8y;J0x+=R13.q7n;J0x+=V5n;jqInput[J0x]();}else{var P0x=h1n;P0x+=J7Z;P0x+=B9J;offset=$(b7Z,jqInput)[P0x];}if(opts){var o0x=f83;o0x+=L3u;o0x+=p7n;var B0x=a5n;B0x+=a1n;B0x+=F1n;B0x+=O83;Editor[B0x](opts,conf[o0x],function(val,label,i,attr){var L83='input:last';var m83='" type="checkbox" />';var g83='<div>';var v83="abel for=\"";var V83="bel>";var h83="</l";var i0x=a1n;i0x+=j7J;i0x+=p7n;var S0x=h83;S0x+=a1n;S0x+=V83;var p0x=d6Z;p0x+=h1n;p0x+=v83;jqInput[h4Z](g83+R83+Editor[z0J](conf[k3n])+W83+(i+offset)+m83+p0x+Editor[z0J](conf[k3n])+W83+(i+offset)+q8Z+label+S0x+c8Z);$(L83,jqInput)[i0x](y3J,val)[B9n][K63]=val;if(attr){$(L83,jqInput)[j0J](attr);}});}},create:function(conf){var l83="ox";var D83="kb";var X83="chec";var E83="ddOp";var e83="O";var F2x=C5n;F2x+=y6Z;F2x+=A5u;var z2x=F1n;z2x+=a5n;z2x+=e83;z2x+=T4y;var j2x=Z5y;j2x+=E83;j2x+=K8J;var y0x=X83;y0x+=D83;y0x+=l83;var H0x=U0Z;H0x+=y8u;H0x+=z9u;var t0x=a8u;t0x+=A5u;conf[t0x]=$(H0x);fieldTypes[y0x][j2x](conf,conf[E1y]||conf[z2x]);return conf[F2x][B9n];},get:function(conf){var U83="electedValue";var c83='input:checked';var a83="dValue";var q83="lecte";var G83="unse";var b83="eparato";var x83="ato";var C2x=Y0n;C2x+=L8y;C2x+=x83;C2x+=p7n;var Z2x=z3n;Z2x+=o7n;Z2x+=y6Z;var d2x=q1n;d2x+=b83;d2x+=p7n;var Y2x=G83;Y2x+=q83;Y2x+=a83;var M2x=l3n;M2x+=r2n;M2x+=U2n;var out=[];var selected=conf[H0u][F4J](c83);if(selected[M2x]){var w2x=t7n;w2x+=a1n;w2x+=L5n;selected[w2x](function(){var r83="pus";var n83="_editor_va";var s2x=n83;s2x+=h1n;var A2x=r83;A2x+=P9Z;out[A2x](this[s2x]);});}else if(conf[Y2x]!==undefined){var T2x=r1n;T2x+=j9J;T2x+=U83;var I2x=a5n;I2x+=r1n;I2x+=q1n;I2x+=P9Z;out[I2x](conf[T2x]);}return conf[w83]===undefined||conf[d2x]===F9Z?out:out[Z2x](conf[C2x]);},set:function(conf,val){var K83='|';var u83="plit";var N83="Array";var V2x=h1n;V2x+=w8J;V2x+=R13.q7n;V2x+=P9Z;var O2x=U9Z;O2x+=N83;var f2x=F1n;f2x+=Q1u;var Q2x=z8Z;Q2x+=F0n;var jqInputs=conf[H0u][Q2x](f2x);if(!$[O2x](val)&&typeof val===r7J){var h2x=q1n;h2x+=u83;val=val[h2x](conf[w83]||K83);}else if(!$[M8J](val)){val=[val];}var i,len=val[V2x],found;jqInputs[D2n](function(){found=J2n;for(i=B9n;i<len;i++){if(this[K63]==val[i]){found=B2n;break;}}this[k83]=found;});_triggerChange(jqInputs);},enable:function(conf){var g2x=a5n;g2x+=p7n;g2x+=o7n;g2x+=a5n;var v2x=w2u;v2x+=M7u;v2x+=R13.q7n;conf[v2x][F4J](b7Z)[g2x](M5u,J2n);},disable:function(conf){var J83="isable";var L2x=m5n;L2x+=J83;L2x+=m5n;var m2x=a5n;m2x+=p7n;m2x+=o7n;m2x+=a5n;var W2x=y6Z;W2x+=a5n;W2x+=x7Z;var R2x=l5n;R2x+=y6Z;R2x+=m5n;conf[H0u][R2x](W2x)[m2x](L2x,B2n);},update:function(conf,options,append){var E2x=Y0n;E2x+=R13.q7n;var e2x=P83;e2x+=m5n;e2x+=B83;var checkbox=fieldTypes[Q83];var currVal=checkbox[w7J](conf);checkbox[e2x](conf,options,append);checkbox[E2x](conf,currVal);}});fieldTypes[o83]=$[X2x](B2n,{},baseFieldType,{_addOptions:function(conf,opts,append){var i83="air";var S83="sP";var p83="option";var val,label;var jqInput=conf[H0u];var offset=B9n;if(!append){jqInput[w7u]();}else{offset=$(b7Z,jqInput)[k2n];}if(opts){var l2x=p83;l2x+=S83;l2x+=i83;var D2x=a5n;D2x+=L3u;D2x+=O83;Editor[D2x](opts,conf[l2x],function(val,label,i,attr){var M93="ast";var F93="t:l";var z93='<label for="';var j93="afeI";var y83="me=\"";var H83="\" type=\"radio\" na";var t83="t:last";var r2x=b0u;r2x+=t83;var n2x=s9u;n2x+=O6Z;var c2x=e2n;c2x+=O6Z;var a2x=e2n;a2x+=w2n;a2x+=Z6Z;a2x+=O6Z;var q2x=H83;q2x+=y83;var G2x=F1n;G2x+=m5n;var b2x=q1n;b2x+=j93;b2x+=m5n;var x2x=U0Z;x2x+=m6Z;jqInput[h4Z](x2x+R83+Editor[b2x](conf[G2x])+W83+(i+offset)+q2x+conf[Q8Z]+a2x+z93+Editor[z0J](conf[k3n])+W83+(i+offset)+c2x+label+n8Z+n2x);$(r2x,jqInput)[j0J](y3J,val)[B9n][K63]=val;if(attr){var U2x=b0u;U2x+=F93;U2x+=M93;$(U2x,jqInput)[j0J](attr);}});}},create:function(conf){var I93=" /";var Y93="radi";var s93="_addOpti";var A93="Opt";var w93="ip";var P2x=o7n;P2x+=a4n;P2x+=y7n;var J2x=o7n;J2x+=y7n;var k2x=w93;k2x+=A93;k2x+=q1n;var K2x=s93;K2x+=g2n;K2x+=q1n;var u2x=Y93;u2x+=o7n;var N2x=d6Z;N2x+=s9J;N2x+=I93;N2x+=O6Z;conf[H0u]=$(N2x);fieldTypes[u2x][K2x](conf,conf[E1y]||conf[k2x]);this[J2x](P2x,function(){conf[H0u][F4J](b7Z)[D2n](function(){if(this[T93]){this[k83]=B2n;}});});return conf[H0u][B9n];},get:function(conf){var d93="input:chec";var B2x=d93;B2x+=I7y;B2x+=m5n;var el=conf[H0u][F4J](B2x);return el[k2n]?el[B9n][K63]:undefined;},set:function(conf,val){var Z93="input:check";var t2x=Z93;t2x+=A1J;var i2x=z8Z;i2x+=F0n;var o2x=z8Z;o2x+=F0n;var that=this;conf[H0u][o2x](b7Z)[D2n](function(){var f93="preChecked";var Q93="ked";var C93="_preC";this[T93]=J2n;if(this[K63]==val){var p2x=C93;p2x+=b0n;p2x+=Q93;this[k83]=B2n;this[p2x]=B2n;}else{var S2x=C5n;S2x+=f93;this[k83]=J2n;this[S2x]=J2n;}});_triggerChange(conf[H0u][i2x](t2x));},enable:function(conf){var z6n=F7Z;z6n+=a1n;z6n+=c1n;var j6n=R4n;j6n+=x5Z;var y2x=F1n;y2x+=y7n;y2x+=C1Z;y2x+=R13.q7n;var H2x=C5n;H2x+=F1n;H2x+=y7n;H2x+=A5u;conf[H2x][F4J](y2x)[j6n](z6n,J2n);},disable:function(conf){var w6n=b0u;w6n+=R13.q7n;var M6n=l5n;M6n+=F1n;M6n+=F0n;var F6n=a8u;F6n+=A5u;conf[F6n][M6n](w6n)[O63](M5u,B2n);},update:function(conf,options,append){var h93='[value="';var O93="lter";var d6n=e2n;d6n+=E2n;var T6n=l5n;T6n+=F1n;T6n+=O93;var I6n=q1n;I6n+=G3n;var Y6n=C5n;Y6n+=b0u;Y6n+=R13.q7n;var s6n=P83;s6n+=m5n;s6n+=B83;var A6n=u1Z;A6n+=m5n;A6n+=g8y;var radio=fieldTypes[A6n];var currVal=radio[w7J](conf);radio[s6n](conf,options,append);var inputs=conf[Y6n][F4J](b7Z);radio[I6n](conf,inputs[T6n](h93+currVal+d6n)[k2n]?currVal:inputs[D9J](B9n)[j0J](y3J));}});fieldTypes[j1n]=$[C8Z](B2n,{},baseFieldType,{create:function(conf){var a93='date';var X93="RFC_2822";var E93="dateFormat";var e93="picker";var L93="eryu";var m93="jqu";var W93="Format";var R93='<input />';var g93="safe";var V93="atep";var l6n=a8u;l6n+=A5u;var Q6n=m5n;Q6n+=V93;Q6n+=F1n;Q6n+=v93;var C6n=g93;C6n+=K6Z;var Z6n=a1n;Z6n+=j7J;Z6n+=p7n;conf[H0u]=$(R93)[Z6n]($[C8Z]({id:Editor[C6n](conf[k3n]),type:g63},conf[j0J]));if($[Q6n]){var h6n=j1n;h6n+=W93;var O6n=m93;O6n+=L93;O6n+=F1n;var f6n=C5n;f6n+=F1n;f6n+=M7u;f6n+=R13.q7n;conf[f6n][O7Z](O6n);if(!conf[h6n]){var V6n=j1n;V6n+=e93;conf[E93]=$[V6n][X93];}setTimeout(function(){var G93="dateImage";var b93="xte";var x93="ateFor";var l93="-datepicker-div";var D93="#ui";var E6n=D93;E6n+=l93;var e6n=o7n;e6n+=a5n;e6n+=R13.q7n;e6n+=q1n;var W6n=m5n;W6n+=x93;W6n+=b8u;var R6n=M5n;R6n+=o7n;R6n+=R13.q7n;R6n+=P9Z;var g6n=t7n;g6n+=b93;g6n+=F0n;var v6n=m5n;v6n+=V93;v6n+=p2n;v6n+=s3n;$(conf[H0u])[v6n]($[g6n]({showOn:R6n,dateFormat:conf[W6n],buttonImage:conf[G93],buttonImageOnly:B2n,onSelect:function(){var q93="click";var L6n=l5n;L6n+=o7n;L6n+=R13.U7n;L6n+=q7Z;var m6n=C5n;m6n+=r8Z;conf[m6n][L6n]()[q93]();}},conf[e6n]));$(E6n)[s9Z](Y9Z,K7Z);},y9n);}else{var D6n=R13.q7n;D6n+=V5n;D6n+=a5n;D6n+=t7n;var X6n=C5n;X6n+=y6Z;X6n+=C1Z;X6n+=R13.q7n;conf[X6n][j0J](D6n,a93);}return conf[l6n][B9n];},set:function(conf,val){var N93="setDate";var U93="datepi";var r93='hasDatepicker';var c93="hasC";var b6n=c93;b6n+=w0n;b6n+=q1n;b6n+=q1n;var x6n=n93;x6n+=o2n;x6n+=I7y;x6n+=p7n;if($[x6n]&&conf[H0u][b6n](r93)){var q6n=U93;q6n+=g1Z;q6n+=s3n;var G6n=C5n;G6n+=r8Z;conf[G6n][q6n](N93,val)[e5u]();}else{var a6n=q1J;a6n+=h1n;$(conf[H0u])[a6n](val);}},enable:function(conf){var u93="epick";var c6n=h8Z;c6n+=u93;c6n+=s3n;if($[c6n]){var r6n=n93;r6n+=K93;var n6n=a8u;n6n+=C1Z;n6n+=R13.q7n;conf[n6n][r6n](Z5J);}else{$(conf[H0u])[O63](M5u,J2n);}},disable:function(conf){var J93="sable";var k93="datepicker";if($[k93]){var N6n=G1n;N6n+=J93;var U6n=C5n;U6n+=l7Z;U6n+=x7Z;conf[U6n][k93](N6n);}else{$(conf[H0u])[O63](M5u,B2n);}},owns:function(conf,node){var B93='div.ui-datepicker-header';var P93='div.ui-datepicker';var K6n=K1Z;K6n+=B9J;var u6n=S7y;u6n+=q1n;return $(node)[S1y](P93)[k2n]||$(node)[u6n](B93)[K6n]?B2n:J2n;}});fieldTypes[o93]=$[k6n](B2n,{},baseFieldType,{create:function(conf){var H93="keyInput";var i93="ut ";var S93="<inp";var p93="oseFn";var j8n=w2u;j8n+=y7n;j8n+=A5u;var y6n=R13.U7n;y6n+=h1n;y6n+=A5y;y6n+=t7n;var t6n=K9J;t6n+=p93;var i6n=o7n;i6n+=a5n;i6n+=R13.q7n;i6n+=q1n;var p6n=F1n;p6n+=M1n;p6n+=w1n;p6n+=y7n;var o6n=W5n;o6n+=m5n;var B6n=F1n;B6n+=m5n;var P6n=G5n;P6n+=R13.q7n;P6n+=C1J;var J6n=S93;J6n+=i93;J6n+=Z6Z;J6n+=O6Z;conf[H0u]=$(J6n)[j0J]($[P6n](B2n,{id:Editor[z0J](conf[B6n]),type:g63},conf[j0J]));conf[t93]=new Editor[X8u](conf[H0u],$[o6n]({format:conf[E9u],i18n:this[p6n][o93],onChange:function(){var S6n=C5n;S6n+=F1n;S6n+=y7n;S6n+=A5u;_triggerChange(conf[S6n]);}},conf[i6n]));conf[t6n]=function(){var H6n=l4n;H6n+=F1n;H6n+=g1Z;H6n+=s3n;conf[H6n][t0Z]();};if(conf[H93]===J2n){conf[H0u][g2n](v1y,function(e){e[G7J]();});}this[g2n](y6n,conf[y93]);return conf[j8n][B9n];},set:function(conf,val){var F8n=P4n;F8n+=a1n;F8n+=h1n;var z8n=l4n;z8n+=F1n;z8n+=v93;conf[z8n][F8n](val);_triggerChange(conf[H0u]);},owns:function(conf,node){var w8n=o6J;w8n+=j9J;var M8n=l4n;M8n+=K93;return conf[M8n][w8n](node);},errorMessage:function(conf,msg){var z73="errorMsg";var j73="ker";var A8n=C5n;A8n+=l1y;A8n+=R13.U7n;A8n+=j73;conf[A8n][z73](msg);},destroy:function(conf){var F73="keyd";var T8n=F73;T8n+=o7n;T8n+=V3n;T8n+=y7n;var I8n=f0Z;I8n+=l5n;var Y8n=s0n;Y8n+=V1n;var s8n=o7n;s8n+=l5n;s8n+=l5n;this[s8n](Y8n,conf[y93]);conf[H0u][I8n](T8n);conf[t93][k1J]();},minDate:function(conf,min){var d8n=n1n;d8n+=F1n;d8n+=y7n;conf[t93][d8n](min);},maxDate:function(conf,max){conf[t93][K3u](max);}});fieldTypes[Z8n]=$[C8n](B2n,{},baseFieldType,{create:function(conf){var editor=this;var container=_commonUpload(editor,conf,function(val){var M73='postUpload';var h8n=S6Z;h8n+=t7n;var O8n=N1y;O8n+=S0Z;var f8n=R13.U7n;f8n+=A0n;f8n+=h1n;var Q8n=r1n;Q8n+=a5n;Q8n+=L2Z;Q8n+=A3n;Editor[E8u][Q8n][H6Z][f8n](editor,conf,val[B9n]);editor[O8n](M73,[conf[h8n],val[B9n]]);});return container;},get:function(conf){return conf[w73];},set:function(conf,val){var O73="oClear";var f73='noClear';var Q73="clearText";var C73='No file';var Z73="<span";var d73="leText";var T73="noFi";var I73='div.rendered';var Y73="lue but";var s73="div.clearVa";var X8n=A73;X8n+=h1n;var E8n=w3n;E8n+=A3n;E8n+=h2Z;E8n+=e3J;var L8n=s73;L8n+=Y73;L8n+=I3J;var v8n=m5n;v8n+=R1Z;var V8n=C5n;V8n+=y6Z;V8n+=A5u;conf[w73]=val;var container=conf[V8n];if(conf[v8n]){var g8n=l5n;g8n+=F1n;g8n+=y7n;g8n+=m5n;var rendered=container[g8n](I73);if(conf[w73]){var R8n=C5n;R8n+=g9Z;rendered[o7Z](conf[Q2Z](conf[R8n]));}else{var m8n=T73;m8n+=d73;var W8n=Z73;W8n+=O6Z;rendered[w7u]()[h4Z](W8n+(conf[m8n]||C73)+o8Z);}}var button=container[F4J](L8n);if(val&&conf[Q73]){button[o7Z](conf[Q73]);container[g7Z](f73);}else{var e8n=y7n;e8n+=O73;container[O7Z](e8n);}conf[H0u][F4J](b7Z)[P9y](E8n,[conf[X8n]]);},enable:function(conf){var h73="isab";var b8n=m5n;b8n+=h73;b8n+=l3n;b8n+=m5n;var x8n=a5n;x8n+=p7n;x8n+=o7n;x8n+=a5n;var l8n=F1n;l8n+=y7n;l8n+=a5n;l8n+=x7Z;var D8n=a8u;D8n+=A5u;conf[D8n][F4J](l8n)[x8n](b8n,J2n);conf[H2u]=B2n;},disable:function(conf){var v73="_inp";var V73="sabled";var c8n=G1n;c8n+=V73;var a8n=b0u;a8n+=R13.q7n;var q8n=z8Z;q8n+=y7n;q8n+=m5n;var G8n=v73;G8n+=x7Z;conf[G8n][q8n](a8n)[O63](c8n,B2n);conf[H2u]=J2n;},canReturnSubmit:function(conf,node){return J2n;}});fieldTypes[g73]=$[n8n](B2n,{},baseFieldType,{_showHide:function(conf){var E73="limit";var e73="_limitLeft";var L73="ntainer";var m73="tHide";var W73="iv.lim";var R73="lengt";var B8n=R73;B8n+=P9Z;var P8n=v1Z;P8n+=g1Z;var J8n=y7n;J8n+=g2n;J8n+=t7n;var k8n=h1n;k8n+=F1n;k8n+=E4n;var K8n=A73;K8n+=h1n;var u8n=m5n;u8n+=U9Z;u8n+=a5n;u8n+=S1Z;var N8n=m5n;N8n+=W73;N8n+=F1n;N8n+=m73;var U8n=r4u;U8n+=L73;var r8n=h1n;r8n+=d5Z;r8n+=F1n;r8n+=R13.q7n;if(!conf[r8n]){return;}conf[U8n][F4J](N8n)[s9Z](u8n,conf[K8n][k2n]>=conf[k8n]?J8n:P8n);conf[e73]=conf[E73]-conf[w73][B8n];},create:function(conf){var G73='button.remove';var D73="dC";var X73="_conta";var I9n=X73;I9n+=F1n;I9n+=y7n;I9n+=s3n;var M9n=s0n;M9n+=p2n;var F9n=n1n;F9n+=r1n;F9n+=H7Z;var z9n=A3n;z9n+=D73;z9n+=g6Z;z9n+=q1n;var editor=this;var container=_commonUpload(editor,conf,function(val){var b73="cat";var x73="Types";var l73="ostUpl";var j9n=C5n;j9n+=P4n;j9n+=a1n;j9n+=h1n;var y8n=a5n;y8n+=l73;y8n+=Z63;y8n+=m5n;var H8n=A73;H8n+=h1n;var t8n=R13.U7n;t8n+=a1n;t8n+=h1n;t8n+=h1n;var i8n=h8J;i8n+=x73;var S8n=Q7Z;S8n+=b73;var p8n=C5n;p8n+=g9Z;var o8n=h63;o8n+=A0n;conf[o8n]=conf[p8n][S8n](val);Editor[i8n][g73][H6Z][t8n](editor,conf,conf[H8n]);editor[w3J](y8n,[conf[Q8Z],conf[j9n]]);},B2n);container[z9n](F9n)[g2n](M9n,G73,function(e){var r73="stopPropagation";var n73="ice";var c73="dTy";var a73="ny";var q73="uploadM";var Y9n=q73;Y9n+=a1n;Y9n+=a73;var s9n=Q5J;s9n+=c73;s9n+=a5n;s9n+=H5n;var A9n=p3n;A9n+=n73;var w9n=F1n;w9n+=m5n;w9n+=H7n;e[r73]();var idx=$(this)[R8Z](w9n);conf[w73][A9n](idx,o9n);Editor[s9n][Y9n][H6Z][l9Z](editor,conf,conf[w73]);});conf[I9n]=container;return container;},get:function(conf){var T9n=A73;T9n+=h1n;return conf[T9n];},set:function(conf,val){var I13='upload.editor';var Y13='No files';var s13="ppend";var A13="pan";var w13="<s";var M13="Text";var F13="noFile";var z13="</sp";var p73='<ul/>';var o73="ndTo";var B73="rendered";var P73="v.";var J73='Upload collections must have an array as a value';var k73="Hide";var K73="np";var u73="ndler";var N73="gerHa";var U73="trig";var b9n=C5n;b9n+=P4n;b9n+=a1n;b9n+=h1n;var x9n=U73;x9n+=N73;x9n+=u73;var l9n=F1n;l9n+=K73;l9n+=r1n;l9n+=R13.q7n;var D9n=u2Z;D9n+=m5n;var X9n=C5n;X9n+=l7Z;X9n+=r1n;X9n+=R13.q7n;var E9n=C5n;E9n+=Q1Z;E9n+=o6J;E9n+=k73;var Z9n=m5n;Z9n+=F1n;Z9n+=q1n;Z9n+=l1u;var d9n=C5n;d9n+=l7Z;d9n+=x7Z;if(!val){val=[];}if(!$[M8J](val)){throw J73;}conf[w73]=val;var that=this;var container=conf[d9n];if(conf[Z9n]){var f9n=h1n;f9n+=t7n;f9n+=D0J;f9n+=P9Z;var Q9n=f7J;Q9n+=f3u;var C9n=G1n;C9n+=P73;C9n+=B73;var rendered=container[F4J](C9n)[Q9n]();if(val[f9n]){var O9n=k7Z;O9n+=a5n;O9n+=t7n;O9n+=o73;var list=$(p73)[O9n](rendered);$[D2n](val,function(i,file){var j13='">&times;</button>';var y73='<li>';var H73=" <butto";var t73="idx=";var i73=" remove\" data";var S73="/li>";var R9n=d6Z;R9n+=S73;var g9n=i73;g9n+=S8u;g9n+=t73;g9n+=e2n;var v9n=R13.U7n;v9n+=w7Z;v9n+=H5n;var V9n=H73;V9n+=O2u;V9n+=B6Z;var h9n=h9y;h9n+=m5n;list[h9n](y73+conf[Q2Z](file,i)+V9n+that[v9n][l9J][B5Z]+g9n+i+j13+R9n);});}else{var e9n=z13;e9n+=k9u;var L9n=F13;L9n+=M13;var m9n=w13;m9n+=A13;m9n+=O6Z;var W9n=a1n;W9n+=s13;rendered[W9n](m9n+(conf[L9n]||Y13)+e9n);}}Editor[E8u][g73][E9n](conf);conf[X9n][D9n](l9n)[x9n](I13,[conf[b9n]]);},enable:function(conf){var T13="_ena";var c9n=T13;c9n+=M5n;c9n+=M7Z;var a9n=m5n;a9n+=q63;var q9n=y6Z;q9n+=C1Z;q9n+=R13.q7n;var G9n=w2u;G9n+=y7n;G9n+=A5u;conf[G9n][F4J](q9n)[O63](a9n,J2n);conf[c9n]=B2n;},disable:function(conf){var r9n=t1J;r9n+=m5n;var n9n=R4n;n9n+=x5Z;conf[H0u][F4J](b7Z)[n9n](r9n,B2n);conf[H2u]=J2n;},canReturnSubmit:function(conf,node){return J2n;}});}());if(DataTable[U9n][d13]){var K9n=L8Z;K9n+=Z13;K9n+=m5n;K9n+=q1n;var u9n=z8Z;u9n+=C13;var N9n=t7n;N9n+=H7n;N9n+=R13.q7n;N9n+=C1J;$[N9n](Editor[u9n],DataTable[o5n][K9n]);}DataTable[k9n][d13]=Editor[E8u];Editor[x2n]={};Editor[G9Z][J9n]=P9n;Editor[Q13]=f13;return Editor;}));

/*! AutoFill 2.3.2
 * ©2008-2018 SpryMedia Ltd - datatables.net/license
 */

/**
 * @summary     AutoFill
 * @description Add Excel like click and drag auto-fill options to DataTables
 * @version     2.3.2
 * @file        dataTables.autoFill.js
 * @author      SpryMedia Ltd (www.sprymedia.co.uk)
 * @contact     www.sprymedia.co.uk/contact
 * @copyright   Copyright 2010-2018 SpryMedia Ltd.
 *
 * This source file is free software, available under the following license:
 *   MIT license - http://datatables.net/license/mit
 *
 * This source file is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the license files for details.
 *
 * For details please refer to: http://www.datatables.net
 */
(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net'], function ( $ ) {
			return factory( $, window, document );
		} );
	}
	else if ( typeof exports === 'object' ) {
		// CommonJS
		module.exports = function (root, $) {
			if ( ! root ) {
				root = window;
			}

			if ( ! $ || ! $.fn.dataTable ) {
				$ = require('datatables.net')(root, $).$;
			}

			return factory( $, root, root.document );
		};
	}
	else {
		// Browser
		factory( jQuery, window, document );
	}
}(function( $, window, document, undefined ) {
'use strict';
var DataTable = $.fn.dataTable;


var _instance = 0;

/** 
 * AutoFill provides Excel like auto-fill features for a DataTable
 *
 * @class AutoFill
 * @constructor
 * @param {object} oTD DataTables settings object
 * @param {object} oConfig Configuration object for AutoFill
 */
var AutoFill = function( dt, opts )
{
	if ( ! DataTable.versionCheck || ! DataTable.versionCheck( '1.10.8' ) ) {
		throw( "Warning: AutoFill requires DataTables 1.10.8 or greater");
	}

	// User and defaults configuration object
	this.c = $.extend( true, {},
		DataTable.defaults.autoFill,
		AutoFill.defaults,
		opts
	);

	/**
	 * @namespace Settings object which contains customisable information for AutoFill instance
	 */
	this.s = {
		/** @type {DataTable.Api} DataTables' API instance */
		dt: new DataTable.Api( dt ),

		/** @type {String} Unique namespace for events attached to the document */
		namespace: '.autoFill'+(_instance++),

		/** @type {Object} Cached dimension information for use in the mouse move event handler */
		scroll: {},

		/** @type {integer} Interval object used for smooth scrolling */
		scrollInterval: null,

		handle: {
			height: 0,
			width: 0
		},

		/**
		 * Enabled setting
		 * @type {Boolean}
		 */
		enabled: false
	};


	/**
	 * @namespace Common and useful DOM elements for the class instance
	 */
	this.dom = {
		/** @type {jQuery} AutoFill handle */
		handle: $('<div class="dt-autofill-handle"/>'),

		/**
		 * @type {Object} Selected cells outline - Need to use 4 elements,
		 *   otherwise the mouse over if you back into the selected rectangle
		 *   will be over that element, rather than the cells!
		 */
		select: {
			top:    $('<div class="dt-autofill-select top"/>'),
			right:  $('<div class="dt-autofill-select right"/>'),
			bottom: $('<div class="dt-autofill-select bottom"/>'),
			left:   $('<div class="dt-autofill-select left"/>')
		},

		/** @type {jQuery} Fill type chooser background */
		background: $('<div class="dt-autofill-background"/>'),

		/** @type {jQuery} Fill type chooser */
		list: $('<div class="dt-autofill-list">'+this.s.dt.i18n('autoFill.info', '')+'<ul/></div>'),

		/** @type {jQuery} DataTables scrolling container */
		dtScroll: null,

		/** @type {jQuery} Offset parent element */
		offsetParent: null
	};


	/* Constructor logic */
	this._constructor();
};



$.extend( AutoFill.prototype, {
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Public methods (exposed via the DataTables API below)
	 */
	enabled: function ()
	{
		return this.s.enabled;
	},


	enable: function ( flag )
	{
		var that = this;

		if ( flag === false ) {
			return this.disable();
		}

		this.s.enabled = true;

		this._focusListener();

		this.dom.handle.on( 'mousedown', function (e) {
			that._mousedown( e );
			return false;
		} );

		return this;
	},

	disable: function ()
	{
		this.s.enabled = false;

		this._focusListenerRemove();

		return this;
	},


	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Constructor
	 */

	/**
	 * Initialise the RowReorder instance
	 *
	 * @private
	 */
	_constructor: function ()
	{
		var that = this;
		var dt = this.s.dt;
		var dtScroll = $('div.dataTables_scrollBody', this.s.dt.table().container());

		// Make the instance accessible to the API
		dt.settings()[0].autoFill = this;

		if ( dtScroll.length ) {
			this.dom.dtScroll = dtScroll;

			// Need to scroll container to be the offset parent
			if ( dtScroll.css('position') === 'static' ) {
				dtScroll.css( 'position', 'relative' );
			}
		}

		if ( this.c.enable !== false ) {
			this.enable();
		}

		dt.on( 'destroy.autoFill', function () {
			that._focusListenerRemove();
		} );
	},


	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Private methods
	 */

	/**
	 * Display the AutoFill drag handle by appending it to a table cell. This
	 * is the opposite of the _detach method.
	 *
	 * @param  {node} node TD/TH cell to insert the handle into
	 * @private
	 */
	_attach: function ( node )
	{
		var dt = this.s.dt;
		var idx = dt.cell( node ).index();
		var handle = this.dom.handle;
		var handleDim = this.s.handle;

		if ( ! idx || dt.columns( this.c.columns ).indexes().indexOf( idx.column ) === -1 ) {
			this._detach();
			return;
		}

		if ( ! this.dom.offsetParent ) {
			// We attach to the table's offset parent
			this.dom.offsetParent = $( dt.table().node() ).offsetParent();
		}

		if ( ! handleDim.height || ! handleDim.width ) {
			// Append to document so we can get its size. Not expecting it to
			// change during the life time of the page
			handle.appendTo( 'body' );
			handleDim.height = handle.outerHeight();
			handleDim.width = handle.outerWidth();
		}

		// Might need to go through multiple offset parents
		var offset = this._getPosition( node, this.dom.offsetParent );

		this.dom.attachedTo = node;
		handle
			.css( {
				top: offset.top + node.offsetHeight - handleDim.height,
				left: offset.left + node.offsetWidth - handleDim.width
			} )
			.appendTo( this.dom.offsetParent );
	},


	/**
	 * Determine can the fill type should be. This can be automatic, or ask the
	 * end user.
	 *
	 * @param {array} cells Information about the selected cells from the key
	 *     up function
	 * @private
	 */
	_actionSelector: function ( cells )
	{
		var that = this;
		var dt = this.s.dt;
		var actions = AutoFill.actions;
		var available = [];

		// "Ask" each plug-in if it wants to handle this data
		$.each( actions, function ( key, action ) {
			if ( action.available( dt, cells ) ) {
				available.push( key );
			}
		} );

		if ( available.length === 1 && this.c.alwaysAsk === false ) {
			// Only one action available - enact it immediately
			var result = actions[ available[0] ].execute( dt, cells );
			this._update( result, cells );
		}
		else {
			// Multiple actions available - ask the end user what they want to do
			var list = this.dom.list.children('ul').empty();

			// Add a cancel option
			available.push( 'cancel' );

			$.each( available, function ( i, name ) {
				list.append( $('<li/>')
					.append(
						'<div class="dt-autofill-question">'+
							actions[ name ].option( dt, cells )+
						'<div>'
					)
					.append( $('<div class="dt-autofill-button">' )
						.append( $('<button class="'+AutoFill.classes.btn+'">'+dt.i18n('autoFill.button', '&gt;')+'</button>')
							.on( 'click', function () {
								var result = actions[ name ].execute(
									dt, cells, $(this).closest('li')
								);
								that._update( result, cells );

								that.dom.background.remove();
								that.dom.list.remove();
							} )
						)
					)
				);
			} );

			this.dom.background.appendTo( 'body' );
			this.dom.list.appendTo( 'body' );

			this.dom.list.css( 'margin-top', this.dom.list.outerHeight()/2 * -1 );
		}
	},


	/**
	 * Remove the AutoFill handle from the document
	 *
	 * @private
	 */
	_detach: function ()
	{
		this.dom.attachedTo = null;
		this.dom.handle.detach();
	},


	/**
	 * Draw the selection outline by calculating the range between the start
	 * and end cells, then placing the highlighting elements to draw a rectangle
	 *
	 * @param  {node}   target End cell
	 * @param  {object} e      Originating event
	 * @private
	 */
	_drawSelection: function ( target, e )
	{
		// Calculate boundary for start cell to this one
		var dt = this.s.dt;
		var start = this.s.start;
		var startCell = $(this.dom.start);
		var end = {
			row: this.c.vertical ?
				dt.rows( { page: 'current' } ).nodes().indexOf( target.parentNode ) :
				start.row,
			column: this.c.horizontal ?
				$(target).index() :
				start.column
		};
		var colIndx = dt.column.index( 'toData', end.column );
		var endRow =  dt.row( ':eq('+end.row+')', { page: 'current' } ); // Workaround for M581
		var endCell = $( dt.cell( endRow.index(), colIndx ).node() );

		// Be sure that is a DataTables controlled cell
		if ( ! dt.cell( endCell ).any() ) {
			return;
		}

		// if target is not in the columns available - do nothing
		if ( dt.columns( this.c.columns ).indexes().indexOf( colIndx ) === -1 ) {
			return;
		}

		this.s.end = end;

		var top, bottom, left, right, height, width;

		top    = start.row    < end.row    ? startCell : endCell;
		bottom = start.row    < end.row    ? endCell   : startCell;
		left   = start.column < end.column ? startCell : endCell;
		right  = start.column < end.column ? endCell   : startCell;

		top    = this._getPosition( top ).top;
		left   = this._getPosition( left ).left;
		height = this._getPosition( bottom ).top + bottom.outerHeight() - top;
		width  = this._getPosition( right ).left + right.outerWidth() - left;

		var select = this.dom.select;
		select.top.css( {
			top: top,
			left: left,
			width: width
		} );

		select.left.css( {
			top: top,
			left: left,
			height: height
		} );

		select.bottom.css( {
			top: top + height,
			left: left,
			width: width
		} );

		select.right.css( {
			top: top,
			left: left + width,
			height: height
		} );
	},


	/**
	 * Use the Editor API to perform an update based on the new data for the
	 * cells
	 *
	 * @param {array} cells Information about the selected cells from the key
	 *     up function
	 * @private
	 */
	_editor: function ( cells )
	{
		var dt = this.s.dt;
		var editor = this.c.editor;

		if ( ! editor ) {
			return;
		}

		// Build the object structure for Editor's multi-row editing
		var idValues = {};
		var nodes = [];
		var fields = editor.fields();

		for ( var i=0, ien=cells.length ; i<ien ; i++ ) {
			for ( var j=0, jen=cells[i].length ; j<jen ; j++ ) {
				var cell = cells[i][j];

				// Determine the field name for the cell being edited
				var col = dt.settings()[0].aoColumns[ cell.index.column ];
				var fieldName = col.editField;

				if ( fieldName === undefined ) {
					var dataSrc = col.mData;

					// dataSrc is the `field.data` property, but we need to set
					// using the field name, so we need to translate from the
					// data to the name
					for ( var k=0, ken=fields.length ; k<ken ; k++ ) {
						var field = editor.field( fields[k] );

						if ( field.dataSrc() === dataSrc ) {
							fieldName = field.name();
							break;
						}
					}
				}

				if ( ! fieldName ) {
					throw 'Could not automatically determine field data. '+
						'Please see https://datatables.net/tn/11';
				}

				if ( ! idValues[ fieldName ] ) {
					idValues[ fieldName ] = {};
				}

				var id = dt.row( cell.index.row ).id();
				idValues[ fieldName ][ id ] = cell.set;

				// Keep a list of cells so we can activate the bubble editing
				// with them
				nodes.push( cell.index );
			}
		}

		// Perform the edit using bubble editing as it allows us to specify
		// the cells to be edited, rather than using full rows
		editor
			.bubble( nodes, false )
			.multiSet( idValues )
			.submit();
	},


	/**
	 * Emit an event on the DataTable for listeners
	 *
	 * @param  {string} name Event name
	 * @param  {array} args Event arguments
	 * @private
	 */
	_emitEvent: function ( name, args )
	{
		this.s.dt.iterator( 'table', function ( ctx, i ) {
			$(ctx.nTable).triggerHandler( name+'.dt', args );
		} );
	},


	/**
	 * Attach suitable listeners (based on the configuration) that will attach
	 * and detach the AutoFill handle in the document.
	 *
	 * @private
	 */
	_focusListener: function ()
	{
		var that = this;
		var dt = this.s.dt;
		var namespace = this.s.namespace;
		var focus = this.c.focus !== null ?
			this.c.focus :
			dt.init().keys || dt.settings()[0].keytable ?
				'focus' :
				'hover';

		// All event listeners attached here are removed in the `destroy`
		// callback in the constructor
		if ( focus === 'focus' ) {
			dt
				.on( 'key-focus.autoFill', function ( e, dt, cell ) {
					that._attach( cell.node() );
				} )
				.on( 'key-blur.autoFill', function ( e, dt, cell ) {
					that._detach();
				} );
		}
		else if ( focus === 'click' ) {
			$(dt.table().body()).on( 'click'+namespace, 'td, th', function (e) {
				that._attach( this );
			} );

			$(document.body).on( 'click'+namespace, function (e) {
				if ( ! $(e.target).parents().filter( dt.table().body() ).length ) {
					that._detach();
				}
			} );
		}
		else {
			$(dt.table().body())
				.on( 'mouseenter'+namespace, 'td, th', function (e) {
					that._attach( this );
				} )
				.on( 'mouseleave'+namespace, function (e) {
					if ( $(e.relatedTarget).hasClass('dt-autofill-handle') ) {
						return;
					}

					that._detach();
				} );
		}
	},


	_focusListenerRemove: function ()
	{
		var dt = this.s.dt;

		dt.off( '.autoFill' );
		$(dt.table().body()).off( this.s.namespace );
		$(document.body).off( this.s.namespace );
	},


	/**
	 * Get the position of a node, relative to another, including any scrolling
	 * offsets.
	 * @param  {Node}   node         Node to get the position of
	 * @param  {jQuery} targetParent Node to use as the parent
	 * @return {object}              Offset calculation
	 * @private
	 */
	_getPosition: function ( node, targetParent )
	{
		var
			currNode = $(node),
			currOffsetParent,
			position,
			top = 0,
			left = 0;

		if ( ! targetParent ) {
			targetParent = $( $( this.s.dt.table().node() )[0].offsetParent );
		}

		do {
			position = currNode.position();

			// jQuery doesn't give a `table` as the offset parent oddly, so use DOM directly
			currOffsetParent = $( currNode[0].offsetParent );

			top += position.top + currOffsetParent.scrollTop();
			left += position.left + currOffsetParent.scrollLeft();

			top += parseInt( currOffsetParent.css('margin-top') ) * 1;
			top += parseInt( currOffsetParent.css('border-top-width') ) * 1;

			// Emergency fall back. Shouldn't happen, but just in case!
			if ( currNode.get(0).nodeName.toLowerCase() === 'body' ) {
				break;
			}

			currNode = currOffsetParent; // for next loop
		}
		while ( currOffsetParent.get(0) !== targetParent.get(0) )

		return {
			top: top,
			left: left
		};
	},


	/**
	 * Start mouse drag - selects the start cell
	 *
	 * @param  {object} e Mouse down event
	 * @private
	 */
	_mousedown: function ( e )
	{
		var that = this;
		var dt = this.s.dt;

		this.dom.start = this.dom.attachedTo;
		this.s.start = {
			row: dt.rows( { page: 'current' } ).nodes().indexOf( $(this.dom.start).parent()[0] ),
			column: $(this.dom.start).index()
		};

		$(document.body)
			.on( 'mousemove.autoFill', function (e) {
				that._mousemove( e );
			} )
			.on( 'mouseup.autoFill', function (e) {
				that._mouseup( e );
			} );

		var select = this.dom.select;
		var offsetParent = $( dt.table().node() ).offsetParent();
		select.top.appendTo( offsetParent );
		select.left.appendTo( offsetParent );
		select.right.appendTo( offsetParent );
		select.bottom.appendTo( offsetParent );

		this._drawSelection( this.dom.start, e );

		this.dom.handle.css( 'display', 'none' );

		// Cache scrolling information so mouse move doesn't need to read.
		// This assumes that the window and DT scroller will not change size
		// during an AutoFill drag, which I think is a fair assumption
		var scrollWrapper = this.dom.dtScroll;
		this.s.scroll = {
			windowHeight: $(window).height(),
			windowWidth:  $(window).width(),
			dtTop:        scrollWrapper ? scrollWrapper.offset().top : null,
			dtLeft:       scrollWrapper ? scrollWrapper.offset().left : null,
			dtHeight:     scrollWrapper ? scrollWrapper.outerHeight() : null,
			dtWidth:      scrollWrapper ? scrollWrapper.outerWidth() : null
		};
	},


	/**
	 * Mouse drag - selects the end cell and update the selection display for
	 * the end user
	 *
	 * @param  {object} e Mouse move event
	 * @private
	 */
	_mousemove: function ( e )
	{	
		var that = this;
		var dt = this.s.dt;
		var name = e.target.nodeName.toLowerCase();
		if ( name !== 'td' && name !== 'th' ) {
			return;
		}

		this._drawSelection( e.target, e );
		this._shiftScroll( e );
	},


	/**
	 * End mouse drag - perform the update actions
	 *
	 * @param  {object} e Mouse up event
	 * @private
	 */
	_mouseup: function ( e )
	{
		$(document.body).off( '.autoFill' );

		var that = this;
		var dt = this.s.dt;
		var select = this.dom.select;
		select.top.remove();
		select.left.remove();
		select.right.remove();
		select.bottom.remove();

		this.dom.handle.css( 'display', 'block' );

		// Display complete - now do something useful with the selection!
		var start = this.s.start;
		var end = this.s.end;

		// Haven't selected multiple cells, so nothing to do
		if ( start.row === end.row && start.column === end.column ) {
			return;
		}

		var startDt = dt.cell( ':eq('+start.row+')', start.column+':visible', {page:'current'} );

		// If Editor is active inside this cell (inline editing) we need to wait for Editor to
		// submit and then we can loop back and trigger the fill.
		if ( $('div.DTE', startDt.node()).length ) {
			var editor = dt.editor();

			editor
				.on( 'submitSuccess.dtaf', function () {
					editor.off( '.dtaf');

					setTimeout( function () {
						that._mouseup( e );
					}, 100 );
				} )
				.on( 'submitComplete.dtaf preSubmitCancelled.dtaf', function () {
					editor.off( '.dtaf');
				} );

			// Make the current input submit
			editor.submit();

			return;
		}

		// Build a matrix representation of the selected rows
		var rows       = this._range( start.row, end.row );
		var columns    = this._range( start.column, end.column );
		var selected   = [];
		var dtSettings = dt.settings()[0];
		var dtColumns  = dtSettings.aoColumns;

		// Can't use Array.prototype.map as IE8 doesn't support it
		// Can't use $.map as jQuery flattens 2D arrays
		// Need to use a good old fashioned for loop
		for ( var rowIdx=0 ; rowIdx<rows.length ; rowIdx++ ) {
			selected.push(
				$.map( columns, function (column) {
					var row = dt.row( ':eq('+rows[rowIdx]+')', {page:'current'} ); // Workaround for M581
					var cell = dt.cell( row.index(), column+':visible' );
					var data = cell.data();
					var cellIndex = cell.index();
					var editField = dtColumns[ cellIndex.column ].editField;

					if ( editField !== undefined ) {
						data = dtSettings.oApi._fnGetObjectDataFn( editField )( dt.row( cellIndex.row ).data() );
					}

					return {
						cell:  cell,
						data:  data,
						label: cell.data(),
						index: cellIndex
					};
				} )
			);
		}

		this._actionSelector( selected );
		
		// Stop shiftScroll
		clearInterval( this.s.scrollInterval );
		this.s.scrollInterval = null;
	},


	/**
	 * Create an array with a range of numbers defined by the start and end
	 * parameters passed in (inclusive!).
	 * 
	 * @param  {integer} start Start
	 * @param  {integer} end   End
	 * @private
	 */
	_range: function ( start, end )
	{
		var out = [];
		var i;

		if ( start <= end ) {
			for ( i=start ; i<=end ; i++ ) {
				out.push( i );
			}
		}
		else {
			for ( i=start ; i>=end ; i-- ) {
				out.push( i );
			}
		}

		return out;
	},


	/**
	 * Move the window and DataTables scrolling during a drag to scroll new
	 * content into view. This is done by proximity to the edge of the scrolling
	 * container of the mouse - for example near the top edge of the window
	 * should scroll up. This is a little complicated as there are two elements
	 * that can be scrolled - the window and the DataTables scrolling view port
	 * (if scrollX and / or scrollY is enabled).
	 *
	 * @param  {object} e Mouse move event object
	 * @private
	 */
	_shiftScroll: function ( e )
	{
		var that = this;
		var dt = this.s.dt;
		var scroll = this.s.scroll;
		var runInterval = false;
		var scrollSpeed = 5;
		var buffer = 65;
		var
			windowY = e.pageY - document.body.scrollTop,
			windowX = e.pageX - document.body.scrollLeft,
			windowVert, windowHoriz,
			dtVert, dtHoriz;

		// Window calculations - based on the mouse position in the window,
		// regardless of scrolling
		if ( windowY < buffer ) {
			windowVert = scrollSpeed * -1;
		}
		else if ( windowY > scroll.windowHeight - buffer ) {
			windowVert = scrollSpeed;
		}

		if ( windowX < buffer ) {
			windowHoriz = scrollSpeed * -1;
		}
		else if ( windowX > scroll.windowWidth - buffer ) {
			windowHoriz = scrollSpeed;
		}

		// DataTables scrolling calculations - based on the table's position in
		// the document and the mouse position on the page
		if ( scroll.dtTop !== null && e.pageY < scroll.dtTop + buffer ) {
			dtVert = scrollSpeed * -1;
		}
		else if ( scroll.dtTop !== null && e.pageY > scroll.dtTop + scroll.dtHeight - buffer ) {
			dtVert = scrollSpeed;
		}

		if ( scroll.dtLeft !== null && e.pageX < scroll.dtLeft + buffer ) {
			dtHoriz = scrollSpeed * -1;
		}
		else if ( scroll.dtLeft !== null && e.pageX > scroll.dtLeft + scroll.dtWidth - buffer ) {
			dtHoriz = scrollSpeed;
		}

		// This is where it gets interesting. We want to continue scrolling
		// without requiring a mouse move, so we need an interval to be
		// triggered. The interval should continue until it is no longer needed,
		// but it must also use the latest scroll commands (for example consider
		// that the mouse might move from scrolling up to scrolling left, all
		// with the same interval running. We use the `scroll` object to "pass"
		// this information to the interval. Can't use local variables as they
		// wouldn't be the ones that are used by an already existing interval!
		if ( windowVert || windowHoriz || dtVert || dtHoriz ) {
			scroll.windowVert = windowVert;
			scroll.windowHoriz = windowHoriz;
			scroll.dtVert = dtVert;
			scroll.dtHoriz = dtHoriz;
			runInterval = true;
		}
		else if ( this.s.scrollInterval ) {
			// Don't need to scroll - remove any existing timer
			clearInterval( this.s.scrollInterval );
			this.s.scrollInterval = null;
		}

		// If we need to run the interval to scroll and there is no existing
		// interval (if there is an existing one, it will continue to run)
		if ( ! this.s.scrollInterval && runInterval ) {
			this.s.scrollInterval = setInterval( function () {
				// Don't need to worry about setting scroll <0 or beyond the
				// scroll bound as the browser will just reject that.
				if ( scroll.windowVert ) {
					document.body.scrollTop += scroll.windowVert;
				}
				if ( scroll.windowHoriz ) {
					document.body.scrollLeft += scroll.windowHoriz;
				}

				// DataTables scrolling
				if ( scroll.dtVert || scroll.dtHoriz ) {
					var scroller = that.dom.dtScroll[0];

					if ( scroll.dtVert ) {
						scroller.scrollTop += scroll.dtVert;
					}
					if ( scroll.dtHoriz ) {
						scroller.scrollLeft += scroll.dtHoriz;
					}
				}
			}, 20 );
		}
	},


	/**
	 * Update the DataTable after the user has selected what they want to do
	 *
	 * @param  {false|undefined} result Return from the `execute` method - can
	 *   be false internally to do nothing. This is not documented for plug-ins
	 *   and is used only by the cancel option.
	 * @param {array} cells Information about the selected cells from the key
	 *     up function, argumented with the set values
	 * @private
	 */
	_update: function ( result, cells )
	{
		// Do nothing on `false` return from an execute function
		if ( result === false ) {
			return;
		}

		var dt = this.s.dt;
		var cell;

		// Potentially allow modifications to the cells matrix
		this._emitEvent( 'preAutoFill', [ dt, cells ] );

		this._editor( cells );

		// Automatic updates are not performed if `update` is null and the
		// `editor` parameter is passed in - the reason being that Editor will
		// update the data once submitted
		var update = this.c.update !== null ?
			this.c.update :
			this.c.editor ?
				false :
				true;

		if ( update ) {
			for ( var i=0, ien=cells.length ; i<ien ; i++ ) {
				for ( var j=0, jen=cells[i].length ; j<jen ; j++ ) {
					cell = cells[i][j];

					cell.cell.data( cell.set );
				}
			}

			dt.draw(false);
		}

		this._emitEvent( 'autoFill', [ dt, cells ] );
	}
} );


/**
 * AutoFill actions. The options here determine how AutoFill will fill the data
 * in the table when the user has selected a range of cells. Please see the
 * documentation on the DataTables site for full details on how to create plug-
 * ins.
 *
 * @type {Object}
 */
AutoFill.actions = {
	increment: {
		available: function ( dt, cells ) {
			var d = cells[0][0].label;

			// is numeric test based on jQuery's old `isNumeric` function
			return !isNaN( d - parseFloat( d ) );
		},

		option: function ( dt, cells ) {
			return dt.i18n(
				'autoFill.increment',
				'Increment / decrement each cell by: <input type="number" value="1">'
			);
		},

		execute: function ( dt, cells, node ) {
			var value = cells[0][0].data * 1;
			var increment = $('input', node).val() * 1;

			for ( var i=0, ien=cells.length ; i<ien ; i++ ) {
				for ( var j=0, jen=cells[i].length ; j<jen ; j++ ) {
					cells[i][j].set = value;

					value += increment;
				}
			}
		}
	},

	fill: {
		available: function ( dt, cells ) {
			return true;
		},

		option: function ( dt, cells ) {
			return dt.i18n('autoFill.fill', 'Fill all cells with <i>'+cells[0][0].label+'</i>' );
		},

		execute: function ( dt, cells, node ) {
			var value = cells[0][0].data;

			for ( var i=0, ien=cells.length ; i<ien ; i++ ) {
				for ( var j=0, jen=cells[i].length ; j<jen ; j++ ) {
					cells[i][j].set = value;
				}
			}
		}
	},

	fillHorizontal: {
		available: function ( dt, cells ) {
			return cells.length > 1 && cells[0].length > 1;
		},

		option: function ( dt, cells ) {
			return dt.i18n('autoFill.fillHorizontal', 'Fill cells horizontally' );
		},

		execute: function ( dt, cells, node ) {
			for ( var i=0, ien=cells.length ; i<ien ; i++ ) {
				for ( var j=0, jen=cells[i].length ; j<jen ; j++ ) {
					cells[i][j].set = cells[i][0].data;
				}
			}
		}
	},

	fillVertical: {
		available: function ( dt, cells ) {
			return cells.length > 1 && cells[0].length > 1;
		},

		option: function ( dt, cells ) {
			return dt.i18n('autoFill.fillVertical', 'Fill cells vertically' );
		},

		execute: function ( dt, cells, node ) {
			for ( var i=0, ien=cells.length ; i<ien ; i++ ) {
				for ( var j=0, jen=cells[i].length ; j<jen ; j++ ) {
					cells[i][j].set = cells[0][j].data;
				}
			}
		}
	},

	// Special type that does not make itself available, but is added
	// automatically by AutoFill if a multi-choice list is shown. This allows
	// sensible code reuse
	cancel: {
		available: function () {
			return false;
		},

		option: function ( dt ) {
			return dt.i18n('autoFill.cancel', 'Cancel' );
		},

		execute: function () {
			return false;
		}
	}
};


/**
 * AutoFill version
 * 
 * @static
 * @type      String
 */
AutoFill.version = '2.3.2';


/**
 * AutoFill defaults
 * 
 * @namespace
 */
AutoFill.defaults = {
	/** @type {Boolean} Ask user what they want to do, even for a single option */
	alwaysAsk: false,

	/** @type {string|null} What will trigger a focus */
	focus: null, // focus, click, hover

	/** @type {column-selector} Columns to provide auto fill for */
	columns: '', // all

	/** @type {Boolean} Enable AutoFill on load */
	enable: true,

	/** @type {boolean|null} Update the cells after a drag */
	update: null, // false is editor given, true otherwise

	/** @type {DataTable.Editor} Editor instance for automatic submission */
	editor: null,

	/** @type {boolean} Enable vertical fill */
	vertical: true,

	/** @type {boolean} Enable horizontal fill */
	horizontal: true
};


/**
 * Classes used by AutoFill that are configurable
 * 
 * @namespace
 */
AutoFill.classes = {
	/** @type {String} Class used by the selection button */
	btn: 'btn'
};


/*
 * API
 */
var Api = $.fn.dataTable.Api;

// Doesn't do anything - Not documented
Api.register( 'autoFill()', function () {
	return this;
} );

Api.register( 'autoFill().enabled()', function () {
	var ctx = this.context[0];

	return ctx.autoFill ?
		ctx.autoFill.enabled() :
		false;
} );

Api.register( 'autoFill().enable()', function ( flag ) {
	return this.iterator( 'table', function ( ctx ) {
		if ( ctx.autoFill ) {
			ctx.autoFill.enable( flag );
		}
	} );
} );

Api.register( 'autoFill().disable()', function () {
	return this.iterator( 'table', function ( ctx ) {
		if ( ctx.autoFill ) {
			ctx.autoFill.disable();
		}
	} );
} );


// Attach a listener to the document which listens for DataTables initialisation
// events so we can automatically initialise
$(document).on( 'preInit.dt.autofill', function (e, settings, json) {
	if ( e.namespace !== 'dt' ) {
		return;
	}

	var init = settings.oInit.autoFill;
	var defaults = DataTable.defaults.autoFill;

	if ( init || defaults ) {
		var opts = $.extend( {}, init, defaults );

		if ( init !== false ) {
			new AutoFill( settings, opts  );
		}
	}
} );


// Alias for access
DataTable.AutoFill = AutoFill;
DataTable.AutoFill = AutoFill;


return AutoFill;
}));


/*! FixedColumns 3.2.5
 * ©2010-2018 SpryMedia Ltd - datatables.net/license
 */

/**
 * @summary     FixedColumns
 * @description Freeze columns in place on a scrolling DataTable
 * @version     3.2.5
 * @file        dataTables.fixedColumns.js
 * @author      SpryMedia Ltd (www.sprymedia.co.uk)
 * @contact     www.sprymedia.co.uk/contact
 * @copyright   Copyright 2010-2018 SpryMedia Ltd.
 *
 * This source file is free software, available under the following license:
 *   MIT license - http://datatables.net/license/mit
 *
 * This source file is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the license files for details.
 *
 * For details please refer to: http://www.datatables.net
 */
(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net'], function ( $ ) {
			return factory( $, window, document );
		} );
	}
	else if ( typeof exports === 'object' ) {
		// CommonJS
		module.exports = function (root, $) {
			if ( ! root ) {
				root = window;
			}

			if ( ! $ || ! $.fn.dataTable ) {
				$ = require('datatables.net')(root, $).$;
			}

			return factory( $, root, root.document );
		};
	}
	else {
		// Browser
		factory( jQuery, window, document );
	}
}(function( $, window, document, undefined ) {
'use strict';
var DataTable = $.fn.dataTable;
var _firefoxScroll;

/**
 * When making use of DataTables' x-axis scrolling feature, you may wish to
 * fix the left most column in place. This plug-in for DataTables provides
 * exactly this option (note for non-scrolling tables, please use the
 * FixedHeader plug-in, which can fix headers and footers). Key
 * features include:
 *
 * * Freezes the left or right most columns to the side of the table
 * * Option to freeze two or more columns
 * * Full integration with DataTables' scrolling options
 * * Speed - FixedColumns is fast in its operation
 *
 *  @class
 *  @constructor
 *  @global
 *  @param {object} dt DataTables instance. With DataTables 1.10 this can also
 *    be a jQuery collection, a jQuery selector, DataTables API instance or
 *    settings object.
 *  @param {object} [init={}] Configuration object for FixedColumns. Options are
 *    defined by {@link FixedColumns.defaults}
 *
 *  @requires jQuery 1.7+
 *  @requires DataTables 1.8.0+
 *
 *  @example
 *      var table = $('#example').dataTable( {
 *        "scrollX": "100%"
 *      } );
 *      new $.fn.dataTable.fixedColumns( table );
 */
var FixedColumns = function ( dt, init ) {
	var that = this;

	/* Sanity check - you just know it will happen */
	if ( ! ( this instanceof FixedColumns ) ) {
		alert( "FixedColumns warning: FixedColumns must be initialised with the 'new' keyword." );
		return;
	}

	if ( init === undefined || init === true ) {
		init = {};
	}

	// Use the DataTables Hungarian notation mapping method, if it exists to
	// provide forwards compatibility for camel case variables
	var camelToHungarian = $.fn.dataTable.camelToHungarian;
	if ( camelToHungarian ) {
		camelToHungarian( FixedColumns.defaults, FixedColumns.defaults, true );
		camelToHungarian( FixedColumns.defaults, init );
	}

	// v1.10 allows the settings object to be got form a number of sources
	var dtSettings = new $.fn.dataTable.Api( dt ).settings()[0];

	/**
	 * Settings object which contains customisable information for FixedColumns instance
	 * @namespace
	 * @extends FixedColumns.defaults
	 * @private
	 */
	this.s = {
		/**
		 * DataTables settings objects
		 *  @type     object
		 *  @default  Obtained from DataTables instance
		 */
		"dt": dtSettings,

		/**
		 * Number of columns in the DataTable - stored for quick access
		 *  @type     int
		 *  @default  Obtained from DataTables instance
		 */
		"iTableColumns": dtSettings.aoColumns.length,

		/**
		 * Original outer widths of the columns as rendered by DataTables - used to calculate
		 * the FixedColumns grid bounding box
		 *  @type     array.<int>
		 *  @default  []
		 */
		"aiOuterWidths": [],

		/**
		 * Original inner widths of the columns as rendered by DataTables - used to apply widths
		 * to the columns
		 *  @type     array.<int>
		 *  @default  []
		 */
		"aiInnerWidths": [],


		/**
		 * Is the document layout right-to-left
		 * @type boolean
		 */
		rtl: $(dtSettings.nTable).css('direction') === 'rtl'
	};


	/**
	 * DOM elements used by the class instance
	 * @namespace
	 * @private
	 *
	 */
	this.dom = {
		/**
		 * DataTables scrolling element
		 *  @type     node
		 *  @default  null
		 */
		"scroller": null,

		/**
		 * DataTables header table
		 *  @type     node
		 *  @default  null
		 */
		"header": null,

		/**
		 * DataTables body table
		 *  @type     node
		 *  @default  null
		 */
		"body": null,

		/**
		 * DataTables footer table
		 *  @type     node
		 *  @default  null
		 */
		"footer": null,

		/**
		 * Display grid elements
		 * @namespace
		 */
		"grid": {
			/**
			 * Grid wrapper. This is the container element for the 3x3 grid
			 *  @type     node
			 *  @default  null
			 */
			"wrapper": null,

			/**
			 * DataTables scrolling element. This element is the DataTables
			 * component in the display grid (making up the main table - i.e.
			 * not the fixed columns).
			 *  @type     node
			 *  @default  null
			 */
			"dt": null,

			/**
			 * Left fixed column grid components
			 * @namespace
			 */
			"left": {
				"wrapper": null,
				"head": null,
				"body": null,
				"foot": null
			},

			/**
			 * Right fixed column grid components
			 * @namespace
			 */
			"right": {
				"wrapper": null,
				"head": null,
				"body": null,
				"foot": null
			}
		},

		/**
		 * Cloned table nodes
		 * @namespace
		 */
		"clone": {
			/**
			 * Left column cloned table nodes
			 * @namespace
			 */
			"left": {
				/**
				 * Cloned header table
				 *  @type     node
				 *  @default  null
				 */
				"header": null,

				/**
				 * Cloned body table
				 *  @type     node
				 *  @default  null
				 */
				"body": null,

				/**
				 * Cloned footer table
				 *  @type     node
				 *  @default  null
				 */
				"footer": null
			},

			/**
			 * Right column cloned table nodes
			 * @namespace
			 */
			"right": {
				/**
				 * Cloned header table
				 *  @type     node
				 *  @default  null
				 */
				"header": null,

				/**
				 * Cloned body table
				 *  @type     node
				 *  @default  null
				 */
				"body": null,

				/**
				 * Cloned footer table
				 *  @type     node
				 *  @default  null
				 */
				"footer": null
			}
		}
	};

	if ( dtSettings._oFixedColumns ) {
		throw 'FixedColumns already initialised on this table';
	}

	/* Attach the instance to the DataTables instance so it can be accessed easily */
	dtSettings._oFixedColumns = this;

	/* Let's do it */
	if ( ! dtSettings._bInitComplete )
	{
		dtSettings.oApi._fnCallbackReg( dtSettings, 'aoInitComplete', function () {
			that._fnConstruct( init );
		}, 'FixedColumns' );
	}
	else
	{
		this._fnConstruct( init );
	}
};



$.extend( FixedColumns.prototype , {
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Public methods
	 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

	/**
	 * Update the fixed columns - including headers and footers. Note that FixedColumns will
	 * automatically update the display whenever the host DataTable redraws.
	 *  @returns {void}
	 *  @example
	 *      var table = $('#example').dataTable( {
	 *          "scrollX": "100%"
	 *      } );
	 *      var fc = new $.fn.dataTable.fixedColumns( table );
	 *
	 *      // at some later point when the table has been manipulated....
	 *      fc.fnUpdate();
	 */
	"fnUpdate": function ()
	{
		this._fnDraw( true );
	},


	/**
	 * Recalculate the resizes of the 3x3 grid that FixedColumns uses for display of the table.
	 * This is useful if you update the width of the table container. Note that FixedColumns will
	 * perform this function automatically when the window.resize event is fired.
	 *  @returns {void}
	 *  @example
	 *      var table = $('#example').dataTable( {
	 *          "scrollX": "100%"
	 *      } );
	 *      var fc = new $.fn.dataTable.fixedColumns( table );
	 *
	 *      // Resize the table container and then have FixedColumns adjust its layout....
	 *      $('#content').width( 1200 );
	 *      fc.fnRedrawLayout();
	 */
	"fnRedrawLayout": function ()
	{
		this._fnColCalc();
		this._fnGridLayout();
		this.fnUpdate();
	},


	/**
	 * Mark a row such that it's height should be recalculated when using 'semiauto' row
	 * height matching. This function will have no effect when 'none' or 'auto' row height
	 * matching is used.
	 *  @param   {Node} nTr TR element that should have it's height recalculated
	 *  @returns {void}
	 *  @example
	 *      var table = $('#example').dataTable( {
	 *          "scrollX": "100%"
	 *      } );
	 *      var fc = new $.fn.dataTable.fixedColumns( table );
	 *
	 *      // manipulate the table - mark the row as needing an update then update the table
	 *      // this allows the redraw performed by DataTables fnUpdate to recalculate the row
	 *      // height
	 *      fc.fnRecalculateHeight();
	 *      table.fnUpdate( $('#example tbody tr:eq(0)')[0], ["insert date", 1, 2, 3 ... ]);
	 */
	"fnRecalculateHeight": function ( nTr )
	{
		delete nTr._DTTC_iHeight;
		nTr.style.height = 'auto';
	},


	/**
	 * Set the height of a given row - provides cross browser compatibility
	 *  @param   {Node} nTarget TR element that should have it's height recalculated
	 *  @param   {int} iHeight Height in pixels to set
	 *  @returns {void}
	 *  @example
	 *      var table = $('#example').dataTable( {
	 *          "scrollX": "100%"
	 *      } );
	 *      var fc = new $.fn.dataTable.fixedColumns( table );
	 *
	 *      // You may want to do this after manipulating a row in the fixed column
	 *      fc.fnSetRowHeight( $('#example tbody tr:eq(0)')[0], 50 );
	 */
	"fnSetRowHeight": function ( nTarget, iHeight )
	{
		nTarget.style.height = iHeight+"px";
	},


	/**
	 * Get data index information about a row or cell in the table body.
	 * This function is functionally identical to fnGetPosition in DataTables,
	 * taking the same parameter (TH, TD or TR node) and returning exactly the
	 * the same information (data index information). THe difference between
	 * the two is that this method takes into account the fixed columns in the
	 * table, so you can pass in nodes from the master table, or the cloned
	 * tables and get the index position for the data in the main table.
	 *  @param {node} node TR, TH or TD element to get the information about
	 *  @returns {int} If nNode is given as a TR, then a single index is 
	 *    returned, or if given as a cell, an array of [row index, column index
	 *    (visible), column index (all)] is given.
	 */
	"fnGetPosition": function ( node )
	{
		var idx;
		var inst = this.s.dt.oInstance;

		if ( ! $(node).parents('.DTFC_Cloned').length )
		{
			// Not in a cloned table
			return inst.fnGetPosition( node );
		}
		else
		{
			// Its in the cloned table, so need to look up position
			if ( node.nodeName.toLowerCase() === 'tr' ) {
				idx = $(node).index();
				return inst.fnGetPosition( $('tr', this.s.dt.nTBody)[ idx ] );
			}
			else
			{
				var colIdx = $(node).index();
				idx = $(node.parentNode).index();
				var row = inst.fnGetPosition( $('tr', this.s.dt.nTBody)[ idx ] );

				return [
					row,
					colIdx,
					inst.oApi._fnVisibleToColumnIndex( this.s.dt, colIdx )
				];
			}
		}
	},



	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Private methods (they are of course public in JS, but recommended as private)
	 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

	/**
	 * Initialisation for FixedColumns
	 *  @param   {Object} oInit User settings for initialisation
	 *  @returns {void}
	 *  @private
	 */
	"_fnConstruct": function ( oInit )
	{
		var i, iLen, iWidth,
			that = this;

		/* Sanity checking */
		if ( typeof this.s.dt.oInstance.fnVersionCheck != 'function' ||
		     this.s.dt.oInstance.fnVersionCheck( '1.8.0' ) !== true )
		{
			alert( "FixedColumns "+FixedColumns.VERSION+" required DataTables 1.8.0 or later. "+
				"Please upgrade your DataTables installation" );
			return;
		}

		if ( this.s.dt.oScroll.sX === "" )
		{
			this.s.dt.oInstance.oApi._fnLog( this.s.dt, 1, "FixedColumns is not needed (no "+
				"x-scrolling in DataTables enabled), so no action will be taken. Use 'FixedHeader' for "+
				"column fixing when scrolling is not enabled" );
			return;
		}

		/* Apply the settings from the user / defaults */
		this.s = $.extend( true, this.s, FixedColumns.defaults, oInit );

		/* Set up the DOM as we need it and cache nodes */
		var classes = this.s.dt.oClasses;
		this.dom.grid.dt = $(this.s.dt.nTable).parents('div.'+classes.sScrollWrapper)[0];
		this.dom.scroller = $('div.'+classes.sScrollBody, this.dom.grid.dt )[0];

		/* Set up the DOM that we want for the fixed column layout grid */
		this._fnColCalc();
		this._fnGridSetup();

		/* Event handlers */
		var mouseController;
		var mouseDown = false;

		// When the mouse is down (drag scroll) the mouse controller cannot
		// change, as the browser keeps the original element as the scrolling one
		$(this.s.dt.nTableWrapper).on( 'mousedown.DTFC', function (e) {
			if ( e.button === 0 ) {
				mouseDown = true;

				$(document).one( 'mouseup', function () {
					mouseDown = false;
				} );
			}
		} );

		// When the body is scrolled - scroll the left and right columns
		$(this.dom.scroller)
			.on( 'mouseover.DTFC touchstart.DTFC', function () {
				if ( ! mouseDown ) {
					mouseController = 'main';
				}
			} )
			.on( 'scroll.DTFC', function (e) {
				if ( ! mouseController && e.originalEvent ) {
					mouseController = 'main';
				}

				if ( mouseController === 'main' ) {
					if ( that.s.iLeftColumns > 0 ) {
						that.dom.grid.left.liner.scrollTop = that.dom.scroller.scrollTop;
					}
					if ( that.s.iRightColumns > 0 ) {
						that.dom.grid.right.liner.scrollTop = that.dom.scroller.scrollTop;
					}
				}
			} );

		var wheelType = 'onwheel' in document.createElement('div') ?
			'wheel.DTFC' :
			'mousewheel.DTFC';

		if ( that.s.iLeftColumns > 0 ) {
			// When scrolling the left column, scroll the body and right column
			$(that.dom.grid.left.liner)
				.on( 'mouseover.DTFC touchstart.DTFC', function () {
					if ( ! mouseDown ) {
						mouseController = 'left';
					}
				} )
				.on( 'scroll.DTFC', function ( e ) {
					if ( ! mouseController && e.originalEvent ) {
						mouseController = 'left';
					}

					if ( mouseController === 'left' ) {
						that.dom.scroller.scrollTop = that.dom.grid.left.liner.scrollTop;
						if ( that.s.iRightColumns > 0 ) {
							that.dom.grid.right.liner.scrollTop = that.dom.grid.left.liner.scrollTop;
						}
					}
				} )
				.on( wheelType, function(e) {
					// Pass horizontal scrolling through
					var xDelta = e.type === 'wheel' ?
						-e.originalEvent.deltaX :
						e.originalEvent.wheelDeltaX;
					that.dom.scroller.scrollLeft -= xDelta;
				} );
		}

		if ( that.s.iRightColumns > 0 ) {
			// When scrolling the right column, scroll the body and the left column
			$(that.dom.grid.right.liner)
				.on( 'mouseover.DTFC touchstart.DTFC', function () {
					if ( ! mouseDown ) {
						mouseController = 'right';
					}
				} )
				.on( 'scroll.DTFC', function ( e ) {
					if ( ! mouseController && e.originalEvent ) {
						mouseController = 'right';
					}

					if ( mouseController === 'right' ) {
						that.dom.scroller.scrollTop = that.dom.grid.right.liner.scrollTop;
						if ( that.s.iLeftColumns > 0 ) {
							that.dom.grid.left.liner.scrollTop = that.dom.grid.right.liner.scrollTop;
						}
					}
				} )
				.on( wheelType, function(e) {
					// Pass horizontal scrolling through
					var xDelta = e.type === 'wheel' ?
						-e.originalEvent.deltaX :
						e.originalEvent.wheelDeltaX;
					that.dom.scroller.scrollLeft -= xDelta;
				} );
		}

		$(window).on( 'resize.DTFC', function () {
			that._fnGridLayout.call( that );
		} );

		var bFirstDraw = true;
		var jqTable = $(this.s.dt.nTable);

		jqTable
			.on( 'draw.dt.DTFC', function () {
				that._fnColCalc();
				that._fnDraw.call( that, bFirstDraw );
				bFirstDraw = false;
			} )
			.on( 'column-sizing.dt.DTFC', function () {
				that._fnColCalc();
				that._fnGridLayout( that );
			} )
			.on( 'column-visibility.dt.DTFC', function ( e, settings, column, vis, recalc ) {
				if ( recalc === undefined || recalc ) {
					that._fnColCalc();
					that._fnGridLayout( that );
					that._fnDraw( true );
				}
			} )
			.on( 'select.dt.DTFC deselect.dt.DTFC', function ( e, dt, type, indexes ) {
				if ( e.namespace === 'dt' ) {
					that._fnDraw( false );
				}
			} )
			.on( 'destroy.dt.DTFC', function () {
				jqTable.off( '.DTFC' );

				$(that.dom.scroller).off( '.DTFC' );
				$(window).off( '.DTFC' );
				$(that.s.dt.nTableWrapper).off( '.DTFC' );

				$(that.dom.grid.left.liner).off( '.DTFC '+wheelType );
				$(that.dom.grid.left.wrapper).remove();

				$(that.dom.grid.right.liner).off( '.DTFC '+wheelType );
				$(that.dom.grid.right.wrapper).remove();
			} );

		/* Get things right to start with - note that due to adjusting the columns, there must be
		 * another redraw of the main table. It doesn't need to be a full redraw however.
		 */
		this._fnGridLayout();
		this.s.dt.oInstance.fnDraw(false);
	},


	/**
	 * Calculate the column widths for the grid layout
	 *  @returns {void}
	 *  @private
	 */
	"_fnColCalc": function ()
	{
		var that = this;
		var iLeftWidth = 0;
		var iRightWidth = 0;

		this.s.aiInnerWidths = [];
		this.s.aiOuterWidths = [];

		$.each( this.s.dt.aoColumns, function (i, col) {
			var th = $(col.nTh);
			var border;

			if ( ! th.filter(':visible').length ) {
				that.s.aiInnerWidths.push( 0 );
				that.s.aiOuterWidths.push( 0 );
			}
			else
			{
				// Inner width is used to assign widths to cells
				// Outer width is used to calculate the container
				var iWidth = th.outerWidth();

				// When working with the left most-cell, need to add on the
				// table's border to the outerWidth, since we need to take
				// account of it, but it isn't in any cell
				if ( that.s.aiOuterWidths.length === 0 ) {
					border = $(that.s.dt.nTable).css('border-left-width');
					iWidth += typeof border === 'string' && border.indexOf('px') === -1 ?
						1 :
						parseInt( border, 10 );
				}

				// Likewise with the final column on the right
				if ( that.s.aiOuterWidths.length === that.s.dt.aoColumns.length-1 ) {
					border = $(that.s.dt.nTable).css('border-right-width');
					iWidth += typeof border === 'string' && border.indexOf('px') === -1 ?
						1 :
						parseInt( border, 10 );
				}

				that.s.aiOuterWidths.push( iWidth );
				that.s.aiInnerWidths.push( th.width() );

				if ( i < that.s.iLeftColumns )
				{
					iLeftWidth += iWidth;
				}

				if ( that.s.iTableColumns-that.s.iRightColumns <= i )
				{
					iRightWidth += iWidth;
				}
			}
		} );

		this.s.iLeftWidth = iLeftWidth;
		this.s.iRightWidth = iRightWidth;
	},


	/**
	 * Set up the DOM for the fixed column. The way the layout works is to create a 1x3 grid
	 * for the left column, the DataTable (for which we just reuse the scrolling element DataTable
	 * puts into the DOM) and the right column. In each of he two fixed column elements there is a
	 * grouping wrapper element and then a head, body and footer wrapper. In each of these we then
	 * place the cloned header, body or footer tables. This effectively gives as 3x3 grid structure.
	 *  @returns {void}
	 *  @private
	 */
	"_fnGridSetup": function ()
	{
		var that = this;
		var oOverflow = this._fnDTOverflow();
		var block;

		this.dom.body = this.s.dt.nTable;
		this.dom.header = this.s.dt.nTHead.parentNode;
		this.dom.header.parentNode.parentNode.style.position = "relative";

		var nSWrapper =
			$('<div class="DTFC_ScrollWrapper" style="position:relative; clear:both;">'+
				'<div class="DTFC_LeftWrapper" style="position:absolute; top:0; left:0;" aria-hidden="true">'+
					'<div class="DTFC_LeftHeadWrapper" style="position:relative; top:0; left:0; overflow:hidden;"></div>'+
					'<div class="DTFC_LeftBodyWrapper" style="position:relative; top:0; left:0; overflow:hidden;">'+
						'<div class="DTFC_LeftBodyLiner" style="position:relative; top:0; left:0; overflow-y:scroll;"></div>'+
					'</div>'+
					'<div class="DTFC_LeftFootWrapper" style="position:relative; top:0; left:0; overflow:hidden;"></div>'+
				'</div>'+
				'<div class="DTFC_RightWrapper" style="position:absolute; top:0; right:0;" aria-hidden="true">'+
					'<div class="DTFC_RightHeadWrapper" style="position:relative; top:0; left:0;">'+
						'<div class="DTFC_RightHeadBlocker DTFC_Blocker" style="position:absolute; top:0; bottom:0;"></div>'+
					'</div>'+
					'<div class="DTFC_RightBodyWrapper" style="position:relative; top:0; left:0; overflow:hidden;">'+
						'<div class="DTFC_RightBodyLiner" style="position:relative; top:0; left:0; overflow-y:scroll;"></div>'+
					'</div>'+
					'<div class="DTFC_RightFootWrapper" style="position:relative; top:0; left:0;">'+
						'<div class="DTFC_RightFootBlocker DTFC_Blocker" style="position:absolute; top:0; bottom:0;"></div>'+
					'</div>'+
				'</div>'+
			'</div>')[0];
		var nLeft = nSWrapper.childNodes[0];
		var nRight = nSWrapper.childNodes[1];

		this.dom.grid.dt.parentNode.insertBefore( nSWrapper, this.dom.grid.dt );
		nSWrapper.appendChild( this.dom.grid.dt );

		this.dom.grid.wrapper = nSWrapper;

		if ( this.s.iLeftColumns > 0 )
		{
			this.dom.grid.left.wrapper = nLeft;
			this.dom.grid.left.head = nLeft.childNodes[0];
			this.dom.grid.left.body = nLeft.childNodes[1];
			this.dom.grid.left.liner = $('div.DTFC_LeftBodyLiner', nSWrapper)[0];

			nSWrapper.appendChild( nLeft );
		}

		if ( this.s.iRightColumns > 0 )
		{
			this.dom.grid.right.wrapper = nRight;
			this.dom.grid.right.head = nRight.childNodes[0];
			this.dom.grid.right.body = nRight.childNodes[1];
			this.dom.grid.right.liner = $('div.DTFC_RightBodyLiner', nSWrapper)[0];

			nRight.style.right = oOverflow.bar+"px";

			block = $('div.DTFC_RightHeadBlocker', nSWrapper)[0];
			block.style.width = oOverflow.bar+"px";
			block.style.right = -oOverflow.bar+"px";
			this.dom.grid.right.headBlock = block;

			block = $('div.DTFC_RightFootBlocker', nSWrapper)[0];
			block.style.width = oOverflow.bar+"px";
			block.style.right = -oOverflow.bar+"px";
			this.dom.grid.right.footBlock = block;

			nSWrapper.appendChild( nRight );
		}

		if ( this.s.dt.nTFoot )
		{
			this.dom.footer = this.s.dt.nTFoot.parentNode;
			if ( this.s.iLeftColumns > 0 )
			{
				this.dom.grid.left.foot = nLeft.childNodes[2];
			}
			if ( this.s.iRightColumns > 0 )
			{
				this.dom.grid.right.foot = nRight.childNodes[2];
			}
		}

		// RTL support - swap the position of the left and right columns (#48)
		if ( this.s.rtl ) {
			$('div.DTFC_RightHeadBlocker', nSWrapper).css( {
				left: -oOverflow.bar+'px',
				right: ''
			} );
		}
	},


	/**
	 * Style and position the grid used for the FixedColumns layout
	 *  @returns {void}
	 *  @private
	 */
	"_fnGridLayout": function ()
	{
		var that = this;
		var oGrid = this.dom.grid;
		var iWidth = $(oGrid.wrapper).width();
		var iBodyHeight = this.s.dt.nTable.parentNode.offsetHeight;
		var iFullHeight = this.s.dt.nTable.parentNode.parentNode.offsetHeight;
		var oOverflow = this._fnDTOverflow();
		var iLeftWidth = this.s.iLeftWidth;
		var iRightWidth = this.s.iRightWidth;
		var rtl = $(this.dom.body).css('direction') === 'rtl';
		var wrapper;
		var scrollbarAdjust = function ( node, width ) {
			if ( ! oOverflow.bar ) {
				// If there is no scrollbar (Macs) we need to hide the auto scrollbar
				node.style.width = (width+20)+"px";
				node.style.paddingRight = "20px";
				node.style.boxSizing = "border-box";
			}
			else if ( that._firefoxScrollError() ) {
				// See the above function for why this is required
				if ( $(node).height() > 34 ) {
					node.style.width = (width+oOverflow.bar)+"px";
				}
			}
			else {
				// Otherwise just overflow by the scrollbar
				node.style.width = (width+oOverflow.bar)+"px";
			}
		};

		// When x scrolling - don't paint the fixed columns over the x scrollbar
		if ( oOverflow.x )
		{
			iBodyHeight -= oOverflow.bar;
		}

		oGrid.wrapper.style.height = iFullHeight+"px";

		if ( this.s.iLeftColumns > 0 )
		{
			wrapper = oGrid.left.wrapper;
			wrapper.style.width = iLeftWidth+'px';
			wrapper.style.height = '1px';

			// Swap the position of the left and right columns for rtl (#48)
			// This is always up against the edge, scrollbar on the far side
			if ( rtl ) {
				wrapper.style.left = '';
				wrapper.style.right = 0;
			}
			else {
				wrapper.style.left = 0;
				wrapper.style.right = '';
			}

			oGrid.left.body.style.height = iBodyHeight+"px";
			if ( oGrid.left.foot ) {
				oGrid.left.foot.style.top = (oOverflow.x ? oOverflow.bar : 0)+"px"; // shift footer for scrollbar
			}

			scrollbarAdjust( oGrid.left.liner, iLeftWidth );
			oGrid.left.liner.style.height = iBodyHeight+"px";
			oGrid.left.liner.style.maxHeight = iBodyHeight+"px";
		}

		if ( this.s.iRightColumns > 0 )
		{
			wrapper = oGrid.right.wrapper;
			wrapper.style.width = iRightWidth+'px';
			wrapper.style.height = '1px';

			// Need to take account of the vertical scrollbar
			if ( this.s.rtl ) {
				wrapper.style.left = oOverflow.y ? oOverflow.bar+'px' : 0;
				wrapper.style.right = '';
			}
			else {
				wrapper.style.left = '';
				wrapper.style.right = oOverflow.y ? oOverflow.bar+'px' : 0;
			}

			oGrid.right.body.style.height = iBodyHeight+"px";
			if ( oGrid.right.foot ) {
				oGrid.right.foot.style.top = (oOverflow.x ? oOverflow.bar : 0)+"px";
			}

			scrollbarAdjust( oGrid.right.liner, iRightWidth );
			oGrid.right.liner.style.height = iBodyHeight+"px";
			oGrid.right.liner.style.maxHeight = iBodyHeight+"px";

			oGrid.right.headBlock.style.display = oOverflow.y ? 'block' : 'none';
			oGrid.right.footBlock.style.display = oOverflow.y ? 'block' : 'none';
		}
	},


	/**
	 * Get information about the DataTable's scrolling state - specifically if the table is scrolling
	 * on either the x or y axis, and also the scrollbar width.
	 *  @returns {object} Information about the DataTables scrolling state with the properties:
	 *    'x', 'y' and 'bar'
	 *  @private
	 */
	"_fnDTOverflow": function ()
	{
		var nTable = this.s.dt.nTable;
		var nTableScrollBody = nTable.parentNode;
		var out = {
			"x": false,
			"y": false,
			"bar": this.s.dt.oScroll.iBarWidth
		};

		if ( nTable.offsetWidth > nTableScrollBody.clientWidth )
		{
			out.x = true;
		}

		if ( nTable.offsetHeight > nTableScrollBody.clientHeight )
		{
			out.y = true;
		}

		return out;
	},


	/**
	 * Clone and position the fixed columns
	 *  @returns {void}
	 *  @param   {Boolean} bAll Indicate if the header and footer should be updated as well (true)
	 *  @private
	 */
	"_fnDraw": function ( bAll )
	{
		this._fnGridLayout();
		this._fnCloneLeft( bAll );
		this._fnCloneRight( bAll );

		/* Draw callback function */
		if ( this.s.fnDrawCallback !== null )
		{
			this.s.fnDrawCallback.call( this, this.dom.clone.left, this.dom.clone.right );
		}

		/* Event triggering */
		$(this).trigger( 'draw.dtfc', {
			"leftClone": this.dom.clone.left,
			"rightClone": this.dom.clone.right
		} );
	},


	/**
	 * Clone the right columns
	 *  @returns {void}
	 *  @param   {Boolean} bAll Indicate if the header and footer should be updated as well (true)
	 *  @private
	 */
	"_fnCloneRight": function ( bAll )
	{
		if ( this.s.iRightColumns <= 0 ) {
			return;
		}

		var that = this,
			i, jq,
			aiColumns = [];

		for ( i=this.s.iTableColumns-this.s.iRightColumns ; i<this.s.iTableColumns ; i++ ) {
			if ( this.s.dt.aoColumns[i].bVisible ) {
				aiColumns.push( i );
			}
		}

		this._fnClone( this.dom.clone.right, this.dom.grid.right, aiColumns, bAll );
	},


	/**
	 * Clone the left columns
	 *  @returns {void}
	 *  @param   {Boolean} bAll Indicate if the header and footer should be updated as well (true)
	 *  @private
	 */
	"_fnCloneLeft": function ( bAll )
	{
		if ( this.s.iLeftColumns <= 0 ) {
			return;
		}

		var that = this,
			i, jq,
			aiColumns = [];

		for ( i=0 ; i<this.s.iLeftColumns ; i++ ) {
			if ( this.s.dt.aoColumns[i].bVisible ) {
				aiColumns.push( i );
			}
		}

		this._fnClone( this.dom.clone.left, this.dom.grid.left, aiColumns, bAll );
	},


	/**
	 * Make a copy of the layout object for a header or footer element from DataTables. Note that
	 * this method will clone the nodes in the layout object.
	 *  @returns {Array} Copy of the layout array
	 *  @param   {Object} aoOriginal Layout array from DataTables (aoHeader or aoFooter)
	 *  @param   {Object} aiColumns Columns to copy
	 *  @param   {boolean} events Copy cell events or not
	 *  @private
	 */
	"_fnCopyLayout": function ( aoOriginal, aiColumns, events )
	{
		var aReturn = [];
		var aClones = [];
		var aCloned = [];

		for ( var i=0, iLen=aoOriginal.length ; i<iLen ; i++ )
		{
			var aRow = [];
			aRow.nTr = $(aoOriginal[i].nTr).clone(events, false)[0];

			for ( var j=0, jLen=this.s.iTableColumns ; j<jLen ; j++ )
			{
				if ( $.inArray( j, aiColumns ) === -1 )
				{
					continue;
				}

				var iCloned = $.inArray( aoOriginal[i][j].cell, aCloned );
				if ( iCloned === -1 )
				{
					var nClone = $(aoOriginal[i][j].cell).clone(events, false)[0];
					aClones.push( nClone );
					aCloned.push( aoOriginal[i][j].cell );

					aRow.push( {
						"cell": nClone,
						"unique": aoOriginal[i][j].unique
					} );
				}
				else
				{
					aRow.push( {
						"cell": aClones[ iCloned ],
						"unique": aoOriginal[i][j].unique
					} );
				}
			}

			aReturn.push( aRow );
		}

		return aReturn;
	},


	/**
	 * Clone the DataTable nodes and place them in the DOM (sized correctly)
	 *  @returns {void}
	 *  @param   {Object} oClone Object containing the header, footer and body cloned DOM elements
	 *  @param   {Object} oGrid Grid object containing the display grid elements for the cloned
	 *                    column (left or right)
	 *  @param   {Array} aiColumns Column indexes which should be operated on from the DataTable
	 *  @param   {Boolean} bAll Indicate if the header and footer should be updated as well (true)
	 *  @private
	 */
	"_fnClone": function ( oClone, oGrid, aiColumns, bAll )
	{
		var that = this,
			i, iLen, j, jLen, jq, nTarget, iColumn, nClone, iIndex, aoCloneLayout,
			jqCloneThead, aoFixedHeader,
			dt = this.s.dt;

		/*
		 * Header
		 */
		if ( bAll )
		{
			$(oClone.header).remove();

			oClone.header = $(this.dom.header).clone(true, false)[0];
			oClone.header.className += " DTFC_Cloned";
			oClone.header.style.width = "100%";
			oGrid.head.appendChild( oClone.header );

			/* Copy the DataTables layout cache for the header for our floating column */
			aoCloneLayout = this._fnCopyLayout( dt.aoHeader, aiColumns, true );
			jqCloneThead = $('>thead', oClone.header);
			jqCloneThead.empty();

			/* Add the created cloned TR elements to the table */
			for ( i=0, iLen=aoCloneLayout.length ; i<iLen ; i++ )
			{
				jqCloneThead[0].appendChild( aoCloneLayout[i].nTr );
			}

			/* Use the handy _fnDrawHead function in DataTables to do the rowspan/colspan
			 * calculations for us
			 */
			dt.oApi._fnDrawHead( dt, aoCloneLayout, true );
		}
		else
		{
			/* To ensure that we copy cell classes exactly, regardless of colspan, multiple rows
			 * etc, we make a copy of the header from the DataTable again, but don't insert the
			 * cloned cells, just copy the classes across. To get the matching layout for the
			 * fixed component, we use the DataTables _fnDetectHeader method, allowing 1:1 mapping
			 */
			aoCloneLayout = this._fnCopyLayout( dt.aoHeader, aiColumns, false );
			aoFixedHeader=[];

			dt.oApi._fnDetectHeader( aoFixedHeader, $('>thead', oClone.header)[0] );

			for ( i=0, iLen=aoCloneLayout.length ; i<iLen ; i++ )
			{
				for ( j=0, jLen=aoCloneLayout[i].length ; j<jLen ; j++ )
				{
					aoFixedHeader[i][j].cell.className = aoCloneLayout[i][j].cell.className;

					// If jQuery UI theming is used we need to copy those elements as well
					$('span.DataTables_sort_icon', aoFixedHeader[i][j].cell).each( function () {
						this.className = $('span.DataTables_sort_icon', aoCloneLayout[i][j].cell)[0].className;
					} );
				}
			}
		}
		this._fnEqualiseHeights( 'thead', this.dom.header, oClone.header );

		/*
		 * Body
		 */
		if ( this.s.sHeightMatch == 'auto' )
		{
			/* Remove any heights which have been applied already and let the browser figure it out */
			$('>tbody>tr', that.dom.body).css('height', 'auto');
		}

		if ( oClone.body !== null )
		{
			$(oClone.body).remove();
			oClone.body = null;
		}

		oClone.body = $(this.dom.body).clone(true)[0];
		oClone.body.className += " DTFC_Cloned";
		oClone.body.style.paddingBottom = dt.oScroll.iBarWidth+"px";
		oClone.body.style.marginBottom = (dt.oScroll.iBarWidth*2)+"px"; /* For IE */
		if ( oClone.body.getAttribute('id') !== null )
		{
			oClone.body.removeAttribute('id');
		}

		$('>thead>tr', oClone.body).empty();
		$('>tfoot', oClone.body).remove();

		var nBody = $('tbody', oClone.body)[0];
		$(nBody).empty();
		if ( dt.aiDisplay.length > 0 )
		{
			/* Copy the DataTables' header elements to force the column width in exactly the
			 * same way that DataTables does it - have the header element, apply the width and
			 * colapse it down
			 */
			var nInnerThead = $('>thead>tr', oClone.body)[0];
			for ( iIndex=0 ; iIndex<aiColumns.length ; iIndex++ )
			{
				iColumn = aiColumns[iIndex];

				nClone = $(dt.aoColumns[iColumn].nTh).clone(true)[0];
				nClone.innerHTML = "";

				var oStyle = nClone.style;
				oStyle.paddingTop = "0";
				oStyle.paddingBottom = "0";
				oStyle.borderTopWidth = "0";
				oStyle.borderBottomWidth = "0";
				oStyle.height = 0;
				oStyle.width = that.s.aiInnerWidths[iColumn]+"px";

				nInnerThead.appendChild( nClone );
			}

			/* Add in the tbody elements, cloning form the master table */
			$('>tbody>tr', that.dom.body).each( function (z) {
				var i = that.s.dt.oFeatures.bServerSide===false ?
					that.s.dt.aiDisplay[ that.s.dt._iDisplayStart+z ] : z;
				var aTds = that.s.dt.aoData[ i ].anCells || $(this).children('td, th');

				var n = this.cloneNode(false);
				n.removeAttribute('id');
				n.setAttribute( 'data-dt-row', i );

				for ( iIndex=0 ; iIndex<aiColumns.length ; iIndex++ )
				{
					iColumn = aiColumns[iIndex];

					if ( aTds.length > 0 )
					{
						nClone = $( aTds[iColumn] ).clone(true, true)[0];
						nClone.removeAttribute( 'id' );
						nClone.setAttribute( 'data-dt-row', i );
						nClone.setAttribute( 'data-dt-column', iColumn );
						n.appendChild( nClone );
					}
				}
				nBody.appendChild( n );
			} );
		}
		else
		{
			$('>tbody>tr', that.dom.body).each( function (z) {
				nClone = this.cloneNode(true);
				nClone.className += ' DTFC_NoData';
				$('td', nClone).html('');
				nBody.appendChild( nClone );
			} );
		}

		oClone.body.style.width = "100%";
		oClone.body.style.margin = "0";
		oClone.body.style.padding = "0";

		// Interop with Scroller - need to use a height forcing element in the
		// scrolling area in the same way that Scroller does in the body scroll.
		if ( dt.oScroller !== undefined )
		{
			var scrollerForcer = dt.oScroller.dom.force;

			if ( ! oGrid.forcer ) {
				oGrid.forcer = scrollerForcer.cloneNode( true );
				oGrid.liner.appendChild( oGrid.forcer );
			}
			else {
				oGrid.forcer.style.height = scrollerForcer.style.height;
			}
		}

		oGrid.liner.appendChild( oClone.body );

		this._fnEqualiseHeights( 'tbody', that.dom.body, oClone.body );

		/*
		 * Footer
		 */
		if ( dt.nTFoot !== null )
		{
			if ( bAll )
			{
				if ( oClone.footer !== null )
				{
					oClone.footer.parentNode.removeChild( oClone.footer );
				}
				oClone.footer = $(this.dom.footer).clone(true, true)[0];
				oClone.footer.className += " DTFC_Cloned";
				oClone.footer.style.width = "100%";
				oGrid.foot.appendChild( oClone.footer );

				/* Copy the footer just like we do for the header */
				aoCloneLayout = this._fnCopyLayout( dt.aoFooter, aiColumns, true );
				var jqCloneTfoot = $('>tfoot', oClone.footer);
				jqCloneTfoot.empty();

				for ( i=0, iLen=aoCloneLayout.length ; i<iLen ; i++ )
				{
					jqCloneTfoot[0].appendChild( aoCloneLayout[i].nTr );
				}
				dt.oApi._fnDrawHead( dt, aoCloneLayout, true );
			}
			else
			{
				aoCloneLayout = this._fnCopyLayout( dt.aoFooter, aiColumns, false );
				var aoCurrFooter=[];

				dt.oApi._fnDetectHeader( aoCurrFooter, $('>tfoot', oClone.footer)[0] );

				for ( i=0, iLen=aoCloneLayout.length ; i<iLen ; i++ )
				{
					for ( j=0, jLen=aoCloneLayout[i].length ; j<jLen ; j++ )
					{
						aoCurrFooter[i][j].cell.className = aoCloneLayout[i][j].cell.className;
					}
				}
			}
			this._fnEqualiseHeights( 'tfoot', this.dom.footer, oClone.footer );
		}

		/* Equalise the column widths between the header footer and body - body get's priority */
		var anUnique = dt.oApi._fnGetUniqueThs( dt, $('>thead', oClone.header)[0] );
		$(anUnique).each( function (i) {
			iColumn = aiColumns[i];
			this.style.width = that.s.aiInnerWidths[iColumn]+"px";
		} );

		if ( that.s.dt.nTFoot !== null )
		{
			anUnique = dt.oApi._fnGetUniqueThs( dt, $('>tfoot', oClone.footer)[0] );
			$(anUnique).each( function (i) {
				iColumn = aiColumns[i];
				this.style.width = that.s.aiInnerWidths[iColumn]+"px";
			} );
		}
	},


	/**
	 * From a given table node (THEAD etc), get a list of TR direct child elements
	 *  @param   {Node} nIn Table element to search for TR elements (THEAD, TBODY or TFOOT element)
	 *  @returns {Array} List of TR elements found
	 *  @private
	 */
	"_fnGetTrNodes": function ( nIn )
	{
		var aOut = [];
		for ( var i=0, iLen=nIn.childNodes.length ; i<iLen ; i++ )
		{
			if ( nIn.childNodes[i].nodeName.toUpperCase() == "TR" )
			{
				aOut.push( nIn.childNodes[i] );
			}
		}
		return aOut;
	},


	/**
	 * Equalise the heights of the rows in a given table node in a cross browser way
	 *  @returns {void}
	 *  @param   {String} nodeName Node type - thead, tbody or tfoot
	 *  @param   {Node} original Original node to take the heights from
	 *  @param   {Node} clone Copy the heights to
	 *  @private
	 */
	"_fnEqualiseHeights": function ( nodeName, original, clone )
	{
		if ( this.s.sHeightMatch == 'none' && nodeName !== 'thead' && nodeName !== 'tfoot' )
		{
			return;
		}

		var that = this,
			i, iLen, iHeight, iHeight2, iHeightOriginal, iHeightClone,
			rootOriginal = original.getElementsByTagName(nodeName)[0],
			rootClone    = clone.getElementsByTagName(nodeName)[0],
			jqBoxHack    = $('>'+nodeName+'>tr:eq(0)', original).children(':first'),
			iBoxHack     = jqBoxHack.outerHeight() - jqBoxHack.height(),
			anOriginal   = this._fnGetTrNodes( rootOriginal ),
			anClone      = this._fnGetTrNodes( rootClone ),
			heights      = [];

		for ( i=0, iLen=anClone.length ; i<iLen ; i++ )
		{
			iHeightOriginal = anOriginal[i].offsetHeight;
			iHeightClone = anClone[i].offsetHeight;
			iHeight = iHeightClone > iHeightOriginal ? iHeightClone : iHeightOriginal;

			if ( this.s.sHeightMatch == 'semiauto' )
			{
				anOriginal[i]._DTTC_iHeight = iHeight;
			}

			heights.push( iHeight );
		}

		for ( i=0, iLen=anClone.length ; i<iLen ; i++ )
		{
			anClone[i].style.height = heights[i]+"px";
			anOriginal[i].style.height = heights[i]+"px";
		}
	},

	/**
	 * Determine if the UA suffers from Firefox's overflow:scroll scrollbars
	 * not being shown bug.
	 *
	 * Firefox doesn't draw scrollbars, even if it is told to using
	 * overflow:scroll, if the div is less than 34px height. See bugs 292284 and
	 * 781885. Using UA detection here since this is particularly hard to detect
	 * using objects - its a straight up rendering error in Firefox.
	 *
	 * @return {boolean} True if Firefox error is present, false otherwise
	 */
	_firefoxScrollError: function () {
		if ( _firefoxScroll === undefined ) {
			var test = $('<div/>')
				.css( {
					position: 'absolute',
					top: 0,
					left: 0,
					height: 10,
					width: 50,
					overflow: 'scroll'
				} )
				.appendTo( 'body' );

			// Make sure this doesn't apply on Macs with 0 width scrollbars
			_firefoxScroll = (
				test[0].clientWidth === test[0].offsetWidth && this._fnDTOverflow().bar !== 0
			);

			test.remove();
		}

		return _firefoxScroll;
	}
} );



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Statics
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

/**
 * FixedColumns default settings for initialisation
 *  @name FixedColumns.defaults
 *  @namespace
 *  @static
 */
FixedColumns.defaults = /** @lends FixedColumns.defaults */{
	/**
	 * Number of left hand columns to fix in position
	 *  @type     int
	 *  @default  1
	 *  @static
	 *  @example
	 *      var  = $('#example').dataTable( {
	 *          "scrollX": "100%"
	 *      } );
	 *      new $.fn.dataTable.fixedColumns( table, {
	 *          "leftColumns": 2
	 *      } );
	 */
	"iLeftColumns": 1,

	/**
	 * Number of right hand columns to fix in position
	 *  @type     int
	 *  @default  0
	 *  @static
	 *  @example
	 *      var table = $('#example').dataTable( {
	 *          "scrollX": "100%"
	 *      } );
	 *      new $.fn.dataTable.fixedColumns( table, {
	 *          "rightColumns": 1
	 *      } );
	 */
	"iRightColumns": 0,

	/**
	 * Draw callback function which is called when FixedColumns has redrawn the fixed assets
	 *  @type     function(object, object):void
	 *  @default  null
	 *  @static
	 *  @example
	 *      var table = $('#example').dataTable( {
	 *          "scrollX": "100%"
	 *      } );
	 *      new $.fn.dataTable.fixedColumns( table, {
	 *          "drawCallback": function () {
	 *	            alert( "FixedColumns redraw" );
	 *	        }
	 *      } );
	 */
	"fnDrawCallback": null,

	/**
	 * Height matching algorthim to use. This can be "none" which will result in no height
	 * matching being applied by FixedColumns (height matching could be forced by CSS in this
	 * case), "semiauto" whereby the height calculation will be performed once, and the result
	 * cached to be used again (fnRecalculateHeight can be used to force recalculation), or
	 * "auto" when height matching is performed on every draw (slowest but must accurate)
	 *  @type     string
	 *  @default  semiauto
	 *  @static
	 *  @example
	 *      var table = $('#example').dataTable( {
	 *          "scrollX": "100%"
	 *      } );
	 *      new $.fn.dataTable.fixedColumns( table, {
	 *          "heightMatch": "auto"
	 *      } );
	 */
	"sHeightMatch": "semiauto"
};




/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Constants
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

/**
 * FixedColumns version
 *  @name      FixedColumns.version
 *  @type      String
 *  @default   See code
 *  @static
 */
FixedColumns.version = "3.2.5";



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * DataTables API integration
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

DataTable.Api.register( 'fixedColumns()', function () {
	return this;
} );

DataTable.Api.register( 'fixedColumns().update()', function () {
	return this.iterator( 'table', function ( ctx ) {
		if ( ctx._oFixedColumns ) {
			ctx._oFixedColumns.fnUpdate();
		}
	} );
} );

DataTable.Api.register( 'fixedColumns().relayout()', function () {
	return this.iterator( 'table', function ( ctx ) {
		if ( ctx._oFixedColumns ) {
			ctx._oFixedColumns.fnRedrawLayout();
		}
	} );
} );

DataTable.Api.register( 'rows().recalcHeight()', function () {
	return this.iterator( 'row', function ( ctx, idx ) {
		if ( ctx._oFixedColumns ) {
			ctx._oFixedColumns.fnRecalculateHeight( this.row(idx).node() );
		}
	} );
} );

DataTable.Api.register( 'fixedColumns().rowIndex()', function ( row ) {
	row = $(row);

	return row.parents('.DTFC_Cloned').length ?
		this.rows( { page: 'current' } ).indexes()[ row.index() ] :
		this.row( row ).index();
} );

DataTable.Api.register( 'fixedColumns().cellIndex()', function ( cell ) {
	cell = $(cell);

	if ( cell.parents('.DTFC_Cloned').length ) {
		var rowClonedIdx = cell.parent().index();
		var rowIdx = this.rows( { page: 'current' } ).indexes()[ rowClonedIdx ];
		var columnIdx;

		if ( cell.parents('.DTFC_LeftWrapper').length ) {
			columnIdx = cell.index();
		}
		else {
			var columns = this.columns().flatten().length;
			columnIdx = columns - this.context[0]._oFixedColumns.s.iRightColumns + cell.index();
		}

		return {
			row: rowIdx,
			column: this.column.index( 'toData', columnIdx ),
			columnVisible: columnIdx
		};
	}
	else {
		return this.cell( cell ).index();
	}
} );




/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Initialisation
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

// Attach a listener to the document which listens for DataTables initialisation
// events so we can automatically initialise
$(document).on( 'init.dt.fixedColumns', function (e, settings) {
	if ( e.namespace !== 'dt' ) {
		return;
	}

	var init = settings.oInit.fixedColumns;
	var defaults = DataTable.defaults.fixedColumns;

	if ( init || defaults ) {
		var opts = $.extend( {}, init, defaults );

		if ( init !== false ) {
			new FixedColumns( settings, opts );
		}
	}
} );



// Make FixedColumns accessible from the DataTables instance
$.fn.dataTable.FixedColumns = FixedColumns;
$.fn.DataTable.FixedColumns = FixedColumns;

return FixedColumns;
}));


/*! FixedHeader 3.1.4
 * ©2009-2018 SpryMedia Ltd - datatables.net/license
 */

/**
 * @summary     FixedHeader
 * @description Fix a table's header or footer, so it is always visible while
 *              scrolling
 * @version     3.1.4
 * @file        dataTables.fixedHeader.js
 * @author      SpryMedia Ltd (www.sprymedia.co.uk)
 * @contact     www.sprymedia.co.uk/contact
 * @copyright   Copyright 2009-2018 SpryMedia Ltd.
 *
 * This source file is free software, available under the following license:
 *   MIT license - http://datatables.net/license/mit
 *
 * This source file is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the license files for details.
 *
 * For details please refer to: http://www.datatables.net
 */

(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net'], function ( $ ) {
			return factory( $, window, document );
		} );
	}
	else if ( typeof exports === 'object' ) {
		// CommonJS
		module.exports = function (root, $) {
			if ( ! root ) {
				root = window;
			}

			if ( ! $ || ! $.fn.dataTable ) {
				$ = require('datatables.net')(root, $).$;
			}

			return factory( $, root, root.document );
		};
	}
	else {
		// Browser
		factory( jQuery, window, document );
	}
}(function( $, window, document, undefined ) {
'use strict';
var DataTable = $.fn.dataTable;


var _instCounter = 0;

var FixedHeader = function ( dt, config ) {
	// Sanity check - you just know it will happen
	if ( ! (this instanceof FixedHeader) ) {
		throw "FixedHeader must be initialised with the 'new' keyword.";
	}

	// Allow a boolean true for defaults
	if ( config === true ) {
		config = {};
	}

	dt = new DataTable.Api( dt );

	this.c = $.extend( true, {}, FixedHeader.defaults, config );

	this.s = {
		dt: dt,
		position: {
			theadTop: 0,
			tbodyTop: 0,
			tfootTop: 0,
			tfootBottom: 0,
			width: 0,
			left: 0,
			tfootHeight: 0,
			theadHeight: 0,
			windowHeight: $(window).height(),
			visible: true
		},
		headerMode: null,
		footerMode: null,
		autoWidth: dt.settings()[0].oFeatures.bAutoWidth,
		namespace: '.dtfc'+(_instCounter++),
		scrollLeft: {
			header: -1,
			footer: -1
		},
		enable: true
	};

	this.dom = {
		floatingHeader: null,
		thead: $(dt.table().header()),
		tbody: $(dt.table().body()),
		tfoot: $(dt.table().footer()),
		header: {
			host: null,
			floating: null,
			placeholder: null
		},
		footer: {
			host: null,
			floating: null,
			placeholder: null
		}
	};

	this.dom.header.host = this.dom.thead.parent();
	this.dom.footer.host = this.dom.tfoot.parent();

	var dtSettings = dt.settings()[0];
	if ( dtSettings._fixedHeader ) {
		throw "FixedHeader already initialised on table "+dtSettings.nTable.id;
	}

	dtSettings._fixedHeader = this;

	this._constructor();
};


/*
 * Variable: FixedHeader
 * Purpose:  Prototype for FixedHeader
 * Scope:    global
 */
$.extend( FixedHeader.prototype, {
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * API methods
	 */
	
	/**
	 * Enable / disable the fixed elements
	 *
	 * @param  {boolean} enable `true` to enable, `false` to disable
	 */
	enable: function ( enable )
	{
		this.s.enable = enable;

		if ( this.c.header ) {
			this._modeChange( 'in-place', 'header', true );
		}

		if ( this.c.footer && this.dom.tfoot.length ) {
			this._modeChange( 'in-place', 'footer', true );
		}

		this.update();
	},
	
	/**
	 * Set header offset 
	 *
	 * @param  {int} new value for headerOffset
	 */
	headerOffset: function ( offset )
	{
		if ( offset !== undefined ) {
			this.c.headerOffset = offset;
			this.update();
		}

		return this.c.headerOffset;
	},
	
	/**
	 * Set footer offset
	 *
	 * @param  {int} new value for footerOffset
	 */
	footerOffset: function ( offset )
	{
		if ( offset !== undefined ) {
			this.c.footerOffset = offset;
			this.update();
		}

		return this.c.footerOffset;
	},

	
	/**
	 * Recalculate the position of the fixed elements and force them into place
	 */
	update: function ()
	{
		this._positions();
		this._scroll( true );
	},


	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Constructor
	 */
	
	/**
	 * FixedHeader constructor - adding the required event listeners and
	 * simple initialisation
	 *
	 * @private
	 */
	_constructor: function ()
	{
		var that = this;
		var dt = this.s.dt;

		$(window)
			.on( 'scroll'+this.s.namespace, function () {
				that._scroll();
			} )
			.on( 'resize'+this.s.namespace, DataTable.util.throttle( function () {
				that.s.position.windowHeight = $(window).height();
				that.update();
			}, 50 ) );

		var autoHeader = $('.fh-fixedHeader');
		if ( ! this.c.headerOffset && autoHeader.length ) {
			this.c.headerOffset = autoHeader.outerHeight();
		}

		var autoFooter = $('.fh-fixedFooter');
		if ( ! this.c.footerOffset && autoFooter.length ) {
			this.c.footerOffset = autoFooter.outerHeight();
		}

		dt.on( 'column-reorder.dt.dtfc column-visibility.dt.dtfc draw.dt.dtfc column-sizing.dt.dtfc responsive-display.dt.dtfc', function () {
			that.update();
		} );

		dt.on( 'destroy.dtfc', function () {
			if ( that.c.header ) {
				that._modeChange( 'in-place', 'header', true );
			}

			if ( that.c.footer && that.dom.tfoot.length ) {
				that._modeChange( 'in-place', 'footer', true );
			}

			dt.off( '.dtfc' );
			$(window).off( that.s.namespace );
		} );

		this._positions();
		this._scroll();
	},


	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Private methods
	 */

	/**
	 * Clone a fixed item to act as a place holder for the original element
	 * which is moved into a clone of the table element, and moved around the
	 * document to give the fixed effect.
	 *
	 * @param  {string}  item  'header' or 'footer'
	 * @param  {boolean} force Force the clone to happen, or allow automatic
	 *   decision (reuse existing if available)
	 * @private
	 */
	_clone: function ( item, force )
	{
		var dt = this.s.dt;
		var itemDom = this.dom[ item ];
		var itemElement = item === 'header' ?
			this.dom.thead :
			this.dom.tfoot;

		if ( ! force && itemDom.floating ) {
			// existing floating element - reuse it
			itemDom.floating.removeClass( 'fixedHeader-floating fixedHeader-locked' );
		}
		else {
			if ( itemDom.floating ) {
				itemDom.placeholder.remove();
				this._unsize( item );
				itemDom.floating.children().detach();
				itemDom.floating.remove();
			}

			itemDom.floating = $( dt.table().node().cloneNode( false ) )
				.css( 'table-layout', 'fixed' )
				.attr( 'aria-hidden', 'true' )
				.removeAttr( 'id' )
				.append( itemElement )
				.appendTo( 'body' );

			// Insert a fake thead/tfoot into the DataTable to stop it jumping around
			itemDom.placeholder = itemElement.clone( false )
			itemDom.placeholder
				.find( '*[id]' )
				.removeAttr( 'id' );

			itemDom.host.prepend( itemDom.placeholder );

			// Clone widths
			this._matchWidths( itemDom.placeholder, itemDom.floating );
		}
	},

	/**
	 * Copy widths from the cells in one element to another. This is required
	 * for the footer as the footer in the main table takes its sizes from the
	 * header columns. That isn't present in the footer so to have it still
	 * align correctly, the sizes need to be copied over. It is also required
	 * for the header when auto width is not enabled
	 *
	 * @param  {jQuery} from Copy widths from
	 * @param  {jQuery} to   Copy widths to
	 * @private
	 */
	_matchWidths: function ( from, to ) {
		var get = function ( name ) {
			return $(name, from)
				.map( function () {
					return $(this).width();
				} ).toArray();
		};

		var set = function ( name, toWidths ) {
			$(name, to).each( function ( i ) {
				$(this).css( {
					width: toWidths[i],
					minWidth: toWidths[i]
				} );
			} );
		};

		var thWidths = get( 'th' );
		var tdWidths = get( 'td' );

		set( 'th', thWidths );
		set( 'td', tdWidths );
	},

	/**
	 * Remove assigned widths from the cells in an element. This is required
	 * when inserting the footer back into the main table so the size is defined
	 * by the header columns and also when auto width is disabled in the
	 * DataTable.
	 *
	 * @param  {string} item The `header` or `footer`
	 * @private
	 */
	_unsize: function ( item ) {
		var el = this.dom[ item ].floating;

		if ( el && (item === 'footer' || (item === 'header' && ! this.s.autoWidth)) ) {
			$('th, td', el).css( {
				width: '',
				minWidth: ''
			} );
		}
		else if ( el && item === 'header' ) {
			$('th, td', el).css( 'min-width', '' );
		}
	},

	/**
	 * Reposition the floating elements to take account of horizontal page
	 * scroll
	 *
	 * @param  {string} item       The `header` or `footer`
	 * @param  {int}    scrollLeft Document scrollLeft
	 * @private
	 */
	_horizontal: function ( item, scrollLeft )
	{
		var itemDom = this.dom[ item ];
		var position = this.s.position;
		var lastScrollLeft = this.s.scrollLeft;

		if ( itemDom.floating && lastScrollLeft[ item ] !== scrollLeft ) {
			itemDom.floating.css( 'left', position.left - scrollLeft );

			lastScrollLeft[ item ] = scrollLeft;
		}
	},

	/**
	 * Change from one display mode to another. Each fixed item can be in one
	 * of:
	 *
	 * * `in-place` - In the main DataTable
	 * * `in` - Floating over the DataTable
	 * * `below` - (Header only) Fixed to the bottom of the table body
	 * * `above` - (Footer only) Fixed to the top of the table body
	 * 
	 * @param  {string}  mode        Mode that the item should be shown in
	 * @param  {string}  item        'header' or 'footer'
	 * @param  {boolean} forceChange Force a redraw of the mode, even if already
	 *     in that mode.
	 * @private
	 */
	_modeChange: function ( mode, item, forceChange )
	{
		var dt = this.s.dt;
		var itemDom = this.dom[ item ];
		var position = this.s.position;

		// Record focus. Browser's will cause input elements to loose focus if
		// they are inserted else where in the doc
		var tablePart = this.dom[ item==='footer' ? 'tfoot' : 'thead' ];
		var focus = $.contains( tablePart[0], document.activeElement ) ?
			document.activeElement :
			null;
		
		if ( focus ) {
			focus.blur();
		}

		if ( mode === 'in-place' ) {
			// Insert the header back into the table's real header
			if ( itemDom.placeholder ) {
				itemDom.placeholder.remove();
				itemDom.placeholder = null;
			}

			this._unsize( item );

			if ( item === 'header' ) {
				itemDom.host.prepend( tablePart );
			}
			else {
				itemDom.host.append( tablePart );
			}

			if ( itemDom.floating ) {
				itemDom.floating.remove();
				itemDom.floating = null;
			}
		}
		else if ( mode === 'in' ) {
			// Remove the header from the read header and insert into a fixed
			// positioned floating table clone
			this._clone( item, forceChange );

			itemDom.floating
				.addClass( 'fixedHeader-floating' )
				.css( item === 'header' ? 'top' : 'bottom', this.c[item+'Offset'] )
				.css( 'left', position.left+'px' )
				.css( 'width', position.width+'px' );

			if ( item === 'footer' ) {
				itemDom.floating.css( 'top', '' );
			}
		}
		else if ( mode === 'below' ) { // only used for the header
			// Fix the position of the floating header at base of the table body
			this._clone( item, forceChange );

			itemDom.floating
				.addClass( 'fixedHeader-locked' )
				.css( 'top', position.tfootTop - position.theadHeight )
				.css( 'left', position.left+'px' )
				.css( 'width', position.width+'px' );
		}
		else if ( mode === 'above' ) { // only used for the footer
			// Fix the position of the floating footer at top of the table body
			this._clone( item, forceChange );

			itemDom.floating
				.addClass( 'fixedHeader-locked' )
				.css( 'top', position.tbodyTop )
				.css( 'left', position.left+'px' )
				.css( 'width', position.width+'px' );
		}

		// Restore focus if it was lost
		if ( focus && focus !== document.activeElement ) {
			setTimeout( function () {
				focus.focus();
			}, 10 );
		}

		this.s.scrollLeft.header = -1;
		this.s.scrollLeft.footer = -1;
		this.s[item+'Mode'] = mode;
	},

	/**
	 * Cache the positional information that is required for the mode
	 * calculations that FixedHeader performs.
	 *
	 * @private
	 */
	_positions: function ()
	{
		var dt = this.s.dt;
		var table = dt.table();
		var position = this.s.position;
		var dom = this.dom;
		var tableNode = $(table.node());

		// Need to use the header and footer that are in the main table,
		// regardless of if they are clones, since they hold the positions we
		// want to measure from
		var thead = tableNode.children('thead');
		var tfoot = tableNode.children('tfoot');
		var tbody = dom.tbody;

		position.visible = tableNode.is(':visible');
		position.width = tableNode.outerWidth();
		position.left = tableNode.offset().left;
		position.theadTop = thead.offset().top;
		position.tbodyTop = tbody.offset().top;
		position.theadHeight = position.tbodyTop - position.theadTop;

		if ( tfoot.length ) {
			position.tfootTop = tfoot.offset().top;
			position.tfootBottom = position.tfootTop + tfoot.outerHeight();
			position.tfootHeight = position.tfootBottom - position.tfootTop;
		}
		else {
			position.tfootTop = position.tbodyTop + tbody.outerHeight();
			position.tfootBottom = position.tfootTop;
			position.tfootHeight = position.tfootTop;
		}
	},


	/**
	 * Mode calculation - determine what mode the fixed items should be placed
	 * into.
	 *
	 * @param  {boolean} forceChange Force a redraw of the mode, even if already
	 *     in that mode.
	 * @private
	 */
	_scroll: function ( forceChange )
	{
		var windowTop = $(document).scrollTop();
		var windowLeft = $(document).scrollLeft();
		var position = this.s.position;
		var headerMode, footerMode;

		if ( ! this.s.enable ) {
			return;
		}

		if ( this.c.header ) {
			if ( ! position.visible || windowTop <= position.theadTop - this.c.headerOffset ) {
				headerMode = 'in-place';
			}
			else if ( windowTop <= position.tfootTop - position.theadHeight - this.c.headerOffset ) {
				headerMode = 'in';
			}
			else {
				headerMode = 'below';
			}

			if ( forceChange || headerMode !== this.s.headerMode ) {
				this._modeChange( headerMode, 'header', forceChange );
			}

			this._horizontal( 'header', windowLeft );
		}

		if ( this.c.footer && this.dom.tfoot.length ) {
			if ( ! position.visible || windowTop + position.windowHeight >= position.tfootBottom + this.c.footerOffset ) {
				footerMode = 'in-place';
			}
			else if ( position.windowHeight + windowTop > position.tbodyTop + position.tfootHeight + this.c.footerOffset ) {
				footerMode = 'in';
			}
			else {
				footerMode = 'above';
			}

			if ( forceChange || footerMode !== this.s.footerMode ) {
				this._modeChange( footerMode, 'footer', forceChange );
			}

			this._horizontal( 'footer', windowLeft );
		}
	}
} );


/**
 * Version
 * @type {String}
 * @static
 */
FixedHeader.version = "3.1.4";

/**
 * Defaults
 * @type {Object}
 * @static
 */
FixedHeader.defaults = {
	header: true,
	footer: false,
	headerOffset: 0,
	footerOffset: 0
};


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * DataTables interfaces
 */

// Attach for constructor access
$.fn.dataTable.FixedHeader = FixedHeader;
$.fn.DataTable.FixedHeader = FixedHeader;


// DataTables creation - check if the FixedHeader option has been defined on the
// table and if so, initialise
$(document).on( 'init.dt.dtfh', function (e, settings, json) {
	if ( e.namespace !== 'dt' ) {
		return;
	}

	var init = settings.oInit.fixedHeader;
	var defaults = DataTable.defaults.fixedHeader;

	if ( (init || defaults) && ! settings._fixedHeader ) {
		var opts = $.extend( {}, defaults, init );

		if ( init !== false ) {
			new FixedHeader( settings, opts );
		}
	}
} );

// DataTables API methods
DataTable.Api.register( 'fixedHeader()', function () {} );

DataTable.Api.register( 'fixedHeader.adjust()', function () {
	return this.iterator( 'table', function ( ctx ) {
		var fh = ctx._fixedHeader;

		if ( fh ) {
			fh.update();
		}
	} );
} );

DataTable.Api.register( 'fixedHeader.enable()', function ( flag ) {
	return this.iterator( 'table', function ( ctx ) {
		var fh = ctx._fixedHeader;

		flag = ( flag !== undefined ? flag : true );
		if ( fh && flag !== fh.s.enable ) {
			fh.enable( flag );
		}
	} );
} );

DataTable.Api.register( 'fixedHeader.disable()', function ( ) {
	return this.iterator( 'table', function ( ctx ) {
		var fh = ctx._fixedHeader;

		if ( fh && fh.s.enable ) {
			fh.enable( false );
		}
	} );
} );

$.each( ['header', 'footer'], function ( i, el ) {
	DataTable.Api.register( 'fixedHeader.'+el+'Offset()', function ( offset ) {
		var ctx = this.context;

		if ( offset === undefined ) {
			return ctx.length && ctx[0]._fixedHeader ?
				ctx[0]._fixedHeader[el +'Offset']() :
				undefined;
		}

		return this.iterator( 'table', function ( ctx ) {
			var fh = ctx._fixedHeader;

			if ( fh ) {
				fh[ el +'Offset' ]( offset );
			}
		} );
	} );
} );


return FixedHeader;
}));


/*! Select for DataTables 1.2.6
 * 2015-2018 SpryMedia Ltd - datatables.net/license/mit
 */

/**
 * @summary     Select for DataTables
 * @description A collection of API methods, events and buttons for DataTables
 *   that provides selection options of the items in a DataTable
 * @version     1.2.6
 * @file        dataTables.select.js
 * @author      SpryMedia Ltd (www.sprymedia.co.uk)
 * @contact     datatables.net/forums
 * @copyright   Copyright 2015-2018 SpryMedia Ltd.
 *
 * This source file is free software, available under the following license:
 *   MIT license - http://datatables.net/license/mit
 *
 * This source file is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the license files for details.
 *
 * For details please refer to: http://www.datatables.net/extensions/select
 */
(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net'], function ( $ ) {
			return factory( $, window, document );
		} );
	}
	else if ( typeof exports === 'object' ) {
		// CommonJS
		module.exports = function (root, $) {
			if ( ! root ) {
				root = window;
			}

			if ( ! $ || ! $.fn.dataTable ) {
				$ = require('datatables.net')(root, $).$;
			}

			return factory( $, root, root.document );
		};
	}
	else {
		// Browser
		factory( jQuery, window, document );
	}
}(function( $, window, document, undefined ) {
'use strict';
var DataTable = $.fn.dataTable;


// Version information for debugger
DataTable.select = {};

DataTable.select.version = '1.2.6';

DataTable.select.init = function ( dt ) {
	var ctx = dt.settings()[0];
	var init = ctx.oInit.select;
	var defaults = DataTable.defaults.select;
	var opts = init === undefined ?
		defaults :
		init;

	// Set defaults
	var items = 'row';
	var style = 'api';
	var blurable = false;
	var info = true;
	var selector = 'td, th';
	var className = 'selected';
	var setStyle = false;

	ctx._select = {};

	// Initialisation customisations
	if ( opts === true ) {
		style = 'os';
		setStyle = true;
	}
	else if ( typeof opts === 'string' ) {
		style = opts;
		setStyle = true;
	}
	else if ( $.isPlainObject( opts ) ) {
		if ( opts.blurable !== undefined ) {
			blurable = opts.blurable;
		}

		if ( opts.info !== undefined ) {
			info = opts.info;
		}

		if ( opts.items !== undefined ) {
			items = opts.items;
		}

		if ( opts.style !== undefined ) {
			style = opts.style;
			setStyle = true;
		}

		if ( opts.selector !== undefined ) {
			selector = opts.selector;
		}

		if ( opts.className !== undefined ) {
			className = opts.className;
		}
	}

	dt.select.selector( selector );
	dt.select.items( items );
	dt.select.style( style );
	dt.select.blurable( blurable );
	dt.select.info( info );
	ctx._select.className = className;


	// Sort table based on selected rows. Requires Select Datatables extension
	$.fn.dataTable.ext.order['select-checkbox'] = function ( settings, col ) {
		return this.api().column( col, {order: 'index'} ).nodes().map( function ( td ) {
			if ( settings._select.items === 'row' ) {
				return $( td ).parent().hasClass( settings._select.className );
			} else if ( settings._select.items === 'cell' ) {
				return $( td ).hasClass( settings._select.className );
			}
			return false;
		});
	};

	// If the init options haven't enabled select, but there is a selectable
	// class name, then enable
	if ( ! setStyle && $( dt.table().node() ).hasClass( 'selectable' ) ) {
		dt.select.style( 'os' );
	}
};

/*

Select is a collection of API methods, event handlers, event emitters and
buttons (for the `Buttons` extension) for DataTables. It provides the following
features, with an overview of how they are implemented:

## Selection of rows, columns and cells. Whether an item is selected or not is
   stored in:

* rows: a `_select_selected` property which contains a boolean value of the
  DataTables' `aoData` object for each row
* columns: a `_select_selected` property which contains a boolean value of the
  DataTables' `aoColumns` object for each column
* cells: a `_selected_cells` property which contains an array of boolean values
  of the `aoData` object for each row. The array is the same length as the
  columns array, with each element of it representing a cell.

This method of using boolean flags allows Select to operate when nodes have not
been created for rows / cells (DataTables' defer rendering feature).

## API methods

A range of API methods are available for triggering selection and de-selection
of rows. Methods are also available to configure the selection events that can
be triggered by an end user (such as which items are to be selected). To a large
extent, these of API methods *is* Select. It is basically a collection of helper
functions that can be used to select items in a DataTable.

Configuration of select is held in the object `_select` which is attached to the
DataTables settings object on initialisation. Select being available on a table
is not optional when Select is loaded, but its default is for selection only to
be available via the API - so the end user wouldn't be able to select rows
without additional configuration.

The `_select` object contains the following properties:

```
{
	items:string     - Can be `rows`, `columns` or `cells`. Defines what item 
	                   will be selected if the user is allowed to activate row
	                   selection using the mouse.
	style:string     - Can be `none`, `single`, `multi` or `os`. Defines the
	                   interaction style when selecting items
	blurable:boolean - If row selection can be cleared by clicking outside of
	                   the table
	info:boolean     - If the selection summary should be shown in the table
	                   information elements
}
```

In addition to the API methods, Select also extends the DataTables selector
options for rows, columns and cells adding a `selected` option to the selector
options object, allowing the developer to select only selected items or
unselected items.

## Mouse selection of items

Clicking on items can be used to select items. This is done by a simple event
handler that will select the items using the API methods.

 */


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Local functions
 */

/**
 * Add one or more cells to the selection when shift clicking in OS selection
 * style cell selection.
 *
 * Cell range is more complicated than row and column as we want to select
 * in the visible grid rather than by index in sequence. For example, if you
 * click first in cell 1-1 and then shift click in 2-2 - cells 1-2 and 2-1
 * should also be selected (and not 1-3, 1-4. etc)
 * 
 * @param  {DataTable.Api} dt   DataTable
 * @param  {object}        idx  Cell index to select to
 * @param  {object}        last Cell index to select from
 * @private
 */
function cellRange( dt, idx, last )
{
	var indexes;
	var columnIndexes;
	var rowIndexes;
	var selectColumns = function ( start, end ) {
		if ( start > end ) {
			var tmp = end;
			end = start;
			start = tmp;
		}
		
		var record = false;
		return dt.columns( ':visible' ).indexes().filter( function (i) {
			if ( i === start ) {
				record = true;
			}
			
			if ( i === end ) { // not else if, as start might === end
				record = false;
				return true;
			}

			return record;
		} );
	};

	var selectRows = function ( start, end ) {
		var indexes = dt.rows( { search: 'applied' } ).indexes();

		// Which comes first - might need to swap
		if ( indexes.indexOf( start ) > indexes.indexOf( end ) ) {
			var tmp = end;
			end = start;
			start = tmp;
		}

		var record = false;
		return indexes.filter( function (i) {
			if ( i === start ) {
				record = true;
			}
			
			if ( i === end ) {
				record = false;
				return true;
			}

			return record;
		} );
	};

	if ( ! dt.cells( { selected: true } ).any() && ! last ) {
		// select from the top left cell to this one
		columnIndexes = selectColumns( 0, idx.column );
		rowIndexes = selectRows( 0 , idx.row );
	}
	else {
		// Get column indexes between old and new
		columnIndexes = selectColumns( last.column, idx.column );
		rowIndexes = selectRows( last.row , idx.row );
	}

	indexes = dt.cells( rowIndexes, columnIndexes ).flatten();

	if ( ! dt.cells( idx, { selected: true } ).any() ) {
		// Select range
		dt.cells( indexes ).select();
	}
	else {
		// Deselect range
		dt.cells( indexes ).deselect();
	}
}

/**
 * Disable mouse selection by removing the selectors
 *
 * @param {DataTable.Api} dt DataTable to remove events from
 * @private
 */
function disableMouseSelection( dt )
{
	var ctx = dt.settings()[0];
	var selector = ctx._select.selector;

	$( dt.table().container() )
		.off( 'mousedown.dtSelect', selector )
		.off( 'mouseup.dtSelect', selector )
		.off( 'click.dtSelect', selector );

	$('body').off( 'click.dtSelect' + dt.table().node().id );
}

/**
 * Attach mouse listeners to the table to allow mouse selection of items
 *
 * @param {DataTable.Api} dt DataTable to remove events from
 * @private
 */
function enableMouseSelection ( dt )
{
	var container = $( dt.table().container() );
	var ctx = dt.settings()[0];
	var selector = ctx._select.selector;
	var matchSelection;

	container
		.on( 'mousedown.dtSelect', selector, function(e) {
			// Disallow text selection for shift clicking on the table so multi
			// element selection doesn't look terrible!
			if ( e.shiftKey || e.metaKey || e.ctrlKey ) {
				container
					.css( '-moz-user-select', 'none' )
					.one('selectstart.dtSelect', selector, function () {
						return false;
					} );
			}

			if ( window.getSelection ) {
				matchSelection = window.getSelection();
			}
		} )
		.on( 'mouseup.dtSelect', selector, function() {
			// Allow text selection to occur again, Mozilla style (tested in FF
			// 35.0.1 - still required)
			container.css( '-moz-user-select', '' );
		} )
		.on( 'click.dtSelect', selector, function ( e ) {
			var items = dt.select.items();
			var idx;

			// If text was selected (click and drag), then we shouldn't change
			// the row's selected state
			if ( window.getSelection ) {
				var selection = window.getSelection();

				// If the element that contains the selection is not in the table, we can ignore it
				// This can happen if the developer selects text from the click event
				if ( ! selection.anchorNode || $(selection.anchorNode).closest('table')[0] === dt.table().node() ) {
					if ( selection !== matchSelection ) {
						return;
					}
				}
			}

			var ctx = dt.settings()[0];
			var wrapperClass = dt.settings()[0].oClasses.sWrapper.replace(/ /g, '.');

			// Ignore clicks inside a sub-table
			if ( $(e.target).closest('div.'+wrapperClass)[0] != dt.table().container() ) {
				return;
			}

			var cell = dt.cell( $(e.target).closest('td, th') );

			// Check the cell actually belongs to the host DataTable (so child
			// rows, etc, are ignored)
			if ( ! cell.any() ) {
				return;
			}

			var event = $.Event('user-select.dt');
			eventTrigger( dt, event, [ items, cell, e ] );

			if ( event.isDefaultPrevented() ) {
				return;
			}

			var cellIndex = cell.index();
			if ( items === 'row' ) {
				idx = cellIndex.row;
				typeSelect( e, dt, ctx, 'row', idx );
			}
			else if ( items === 'column' ) {
				idx = cell.index().column;
				typeSelect( e, dt, ctx, 'column', idx );
			}
			else if ( items === 'cell' ) {
				idx = cell.index();
				typeSelect( e, dt, ctx, 'cell', idx );
			}

			ctx._select_lastCell = cellIndex;
		} );

	// Blurable
	$('body').on( 'click.dtSelect' + dt.table().node().id, function ( e ) {
		if ( ctx._select.blurable ) {
			// If the click was inside the DataTables container, don't blur
			if ( $(e.target).parents().filter( dt.table().container() ).length ) {
				return;
			}

			// Ignore elements which have been removed from the DOM (i.e. paging
			// buttons)
			if ( $(e.target).parents('html').length === 0 ) {
			 	return;
			}

			// Don't blur in Editor form
			if ( $(e.target).parents('div.DTE').length ) {
				return;
			}

			clear( ctx, true );
		}
	} );
}

/**
 * Trigger an event on a DataTable
 *
 * @param {DataTable.Api} api      DataTable to trigger events on
 * @param  {boolean}      selected true if selected, false if deselected
 * @param  {string}       type     Item type acting on
 * @param  {boolean}      any      Require that there are values before
 *     triggering
 * @private
 */
function eventTrigger ( api, type, args, any )
{
	if ( any && ! api.flatten().length ) {
		return;
	}

	if ( typeof type === 'string' ) {
		type = type +'.dt';
	}

	args.unshift( api );

	$(api.table().node()).trigger( type, args );
}

/**
 * Update the information element of the DataTable showing information about the
 * items selected. This is done by adding tags to the existing text
 * 
 * @param {DataTable.Api} api DataTable to update
 * @private
 */
function info ( api )
{
	var ctx = api.settings()[0];

	if ( ! ctx._select.info || ! ctx.aanFeatures.i ) {
		return;
	}

	if ( api.select.style() === 'api' ) {
		return;
	}

	var rows    = api.rows( { selected: true } ).flatten().length;
	var columns = api.columns( { selected: true } ).flatten().length;
	var cells   = api.cells( { selected: true } ).flatten().length;

	var add = function ( el, name, num ) {
		el.append( $('<span class="select-item"/>').append( api.i18n(
			'select.'+name+'s',
			{ _: '%d '+name+'s selected', 0: '', 1: '1 '+name+' selected' },
			num
		) ) );
	};

	// Internal knowledge of DataTables to loop over all information elements
	$.each( ctx.aanFeatures.i, function ( i, el ) {
		el = $(el);

		var output  = $('<span class="select-info"/>');
		add( output, 'row', rows );
		add( output, 'column', columns );
		add( output, 'cell', cells  );

		var exisiting = el.children('span.select-info');
		if ( exisiting.length ) {
			exisiting.remove();
		}

		if ( output.text() !== '' ) {
			el.append( output );
		}
	} );
}

/**
 * Initialisation of a new table. Attach event handlers and callbacks to allow
 * Select to operate correctly.
 *
 * This will occur _after_ the initial DataTables initialisation, although
 * before Ajax data is rendered, if there is ajax data
 *
 * @param  {DataTable.settings} ctx Settings object to operate on
 * @private
 */
function init ( ctx ) {
	var api = new DataTable.Api( ctx );

	// Row callback so that classes can be added to rows and cells if the item
	// was selected before the element was created. This will happen with the
	// `deferRender` option enabled.
	// 
	// This method of attaching to `aoRowCreatedCallback` is a hack until
	// DataTables has proper events for row manipulation If you are reviewing
	// this code to create your own plug-ins, please do not do this!
	ctx.aoRowCreatedCallback.push( {
		fn: function ( row, data, index ) {
			var i, ien;
			var d = ctx.aoData[ index ];

			// Row
			if ( d._select_selected ) {
				$( row ).addClass( ctx._select.className );
			}

			// Cells and columns - if separated out, we would need to do two
			// loops, so it makes sense to combine them into a single one
			for ( i=0, ien=ctx.aoColumns.length ; i<ien ; i++ ) {
				if ( ctx.aoColumns[i]._select_selected || (d._selected_cells && d._selected_cells[i]) ) {
					$(d.anCells[i]).addClass( ctx._select.className );
				}
			}
		},
		sName: 'select-deferRender'
	} );

	// On Ajax reload we want to reselect all rows which are currently selected,
	// if there is an rowId (i.e. a unique value to identify each row with)
	api.on( 'preXhr.dt.dtSelect', function () {
		// note that column selection doesn't need to be cached and then
		// reselected, as they are already selected
		var rows = api.rows( { selected: true } ).ids( true ).filter( function ( d ) {
			return d !== undefined;
		} );

		var cells = api.cells( { selected: true } ).eq(0).map( function ( cellIdx ) {
			var id = api.row( cellIdx.row ).id( true );
			return id ?
				{ row: id, column: cellIdx.column } :
				undefined;
		} ).filter( function ( d ) {
			return d !== undefined;
		} );

		// On the next draw, reselect the currently selected items
		api.one( 'draw.dt.dtSelect', function () {
			api.rows( rows ).select();

			// `cells` is not a cell index selector, so it needs a loop
			if ( cells.any() ) {
				cells.each( function ( id ) {
					api.cells( id.row, id.column ).select();
				} );
			}
		} );
	} );

	// Update the table information element with selected item summary
	api.on( 'draw.dtSelect.dt select.dtSelect.dt deselect.dtSelect.dt info.dt', function () {
		info( api );
	} );

	// Clean up and release
	api.on( 'destroy.dtSelect', function () {
		disableMouseSelection( api );
		api.off( '.dtSelect' );
	} );
}

/**
 * Add one or more items (rows or columns) to the selection when shift clicking
 * in OS selection style
 *
 * @param  {DataTable.Api} dt   DataTable
 * @param  {string}        type Row or column range selector
 * @param  {object}        idx  Item index to select to
 * @param  {object}        last Item index to select from
 * @private
 */
function rowColumnRange( dt, type, idx, last )
{
	// Add a range of rows from the last selected row to this one
	var indexes = dt[type+'s']( { search: 'applied' } ).indexes();
	var idx1 = $.inArray( last, indexes );
	var idx2 = $.inArray( idx, indexes );

	if ( ! dt[type+'s']( { selected: true } ).any() && idx1 === -1 ) {
		// select from top to here - slightly odd, but both Windows and Mac OS
		// do this
		indexes.splice( $.inArray( idx, indexes )+1, indexes.length );
	}
	else {
		// reverse so we can shift click 'up' as well as down
		if ( idx1 > idx2 ) {
			var tmp = idx2;
			idx2 = idx1;
			idx1 = tmp;
		}

		indexes.splice( idx2+1, indexes.length );
		indexes.splice( 0, idx1 );
	}

	if ( ! dt[type]( idx, { selected: true } ).any() ) {
		// Select range
		dt[type+'s']( indexes ).select();
	}
	else {
		// Deselect range - need to keep the clicked on row selected
		indexes.splice( $.inArray( idx, indexes ), 1 );
		dt[type+'s']( indexes ).deselect();
	}
}

/**
 * Clear all selected items
 *
 * @param  {DataTable.settings} ctx Settings object of the host DataTable
 * @param  {boolean} [force=false] Force the de-selection to happen, regardless
 *     of selection style
 * @private
 */
function clear( ctx, force )
{
	if ( force || ctx._select.style === 'single' ) {
		var api = new DataTable.Api( ctx );
		
		api.rows( { selected: true } ).deselect();
		api.columns( { selected: true } ).deselect();
		api.cells( { selected: true } ).deselect();
	}
}

/**
 * Select items based on the current configuration for style and items.
 *
 * @param  {object}             e    Mouse event object
 * @param  {DataTables.Api}     dt   DataTable
 * @param  {DataTable.settings} ctx  Settings object of the host DataTable
 * @param  {string}             type Items to select
 * @param  {int|object}         idx  Index of the item to select
 * @private
 */
function typeSelect ( e, dt, ctx, type, idx )
{
	var style = dt.select.style();
	var isSelected = dt[type]( idx, { selected: true } ).any();

	if ( style === 'os' ) {
		if ( e.ctrlKey || e.metaKey ) {
			// Add or remove from the selection
			dt[type]( idx ).select( ! isSelected );
		}
		else if ( e.shiftKey ) {
			if ( type === 'cell' ) {
				cellRange( dt, idx, ctx._select_lastCell || null );
			}
			else {
				rowColumnRange( dt, type, idx, ctx._select_lastCell ?
					ctx._select_lastCell[type] :
					null
				);
			}
		}
		else {
			// No cmd or shift click - deselect if selected, or select
			// this row only
			var selected = dt[type+'s']( { selected: true } );

			if ( isSelected && selected.flatten().length === 1 ) {
				dt[type]( idx ).deselect();
			}
			else {
				selected.deselect();
				dt[type]( idx ).select();
			}
		}
	} else if ( style == 'multi+shift' ) {
		if ( e.shiftKey ) {
			if ( type === 'cell' ) {
				cellRange( dt, idx, ctx._select_lastCell || null );
			}
			else {
				rowColumnRange( dt, type, idx, ctx._select_lastCell ?
					ctx._select_lastCell[type] :
					null
				);
			}
		}
		else {
			dt[ type ]( idx ).select( ! isSelected );
		}
	}
	else {
		dt[ type ]( idx ).select( ! isSelected );
	}
}



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * DataTables selectors
 */

// row and column are basically identical just assigned to different properties
// and checking a different array, so we can dynamically create the functions to
// reduce the code size
$.each( [
	{ type: 'row', prop: 'aoData' },
	{ type: 'column', prop: 'aoColumns' }
], function ( i, o ) {
	DataTable.ext.selector[ o.type ].push( function ( settings, opts, indexes ) {
		var selected = opts.selected;
		var data;
		var out = [];

		if ( selected !== true && selected !== false ) {
			return indexes;
		}

		for ( var i=0, ien=indexes.length ; i<ien ; i++ ) {
			data = settings[ o.prop ][ indexes[i] ];

			if ( (selected === true && data._select_selected === true) ||
			     (selected === false && ! data._select_selected )
			) {
				out.push( indexes[i] );
			}
		}

		return out;
	} );
} );

DataTable.ext.selector.cell.push( function ( settings, opts, cells ) {
	var selected = opts.selected;
	var rowData;
	var out = [];

	if ( selected === undefined ) {
		return cells;
	}

	for ( var i=0, ien=cells.length ; i<ien ; i++ ) {
		rowData = settings.aoData[ cells[i].row ];

		if ( (selected === true && rowData._selected_cells && rowData._selected_cells[ cells[i].column ] === true) ||
		     (selected === false && ( ! rowData._selected_cells || ! rowData._selected_cells[ cells[i].column ] ) )
		) {
			out.push( cells[i] );
		}
	}

	return out;
} );



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * DataTables API
 *
 * For complete documentation, please refer to the docs/api directory or the
 * DataTables site
 */

// Local variables to improve compression
var apiRegister = DataTable.Api.register;
var apiRegisterPlural = DataTable.Api.registerPlural;

apiRegister( 'select()', function () {
	return this.iterator( 'table', function ( ctx ) {
		DataTable.select.init( new DataTable.Api( ctx ) );
	} );
} );

apiRegister( 'select.blurable()', function ( flag ) {
	if ( flag === undefined ) {
		return this.context[0]._select.blurable;
	}

	return this.iterator( 'table', function ( ctx ) {
		ctx._select.blurable = flag;
	} );
} );

apiRegister( 'select.info()', function ( flag ) {
	if ( info === undefined ) {
		return this.context[0]._select.info;
	}

	return this.iterator( 'table', function ( ctx ) {
		ctx._select.info = flag;
	} );
} );

apiRegister( 'select.items()', function ( items ) {
	if ( items === undefined ) {
		return this.context[0]._select.items;
	}

	return this.iterator( 'table', function ( ctx ) {
		ctx._select.items = items;

		eventTrigger( new DataTable.Api( ctx ), 'selectItems', [ items ] );
	} );
} );

// Takes effect from the _next_ selection. None disables future selection, but
// does not clear the current selection. Use the `deselect` methods for that
apiRegister( 'select.style()', function ( style ) {
	if ( style === undefined ) {
		return this.context[0]._select.style;
	}

	return this.iterator( 'table', function ( ctx ) {
		ctx._select.style = style;

		if ( ! ctx._select_init ) {
			init( ctx );
		}

		// Add / remove mouse event handlers. They aren't required when only
		// API selection is available
		var dt = new DataTable.Api( ctx );
		disableMouseSelection( dt );
		
		if ( style !== 'api' ) {
			enableMouseSelection( dt );
		}

		eventTrigger( new DataTable.Api( ctx ), 'selectStyle', [ style ] );
	} );
} );

apiRegister( 'select.selector()', function ( selector ) {
	if ( selector === undefined ) {
		return this.context[0]._select.selector;
	}

	return this.iterator( 'table', function ( ctx ) {
		disableMouseSelection( new DataTable.Api( ctx ) );

		ctx._select.selector = selector;

		if ( ctx._select.style !== 'api' ) {
			enableMouseSelection( new DataTable.Api( ctx ) );
		}
	} );
} );



apiRegisterPlural( 'rows().select()', 'row().select()', function ( select ) {
	var api = this;

	if ( select === false ) {
		return this.deselect();
	}

	this.iterator( 'row', function ( ctx, idx ) {
		clear( ctx );

		ctx.aoData[ idx ]._select_selected = true;
		$( ctx.aoData[ idx ].nTr ).addClass( ctx._select.className );
	} );

	this.iterator( 'table', function ( ctx, i ) {
		eventTrigger( api, 'select', [ 'row', api[i] ], true );
	} );

	return this;
} );

apiRegisterPlural( 'columns().select()', 'column().select()', function ( select ) {
	var api = this;

	if ( select === false ) {
		return this.deselect();
	}

	this.iterator( 'column', function ( ctx, idx ) {
		clear( ctx );

		ctx.aoColumns[ idx ]._select_selected = true;

		var column = new DataTable.Api( ctx ).column( idx );

		$( column.header() ).addClass( ctx._select.className );
		$( column.footer() ).addClass( ctx._select.className );

		column.nodes().to$().addClass( ctx._select.className );
	} );

	this.iterator( 'table', function ( ctx, i ) {
		eventTrigger( api, 'select', [ 'column', api[i] ], true );
	} );

	return this;
} );

apiRegisterPlural( 'cells().select()', 'cell().select()', function ( select ) {
	var api = this;

	if ( select === false ) {
		return this.deselect();
	}

	this.iterator( 'cell', function ( ctx, rowIdx, colIdx ) {
		clear( ctx );

		var data = ctx.aoData[ rowIdx ];

		if ( data._selected_cells === undefined ) {
			data._selected_cells = [];
		}

		data._selected_cells[ colIdx ] = true;

		if ( data.anCells ) {
			$( data.anCells[ colIdx ] ).addClass( ctx._select.className );
		}
	} );

	this.iterator( 'table', function ( ctx, i ) {
		eventTrigger( api, 'select', [ 'cell', api[i] ], true );
	} );

	return this;
} );


apiRegisterPlural( 'rows().deselect()', 'row().deselect()', function () {
	var api = this;

	this.iterator( 'row', function ( ctx, idx ) {
		ctx.aoData[ idx ]._select_selected = false;
		$( ctx.aoData[ idx ].nTr ).removeClass( ctx._select.className );
	} );

	this.iterator( 'table', function ( ctx, i ) {
		eventTrigger( api, 'deselect', [ 'row', api[i] ], true );
	} );

	return this;
} );

apiRegisterPlural( 'columns().deselect()', 'column().deselect()', function () {
	var api = this;

	this.iterator( 'column', function ( ctx, idx ) {
		ctx.aoColumns[ idx ]._select_selected = false;

		var api = new DataTable.Api( ctx );
		var column = api.column( idx );

		$( column.header() ).removeClass( ctx._select.className );
		$( column.footer() ).removeClass( ctx._select.className );

		// Need to loop over each cell, rather than just using
		// `column().nodes()` as cells which are individually selected should
		// not have the `selected` class removed from them
		api.cells( null, idx ).indexes().each( function (cellIdx) {
			var data = ctx.aoData[ cellIdx.row ];
			var cellSelected = data._selected_cells;

			if ( data.anCells && (! cellSelected || ! cellSelected[ cellIdx.column ]) ) {
				$( data.anCells[ cellIdx.column  ] ).removeClass( ctx._select.className );
			}
		} );
	} );

	this.iterator( 'table', function ( ctx, i ) {
		eventTrigger( api, 'deselect', [ 'column', api[i] ], true );
	} );

	return this;
} );

apiRegisterPlural( 'cells().deselect()', 'cell().deselect()', function () {
	var api = this;

	this.iterator( 'cell', function ( ctx, rowIdx, colIdx ) {
		var data = ctx.aoData[ rowIdx ];

		data._selected_cells[ colIdx ] = false;

		// Remove class only if the cells exist, and the cell is not column
		// selected, in which case the class should remain (since it is selected
		// in the column)
		if ( data.anCells && ! ctx.aoColumns[ colIdx ]._select_selected ) {
			$( data.anCells[ colIdx ] ).removeClass( ctx._select.className );
		}
	} );

	this.iterator( 'table', function ( ctx, i ) {
		eventTrigger( api, 'deselect', [ 'cell', api[i] ], true );
	} );

	return this;
} );



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Buttons
 */
function i18n( label, def ) {
	return function (dt) {
		return dt.i18n( 'buttons.'+label, def );
	};
}

// Common events with suitable namespaces
function namespacedEvents ( config ) {
	var unique = config._eventNamespace;

	return 'draw.dt.DT'+unique+' select.dt.DT'+unique+' deselect.dt.DT'+unique;
}

function enabled ( dt, config ) {
	if ( $.inArray( 'rows', config.limitTo ) !== -1 && dt.rows( { selected: true } ).any() ) {
		return true;
	}

	if ( $.inArray( 'columns', config.limitTo ) !== -1 && dt.columns( { selected: true } ).any() ) {
		return true;
	}

	if ( $.inArray( 'cells', config.limitTo ) !== -1 && dt.cells( { selected: true } ).any() ) {
		return true;
	}

	return false;
}

var _buttonNamespace = 0;

$.extend( DataTable.ext.buttons, {
	selected: {
		text: i18n( 'selected', 'Selected' ),
		className: 'buttons-selected',
		limitTo: [ 'rows', 'columns', 'cells' ],
		init: function ( dt, node, config ) {
			var that = this;
			config._eventNamespace = '.select'+(_buttonNamespace++);

			// .DT namespace listeners are removed by DataTables automatically
			// on table destroy
			dt.on( namespacedEvents(config), function () {
				that.enable( enabled(dt, config) );
			} );

			this.disable();
		},
		destroy: function ( dt, node, config ) {
			dt.off( config._eventNamespace );
		}
	},
	selectedSingle: {
		text: i18n( 'selectedSingle', 'Selected single' ),
		className: 'buttons-selected-single',
		init: function ( dt, node, config ) {
			var that = this;
			config._eventNamespace = '.select'+(_buttonNamespace++);

			dt.on( namespacedEvents(config), function () {
				var count = dt.rows( { selected: true } ).flatten().length +
				            dt.columns( { selected: true } ).flatten().length +
				            dt.cells( { selected: true } ).flatten().length;

				that.enable( count === 1 );
			} );

			this.disable();
		},
		destroy: function ( dt, node, config ) {
			dt.off( config._eventNamespace );
		}
	},
	selectAll: {
		text: i18n( 'selectAll', 'Select all' ),
		className: 'buttons-select-all',
		action: function () {
			var items = this.select.items();
			this[ items+'s' ]().select();
		}
	},
	selectNone: {
		text: i18n( 'selectNone', 'Deselect all' ),
		className: 'buttons-select-none',
		action: function () {
			clear( this.settings()[0], true );
		},
		init: function ( dt, node, config ) {
			var that = this;
			config._eventNamespace = '.select'+(_buttonNamespace++);

			dt.on( namespacedEvents(config), function () {
				var count = dt.rows( { selected: true } ).flatten().length +
				            dt.columns( { selected: true } ).flatten().length +
				            dt.cells( { selected: true } ).flatten().length;

				that.enable( count > 0 );
			} );

			this.disable();
		},
		destroy: function ( dt, node, config ) {
			dt.off( config._eventNamespace );
		}
	}
} );

$.each( [ 'Row', 'Column', 'Cell' ], function ( i, item ) {
	var lc = item.toLowerCase();

	DataTable.ext.buttons[ 'select'+item+'s' ] = {
		text: i18n( 'select'+item+'s', 'Select '+lc+'s' ),
		className: 'buttons-select-'+lc+'s',
		action: function () {
			this.select.items( lc );
		},
		init: function ( dt ) {
			var that = this;

			dt.on( 'selectItems.dt.DT', function ( e, ctx, items ) {
				that.active( items === lc );
			} );
		}
	};
} );



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Initialisation
 */

// DataTables creation - check if select has been defined in the options. Note
// this required that the table be in the document! If it isn't then something
// needs to trigger this method unfortunately. The next major release of
// DataTables will rework the events and address this.
$(document).on( 'preInit.dt.dtSelect', function (e, ctx) {
	if ( e.namespace !== 'dt' ) {
		return;
	}

	DataTable.select.init( new DataTable.Api( ctx ) );
} );


return DataTable.select;
}));


