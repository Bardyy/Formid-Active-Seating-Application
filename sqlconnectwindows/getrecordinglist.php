<?php

$con = mysqli_connect('localhost:8889','root','root','formidactiveseating');

//check for connection
if(mysqli_connect_errno()){
	echo "1";
	exit();
}

$username = $_POST["username"];

$selectrecordingsquery = "SELECT recordingId, startTime FROM Recording WHERE username = '" . $username . "';";

$check = mysqli_query($con, $selectrecordingsquery);

if($check && mysqli_num_rows($check) >= 1) {
    $output = "";
    while($row = mysqli_fetch_row($check)) {
        $output .= $row[0] . "," . $row[1] . ".";
    }
    echo($output);  
}
else {
    echo("No recording found");
}

?>