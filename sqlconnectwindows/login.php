<?php 

$con = mysqli_connect('localhost:8889','root','root','formidactiveseating');

//check for connection
if(mysqli_connect_errno()){
    echo "Unable to connect....";
    exit();

}

$username = $_POST["username"];
$password = $_POST["password"];

$hashed_pass = hash('sha256', $password);

$insertuserquery = " SELECT username, pass FROM User WHERE username = '" . $username . "';";

$check = mysqli_query($con, $insertuserquery);

if (mysqli_num_rows($check) != 1){

    echo("No user exists");
}
else {
    $userinfo = mysqli_fetch_assoc($check);
    $enteredPass = $userinfo["pass"];
    
    
    if($enteredPass != $hashed_pass){
        echo("invalid");
    }else{
        echo("success");
    }
}


 ?>