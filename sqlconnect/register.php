<?php 

$con = mysqli_connect('localhost','root','root','FormidActiveSeating');

//check for connection
if(mysqli_connect_errno()){
	echo "1";
	exit();

}

$username = $_POST["username"];
$password = $_POST["password"];
$firstname = $_POST["firstname"];
$lastname = $_POST["lastname"];
$email = $_POST["email"];

$hashed_pass = hash('sha256', $password);


$insertuserquery = "INSERT INTO User (username,pass,firstName,lastName,email) VALUES ( '" . $username . "' , '" . $hashed_pass . "' , '" . $firstname . " ', '" . $lastname . " ', '" . $email . "');";

mysqli_query($con, $insertuserquery) or die("4: Insert user query failed");

echo("0");

?>