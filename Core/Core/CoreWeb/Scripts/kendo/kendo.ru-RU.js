﻿/*
* Kendo UI Localization Project for v2012.3.1114 
* Copyright 2012 Telerik AD. All rights reserved.
* 
* Standard Norwegian (nb-NO) Language Pack
*
* Project home  : https://github.com/loudenvier/kendo-global
* Kendo UI home : http://kendoui.com
* Author        : Mikael Gyth
*                 
*
* This project is released to the public domain, although one must abide to the 
* licensing terms set forth by Telerik to use Kendo UI, as shown bellow.
*
* Telerik's original licensing terms:
* -----------------------------------
* Kendo UI Web commercial licenses may be obtained at
* https://www.kendoui.com/purchase/license-agreement/kendo-ui-web-commercial.aspx
* If you do not own a commercial license, this file shall be governed by the
* GNU General Public License (GPL) version 3.
* For GPL requirements, please review: http://www.gnu.org/copyleft/gpl.html
*/

kendo.ui.Locale = "Russian (ru-RU)";
kendo.ui.ColumnMenu.prototype.options.messages =
  $.extend(kendo.ui.ColumnMenu.prototype.options.messages, {

      /* COLUMN MENU MESSAGES 
      ****************************************************************************/
      sortAscending: "По возрастанию",
      sortDescending: "По убыванию",
      filter: "Фильтр",
      columns: "Столбцы"
      /***************************************************************************/
  });

kendo.ui.Groupable.prototype.options.messages =
  $.extend(kendo.ui.Groupable.prototype.options.messages, {

      /* GRID GROUP PANEL MESSAGES 
      ****************************************************************************/
      empty: "Trekk en kollonneoverskrift og slipp den her for å gruppere etter denne kolonnen"
      /***************************************************************************/
  });

kendo.ui.FilterMenu.prototype.options.messages =
  $.extend(kendo.ui.FilterMenu.prototype.options.messages, {

      /* FILTER MENU MESSAGES 
      ***************************************************************************/
      info: "",        // sets the text on top of the filter menu
      filter: "Фильтр",      // sets the text for the "Filter" button
      clear: "Очистить",        // sets the text for the "Clear" button
      // when filtering boolean numbers
      isTrue: "Да", // sets the text for "isTrue" radio button
      isFalse: "Нет",     // sets the text for "isFalse" radio button
      //changes the text of the "And" and "Or" of the filter menu
      and: "И",
      or: "Или",
      selectValue: "-Значение-"
      /***************************************************************************/
  });

kendo.ui.FilterMenu.prototype.options.operators =
  $.extend(kendo.ui.FilterMenu.prototype.options.operators, {

      /* FILTER MENU OPERATORS (for each supported data type) 
      ****************************************************************************/
      string: {
          eq: "Равно",
          neq: "Не равно",
          startswith: "Начинается с",
          contains: "Содержит",
          doesnotcontain: "Не содержит",
          endswith: "Заканчивается на"
      },
      number: {
          eq: "Равно",
          neq: "Не равно",
          gte: "Больше или равно",
          gt: "Больше",
          lte: "Меньше или равно",
          lt: "Меньше"
      },
      date: {
          eq: "Равно",
          neq: "Не равно",
          gte: "Больше или равно",
          gt: "Больше",
          lte: "Меньше или равно",
          lt: "Меньше"
      },
      enums: {
          eq: "Равно",
          neq: "Не равно"
      }
      /***************************************************************************/
  });

kendo.ui.Pager.prototype.options.messages =
  $.extend(kendo.ui.Pager.prototype.options.messages, {

      /* PAGER MESSAGES 
      ****************************************************************************/
      display: "{0} - {1} из {2} записей",
      empty: "Нет данных",
      page: "Страница",
      of: "av {0}",
      itemsPerPage: "Записей на страницу",
      first: "Первая страница",
      previous: "Назад",
      next: "Далее",
      last: "Последняя страница",
      refresh: "Обновить"
      /***************************************************************************/
  });

//kendo.ui.Validator.prototype.options.messages =
//  $.extend(kendo.ui.Validator.prototype.options.messages, {

//      /* VALIDATOR MESSAGES 
//      ****************************************************************************/
//      required: "{0} er obligatorisk",
//      pattern: "{0} er ugyldig",
//      min: "{0} skal være større enn eller lik med {1}",
//      max: "{0} skal være mindre enn eller lik med {1}",
//      step: "{0} er ugyldig",
//      email: "{0} er ikke en gyldig e-mail adresse",
//      url: "{0} er ikke en gyldig URL",
//      date: "{0} er ikke en gyldig dato"
//      /***************************************************************************/
//  });

//kendo.ui.ImageBrowser.prototype.options.messages =
//  $.extend(kendo.ui.ImageBrowser.prototype.options.messages, {

//      /* IMAGE BROWSER MESSAGES 
//      ****************************************************************************/
//      uploadFile: "Sender",
//      orderBy: "Sorter etter",
//      orderByName: "Navn",
//      orderBySize: "Størrelse",
//      directoryNotFound: "Biblioteket ble ikke funnet.",
//      emptyFolder: "Tom mappe",
//      deleteFile: "Er du sikker på du vil slette filen: \"{0}\"?",
//      invalidFileType: "Det valgte filformatet: \"{0}\" er ugyldig. Støttede filformater er {1}.",
//      overwriteFile: "En fil med samme navn \"{0}\" eksisterer fra før, vil du overskrive?",
//      dropFilesHere: "Slip filer her"
//      /***************************************************************************/
//  });

//kendo.ui.Editor.prototype.options.messages =
//  $.extend(kendo.ui.Editor.prototype.options.messages, {

//      /* EDITOR MESSAGES 
//      ****************************************************************************/
//      bold: "Fet",
//      italic: "Kursiv",
//      underline: "Understreket",
//      strikethrough: "Gjennomstreket",
//      superscript: "Fremhevet",
//      subscript: "Senket",
//      justifyCenter: "Sentrert",
//      justifyLeft: "Venstrejuster",
//      justifyRight: "Høyrejuster",
//      justifyFull: "like marger",
//      insertUnorderedList: "Sett inn uordnet liste",
//      insertOrderedList: "Sett inn ordnet liste",
//      indent: "Øk innrykk",
//      outdent: "Reduser innrykk",
//      createLink: "Opprett link",
//      unlink: "Fjern link",
//      insertImage: "Sett inn bilde",
//      insertHtml: "Sett inn HTML",
//      fontName: "Skrifttype",
//      fontNameInherit: "(Arv skrifttype)",
//      fontSize: "Skriftstørrelse",
//      fontSizeInherit: "(Arv skriftstørrelse)",
//      formatBlock: "Format",
//      foreColor: "Farge",
//      backColor: "Bakgrunns farge",
//      style: "Stil",
//      emptyFolder: "Tom mappe",
//      uploadFile: "Sender",
//      orderBy: "Sorter etter:",
//      orderBySize: "Størrelse",
//      orderByName: "Navn",
//      invalidFileType: "Det valgte filformatet: \"{0}\" er ugyldig. Støttede filformater er {1}.",
//      deleteFile: "Er du sikker på du vil slette filen: \"{0}\"?",
//      overwriteFile: "En fil med samme navn \"{0}\" eksisterer fra før, vil du overskrive?",
//      directoryNotFound: "Biblioteket ble ikke funnet.",
//      imageWebAddress: "Internett adresse",
//      imageAltText: "Alternativ Tekst",
//      dialogInsert: "Sett inn",
//      dialogButtonSeparator: "eller",
//      dialogCancel: "Annuller"
//      /***************************************************************************/
//  });