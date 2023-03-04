<?php 

$con = mysqli_connect('localhost','root','root','FormidActiveSeating');

//check for connection
if(mysqli_connect_errno()){
	echo "1";
	exit();

}

$username = $_POST["username"];
$passsword = $_POST["password"];
$firstname = $_POST["firstname"];
$lastname = $_POST["lastname"];
$email = $_POST["email"];




$insertuserquery = "INSERT INTO User (username,pass,firstName,lastName,email) VALUES ( '" . $username . "' , '" . $passsword . "' , '" . $firstname . " ', '" . $lastname . " ', '" . $email . "');";

mysqli_query($con, $insertuserquery) or die("4: Insert user query failed");

echo("0");

?>