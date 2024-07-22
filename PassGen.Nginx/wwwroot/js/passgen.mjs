import "../lib/js-sha512/sha512.min.mjs"; // import sha512

export default function generatePassword(targetSite, salt) {
    const stringToHash = targetSite + "@" + salt;
    const utf8Bytes = new TextEncoder().encode(stringToHash);
    const sha512Result = sha512.create().update(utf8Bytes).hex();
    return "p" + sha512Result.substring(0, 8) + "#7G";
}
