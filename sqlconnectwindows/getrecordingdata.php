<?php

$con = mysqli_connect('localhost:8889','root','root','formidactiveseating');

//check for connection
if(mysqli_connect_errno()){
	echo "1";
	exit();
}

$recordingId = $_POST["recordingId"];

$selectrecordingsquery = "SELECT recordingData FROM Recording WHERE recordingId = '" . $recordingId . "';";

$check = mysqli_query($con, $selectrecordingsquery);

if($check && mysqli_num_rows($check) == 1) {
    $output = "";
    while($row = mysqli_fetch_row($check)) {
        $output .= $row[0];
    }
    echo($output);  
}
else {
    echo($selectrecordingsquery);
}

?>