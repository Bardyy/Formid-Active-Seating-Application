<?php 

$con = mysqli_connect('localhost','root','root','FormidActiveSeating');

//check for connection
if(mysqli_connect_errno()){
	echo "1";
	exit();

}

$startTime = $_POST["startTime"];
$duration = $_POST["duration"];
$data = $_POST["data"];
$username = $_POST["username"];


$insertrecordingquery = "INSERT INTO Recording (startTime, duration, recordingData, username) VALUES ('" . $startTime . "' , '" . $duration . "', '" . $data . "', '" . $username . "');";


mysqli_query($con, $insertrecordingquery) or die($insertrecordingquery);

echo("success");




 ?>