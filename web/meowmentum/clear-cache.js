const fs = require('fs');
const path = require('path');

function removeItem(itemPath) {
  fs.rmSync(itemPath, { force: true, recursive: true });
}

const itemsToRemove = [
  path.join(__dirname, '.next'),
  path.join(__dirname, 'tsconfig.tsbuildinfo'),
  // path.join(__dirname, 'package-lock.json'),
  path.join(__dirname, 'out'),
];

itemsToRemove.forEach(removeItem);

/*
!clear bash history
cat /dev/null > ~/.bash_history && history -c && exit
*/
