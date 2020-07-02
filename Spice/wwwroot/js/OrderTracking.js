const requestTrackingOrdering = async () => {
    try {
        const countRaw = await fetch('/Customer/Order/TrackingOrder')

        if (countRaw) {
            const count = await countRaw.json()
        }

        if (count) {
            if (count === 0) {
                document.getElementById('trackingOrdering').innerHTML = 'Tracking Ordering'
            }
            else {
                document.getElementById('trackingOrdering').innerHTML = 'Tracking Ordering (' + count + ')'
            }
        }
    }
    catch (error) {
        console.log(error)
    }
}
requestTrackingOrdering()