﻿' CLIMSOFT - Climate Database Management System
' Copyright (C) 2015
'
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program.  If not, see <http://www.gnu.org/licenses/>.

Public Class formSynopRA1
    Dim conn As New MySql.Data.MySqlClient.MySqlConnection
    Dim myConnectionString As String
    Dim usrName As String
    Dim usrPwd As String
    Dim dbServer As String
    Dim dbName As String
    Dim inc As Integer
    Dim da As MySql.Data.MySqlClient.MySqlDataAdapter
    Dim ds As New DataSet
    Dim sql As String
    Dim maxRows As Integer
    'Declare datasets required for QC
    Dim elemCode As String
    Dim dsValueLimits As New DataSet
    Dim sqlValueLimits As String
    Dim daValueLimits As MySql.Data.MySqlClient.MySqlDataAdapter
    Dim stationCode As String
    Dim dsStationElevation As New DataSet
    Dim sqlStationElevation As String
    Dim daStationElevation As MySql.Data.MySqlClient.MySqlDataAdapter
    Dim dsStationId As New DataSet
    Dim sqlStationId As String
    Dim daStationId As MySql.Data.MySqlClient.MySqlDataAdapter
    Dim valUpperLimit As String, valLowerLimit As String, stnElevation As String
    Dim obsValue As String
    Private Sub navigateRecords()
        'Display the values of data fields from the dataset in the corresponding textboxes on the form.
        'The record with values to be displayed in the texboxes is determined by the value of the variable "inc"
        'which is a parameter of the "Row" attribute or property of the dataset.

        cboStation.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("stationId")
        txtYear.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("yyyy")
        cboMonth.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("mm")
        cboDay.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("dd")
        cboHour.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("hh")

        Dim m As Integer
        Dim ctl As Control

        'Display observation values in coressponding textboxes
        'Observation values start in column 6 i.e. column index 5, and end in column 54 i.e. column Index 53
        For m = 5 To 53
            For Each ctl In Me.Controls
                If Strings.Left(ctl.Name, 6) = "txtVal" And Val(Strings.Right(ctl.Name, 3)) = m Then
                    If Not IsDBNull(ds.Tables("form_synoptic_2_RA1").Rows(inc).Item(m)) Then ctl.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item(m)
                End If
            Next ctl
        Next m

        'Display observation flags in coressponding textboxes
        'Observation values start in column 55 i.e. column index 54, and end in column 103 i.e. column Index 102
        For m = 54 To 102
            For Each ctl In Me.Controls
                If Strings.Left(ctl.Name, 7) = "txtFlag" And Val(Strings.Right(ctl.Name, 3)) = m Then
                    If Not IsDBNull(ds.Tables("form_synoptic_2_RA1").Rows(inc).Item(m)) Then ctl.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item(m)
                End If
            Next ctl
        Next m

        displayRecordNumber()
    End Sub
    Public Sub displayRecordNumber()
        'Display the record number in the data navigation Textbox
        recNumberTextBox.Text = "Record " & inc + 1 & " of " & maxRows
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub
    Private Sub btnMoveNext_Click(sender As Object, e As EventArgs) Handles btnMoveNext.Click
        'Instantiate the "dataEntryGlobalRoutines" in order to access its methods.
        Dim noNextRec As New dataEntryGlobalRoutines
        If inc < (maxRows - 1) Then
            inc = inc + 1
            'Call subroutine for record navigation
            navigateRecords()
        Else
            'If the record Index is equal to maximum number of rows minus one, an error message must be displayed to show that
            'there is no more next record.The message to be displayed is provided by a subroutine in the "dataEntryCommonRoutines" class
            'hence the need to instantiate the "dataEntryCommonRoutines" class in the Declaration above.
            noNextRec.messageBoxNoNextRecord()
        End If

    End Sub

    Private Sub btnMoveLast_Click(sender As Object, e As EventArgs) Handles btnMoveLast.Click
        'In order to move to move to the last record the record index is set to the maximum number of records minus one.
        inc = maxRows - 1
        'Call subroutine for record navigation
        navigateRecords()
    End Sub

    Private Sub btnMoveFirst_Click(sender As Object, e As EventArgs) Handles btnMoveFirst.Click
        'In order to move to move to the first record the record index is set to zero.
        inc = 0
        'Call subroutine for record navigation
        navigateRecords()
    End Sub

    Private Sub btnMovePrevious_Click(sender As Object, e As EventArgs) Handles btnMovePrevious.Click
        'Instantiate the "dataEntryGlobalRoutines" in order to access its methods.
        Dim noPreviousRec As New dataEntryGlobalRoutines

        If inc > 0 Then
            inc = inc - 1
            navigateRecords()
        Else
            'If the record Index is equal to zero an error message must be displayed to show that there is no more previous record.
            'The message to be displayed is provided by a subroutine in the "dataEntryCommonRoutines" class hence the need to
            'instantiate the "dataEntryCommonRoutines" class in the Declaration above.
            noPreviousRec.messageBoxNoPreviousRecord()
        End If
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        'The CommandBuilder providers the imbedded command for updating the record in the record source table. So the CommandBuilder
        'must be declared for the Update method to work.
        Dim cb As New MySql.Data.MySqlClient.MySqlCommandBuilder(da)

        'Instantiate the "dataEntryGlobalRoutines" in order to access its methods.
        Dim recUpdate As New dataEntryGlobalRoutines
        'Update header fields for form in database
        ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("stationId") = cboStation.SelectedValue
        ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("yyyy") = txtYear.Text
        ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("mm") = cboMonth.Text
        ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("dd") = cboDay.Text
        ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("hh") = cboHour.Text
      
        'Update observation values in database
        'Observation values range from column 6 i.e. column index 5 to column 54 i.e. column index 53
        For m = 5 To 53
            For Each ctl In Me.Controls
                If Strings.Left(ctl.Name, 6) = "txtVal" And Val(Strings.Right(ctl.Name, 3)) = m Then
                    ds.Tables("form_synoptic_2_RA1").Rows(inc).Item(m) = ctl.Text
                End If
            Next ctl
        Next m

        'Update observation flags in database
        'Observation values range from column 55 i.e. column index 54 to column 103 i.e. column index 102
        For m = 54 To 102
            For Each ctl In Me.Controls
                If Strings.Left(ctl.Name, 7) = "txtFlag" And Val(Strings.Right(ctl.Name, 3)) = m Then
                    ds.Tables("form_synoptic_2_RA1").Rows(inc).Item(m) = ctl.Text
                End If
            Next ctl
        Next m

        'The data adapter is used to update the record in the data source table
        da.Update(ds, "form_synoptic_2_RA1")

        'Show message for successful updating or record.
        recUpdate.messageBoxRecordedUpdated()
    End Sub

    Private Sub btnAddNew_Click(sender As Object, e As EventArgs) Handles btnAddNew.Click
        'The AddNew button is for the purpose of adding a new record to the DataSet and "NOT FOR ADDING A NEW RECORD TO THE DATASOURCE TABLE".
        'It is the job of the Commit button to add the new record to the datasource table.
        'On pressing the AddNew button, disable buttons for record navigation and also the buttons for deleting, updating
        'or committing a record. This means that these buttons are not required to operate at the time of adding a new record.
        'The addNew button itself should also become disabled soon after clicking it until after the newly added recorded is committed
        'to the data source table linked to the current dataset.
        'Only the Commit button should be enabled.

        btnMoveFirst.Enabled = False
        btnMoveLast.Enabled = False
        btnMoveNext.Enabled = False
        btnMovePrevious.Enabled = False
        btnAddNew.Enabled = False
        btnClear.Enabled = True
        btnDelete.Enabled = False
        btnUpdate.Enabled = False
        btnCommit.Enabled = True
        cboStation.Text = ""
        txtYear.Clear()
        cboMonth.Text = ""
        cboDay.Text = ""
        cboHour.Text = ""

        Dim m As Integer
        Dim ctl As Control
        'Clear textboxes for observation values
        'Observation values range from column 6 i.e. column index 5 to column 54 i.e. column index 53
        For m = 5 To 53
            For Each ctl In Me.Controls
                If Strings.Left(ctl.Name, 6) = "txtVal" And Val(Strings.Right(ctl.Name, 3)) = m Then
                    ctl.Text = ""
                End If
            Next ctl
        Next m

        'Clear textboxes for observation values
        'Observation flags range from column 55 i.e. column index 54 to column 103 i.e. column index 102
        For m = 54 To 102
            For Each ctl In Me.Controls
                If Strings.Left(ctl.Name, 7) = "txtFlag" And Val(Strings.Right(ctl.Name, 3)) = m Then
                    ctl.Text = ""
                End If
            Next ctl
        Next m

        'Set record index to last record
        inc = maxRows

        'Display record position in record navigation Text Box
        recNumberTextBox.Text = "Record " & maxRows + 1 & " of " & maxRows + 1

    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        'The "btnClear" when clicked is meant to clear the form of any new data entered after clicking the Addnew button or in other words 
        'to undo the AddNew button process before the recorded can be committed to the datasource table linked to the DataSet.
        'So all the buttons that were disabled after the AddNew button was clicked should be enabled back again and the Commit button
        'disabled until the AddNew button is clicked

        btnAddNew.Enabled = True
        btnCommit.Enabled = False
        btnDelete.Enabled = True
        btnUpdate.Enabled = True
        btnMoveFirst.Enabled = True
        btnMoveLast.Enabled = True
        btnMoveNext.Enabled = True
        btnMovePrevious.Enabled = True

        'Set Record position index to first record
        inc = 0

        'Call subroutine for record navigation
        navigateRecords()
    End Sub

    Private Sub btnCommit_Click(sender As Object, e As EventArgs) Handles btnCommit.Click
        'The CommandBuilder providers the imbedded command for updating the record in the record source table. So the CommandBuilder
        'must be declared for the Update method to work.
        Dim m As Integer
        Dim ctl As Control
        Dim cb As New MySql.Data.MySqlClient.MySqlCommandBuilder(da)
        Dim dsNewRow As DataRow
        'Instantiate the "dataEntryGlobalRoutines" in order to access its methods.
        Dim recCommit As New dataEntryGlobalRoutines

        dsNewRow = ds.Tables("form_synoptic_2_RA1").NewRow
        'Add a new record to the data source table
        ds.Tables("form_synoptic_2_RA1").Rows.Add(dsNewRow)
        'Commit observation header information to database
        ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("stationId") = cboStation.SelectedValue
        ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("yyyy") = txtYear.Text
        ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("mm") = cboMonth.Text
        ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("dd") = cboDay.Text
        ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("hh") = cboHour.Text

        'Commit observation values to database
        'Observation values range from column 6 i.e. column index 5 to column 54 i.e. column index 53
        For m = 5 To 53
            For Each ctl In Me.Controls
                If Strings.Left(ctl.Name, 6) = "txtVal" And Val(Strings.Right(ctl.Name, 3)) = m Then
                    ds.Tables("form_synoptic_2_RA1").Rows(inc).Item(m) = ctl.Text
                End If
            Next ctl
        Next m

        'Commit observation flags to database
        'Observation values range from column 55 i.e. column index 54 to column 103 i.e. column index 102
        For m = 54 To 102
            For Each ctl In Me.Controls
                If Strings.Left(ctl.Name, 7) = "txtVal" And Val(Strings.Right(ctl.Name, 3)) = m Then
                    ds.Tables("form_synoptic_2_RA1").Rows(inc).Item(m) = ctl.Text
                End If
            Next ctl
        Next m

        da.Update(ds, "form_synoptic_2_RA1")

        'Display message for successful record commit to table
        recCommit.messageBoxCommit()

        btnAddNew.Enabled = True
        btnClear.Enabled = False
        btnCommit.Enabled = False
        btnDelete.Enabled = True
        btnUpdate.Enabled = True
        btnMoveFirst.Enabled = True
        btnMoveLast.Enabled = True
        btnMoveNext.Enabled = True
        btnMovePrevious.Enabled = True
        maxRows = ds.Tables("form_synoptic_2_RA1").Rows.Count
        inc = maxRows - 1

        'Call subroutine for record navigation
        navigateRecords()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        'The CommandBuilder providers the imbedded command for updating the record in the record source table. So the CommandBuilder
        'must be declared for the Update method to work.
        Dim cb As New MySql.Data.MySqlClient.MySqlCommandBuilder(da)
        'Instantiate the "dataEntryGlobalRoutines" in order to access its methods.
        Dim recDelete As New dataEntryGlobalRoutines
        If MessageBox.Show("Do you really want to Delete this Record?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = Windows.Forms.DialogResult.No Then

            'Display message to show that delete operation has been cancelled
            recDelete.messageBoxOperationCancelled()
            Exit Sub
        End If

        ds.Tables("form_synoptic_2_RA1").Rows(inc).Delete()
        da.Update(ds, "form_synoptic_2_RA1")
        maxRows = maxRows - 1
        inc = 0

        'Call subroutine for record navigation
        navigateRecords()
    End Sub

    Private Sub formSynopRA1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Dim objKeyPress As New dataEntryGlobalRoutines
        Dim obsVal As String, obsFlag As String, ctrl As Control, flagtextBoxSuffix As String
        ' Dim obsValColIndex As Integer, flagColIndex As Integer

        'Initialize string variables
        obsVal = ""
        obsFlag = ""
        flagtextBoxSuffix = ""

        'If {ENTER} key is pressed
        If e.KeyCode = Keys.Enter Then
            
            If Strings.Left(Me.ActiveControl.Name, 6) = "txtVal" And Strings.Len(Me.ActiveControl.Text) > 0 Then
               
                'Check for an observation flag in the texbox for observation value.
                ' If a flag exists then separate the flag from the value and place the flag in the corresponding flag field.
                If Not IsNumeric(Strings.Right(Me.ActiveControl.Text, 1)) Then
                    'Get observation flag from the texbox and convert it to Uppercase. Flag is a single letter added as the last character
                    'to the value string in the textbox.
                    obsFlag = Strings.Right(Me.ActiveControl.Text, 1).ToUpper
                    'Get the observation value by leaving out the last character from the string entered in the textbox
                    obsVal = Strings.Left(Me.ActiveControl.Text, Strings.Len(Me.ActiveControl.Text) - 1)

                    Me.ActiveControl.Text = obsVal
                End If
                'Now assign obsFlag to correct texbox on the form
                For Each ctrl In Me.Controls
                    'Loop through all controls on form
                    'Locate the textbox for the flag field by calling the Function "getFlagTexboxSuffix"
                    If Strings.Right(ctrl.Name, 3) = objKeyPress.getFlagTexboxSuffix(Me.ActiveControl.Text, Me.ActiveControl) Then
                        ctrl.Text = obsFlag
                    End If
                Next ctrl

                'Check that numeric value has been entered for observation value
                objKeyPress.checkIsNumeric(Me.ActiveControl.Text, Me.ActiveControl)

                ''Get the element limits

                elemCode = Strings.Mid(Me.ActiveControl.Name, 12, 3)
                sqlValueLimits = "SELECT elementId,upperLimit,lowerLimit FROM obselement WHERE elementId=" & elemCode
                '
                daValueLimits = New MySql.Data.MySqlClient.MySqlDataAdapter(sqlValueLimits, conn)
                'Clear all rows in dataset before filling dataset with new row record for element code associated with active control
                dsValueLimits.Clear()
                'Add row for element code associated with active control
                daValueLimits.Fill(dsValueLimits, "obselement")

                obsValue = Me.ActiveControl.Text
                'Get element lower limit
                If Not IsDBNull(dsValueLimits.Tables("obselement").Rows(0).Item("lowerlimit")) Then
                    valLowerLimit = dsValueLimits.Tables("obselement").Rows(0).Item("lowerlimit")
                Else
                    valLowerLimit = ""
                End If
                'Get element upper limit
                If Not IsDBNull(dsValueLimits.Tables("obselement").Rows(0).Item("upperlimit")) Then
                    valUpperLimit = dsValueLimits.Tables("obselement").Rows(0).Item("upperlimit")
                Else
                    valUpperLimit = ""
                End If

                'Check lower limit
                If obsValue <> "" And valLowerLimit <> "" Then
                    objKeyPress.checkLowerLimit(Me.ActiveControl, obsValue, valLowerLimit)
                End If
                'Check upper limit
                If obsValue <> "" And valUpperLimit <> "" Then
                    objKeyPress.checkUpperLimit(Me.ActiveControl, obsValue, valUpperLimit)
                End If
                'MsgBox("Obs Value: " & obsValue & " Upper Limit: " & valUpperLimit & " Lower Limit: " & valLowerLimit)
            ElseIf Me.ActiveControl.Name = "txtYear" Then
                'Check for numeric
                objKeyPress.checkIsNumeric(txtYear.Text, txtYear)
                'Check valid year
                objKeyPress.checkValidYear(txtYear.Text, txtYear)
            ElseIf Me.ActiveControl.Name = "cboMonth" Then
                'Check for numeric
                objKeyPress.checkIsNumeric(cboMonth.Text, cboMonth)
                'Check valid month
                objKeyPress.checkValidMonth(cboMonth.Text, cboMonth)
            ElseIf Me.ActiveControl.Name = "cboDay" Then
                'Check for numeric
                objKeyPress.checkIsNumeric(cboDay.Text, cboDay)
                'Check valid day
                objKeyPress.checkValidDay(cboDay.Text, cboDay)
                objKeyPress.checkValidDate(cboDay.Text, cboMonth.Text, txtYear.Text, cboDay)
                objKeyPress.checkFutureDate(cboDay.Text, cboMonth.Text, txtYear.Text, cboDay)
            ElseIf Me.ActiveControl.Name = "cboHour" Then
                'Check for numeric
                objKeyPress.checkIsNumeric(cboHour.Text, cboHour)
                'Check valid hour
                objKeyPress.checkValidHour(cboHour.Text, cboHour)
            ElseIf Me.ActiveControl.Name = "cboStation" Then
                Dim itemFound As Boolean
                If Len(cboStation.SelectedValue) > 1 Then
                    itemFound = True
                Else
                    itemFound = False
                End If
                objKeyPress.checkExists(itemFound, cboStation)
            End If
        End If
    End Sub


    Private Sub formSynopRA1_Load(sender As Object, e As EventArgs) Handles Me.Load

        'Set the record index counter to the first row
        inc = 0

        myConnectionString = frmLogin.txtusrpwd.Text
        Try
            conn.ConnectionString = myConnectionString
            conn.Open()

            'MsgBox("Connection Successful !", MsgBoxStyle.Information)

            sql = "SELECT * FROM form_synoptic_2_RA1"
            da = New MySql.Data.MySqlClient.MySqlDataAdapter(sql, conn)
            da.Fill(ds, "form_synoptic_2_RA1")
            conn.Close()
            ' MsgBox("Dataset Field !", MsgBoxStyle.Information)

            'FormLaunchPad.Show()
        Catch ex As MySql.Data.MySqlClient.MySqlException
            MessageBox.Show(ex.Message)
        End Try

        maxRows = ds.Tables("form_synoptic_2_RA1").Rows.Count

        '--------------------------------
        'Fill combobox for station identifier with station list from station table
        Dim m As Integer
        Dim ctl As Control
        Dim ds1 As New DataSet
        Dim sql1 As String
        Dim da1 As MySql.Data.MySqlClient.MySqlDataAdapter
        sql1 = "SELECT stationId,stationName FROM station"
        da1 = New MySql.Data.MySqlClient.MySqlDataAdapter(sql1, conn)

        da1.Fill(ds1, "station")

        With cboStation
            .DataSource = ds1.Tables("station")
            .DisplayMember = "stationName"
            .ValueMember = "stationId"
            .SelectedIndex = 0
        End With
       

            ''sql1 = "SELECT stationId,stationName FROM station"
            ''da1 = New MySql.Data.MySqlClient.MySqlDataAdapter(sql1, conn)

            '---------------------------------
            'Initialize header information for data-entry form

            If maxRows > 0 Then
                'StationIdTextBox.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("stationId")
                cboStation.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("stationId")
                txtYear.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("yyyy")
                cboMonth.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("mm")
                cboDay.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("dd")
                cboHour.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item("hh")

                'Initialize textboxes for observation values
                'Observation values range from column 6 i.e. column index 5 to column 54 i.e. column index 53
                For m = 5 To 53
                    For Each ctl In Me.Controls
                        If Strings.Left(ctl.Name, 6) = "txtVal" And Val(Strings.Right(ctl.Name, 3)) = m Then
                            If Not IsDBNull(ds.Tables("form_synoptic_2_RA1").Rows(inc).Item(m)) Then
                                ctl.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item(m)
                            End If
                        End If
                    Next ctl
                Next m

                'Initialize textboxes for observation flags
                'Observation flags range from column 54 i.e. column index 5 to column 103 i.e. column index 102
                For m = 54 To 102
                    For Each ctl In Me.Controls
                        If Strings.Left(ctl.Name, 7) = "txtFlag" And Val(Strings.Right(ctl.Name, 3)) = m Then
                            If Not IsDBNull(ds.Tables("form_synoptic_2_RA1").Rows(inc).Item(m)) Then
                                ctl.Text = ds.Tables("form_synoptic_2_RA1").Rows(inc).Item(m)
                            End If
                        End If
                    Next ctl
                Next m

                displayRecordNumber()

            End If

    End Sub


    Private Sub txtYear_LostFocus(sender As Object, e As EventArgs) Handles txtYear.LostFocus
        
        Dim numericValueCheck As New dataEntryGlobalRoutines
        'Check value is numeric
        numericValueCheck.checkIsNumeric(txtYear.Text, txtYear)
    End Sub

    Private Sub cboMonth_LostFocus(sender As Object, e As EventArgs) Handles cboMonth.LostFocus
        'Dim numericValueCheck As New dataEntryGlobalRoutines
        ''Check for numeric
        'numericValueCheck.checkIsNumeric(cboMonth.Text, cboMonth)

        'If IsNumeric(cboMonth.Text) And Val(cboMonth.Text) > 12 Then
        '    cboMonth.BackColor = Color.Red
        '    MsgBox("Month of or range !", MsgBoxStyle.Critical)
        'Else
        '    cboMonth.BackColor = Color.White
        'End If
        
    End Sub

    Private Sub Val_Elem106TextBox_LostFocus(sender As Object, e As EventArgs) Handles txtVal_Elem106Field005.LostFocus
        
    End Sub

    Private Sub Val_Elem107TextBox_LostFocus(sender As Object, e As EventArgs) Handles txtVal_Elem107Field006.LostFocus

    End Sub

    Private Sub Val_Elem003TextBox_LostFocus(sender As Object, e As EventArgs) Handles txtVal_Elem003Field046.LostFocus
        If Val(txtVal_Elem003Field046.Text) > Val(txtVal_Elem002Field045.Text) Then
            txtVal_Elem002Field045.BackColor = Color.Cyan
            txtVal_Elem003Field046.BackColor = Color.Cyan
            MsgBox("Tmax must be greater or equal to Tmin!", MsgBoxStyle.Exclamation)
        Else
            txtVal_Elem002Field045.BackColor = Color.White
            txtVal_Elem003Field046.BackColor = Color.White
        End If
    End Sub

    

    Private Sub Val_Elem102TextBox_LostFocus(sender As Object, e As EventArgs) Handles txtVal_Elem102Field013.LostFocus
        Dim dewPoint As New dataEntryGlobalRoutines
        Dim dryBulb As String, wetBulb As String
        'Drybulb is element code 101 and wetbulb is element code 102
        If Val(txtVal_Elem102Field013.Text) > Val(txtVal_Elem101Field012.Text) Then
            'If wetbulb is greater than dewpoint both elements are flagged because either of them could be wrong.
            'i.e. wetbulb value could be higher than the correct value or drybulb could be lower than the correct value.
            txtVal_Elem101Field012.BackColor = Color.Cyan
            txtVal_Elem102Field013.BackColor = Color.Cyan
            MsgBox("Drybulb must be greater or equal to Wetbulb!", MsgBoxStyle.Exclamation)
        Else
            txtVal_Elem101Field012.BackColor = Color.White
            txtVal_Elem102Field013.BackColor = Color.White
            'Apply element scale factor to drybulb and wetbulb before calling function to calculate dewpoint
            dryBulb = Val(txtVal_Elem101Field012.Text) / 10
            wetBulb = Val(txtVal_Elem102Field013.Text) / 10
            'Remove element scale factor from dewpoint
            txtVal_Elem103Field014.Text = dewPoint.calculateDewpoint(dryBulb, wetBulb) * 10
        End If
    End Sub

    Private Sub Val_Elem177Label_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub txtVal_Elem103Field014_LostFocus(sender As Object, e As EventArgs) Handles txtVal_Elem103Field014.LostFocus
        Dim RH As New dataEntryGlobalRoutines
        Dim dryBulb As String, dewPoint As String
        'Apply element scale factor to drybulb and wetbulb before calling function to calculate RH
        dryBulb = Val(txtVal_Elem101Field012.Text) / 10
        dewPoint = Val(txtVal_Elem103Field014.Text) / 10
        txtVal_Elem105Field015.Text = RH.calculateRH(dewPoint, dryBulb)
    End Sub

    Private Sub txtVal_Elem101Field012_LostFocus(sender As Object, e As EventArgs) Handles txtVal_Elem101Field012.LostFocus
        Dim calculateGPMandMSLP As New dataEntryGlobalRoutines
        Dim ppp As String, dryBulb As String, gpm As String
        stationCode = cboStation.SelectedValue
        ppp = Val(txtVal_Elem106Field005.Text) / 10
        dryBulb = Val(txtVal_Elem101Field012.Text) / 10
        gpm = txtVal_Elem301Field010.Text

        sqlStationElevation = "SELECT stationid,elevation from station WHERE stationid=" & stationCode
        daStationElevation = New MySql.Data.MySqlClient.MySqlDataAdapter(sqlStationElevation, conn)
        'Clear all rows in dataset before filling dataset with new row record for active station
        dsStationElevation.Clear()
        'Add row for element code associated with active control
        daStationElevation.Fill(dsStationElevation, "station")
        stnElevation = dsStationElevation.Tables("station").Rows(0).Item("elevation")

        If stnElevation <> "" And txtVal_Elem106Field005.Text <> "" And txtVal_Elem101Field012.Text <> "" Then
            'Calculate geopotential
            txtVal_Elem196Field011.Text = calculateGPMandMSLP.calculateGeopotential(ppp, dryBulb, stnElevation, gpm)
            'calculate MSL pressure
            txtVal_Elem107Field006.Text = calculateGPMandMSLP.calculateMSLppp(ppp, dryBulb, stnElevation)
        End If
        
    End Sub

End Class
