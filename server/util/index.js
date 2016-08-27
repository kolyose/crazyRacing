class Util{
    getRandomValueFromArray(array){
        const randomIndex = Math.floor(Math.random() * array.length)
        return array.splice(randomIndex, 1)
    }
}

export default new Util()