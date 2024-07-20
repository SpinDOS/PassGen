function generatePassword(targetSite, salt) {
    let stringToHash = targetSite + "@" + salt;
    let sha512Hash = sha512(stringToHash);
    return "p" + sha512Hash.substring(0, 8) + "#7G";
}