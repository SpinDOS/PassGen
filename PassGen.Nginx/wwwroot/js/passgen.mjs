import "../lib/js-sha512/sha512.min.mjs";

export default function generatePassword(targetSite, salt) {
    const stringToHash = targetSite + "@" + salt;
    const sha512Result = sha512(stringToHash);
    return "p" + sha512Result.substring(0, 8) + "#7G";
}
