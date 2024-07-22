import assert from 'node:assert/strict';
import test from 'node:test';

import generatePassword from '../wwwroot/js/passgen.mjs';

test('simple test', function() {
    assert.equal(generatePassword('a', 'b'), 'p078064d5#7G');
});

test('unicode test', function() {
    assert.equal(generatePassword('а', 'б'), 'p1e8cb032#7G');
});
