<?php

$con = mysqli_connect('localhost','root','root','FormidActiveSeating');

//check for connection
if(mysqli_connect_errno()){
	echo "1";
	exit();
}

$fromdate = $_POST["FromDate"];
$todate = $_POST["ToDate"];
$username = $_POST["Username"];

$selectrecordingsquery = "SELECT recordingData FROM recording WHERE username = '" . $username . "' AND startTime BETWEEN '" . $fromdate . "' AND '" . $todate .  "';";

$check = mysqli_query($con, $selectrecordingsquery);

if($check && mysqli_num_rows($check) >= 1) {
    $output = "";
    while($row = mysqli_fetch_row($check)) {
        $output .= $row[0] . "," . $row[1] . ".";
    }
    echo($output);  
    
}
else {
    echo($selectrecordingsquery);
}

?>