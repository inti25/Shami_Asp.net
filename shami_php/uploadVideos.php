<?php

if (isset($_POST["memId"]) && isset($_POST["isPrivate"]) && !empty($_FILES["myFile"])) {
    $myFile = $_FILES["myFile"];
    $memId = $_POST["memId"];
    $isPrivate = $_POST["isPrivate"];
    $description = rawurlencode($_POST["description"]);
    if ($myFile["error"] !== UPLOAD_ERR_OK) {
        getResult('http://shami.somee.com/upload.aspx?error=die');
    } else {
        $newName = round(microtime(true));
        $videoName = $newName . '.mp4';
        $path = './videos/' . $videoName;
        $videoUrl = 'http://shami.96.lt' . $path;
        $thumbnails;
        if (isset($_FILES["thumbnails"])) {
            $thumName = $newName . '.jpeg';
            $thumPath = './thumbnails/' . $thumName;
            if (move_uploaded_file($_FILES["thumbnails"]["tmp_name"], $thumPath)) {
                $thumbnails = 'http://shami.96.lt' . $thumPath;
            }
        }
        if (move_uploaded_file($myFile["tmp_name"], $path)) {
            getResult("http://shami.somee.com/upload.aspx?memId=$memId&description=$description&isPrivate=$isPrivate&videoUrl=$videoUrl&thumbnails=$thumbnails");
        } else {
            getResult('http://shami.somee.com/upload.aspx?error=die');
        }
    }
} else {
    getResult('http://shami.somee.com/upload.aspx?error=miss');
}

function getResult($url)
{
    $ch = curl_init($url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    $content = curl_exec($ch);
    curl_close($ch);
    echo $content;
}

?>


